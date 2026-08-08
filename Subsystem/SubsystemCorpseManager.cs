using System.Collections.Generic;
using GameEntitySystem;
using Game;
using Engine;
using Engine.Media;
using Engine.Graphics;
using TemplatesDatabase;

namespace HYKJ
{
    /// <summary>
    /// 尸体管理子系统：处理自然腐烂、解剖完成清理、尸体信息渲染
    /// </summary>
    public class SubsystemCorpseManager : Subsystem, IUpdateable, IDrawable
    {
        public SubsystemGameInfo m_subsystemGameInfo;
        public PrimitivesRenderer3D m_primitivesRenderer;

        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public int[] DrawOrders => new int[1];

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

                if (!entity.IsAddedToProject)
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

                if (data.NaturalDecay > 0f && elapsed > data.NaturalDecay)
                {
                    CorpseManager.DropDecayedLoot(entity);
                    expiredList.Add(entity);
                    ForceDespawn(entity);
                }
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
                if (!entity.IsAddedToProject) return;
                ComponentCreature creature = entity.FindComponent<ComponentCreature>();
                if (creature?.ComponentSpawn != null)
                    creature.ComponentSpawn.Despawn();
            }
            catch { }
        }

        // ==================== 尸体信息渲染 ====================

        public void Draw(Camera camera, int drawOrder)
        {
            if (CorpseManager.Corpses.Count == 0) return;

            // 懒加载渲染器
            if (m_primitivesRenderer == null)
            {
                var modelsRenderer = Project.FindSubsystem<SubsystemModelsRenderer>(false);
                if (modelsRenderer == null) return;
                m_primitivesRenderer = modelsRenderer.PrimitivesRenderer;
            }

            BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");

            foreach (KeyValuePair<Entity, CorpseManager.CorpseData> kvp in CorpseManager.Corpses)
            {
                Entity entity = kvp.Key;
                if (!entity.IsAddedToProject) continue;

                ComponentBody body = entity.FindComponent<ComponentBody>();
                if (body == null) continue;

                ComponentHealth health = entity.FindComponent<ComponentHealth>();
                if (health == null || health.Health > 0f) continue;

                ComponentCreature creature = entity.FindComponent<ComponentCreature>();
                if (creature == null) continue;

                float height = body.BoxSize.Y;
                var pos = body.Position + Vector3.UnitY * height + new Vector3(0, 0.4f, 0);
                var vector = Vector3.Transform(pos, camera.ViewMatrix);

                if (vector.Z >= 0f) continue;

                float fade = MathUtils.Saturate((vector.Length() - 16f) / 3f);
                var color = Color.Lerp(Color.White, Color.Transparent, fade);
                if (color.A <= 6) continue;

                var right = Vector3.TransformNormal(0.005f * Vector3.Normalize(Vector3.Cross(camera.ViewDirection, camera.ViewUp)), camera.ViewMatrix);
                var down = Vector3.TransformNormal(-0.005f * Vector3.UnitY, camera.ViewMatrix);

                // 第1行：生物名称
                string name = creature.DisplayName;
                m_primitivesRenderer.FontBatch(font, 1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.LinearClamp)
                    .QueueText(name, vector, right, down, color, TextAnchor.HorizontalCenter | TextAnchor.Bottom);

                // 第2行：解剖进度
                var vector2 = Vector3.Transform(pos - new Vector3(0, 0.22f, 0), camera.ViewMatrix);
                int remaining = CorpseManager.GetRemainingHits(entity);
                int total = CorpseManager.GetTotalHits(entity);
                string progress;
                if (remaining <= 0)
                    progress = "解剖完成";
                else
                    progress = $"解剖 {total - remaining}/{total}";

                // 进度条背景
                var barPos = Vector3.Transform(pos - new Vector3(0, 0.38f, 0), camera.ViewMatrix);
                float barWidth = 1.2f;
                float barHeight = 0.08f;
                float progressPct = total > 0 ? (float)(total - remaining) / total : 0f;

                var barLeft = barPos - new Vector3(barWidth / 2f, 0, 0);
                var barRight = barPos + new Vector3(barWidth / 2f, 0, 0);
                var barProgress = barPos - new Vector3(barWidth / 2f - barWidth * progressPct, 0, 0);

                if (vector2.Z < 0f)
                {
                    m_primitivesRenderer.FontBatch(font, 1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.LinearClamp)
                        .QueueText(progress, vector2, right, down, color * 0.85f, TextAnchor.HorizontalCenter | TextAnchor.Bottom);
                }
            }
        }
    }
}
