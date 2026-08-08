using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GameEntitySystem;
using Game;
using Engine;

namespace HYKJ
{
    /// <summary>
    /// 尸体管理器：存储尸体数据、加载配置、处理解剖逻辑
    /// </summary>
    public static class CorpseManager
    {
        public static Dictionary<Entity, CorpseData> Corpses = new();

        // JSON 配置缓存
        private static Dictionary<string, float> s_toolEfficiency = new();
        private static Dictionary<string, CreatureCfg> s_creatureConfig = new();
        private static CreatureCfg s_defaultConfig = new() { CorpseDuration = 120f, HitsNeeded = 0 };
        private static bool s_loaded;

        public class CorpseData
        {
            public int TotalHits;
            public int CurrentHits;
            public double DeathTime;
            public float NaturalDecay;
            public float Efficiency;
        }

        public class CreatureCfg
        {
            public float CorpseDuration { get; set; }
            public int HitsNeeded { get; set; }
        }

        // ==================== 配置加载 ====================

        /// <summary>
        /// 加载 JSON 配置文件
        /// </summary>
        public static void LoadConfig()
        {
            if (s_loaded) return;
            s_loaded = true;

            try
            {
                string json = null;
                try
                {
                    using var stream = ContentManager.Get<Stream>("CorpseConfig.json");
                    using var reader = new StreamReader(stream);
                    json = reader.ReadToEnd();
                }
                catch
                {
                    Log.Warning("[HYKJ] 无法读取CorpseConfig.json，使用默认配置");
                    SetupDefaults();
                    return;
                }

                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                // 解析工具
                if (root.TryGetProperty("Tools", out JsonElement tools))
                {
                    foreach (JsonProperty prop in tools.EnumerateObject())
                    {
                        if (prop.Name.StartsWith("_")) continue;
                        float eff = (float)prop.Value.GetDouble();
                        s_toolEfficiency[prop.Name] = eff;
                    }
                }

                // 解析生物
                if (root.TryGetProperty("Creatures", out JsonElement creatures))
                {
                    foreach (JsonProperty prop in creatures.EnumerateObject())
                    {
                        if (prop.Name.StartsWith("_")) continue;
                        var cfg = new CreatureCfg
                        {
                            CorpseDuration = prop.Value.TryGetProperty("CorpseDuration", out JsonElement cd)
                                ? cd.GetSingle() : 120f,
                            HitsNeeded = prop.Value.TryGetProperty("HitsNeeded", out JsonElement hn)
                                ? hn.GetInt32() : 0
                        };
                        s_creatureConfig[prop.Name] = cfg;
                    }
                }

                // 默认配置
                if (root.TryGetProperty("Default", out JsonElement def))
                {
                    s_defaultConfig = new CreatureCfg
                    {
                        CorpseDuration = def.TryGetProperty("CorpseDuration", out JsonElement dcd)
                            ? dcd.GetSingle() : 120f,
                        HitsNeeded = def.TryGetProperty("HitsNeeded", out JsonElement dhn)
                            ? dhn.GetInt32() : 0
                    };
                }

                Log.Information($"[HYKJ] 尸体配置加载: {s_toolEfficiency.Count}种工具, {s_creatureConfig.Count}种生物");
            }
            catch (Exception ex)
            {
                Log.Error($"[HYKJ] 加载尸体配置失败: {ex.Message}");
                SetupDefaults();
            }
        }

        private static void SetupDefaults()
        {
            s_toolEfficiency.Clear();
            s_toolEfficiency["Flint_knifeBlock"] = 1.0f;
            s_toolEfficiency["leather_knifeBlock"] = 1.2f;
            s_toolEfficiency["bone_MacheteBlock"] = 1.5f;
            s_toolEfficiency["copper_sawBlock"] = 1.8f;
            s_toolEfficiency["iron_sawBlock"] = 2.0f;
        }

        // ==================== 工具判断 ====================

        public static bool IsDissectionTool(int blockIndex)
        {
            var block = BlocksManager.Blocks[blockIndex];
            string name = block.GetType().Name;
            return s_toolEfficiency.ContainsKey(name);
        }

        public static float GetToolEfficiency(int blockIndex)
        {
            var block = BlocksManager.Blocks[blockIndex];
            string name = block.GetType().Name;
            return s_toolEfficiency.TryGetValue(name, out float eff) ? eff : 1.0f;
        }

