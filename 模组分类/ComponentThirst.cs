using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    public class ComponentThirst : Component, IUpdateable
    {
        private float m_water;  //私有口渴值

        public float Water  //可调用口渴值（get获取值，set设置值）
        {
            get { return m_water; }
            set { m_water = MathUtils.Saturate(value); } //Saturate函数作用是将值限定在0到1
        }

        public ComponentPlayer m_componentPlayer;   //玩家对象

        public ValueBarWidget WaterBarWidget    //玩家界面的口渴条
        {
            get;
            set;
        }

        public UpdateOrder UpdateOrder => UpdateOrder.Default;  //IUpdateable更新接口成员

        public void Update(float dt)    //更新接口的更新方法，每帧执行
        {
            WaterBarWidget.Value = Water;   //口渴条的值（界面显示的格数）赋值为Water值（后台数据）
            Test(); //测试效果
        }

        //装载方法，进存档时执行一次
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            m_componentPlayer = Entity.FindComponent<ComponentPlayer>(throwOnError: true);  //获取玩家对象
            Water = valuesDictionary.GetValue<float>("Water");  //从存档中project.xml文件的特定位置获取Water值
            WaterBarWidget = new ValueBarWidget();  //创建口渴条对象
            WaterBarWidget.LayoutDirection = LayoutDirection.Horizontal;    //布局方向：水平（口渴条水平摆放）
            WaterBarWidget.HorizontalAlignment = WidgetAlignment.Center;    //位置调整：水平居中
            WaterBarWidget.VerticalAlignment = WidgetAlignment.Far;  //位置调整:垂直方向远离（此时口渴条在中间底部）
            WaterBarWidget.Margin = new Vector2(0, 100);  //位置调整：水平偏移0像素，向上偏移100像素（定位口渴条在中偏下方）
            WaterBarWidget.BarsCount = 10;  //格子数：10（口渴条由10格组成）
            WaterBarWidget.BarSize = new Vector2(10, 12);   //格子大小：长10像素，宽12像素
            WaterBarWidget.HalfBars = true;   //半格显示：开启（半格为最小刻度）
            WaterBarWidget.LitBarColor = new Color(10, 10, 200, 255); //显示的颜色：rgba分为10, 10, 200, 255（最后一个为alpha，控制透明度）
            WaterBarWidget.BarSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/ProgressBar");    //口渴条单格材质
            WaterBarWidget.IsVisible = true;    //是否可见：是（值为false则隐藏）
            WaterBarWidget.Value = Water;   //口渴条的值设置为Water的值（取值范围0-1）
            ContainerWidget guiWidget = m_componentPlayer.GuiWidget;    //获取玩家gui界面
            guiWidget.Children.Find<CanvasWidget>("ControlsContainer").Children.Add(WaterBarWidget);   //将口渴条添加到玩家gui界面中的ControlsContainer板块
        }

        //保存方法，点退出按钮或关闭游戏窗口时执行一次，且理论上每120秒执行一次
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
        {
            valuesDictionary.SetValue("Water", Water);  //将Water值保存到存档中project.xml文件的特定位置
        }

        //测试方法（放在Update方法中）
        public void Test()
        {
            if (m_componentPlayer.ComponentBody.ImmersionDepth > 0.4f)  //当玩家泡在水(或熔浆）里时
            {
                Water = Water + 0.05f;  //Water值每次增加0.05
            }
            if (m_componentPlayer.ComponentInput.PlayerInput.Move.Length() > 0)  //当玩家移动时
            {
                Water = Water - 0.001f;  //Water值每次减少0.001
            }
            if (Water >= 0.2f && Water <= 0.3f) //当Water值处在0.2到0.3之间时
            {
                m_componentPlayer.ComponentGui.DisplaySmallMessage("好渴啊啊啊", Color.Blue, false, false);    //弹出口渴提示
            }
            if (Water <= 0)  //当Water值不大于0时
            {
                m_componentPlayer.ComponentSleep.Sleep(true);   //玩家直接入睡
                Water = 1f;  //随后把Water值设置为1（条满状态）
            }
        }
        /*
          另，外部调用water的方式如下，由玩家对象componentPlayer出发：
          ComponentThirst componentThirst = componentPlayer.Entity.FindComponent<ComponentThirst>(true);
          然后即可获取或修改componentThirst.Water
        */
    }
}
