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
        public ModEntity Entity;

        public static ReadOnlyList<string> Categories => new(m_categories);

        public static List<string> m_categories = [];

        public ComponentPlayer m_componentPlayer;
        public ComponentGui m_componentGui;

        public XElement items;

        private BevelledButtonWidget modButton;
        private BevelledButtonWidget hykjButton;
        public GameMode gameMode;

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
            ModsManager.RegisterHook("ClothingProcessSlotItems", this);
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

            // 创建按钮
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
                // 确保按钮已创建
                if (modButton == null && hykjButton == null)
                {
                    // 尝试查找现有按钮
                    modButton = mainMenuScreen.Children.Find<BevelledButtonWidget>("ModButton", true);
                    hykjButton = mainMenuScreen.Children.Find<BevelledButtonWidget>("HYKJButton", true);
                }
                // 检测按钮点击
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
        
        //兼容性不好，后续参考十亿伏特更换
        /// <summary>
        /// 方块初始化完成时执行
        /// </summary>
        public override void BlocksInitalized()
        {
            /*BlocksManager.m_categories.Clear();
            BlocksManager.m_categories.Add("Terrain");
            BlocksManager.m_categories.Add("Minerals");
            BlocksManager.m_categories.Add("Plants");
            BlocksManager.m_categories.Add("Construction");
            BlocksManager.m_categories.Add("Items");
            BlocksManager.m_categories.Add("荒野科技材料");
            BlocksManager.m_categories.Add("Tools");
            BlocksManager.m_categories.Add("荒野科技工具");
            BlocksManager.m_categories.Add("Weapons");
            BlocksManager.m_categories.Add("荒野科技武器");
            BlocksManager.m_categories.Add("Clothes");
            BlocksManager.m_categories.Add("Electrics");
            BlocksManager.m_categories.Add("Food");
            BlocksManager.m_categories.Add("Spawner Eggs");
            BlocksManager.m_categories.Add("Painted");
            BlocksManager.m_categories.Add("Dyed");
            BlocksManager.m_categories.Add("Fireworks");*/
        }
    }
}