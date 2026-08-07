using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GameEntitySystem;
using Game;
using Engine;

namespace HYKJ
{
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

        public class CreatureCfg
        {
            public float CorpseDuration { get; set; }
            public int HitsNeeded { get; set; }
        }

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
                    // 从模组 Assets 中读取
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

        /// <summary>
        /// 判断是否为解剖工具
        /// </summary>
        public static bool IsDissectionTool(int blockIndex)
        {
            var block = BlocksManager.Blocks[blockIndex];
            string name = block.GetType().Name;
            return s_toolEfficiency.ContainsKey(name);
        }

        /// <summary>
        /// 获取工具解剖效率
        /// </summary>
        public static float GetToolEfficiency(int blockIndex)
        {
            var block = BlocksManager.Blocks[blockIndex];
            string name = block.GetType().Name;
            return s_toolEfficiency.TryGetValue(name, out float eff) ? eff : 1.0f;
        }

        /// <summary>
        /// 根据生物名称查找配置
        /// </summary>
        public static CreatureCfg GetCreatureConfig(string displayName)
        {
            if (s_creatureConfig.TryGetValue(displayName, out CreatureCfg cfg))
                return cfg;
            return s_defaultConfig;
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
            // 每刀的实际进度 = 工具效率
            data.CurrentHits += (int)System.Math.Ceiling(efficiency);

            // 血粒子
            if (terrain != null)
            {
                var blood = new KillParticleSystem(terrain, position, 0.8f);
                var particles = terrain.Project.FindSubsystem<SubsystemParticles>(false);
                particles?.AddParticleSystem(blood);
            }

            // 重新计算基于效率的等效刀数
            int effectiveTotal = (int)System.Math.Ceiling(data.TotalHits / efficiency);
            int effectiveCurrent = (int)System.Math.Ceiling(data.CurrentHits / efficiency);
            // 直接返回实际剩余刀数
            int remaining = System.Math.Max(data.TotalHits - data.CurrentHits, 0);
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
            return System.Math.Max(data.TotalHits - data.CurrentHits, 0);
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

        /// <summary>
        /// 根据生物类型和配置计算解剖需要刀数
        /// </summary>
        public static int CalculateHitsNeeded(ComponentHealth health, string displayName)
        {
            CreatureCfg cfg = GetCreatureConfig(displayName);
            if (cfg.HitsNeeded > 0)
                return cfg.HitsNeeded;
            // 未配置时按血量估算
            float resilience = health.AttackResilience;
            return (int)MathUtils.Clamp(resilience / 10f, 2f, 20f);
        }

        // ==================== 掉落逻辑 ====================

        public static void DropDecayedLoot(Entity entity)
        {
            Vector3 position = entity.FindComponent<ComponentBody>()?.Position ?? Vector3.Zero;
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
            foreach (IInventory inventory in entity.FindComponents<IInventory>())
            {
                inventory.DropAllItems(position);
            }
        }
    }
}
