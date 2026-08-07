using System.Linq;
using Random = Game.Random;
using Engine.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using GameEntitySystem;
using System;
using Engine;
using Game;

namespace HYKJ
{
    public class HYKJModLoader : ModLoader
    {
        public static ReadOnlyList<string> Categories => new(m_categories);

        public static List<string> m_categories = [];

        public ComponentPlayer m_componentPlayer;
        public ComponentGui m_componentGui;

        public XElement items;

        private BevelledButtonWidget modButton;
        private BevelledButtonWidget hykjButton;
        public GameMode gameMode;

        private SubsystemTimeOfDay m_subsystemTimeOfDay;

        public SubsystemParticles m_subsystemParticles;

        public const string fName = "HYKJModLoader";

        public static Subtexture ToSubtexture(string imgpath, Vector2? TopLeft = null, Vector2? BottomRight = null)
        {
            return new Subtexture(ContentManager.Get<Texture2D>(imgpath), TopLeft ?? Vector2.Zero, BottomRight ?? Vector2.One);
        }

        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("BlocksInitalized", this);
            ModsManager.RegisterHook("OnMainMenuScreenCreated", this);
            ModsManager.RegisterHook("AfterWidgetUpdate", this);
            ModsManager.RegisterHook("OnProjectLoaded", this);
            ModsManager.RegisterHook("TerrainContentsGenerator24Initialize", this);
            ModsManager.RegisterHook("DeadBeforeDrops", this);
            ModsManager.RegisterHook("OnMinerHit2", this);

            CorpseManager.LoadConfig(); // 加载尸体配置
        }

        /// <summary>
        /// 当Project被加载时执行
        /// </summary>
        /// <param name="project"></param>
        public override void OnProjectLoaded(Project project)
        {
            //后续应尝试换成设置
            //获取掉落物子系统对象
            m_subsystemParticles = project.FindSubsystem<SubsystemParticles>(throwOnError: true);
            //获取天数时长
            m_subsystemTimeOfDay = project.FindSubsystem<SubsystemTimeOfDay>(true);
            if (m_subsystemTimeOfDay.DayDuration != 1800f)
            {
                m_subsystemTimeOfDay.DayDuration = 1800f;
                Log.Warning("[HYKJ]:一天时长修改为1800");
            }
        }

        /// <summary>
        /// API 地形生成器初始化时回调，在此追加本模组自定义的生成步骤。
        /// </summary>
        public override void TerrainContentsGenerator24Initialize(ITerrainContentsGenerator terrainContentsGenerator, SubsystemTerrain subsystemTerrain)
        {
            if (terrainContentsGenerator is TerrainContentsGenerator24 generator)
            {
                ExtraGravelGenerator.Register(generator);
            }
        }

        /// <summary>
        /// 在主界面初始化后执行，你可以通过这个给主界面加些你想要的按钮或者文字等
        /// 不过建议开发者使用BeforeWidgetUpdate和AfterWidgetUpdate这两个接口实现
        /// </summary>
        /// <param name="mainMenuScreen">初始化完毕后的主界面</param>
        /// <param name="leftBottomBar">主界面左下角的按钮栏，里面有着API的切换语言和资源管理按钮</param>
        /// <param name="rightBottomBar">主界面右下角的按钮栏，Mod作者们可以在这里面放入想要的按钮（例如Mod设置按钮、Mod作者介绍按钮等）</param>
        public override void OnMainMenuScreenCreated(MainMenuScreen mainMenuScreen, StackPanelWidget leftBottomBar, StackPanelWidget rightBottomBar)
        {
            // 防止重复添加按钮
            if (rightBottomBar.Children.Any(c => c.Name == "ModButton"))
                return;
            if (rightBottomBar.Children.Any(c => c.Name == "HYKJButton"))
                return;

            modButton = new BevelledButtonWidget
            {
                Name = "ModButton", // 唯一标识
                Size = new Vector2(60f, 60f), // 与左侧按钮相同尺寸
                Margin = new Vector2(0, 8), // 与左侧按钮相同的间距
                Text = "" // 无文字
            };

            modButton.Children.Add(new RectangleWidget
            {
                Size = new Vector2(28f, 28f), // 与左侧图标相同尺寸
                TextureLinearFilter = true,
                TextureAnisotropicFilter = true,
                HorizontalAlignment = WidgetAlignment.Center,
                VerticalAlignment = WidgetAlignment.Center,
                OutlineColor = Color.Transparent, // 无边框
                FillColor = Color.White, // 白色图标
                Subtexture = ToSubtexture("HYKJTextures/Button/ModBulletin"),
            });

            // 创建按钮
            hykjButton = new BevelledButtonWidget
            {
                Name = "HYKJButton", // 唯一标识
                Size = new Vector2(60f, 60f), // 与左侧按钮相同尺寸
                Margin = new Vector2(0, 8), // 与左侧按钮相同的间距
                Text = "" // 无文字
            };

            hykjButton.Children.Add(new RectangleWidget
            {
                Size = new Vector2(28f, 28f), // 与左侧图标相同尺寸
                TextureLinearFilter = true,
                TextureAnisotropicFilter = true,
                HorizontalAlignment = WidgetAlignment.Center,
                VerticalAlignment = WidgetAlignment.Center,
                OutlineColor = Color.Transparent, // 无边框
                FillColor = Color.White, // 白色图标
                Subtexture = ToSubtexture("HYKJTextures/Button/1"),
            });
            // 添加到右下角按钮栏
            rightBottomBar.Children.Add(modButton);
            rightBottomBar.Children.Add(hykjButton);
        }

