using System.Globalization;
using Engine;
using Engine.Graphics;
using Engine.Media;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace HYKJ
{
    public class ComponentAnimasDie : Component, IUpdateable, IDrawable
    {
        public ComponentPlayer Player;
        public ComponentOnFire OnFire;
        public ComponentHealth Health;
        public ComponentHerdBehavior herdBehavior;
        private ComponentCreature m_componentCreature;
        private SubsystemModelsRenderer m_subsystemModelsRenderer;
        private SubsystemPlayers players;
        public SubsystemTime Time;
        public float czDH;
        public float DismembermentProgressMax;
        public float DismembermentProgress;
        public string text;
        public bool isDie;
        public int Creature;
        public static int[] m_drawOrders = new int[1];
        public int[] DrawOrders => m_drawOrders;
        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            Time = Project.FindSubsystem<SubsystemTime>(true);
            Health = Entity.FindComponent<ComponentHealth>(true);
            m_componentCreature = Entity.FindComponent<ComponentCreature>(true);
            OnFire = Entity.FindComponent<ComponentOnFire>(true);
            m_subsystemModelsRenderer = Project.FindSubsystem<SubsystemModelsRenderer>(true);
            players = Project.FindSubsystem<SubsystemPlayers>(true);
            herdBehavior = Entity.FindComponent<ComponentHerdBehavior>();

            isDie = valuesDictionary.GetValue<bool>("isDie");
            Creature = valuesDictionary.GetValue<int>("Creature");
            DismembermentProgress = valuesDictionary.GetValue<float>("DismembermentProgress");
            DismembermentProgressMax = valuesDictionary.GetValue<float>("DismembermentProgressMax");
            czDH = valuesDictionary.GetValue<float>("czDH");
        }

        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            valuesDictionary.SetValue<bool>("isDie", isDie);
            valuesDictionary.SetValue<int>("Creature", Creature);
            valuesDictionary.SetValue<float>("DismembermentProgress", DismembermentProgress);
            valuesDictionary.SetValue<float>("DismembermentProgressMax", DismembermentProgressMax);
            valuesDictionary.SetValue<float>("czDH", czDH);
        }

        public void Update(float dt)
        {
            bool flag = Time.PeriodicGameTimeEvent(1.0, 0.0);
            Player = players.ComponentPlayers.Count > 0 ? players.ComponentPlayers[0] : null;

            if (czDH >= 0f && flag)
                czDH -= 1f;

            if (DismembermentProgress <= 0f)
            {
                DismembermentProgress = 0f;
                m_componentCreature.ComponentSpawn.Despawn();
                m_componentCreature.ComponentHealth.CorpseDuration = 0f;
            }

            if (Health.Health <= 0f && OnFire.IsOnFire && flag)
                DismembermentProgress -= 10f;

            if (Health.Health <= 0f && !isDie)
                isDie = true;
        }

        public void Draw(Camera camera, int drawOrder)
        {
            // 核心修复：确保生物实体存在且文本有内容
            if (m_componentCreature == null || m_subsystemModelsRenderer == null)
                return;

            string displayName = m_componentCreature.DisplayName;
            float healthValue = m_componentCreature.ComponentHealth.Health * m_componentCreature.ComponentHealth.AttackResilience;

            // 修复文本为空的情况（补充默认状态文本）
            if (m_componentCreature.ComponentHealth.Health > 0f)
            {
                text = $"{displayName} 血量: {healthValue:0.0}";
            }
            else
            {
                // 避免除数为0导致的渲染异常
                float progress = DismembermentProgressMax <= 0f ? 0f : DismembermentProgress / DismembermentProgressMax * 100f;
                //text = $"{displayName}【肢解进度：{progress:0}%】";
                text = $"{displayName} 正在自然分解....";
            }

            // 计算文本位置（调整高度偏移，确保在视野内）
            Vector3 worldPosition = ((ComponentFrame)m_componentCreature.ComponentBody).Position
                + new Vector3(0f, m_componentCreature.ComponentBody.BoxSize.Y + 0.5f, 0f); // 简化高度计算
            Vector3 screenPosition = Vector3.Transform(worldPosition, camera.ViewMatrix);

            // 文本排版参数
            Vector3 textDirX = Vector3.TransformNormal(
                0.005f * Vector3.Normalize(Vector3.Cross(camera.ViewDirection, camera.ViewUp)),
                camera.ViewMatrix
            );
            Vector3 textDirY = Vector3.TransformNormal(-0.005f * Vector3.UnitY, camera.ViewMatrix);

            // 仅在相机视野内渲染
            if (screenPosition.Z >= 0f)
                return;

            // 修复颜色未应用的问题（使用计算出的colorVal）
            Color baseColor = m_componentCreature.ComponentHealth.Health > 0f
                ? Color.White
                : m_componentCreature.ComponentHealth.Health < 0.3f
                    ? Color.Red
                    : m_componentCreature.ComponentHealth.Health < 0.7f
                        ? Color.Yellow
                        : Color.Green;

            // 透明度随距离变化（限制距离范围，避免异常值）
            float distance = MathUtils.Clamp(screenPosition.Length(), 0f, 7f); // 限制最大距离为7
            float alpha = MathUtils.Saturate(1f - (distance - 4f) / 3f); // 距离4以内不透明，7以外完全透明
            Color finalColor = Color.Lerp(baseColor, Color.Transparent, 1f - alpha);

            // 确保字体加载和文本绘制有效
            if (finalColor.A > 8 && !string.IsNullOrEmpty(text))
            {
                BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles", null);
                if (font != null)
                {
                    m_subsystemModelsRenderer.PrimitivesRenderer.FontBatch(
                        font, 1,
                        DepthStencilState.DepthRead,
                        RasterizerState.CullNoneScissor,
                        BlendState.AlphaBlend,
                        SamplerState.LinearClamp
                    ).QueueText(text, screenPosition, textDirX, textDirY, finalColor, (TextAnchor)9);
                }
            }
        }
    }
}