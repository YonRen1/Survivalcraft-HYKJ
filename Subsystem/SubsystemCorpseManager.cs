using System.Collections.Generic;
using GameEntitySystem;
using Game;
using Engine;

namespace HYKJ
{
    /// <summary>
    /// 尸体管理子系统：处理自然腐烂和解剖完成的尸体清理
    /// </summary>
    public class SubsystemCorpseManager : Subsystem, IUpdateable
    {
        public SubsystemGameInfo m_subsystemGameInfo;
        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
        }

        public void Update(float dt)
        {
            if (CorpseManager.Corpses.Count == 0)
                return;

            var expiredList = new List<Entity>();

            foreach (KeyValuePair<Entity, CorpseManager.CorpseData> kvp in CorpseManager.Corpses)
            {
                Entity entity = kvp.Key;
                CorpseManager.CorpseData data = kvp.Value;

                if (entity.IsDisposed || !entity.IsAddedToProject)
                {
                    expiredList.Add(entity);
                    continue;
                }

                ComponentHealth health = entity.FindComponent<ComponentHealth>();
                if (health == null || health.Health > 0f)
                {
                    expiredList.Add(entity);
                    continue;
                }

                double elapsed = m_subsystemGameInfo.TotalElapsedGameTime - data.DeathTime;

                // 自然腐烂到期
                if (data.NaturalDecay > 0f && elapsed > data.NaturalDecay)
                {
                    CorpseManager.DropDecayedLoot(entity);
                    expiredList.Add(entity);
                    ForceDespawn(entity);
                }
                // 完全解剖
                else if (CorpseManager.IsFullyDissected(entity))
                {
                    CorpseManager.DropFullLoot(entity);
                    expiredList.Add(entity);
                    ForceDespawn(entity);
                }
            }

            foreach (Entity entity in expiredList)
            {
                CorpseManager.Remove(entity);
            }
        }

        private void ForceDespawn(Entity entity)
        {
            try
            {
                if (entity.IsDisposed || !entity.IsAddedToProject) return;
                ComponentCreature creature = entity.FindComponent<ComponentCreature>();
                if (creature?.ComponentSpawn != null)
                    creature.ComponentSpawn.Despawn();
            }
            catch { }
        }
    }
}