        // ==================== 生物配置 ====================

        /// <summary>
        /// 根据实体模板名查找配置
        /// </summary>
        public static CreatureCfg GetCreatureConfig(Entity entity)
        {
            string templateName = entity.ValuesDictionary.DatabaseObject.Name;
            if (s_creatureConfig.TryGetValue(templateName, out CreatureCfg cfg))
                return cfg;
            return s_defaultConfig;
        }

        /// <summary>
        /// 根据配置计算解剖需要刀数
        /// </summary>
        public static int CalculateHitsNeeded(ComponentHealth health, Entity entity)
        {
            CreatureCfg cfg = GetCreatureConfig(entity);
            if (cfg.HitsNeeded > 0)
                return cfg.HitsNeeded;
            float resilience = health.AttackResilience;
            return (int)MathUtils.Clamp(resilience / 10f, 2f, 20f);
        }

        // ==================== 尸体生命周期 ====================

        public static void Register(Entity entity, int totalHits, float naturalDecay, double deathTime)
        {
            Corpses[entity] = new CorpseData
            {
                TotalHits = totalHits,
                CurrentHits = 0,
                DeathTime = deathTime,
                NaturalDecay = naturalDecay,
                Efficiency = 1.0f
            };
        }

        public static int Dissect(Entity entity, int blockIndex, SubsystemTerrain terrain, Vector3 position)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return -1;

            float efficiency = GetToolEfficiency(blockIndex);
            data.Efficiency = efficiency;
            // 每刀进度 = 工具效率（效率2.0 = 一刀顶两刀）
            data.CurrentHits += (int)Math.Ceiling(efficiency);

            // 血粒子
            if (terrain != null)
            {
                var blood = new KillParticleSystem(terrain, position, 0.8f);
                var particles = terrain.Project.FindSubsystem<SubsystemParticles>(false);
                particles?.AddParticleSystem(blood);
            }

            // 返回剩余刀数（按当前效率折算）
            int remaining = Math.Max(data.TotalHits - data.CurrentHits, 0);
            return remaining;
        }

        public static bool IsFullyDissected(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return false;
            return data.CurrentHits >= data.TotalHits;
        }

        public static float GetProgress(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data) || data.TotalHits == 0)
                return 0f;
            return (float)data.CurrentHits / data.TotalHits;
        }

        public static int GetRemainingHits(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return -1;
            return Math.Max(data.TotalHits - data.CurrentHits, 0);
        }

        public static int GetTotalHits(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return 0;
            return data.TotalHits;
        }

        public static bool IsCorpse(Entity entity)
        {
            return entity != null && Corpses.ContainsKey(entity);
        }

        public static void Remove(Entity entity)
        {
            Corpses.Remove(entity);
        }

        // ==================== 掉落逻辑 ====================

        public static void DropDecayedLoot(Entity entity)
        {
            Vector3 position = entity.FindComponent<ComponentBody>()?.Position ?? Vector3.Zero;
            // 1. 原版 Loot 组件掉落（动物配置的肉/皮毛等）——自然腐烂也掉
            //entity.FindComponent<ComponentLoot>()?.DropLootNow();
            // 2. 实体背包物品减半掉落（若有 IInventory 容器）
            foreach (IInventory inventory in entity.FindComponents<IInventory>())
            {
                int totalSlots = inventory.SlotsCount;
                for (int i = 0; i < totalSlots; i++)
                {
                    int count = inventory.GetSlotCount(i);
                    if (count > 0)
                    {
                        int decayedCount = count / 2;
                        if (decayedCount > 0)
                            inventory.RemoveSlotItems(i, count - decayedCount);
                    }
                }
                inventory.DropAllItems(position);
            }
        }

        public static void DropFullLoot(Entity entity)
        {
            Vector3 position = entity.FindComponent<ComponentBody>()?.Position ?? Vector3.Zero;
            // 1. 原版 Loot 组件掉落（动物配置的肉/皮毛等）——解剖完成全量掉落
            //entity.FindComponent<ComponentLoot>()?.DropLootNow();
            // 2. 实体背包物品全量掉落（若有 IInventory 容器）
            foreach (IInventory inventory in entity.FindComponents<IInventory>())
            {
                inventory.DropAllItems(position);
            }
        }
    }
}