        /// <summary>
        /// 在Widget完成Update()后立即执行，能用于修改游戏中已有按钮的功能
        /// </summary>
        /// <param name="widget"></param>
        public override void AfterWidgetUpdate(Widget widget)
        {
            // 只处理主菜单屏幕
            if (widget is MainMenuScreen mainMenuScreen)
            {
                if (modButton == null && hykjButton == null)
                {
                    modButton = mainMenuScreen.Children.Find<BevelledButtonWidget>("ModButton", true);
                    hykjButton = mainMenuScreen.Children.Find<BevelledButtonWidget>("HYKJButton", true);
                }
                if (modButton != null && modButton.IsClicked)
                {
                    GxUpdate.ShowUpdate();
                }
                if (hykjButton != null && hykjButton.IsClicked)
                {
                    HYKJUpdate.ShowUpdate();
                }
            }
        }

        /// <summary>
        /// 方块初始化完成时执行
        /// </summary>
        public override void BlocksInitalized()
        {
            BlocksManager.m_categories.RemoveAll(c => c == "HYKJ Material");
            BlocksManager.m_categories.RemoveAll(c => c == "HYKJ Tool");
            BlocksManager.m_categories.RemoveAll(c => c == "HYKJ Weapons");
            BlocksManager.m_categories.RemoveAll(c => c == "测试");

            int idx1 = BlocksManager.m_categories.FindIndex(c => c == "Items");
            if (idx1 != -1)
            {
                BlocksManager.m_categories.Insert(idx1 + 1, "HYKJ Material");
            }

            int idx2 = BlocksManager.m_categories.FindIndex(c => c == "Weapons");
            if (idx2 != -1)
            {
                BlocksManager.m_categories.Insert(idx2, "HYKJ Tool");
            }

            int idx3 = BlocksManager.m_categories.FindIndex(c => c == "Weapons");
            if (idx3 != -1)
            {
                BlocksManager.m_categories.Insert(idx3 + 1, "HYKJ Weapons");
            }

            int idx4 = BlocksManager.m_categories.FindIndex(c => c == "Fireworks");
            if (idx4 != -1)
            {
                BlocksManager.m_categories.Insert(idx3 + 1, "测试");
            }
        }

        // ==================== 尸体/解剖系统 ====================

        /// <summary>
        /// 生物死亡时阻止立即掉落物品，改为尸体保留+解剖机制
        /// </summary>
        public override void DeadBeforeDrops(ComponentHealth componentHealth, ref KillParticleSystem killParticleSystem, ref bool dropAllItems)
        {
            ComponentPlayer player = componentHealth.Entity.FindComponent<ComponentPlayer>();
            if (player != null) return;

            dropAllItems = false;

            // 从配置文件获取腐烂时间和解剖刀数（按实体模板名匹配）
            string templateName = componentHealth.Entity.ValuesDictionary.DatabaseObject.Name;
            CorpseManager.CreatureCfg cfg = CorpseManager.GetCreatureConfig(componentHealth.Entity);
            int hitsNeeded = CorpseManager.CalculateHitsNeeded(componentHealth, componentHealth.Entity);
            float naturalDecay = cfg.CorpseDuration > 0f ? cfg.CorpseDuration : 120f;

            CorpseManager.Register(
                componentHealth.Entity, hitsNeeded, naturalDecay,
                componentHealth.DeathTime ?? 0f
            );

            componentHealth.CorpseDuration = float.MaxValue;

            if (killParticleSystem == null)
            {
                Vector3 pos = componentHealth.Entity.FindComponent<ComponentBody>()?.Position ?? Vector3.Zero;
                killParticleSystem = new KillParticleSystem(
                    componentHealth.Project.FindSubsystem<SubsystemTerrain>(throwOnError: false),
                    pos + new Vector3(0f, 0.5f, 0f), 1.5f
                );
            }

            Log.Information($"[HYKJ] 尸体注册: {templateName}, 需解剖{hitsNeeded}刀, 腐烂{naturalDecay}秒");
        }

        /// <summary>
        /// 玩家攻击命中时检测是否在解剖尸体
        /// </summary>
        public override void OnMinerHit2(ComponentMiner componentMiner,
            ComponentBody componentBody,
            Vector3 hitPoint,
            Vector3 hitDirection,
            ref int durabilityReduction,
            ref Attackment attackment)
        {
            if (componentBody == null || componentBody.Entity == null) return;
            if (!CorpseManager.IsCorpse(componentBody.Entity)) return;

            int activeValue = componentMiner.ActiveBlockValue;
            if (activeValue == 0) return;
            int blockIndex = Terrain.ExtractContents(activeValue);

            // 只有配置表中的解剖工具才能解剖
            if (!CorpseManager.IsDissectionTool(blockIndex)) return;

            // 执行解剖
            SubsystemTerrain terrain = componentMiner.Project.FindSubsystem<SubsystemTerrain>(false);
            int remaining = CorpseManager.Dissect(componentBody.Entity, blockIndex, terrain, hitPoint);

            // 消耗工具耐久
            durabilityReduction = 1;

            // 显示进度信息
            ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
            if (componentPlayer != null)
            {
                if (remaining == 0)
                {
                    componentPlayer.ComponentGui.DisplaySmallMessage(
                        "解剖完成！", Color.Green, blinking: true, playNotificationSound: true);
                }
                else
                {
                    componentPlayer.ComponentGui.DisplaySmallMessage(
                        $"解剖中... 还需{remaining}刀", Color.White, blinking: false, playNotificationSound: false);
                }
            }
        }
    }
}