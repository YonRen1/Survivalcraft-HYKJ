using System.Collections.Generic;
using GameEntitySystem;
using Game;
using Engine;

namespace HYKJ
{
    /// <summary>
    /// 尸体管理器：存储每个生物尸体数据（解剖进度、奖励掉落等）
    /// </summary>
    public static class CorpseManager
    {
        public static Dictionary<Entity, CorpseData> Corpses = new();

        public class CorpseData
        {
            public int TotalHits;      // 总共需要解剖刀数
            public int CurrentHits;    // 已解剖刀数
            public double DeathTime;   // 死亡时间
            public float NaturalDecay; // 自然腐烂时间（秒）
        }

        /// <summary>
        /// 注册一个新尸体
        /// </summary>
        public static void Register(Entity entity, int totalHits, float naturalDecay, double deathTime)
        {
            Corpses[entity] = new CorpseData
            {
                TotalHits = totalHits,
                CurrentHits = 0,
                DeathTime = deathTime,
                NaturalDecay = naturalDecay
            };
        }

        /// <summary>
        /// 执行一次解剖，返回剩余刀数（0=完成）
        /// </summary>
        public static int Dissect(Entity entity, SubsystemTerrain terrain, Vector3 position)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return -1;

            data.CurrentHits++;

            // 血粒子
            if (terrain != null)
            {
                var blood = new KillParticleSystem(terrain, position, 0.8f);
                // KillParticleSystem 通过 SubsystemParticles 添加
                var particles = terrain.Project.FindSubsystem<SubsystemParticles>(false);
                particles?.AddParticleSystem(blood);
            }

            int remaining = data.TotalHits - data.CurrentHits;
            return MathUtils.Max(remaining, 0);
        }

        /// <summary>
        /// 尸体是否已完全解剖
        /// </summary>
        public static bool IsFullyDissected(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return false;
            return data.CurrentHits >= data.TotalHits;
        }

        /// <summary>
        /// 获取解剖进度 0~1
        /// </summary>
        public static float GetProgress(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data) || data.TotalHits == 0)
                return 0f;
            return (float)data.CurrentHits / data.TotalHits;
        }

        /// <summary>
        /// 获取剩余刀数
        /// </summary>
        public static int GetRemainingHits(Entity entity)
        {
            if (!Corpses.TryGetValue(entity, out CorpseData data))
                return -1;
            return MathUtils.Max(data.TotalHits - data.CurrentHits, 0);
        }

        /// <summary>
        /// 移除尸体记录
        /// </summary>
        public static void Remove(Entity entity)
        {
            Corpses.Remove(entity);
        }

        /// <summary>
        /// 判断实体是否是尸体
        /// </summary>
        public static bool IsCorpse(Entity entity)
        {
            return entity != null && Corpses.ContainsKey(entity);
        }

        /// <summary>
        /// 根据生物类型计算解剖需要刀数
        /// </summary>
        public static int CalculateHitsNeeded(ComponentHealth health)
        {
            float resilience = health.AttackResilience;
            // 基础: 攻击韧性 / 10，最少 2 刀，最多 20 刀
            return (int)MathUtils.Clamp(resilience / 10f, 2f, 20f);
        }

        /// <summary>
        /// 自然腐烂时掉落（减少50%食物，保留骨头）
        /// </summary>
        public static void DropDecayedLoot(Entity entity)
        {
            Vector3 position = entity.FindComponent<ComponentBody>()?.Position ?? Vector3.Zero;
            foreach (IInventory inventory in entity.FindComponents<IInventory>())
            {
                // 只掉落一半
                int totalSlots = inventory.SlotsCount;
                for (int i = 0; i < totalSlots; i++)
                {
                    int count = inventory.GetSlotCount(i);
                    if (count > 0)
                    {
                        int decayedCount = count / 2; // 腐烂减半
                        if (decayedCount > 0)
                        {
                            inventory.RemoveSlotItems(i, count - decayedCount);
                        }
                    }
                }
                inventory.DropAllItems(position);
            }
        }

        /// <summary>
        /// 完全解剖后掉落（全量 + 额外骨头）
        /// </summary>
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
