using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using Engine.Graphics;
using Game;
using System.Runtime.CompilerServices;


namespace HYKJ
{
    public class ComponentNewGui : Component, IUpdateable
    {
        public ComponentPlayer m_componentPlayer;
        public ComponentGui m_componentGui;
        public SubsystemTime m_subsystemTime;
        public SubsystemAudio m_subsystemAudio;
        public GameMode gameMode;

        //初始化选中疾跑按钮
        private bool isSprintChecked = false;
        //private bool isPlayerChecked = false;
        //屏幕总控件容器
        public CanvasWidget controlsContainer;
        //屏幕左侧控件容器
        public StackPanelWidget leftContainer;
        //屏幕右侧控件容器
        public StackPanelWidget lightContainer;
        //屏幕右侧最顶隐藏容器
        public StackPanelWidget moreContents;
        //玩家生存模式状态栏容器 
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
                sprint = gameWidget.Children.Find<BitmapButtonWidget>("SprintButton");
                staminaBar = gameWidget.Children.Find<ValueBarWidget>("StaminaBar");
            }
            catch
            {
                //如果元素不存在抛出异常将控件添加到容器
                //疾跑按钮
                sprint.AddChildren(sprintImg); //给空白按钮添加图标
                lightContainer.AddChildren(sprint);
                //属性按钮
                attribute.AddChildren(attribute_one); //给空白按钮添加图标
                leftContainer.AddChildren(attribute);
                //耐力条
                InsertWidget(gameWidget.Children.Find("TemperatureBar"), staminaBar, 1);
                //水分条
                InsertWidget(gameWidget.Children.Find("TemperatureBar"), Water, 0);
                //工具按钮
                moreContents.AddChildren(tool);
            }
            //获取游戏难度
            gameMode = m_componentGui.m_subsystemGameInfo.WorldSettings.GameMode;
            //控件是否显示
            Water.IsVisible = gameMode != GameMode.Creative;
            staminaBar.IsVisible = gameMode != GameMode.Creative;
        }


        public void Update(float dt)
        {
            ComponentThirst componentThirst = m_componentPlayer.Entity.FindComponent<ComponentThirst>(true);

            if (tool.IsClicked)
            {
                m_componentPlayer.ComponentGui.ModalPanelWidget = new ToolWidget(m_componentPlayer);
                DialogsManager.ShowDialog(null, new MessageDialog("提示", "第一次打开没有ui，为BUG现象\n(不知道咋修)退出重进存档即可\n祝你游玩愉快", LanguageControl.Ok, null, null));
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
            if (Water.IsVisible)
            {
                Water.Value = componentThirst.Water;
            }
            //跑步
            if (sprint.IsClicked)
            {
                isSprintChecked = (isSprintChecked) ? false : true;
                if (isSprintChecked)
                {
                    m_componentPlayer.ComponentLocomotion.WalkSpeed += 1.0f;
                    m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(fName, 1), Color.White, false, false);
                }
                else
                {
                    m_componentPlayer.ComponentLocomotion.WalkSpeed -= 1.0f;
                    m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(fName, 2), Color.White, false, false);
                }
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
        //跑步
        private BitmapButtonWidget sprint = new BitmapButtonWidget
        {
            Name = "SprintButton",
            Size = new Vector2(64f, 64f),
            HorizontalAlignment = WidgetAlignment.Far,
            NormalSubtexture = ToSubtexture("HYKJTextures/Button/BlankLightButton"),
            ClickedSubtexture = ToSubtexture("HYKJTextures/Button/BlankLightButton_Pressed"),
            Margin = new Vector2(0, 3), //外边距
            //开启自动选中
            IsAutoCheckingEnabled = true,
        };
        //跑步
        private RectangleWidget sprintImg = new RectangleWidget
        {
            Size = new Vector2(40f, 40f),
            OutlineColor = new Color(0, 0, 0, 0),
            FillColor = new Color(255, 255, 255),
            Subtexture = ToSubtexture("HYKJTextures/Button/Sprint"),
            //居中
            HorizontalAlignment = WidgetAlignment.Center,
            VerticalAlignment = WidgetAlignment.Center,
        };
        //耐力
        private ValueBarWidget staminaBar = new ValueBarWidget
        {
            Name = "StaminaBar",
            BarSize = new Vector2(16f, 16f), //条大小
            LitBarColor = new Color(220, 241, 37),
            UnlitBarColor = new Color(0, 0, 0, 255),
            //条显示图标
            BarSubtexture = ToSubtexture("HYKJTextures/Button/Stamina"),
            BarsCount = 10, //条图标个数
            BarBlending = false,
            HalfBars = true,
            TextureLinearFilter = true,
        };
        //水分
        private ValueBarWidget Water = new ValueBarWidget
        {
            Name = "Water",
            BarSize = new Vector2(16f, 16f), //条大小
            LitBarColor = new Color(0, 149, 255),
            UnlitBarColor = new Color(0, 0, 0, 255),
            //条显示图标
            BarSubtexture = ToSubtexture("HYKJTextures/Button/Water"),
            BarsCount = 10, //条图标个数
            BarBlending = false,
            HalfBars = true,
            TextureLinearFilter = true,
        };
    }
}
/*public virtual void UpdateThirst() 
{
    //用于更新口渴值
    //此为简易例子请勿直接使用
    float num3 = 1f / MathF.Max(m_componentPlayer.ComponentLevel.SpeedFactor, 0.75f);
    bool flag3 = m_subsystemTime.PeriodicGameTimeEvent(240.0, 13.0) && !m_componentPlayer.ComponentSickness.IsSick;
    Thirst -= 0.01f * m_componentPlayer.ComponentLocomotion.LastJumpOrder * num3;

    if (Thirst < 0.7f && ((m_lastThirst >= 0.7f) | flag3)) {
        m_componentGui.DisplaySmallMessage(LanguageControl.Get(fName, 8), Color.White, false, false);

    } else if (Thirst < 0.4f && ((m_lastThirst >= 0.4f) | flag3)) {
        m_componentGui.DisplaySmallMessage(LanguageControl.Get(fName, 9), Color.Yellow, false, false);
        //步行速度变慢
        m_componentPlayer.ComponentLocomotion.WalkSpeed -= 0.6f;

    } else if (Thirst < 0.1f && ((m_lastThirst >= 0.1f) | flag3)) {
        m_componentGui.DisplaySmallMessage(LanguageControl.Get(fName, 10), Color.Red, false, false);
    }
    //当口渴值小于0.1时开始扣血
    if (Thirst < 0.1f && m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0)) {
        m_componentPlayer.ComponentHealth.Injure(0.5f, null, ignoreInvulnerability: false, LanguageControl.Get(fName, 12));
    }
    if (m_componentPlayer.ComponentBody.ImmersionFactor > 0.2f && m_componentPlayer.ComponentBody.ImmersionFluidBlock is WaterBlock && Thirst < 1f) {
        //如果玩家进入水中且口渴值小于1
        Thirst += 0.05f;
        m_componentGui.DisplaySmallMessage(LanguageControl.Get(fName, 11), Color.White, false, false);
    }
    m_lastThirst = Thirst;
}*/