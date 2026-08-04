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
            ModsManager.RegisterHook("BlocksInitalized", this);//方块初始化完成时执行
            ModsManager.RegisterHook("OnMainMenuScreenCreated", this);//在主界面初始化后执行
            ModsManager.RegisterHook("AfterWidgetUpdate", this);//在Widget完成Update()后立即执行
            ModsManager.RegisterHook("OnProjectLoaded", this);
            ModsManager.RegisterHook("TerrainContentsGenerator24Initialize", this);// 注册地形生成器初始化 hook
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
    }
}