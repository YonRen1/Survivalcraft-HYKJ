using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using Engine.Graphics;
using Game;

namespace HYKJ
{
    public class ComponentNewGui : Component, IUpdateable
    {
        public ComponentPlayer m_componentPlayer;
        public ComponentGui m_componentGui;
        public SubsystemTime m_subsystemTime;
        public SubsystemAudio m_subsystemAudio;
        public GameMode gameMode;

        public CanvasWidget controlsContainer;
        public StackPanelWidget leftContainer;
        public StackPanelWidget lightContainer;
        public StackPanelWidget moreContents;
        public StackPanelWidget statusContents;

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public static string fName = "ModGui";

        public static Subtexture ToSubtexture(string imgpath, Vector2? TopLeft = null, Vector2? BottomRight = null)
        {
            return new Subtexture(ContentManager.Get<Texture2D>(imgpath), TopLeft ?? Vector2.Zero, BottomRight ?? Vector2.One);
        }

        /// <summary>
        /// 插入控件
        /// </summary>
        /// <param name="targetWidget">目标控件</param>
        /// <param name="addWidget">要添加的控件</param>
        /// <param name="order">添加顺序(0为目标控件前面1为后面)</param>
        public void InsertWidget(Widget targetWidget, Widget addWidget, int order)
        {
            //限制变量在0和1
            int num = (order < 0) ? 0 : (order > 1) ? 1 : order;
            //获取目标控件的容器
            ContainerWidget container = targetWidget.ParentWidget;
            //获取目标控件在容器里的索引
            int widgetIndex = container.Children.IndexOf(targetWidget);
            container.Children.Insert(widgetIndex + num, addWidget);
        }

        /// <summary>
        /// 替换控件
        /// </summary>
        /// <param name="targetWidget">目标控件</param>
        /// <param name="replaceWidget">要替换的新控件</param>
        public void ReplaceWidget(Widget targetWidget, Widget replaceWidget)
        {
            //获取目标控件的容器
            ContainerWidget container = targetWidget.ParentWidget;
            //获取目标控件容器WidgetList
            WidgetsList children = container.Children;
            //获取目标控件在容器里的索引
            int widgetIndex = children.IndexOf(targetWidget);
            //隐藏目标控件
            targetWidget.IsVisible = false;
            children.Insert(widgetIndex, replaceWidget);
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            //进入游戏初始化
            m_componentPlayer = Entity.FindComponent<ComponentPlayer>(throwOnError: true);
            m_componentGui = m_componentPlayer.ComponentGui;
            ContainerWidget guiWidget = m_componentPlayer.GuiWidget;
            GameWidget gameWidget = m_componentPlayer.GameWidget;
            //获取各容器
            controlsContainer = guiWidget.Children.Find<CanvasWidget>("ControlsContainer");
            leftContainer = guiWidget.Children.Find<StackPanelWidget>("LeftControlsContainer");
            lightContainer = guiWidget.Children.Find<StackPanelWidget>("RightControlsContainer");
            moreContents = guiWidget.Children.Find<StackPanelWidget>("MoreContents");
            //利用玩家血槽获取父容器
            statusContents = (StackPanelWidget)guiWidget.Children.Find<ValueBarWidget>("HealthBar").ParentWidget;
            try
            {
                //查找界面元素是否存在(防止玩家复活后控件重复添加)
                tool = gameWidget.Children.Find<BitmapButtonWidget>("toolButton");
                staminaBar = gameWidget.Children.Find<ValueBarWidget>("StaminaBar");
            }
            catch
            {
                //如果元素不存在抛出异常将控件添加到容器
                //属性按钮
                attribute.AddChildren(attribute_one); //给空白按钮添加图标
                leftContainer.AddChildren(attribute);
                //耐力条
                InsertWidget(gameWidget.Children.Find("TemperatureBar"), staminaBar, 0);
                //工具按钮
                moreContents.AddChildren(tool);
            }
            //获取游戏难度
            gameMode = m_componentGui.m_subsystemGameInfo.WorldSettings.GameMode;
            staminaBar.IsVisible = gameMode != GameMode.Creative;
        }


        public void Update(float dt)
        {
            if (tool.IsClicked)
            {
                try
                {
                    m_componentPlayer.ComponentGui.ModalPanelWidget = new ToolWidget(m_componentPlayer);
                }
                catch (Exception ex)
                {
                    Log.Warning("ShowBulletin失败。原因: " + ex.Message);
                }
            }
            if (attribute.IsClicked)
            {
                m_componentPlayer.ComponentGui.ModalPanelWidget = new HYKJClothingWidget(m_componentPlayer);
            }
            //耐力
            if (staminaBar.IsVisible)
            {
                staminaBar.Value = m_componentPlayer.ComponentVitalStats.Stamina;
            }
        }

        //定义新的按钮：工具按钮
        private BitmapButtonWidget tool = new BitmapButtonWidget
        {
            Name = "toolButton",
            Size = new Vector2(68f, 64f),
            NormalSubtexture = ToSubtexture("HYKJTextures/Button/tool"),
            ClickedSubtexture = ToSubtexture("HYKJTextures/Button/tool_Pressed"),
            Text = "",
            Margin = new Vector2(4, 0),
        };
        //定义新的按钮：属性按钮
        private BitmapButtonWidget attribute = new BitmapButtonWidget
        {
            Name = "attributeButton",
            Size = new Vector2(68f, 64f),
            NormalSubtexture = ToSubtexture("HYKJTextures/Button/BlankLightButton_1"),
            ClickedSubtexture = ToSubtexture("HYKJTextures/Button/BlankLightButton_Pressed_1"),
            Text = "",
        };
        //属性图像
        private RectangleWidget attribute_one = new RectangleWidget
        {
            Size = new Vector2(45f, 45f),
            OutlineColor = new Color(0, 0, 0, 0),
            FillColor = new Color(255, 255, 255),
            Subtexture = ToSubtexture("HYKJTextures/Button/attribute"),
            //居中
            HorizontalAlignment = WidgetAlignment.Center,
            VerticalAlignment = WidgetAlignment.Center,
        };
        //耐力
        private ValueBarWidget staminaBar = new ValueBarWidget
        {
            Name = "StaminaBar",
            BarSize = new Vector2(16f, 16f), //条大小
            LitBarColor2 = new Color(220, 241, 37),
            LitBarColor = new Color(190, 125, 0),
            UnlitBarColor = new Color(0, 0, 0, 255),
            //条显示图标
            BarSubtexture = ToSubtexture("HYKJTextures/Button/Stamina"),
            BarsCount = 10, //条图标个数
        };
    }
}