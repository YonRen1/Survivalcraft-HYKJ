using System.Linq;
using Game;
using Engine;
using Random = Game.Random;
using Engine.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;
using TemplatesDatabase;
using System.Text;
using GameEntitySystem;

namespace HYKJ
{
    public class HYKJModLoader : ModLoader
    {
        public static ReadOnlyList<string> Categories => new(m_categories);

        public static List<string> m_categories = [];

        public ComponentGui m_componentGui;

        public static List<WaterData> m_waterDatas = new List<WaterData>();

        public XElement items;

        private BevelledButtonWidget modButton;

        private BevelledButtonWidget hykjButton;

        private SubsystemTimeOfDay m_subsystemTimeOfDay;

        public SubsystemParticles m_subsystemParticles;

        public static ReadOnlyList<WaterData> WaterDatas
        {
            get
            {
                return new ReadOnlyList<WaterData>(HYKJModLoader.m_waterDatas);
            }
        }

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
            ModsManager.RegisterHook("OnProjectLoaded", this);//当Project被加载时执行
            ModsManager.RegisterHook("OnMinerDig", this);//挖掘时执行
            ModsManager.RegisterHook("ClothingProcessSlotItems", this);
            this.items = this.GetHwValue();
            Log.Warning("[HYKJ]:ModLoader正常加载...");

        }
        /// <summary>
        /// 当Project被加载时执行
        /// </summary>
        /// <param name="project"></param>
        public override void OnProjectLoaded(Project project)
        {
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
            Log.Warning("HYKJ:主界面正常初始化");
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

        /// <summary>
        /// 当人物挖掘时执行
        /// </summary>
        /// <param name="miner"></param>
        /// <param name="raycastResult"></param>
        /// <returns></returns>
        public override void OnMinerDig(ComponentMiner miner, TerrainRaycastResult raycastResult, ref float DigProgress, out bool Digged)
        {
            //如果挖掘者为玩家且玩家空手挖掘
            if (miner.ComponentPlayer != null && Terrain.ExtractContents(miner.ActiveBlockValue) == 0)
            {
                //玩家受伤
                miner.ComponentPlayer.ComponentHealth.Injure(0.006f, null, true, LanguageControl.Get(fName, "1"));
                //空手挖掘5%的概率喷射红粒子
                if (new Random().Float(0, 1f) <= 0.05f)
                {
                    m_subsystemParticles.AddParticleSystem(new FireworksParticleSystem(raycastResult.HitPoint(0), Color.DarkRed, FireworksBlock.Shape.SmallBurst, 0.3f, 0.1f));
                }
                //血量小于20%时弹出提示
                if (miner.ComponentPlayer.ComponentHealth.Health < 0.2f)
                {
                    miner.ComponentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(fName, "2"), Color.White, false, false);
                }
            }
            Digged = DigProgress >= 1f;
        }

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
            // 遍历配置文件中的所有元素（每个元素对应一个物品的水分配置）
            using (IEnumerator<XElement> enumerator = this.items.Elements().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    XElement currentElement = enumerator.Current;
                    // 解析物品标识值
                    string resultAttr = currentElement.Attribute("Result")?.Value;
                    int blockValue = CraftingRecipesManager.DecodeResult(resultAttr);

                    // 解析水分值
                    float attributeValue = float.Parse(currentElement.Attribute("WaterValue")?.Value ?? "0");
                    // 解析致病概率
                    float attributeValue2 = float.Parse(currentElement.Attribute("SicknessProbability")?.Value ?? "0");
                    // 创建水分数据对象并赋值
                    WaterData waterData = new WaterData
                    {
                        BlockValue = blockValue,
                        WaterValue = attributeValue,
                        SicknessProbability = attributeValue2
                    };

                    if (attributeValue != 0f)
                    {
                        HYKJModLoader.m_waterDatas.Add(waterData);
                    }
                }
            }
        }

        /// <summary>
        /// 处理装备栏物品的逻辑（主要用于处理饮水物品）
        /// </summary>
        /// <param name="componentPlayer">玩家组件</param>
        /// <param name="block">物品对应的方块</param>
        /// <param name="slotIndex">物品所在槽位索引</param>
        /// <param name="value">物品标识值</param>
        /// <param name="count">物品数量</param>
        /// <returns>是否阻止默认处理（此处固定返回false，允许默认逻辑）</returns>
        public override bool ClothingProcessSlotItems(ComponentPlayer componentPlayer, Block block, int slotIndex, int value, int count)
        {
            IEnumerator<XElement> enumerator = this.items.Elements().GetEnumerator();
            XElement currentElement = enumerator.Current;
            // 根据物品标识值查找对应的水分数据
            WaterData waterData = HYKJModLoader.m_waterDatas.Find((WaterData w) => w.BlockValue == value);
            // 获取玩家的饮水组件
            ComponentWaterDrinking componentWaterDrinking = componentPlayer.Entity.FindComponent<ComponentWaterDrinking>();
            // 解析水分值
            float attributeValue = float.Parse(currentElement.Attribute("WaterValue")?.Value ?? "0");

            // 若找到水分数据且玩家有饮水组件，则执行饮水逻辑
            if (waterData != null && componentWaterDrinking != null)
            {
                if (block.GetNutritionalValue(value) == 0f)
                {
                    componentPlayer.ComponentMiner.Inventory.RemoveSlotItems(slotIndex, 1);

                    if (block is BucketBlock)
                    {
                        componentPlayer.ComponentMiner.Inventory.AddSlotItems(slotIndex, 90, 1);
                    }*/等待完善，，，，，，，，退回木杯
                }
                //判断增加的水分值是否大于0
                else if (attributeValue > 0f)
                {
                    // 增加玩家的饮水值
                    componentWaterDrinking.Water += waterData.WaterValue;
                    // 播放饮水音效
                    componentPlayer.m_subsystemAudio.PlaySound("Audio/Water/Drinking", 1f, 0f, componentPlayer.ComponentBody.Position, 1f, true);
                }
                // 计算过量饮水的伤害值（超出1的部分减半，限制在0-0.25之间）
                float num = MathUtils.Clamp((componentWaterDrinking.Water + waterData.WaterValue - 1f) / 2f, 0f, 0.25f);
                // 增加玩家的饮水值
                componentWaterDrinking.Water += waterData.WaterValue;

                if (num > 0f)
                {
                    componentPlayer.ComponentHealth.Injure(num, null, false, LanguageControl.Get(fName, "3"));

                    // 若玩家当前无疾病且物品无致病概率，则触发疾病
                    if (componentPlayer.ComponentSickness.m_sicknessDuration == 0f && block.GetSicknessProbability(value) <= 0f)
                    {
                        componentPlayer.ComponentSickness.StartSickness(); // 开始生病
                        componentPlayer.ComponentSickness.m_sicknessDuration = 0.5f; // 疾病持续时间设为0.5
                    }

                    ComponentVitalStats expr_14D = componentPlayer.ComponentVitalStats;
                    expr_14D.Food = expr_14D.Food - (num / 2f + block.GetNutritionalValue(value) * 0.05f);
                }
            }
            return false;
        }

        /// <summary>
        /// 加载饮水相关的配置数据
        /// </summary>
        public XElement GetHwValue()
        {
            XElement items = null;
            this.Entity.GetFiles(".Water", (filename, stream) =>
            {
                using (stream)
                {
                    items = XDocument.Load(stream).Root;
                }
            });
            return items;
        }
    }
}