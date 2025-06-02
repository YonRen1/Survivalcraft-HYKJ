using System;
using Engine;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace HYKJ
{
	public class ComponentBetterDrinking : Component, IUpdateable
	{
		public float Water
		{
			get
			{
				return this.m_water;
			}
			set
			{
				this.m_water = MathUtils.Saturate(value);
			}
		}

		public UpdateOrder UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		public void Update(float dt)
		{
			if (this.m_componentPlayer.get_ComponentHealth().get_Health() > 0f)//玩家生命不等于0时
			{
				this.UpdateWater();//更新水分
				//获取当前界面
				Widget modalPanelWidget = this.m_componentPlayer.get_ComponentGui().get_ModalPanelWidget();
				if (modalPanelWidget is ClothingWidget)//当前界面如果是衣服界面
				{
				    //查找饮水按钮
					BevelledButtonWidget bevelledButtonWidget = ((ClothingWidget)modalPanelWidget).Children.Find<BevelledButtonWidget>("WaterBotton", true);
					Widget arg_2A5_0 = bevelledButtonWidget.Children.Find<RectangleWidget>("WaterIcon", true);
					//获取脚下方块
					Vector3 position = this.m_componentPlayer.get_ComponentBody().get_Position();
					Point3 point;
					//下偏移一格
					point..ctor(Terrain.ToCell(position.X), Terrain.ToCell(position.Y) - 1, Terrain.ToCell(position.Z));
					//检测周围是否有水
					bool flag = this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X - 1, point.Y, point.Z + 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X + 1, point.Y, point.Z - 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X - 1, point.Y, point.Z - 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X + 1, point.Y, point.Z + 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X - 1, point.Y, point.Z) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X, point.Y, point.Z - 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X + 1, point.Y, point.Z) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X, point.Y, point.Z + 1) == 18 || this.m_componentPlayer.m_subsystemTerrain.get_Terrain().GetCellContents(point.X, point.Y, point.Z) == 18;
					arg_2A5_0.set_ColorTransform((this.m_componentPlayer.get_ComponentBody().get_ImmersionFactor() > 0.25f) ? (Color.InkBlue * this.m_componentPlayer.get_ComponentBody().get_ImmersionFactor()) : ((this.m_componentPlayer.get_ComponentBody().get_IsSneaking() & flag) ? Color.InkBlue : Color.Gray));
					if (bevelledButtonWidget.get_IsClicked())//当按钮被点击时
					{
					    //增加饮水
						float num = this.m_random.Float(0.05f, 0.15f) * MathUtils.Max(this.m_componentPlayer.get_ComponentBody().get_ImmersionFactor(), 0.25f);
						//播放音效
						this.m_componentPlayer.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, position, 0.5f, true);
						//如果超过1
						float num2 = MathUtils.Clamp((this.Water + num - 1f) / 2f, 0f, 0.25f);
						this.Water += num;
						if (this.m_random.Bool(0f))//概率生病
						{
							if (!this.m_componentPlayer.get_ComponentSickness().get_IsSick() && this.m_componentPlayer.get_ComponentSickness().m_sicknessDuration == 0f)
							{
								this.m_componentPlayer.get_ComponentSickness().StartSickness();//启动生病状态
								this.m_componentPlayer.get_ComponentSickness().m_sicknessDuration = 5f;//持续5秒
							}
							else
							{
								this.m_componentPlayer.get_ComponentSickness().m_sicknessDuration += 5f;//再加5秒
							}
						}
						if (num2 > 0f)
						{
						    //提示
							this.m_componentPlayer.get_ComponentHealth().Injure(num2 / 2f, null, false, "\n你.....撑死了?\nYou....? Support to death");
							//没有生病则启动段时间生病
							if (this.m_componentPlayer.get_ComponentSickness().m_sicknessDuration == 0f)
							{
								this.m_componentPlayer.get_ComponentSickness().StartSickness();
								this.m_componentPlayer.get_ComponentSickness().m_sicknessDuration = 0.5f;
							}
							ComponentVitalStats expr_42F = this.m_componentPlayer.get_ComponentVitalStats();
							expr_42F.set_Food(expr_42F.get_Food() - num2);//减少食物
						}
					}
					if ((this.m_componentPlayer.get_ComponentBody().get_IsSneaking() & flag) || this.m_componentPlayer.get_ComponentBody().get_ImmersionFactor() > 0f)
					{
						bevelledButtonWidget.set_IsEnabled(true);
						return;
					}
					bevelledButtonWidget.set_IsEnabled(false);
				}
			}
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.get_Project().FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.get_Project().FindSubsystem<SubsystemTime>(true);
			this.m_componentPlayer = base.get_Entity().FindComponent<ComponentPlayer>(true);
			this.m_subsystemAudio = base.get_Project().FindSubsystem<SubsystemAudio>(true);
			this.m_componentCreature = base.get_Entity().FindComponent<ComponentCreature>(true);
			this.Water = valuesDictionary.GetValue<float>("Water");
			this.m_lastWater = this.Water;
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<float>("Water", this.Water);
		}

		public override void OnEntityAdded()
		{
			if (this.m_componentPlayer.get_ComponentGui() != null)
			{
				ValueBarWidget expr_15 = new ValueBarWidget();
				expr_15.set_Name("WaterL");
				expr_15.set_LayoutDirection(0);
				expr_15.set_VerticalAlignment(1);
				expr_15.set_BarsCount(10);
				expr_15.set_BarBlending(false);
				expr_15.set_HalfBars(true);
				expr_15.set_LitBarColor(Color.SkyBlue);
				expr_15.set_UnlitBarColor(Color.Transparent);
				expr_15.set_BarSize(new Vector2(9f, 15f));
				expr_15.set_Spacing(1.25f);
				expr_15.set_BarSubtexture(ContentManager.Get<Subtexture>("BetterDrinking/WaterL", null));
				expr_15.set_TextureLinearFilter(false);
				expr_15.set_Value(0.56f);
				ValueBarWidget valueBarWidget = expr_15;
				ValueBarWidget expr_A3 = new ValueBarWidget();
				expr_A3.set_Name("WaterU");
				expr_A3.set_LayoutDirection(0);
				expr_A3.set_VerticalAlignment(1);
				expr_A3.set_BarsCount(10);
				expr_A3.set_BarBlending(false);
				expr_A3.set_HalfBars(true);
				expr_A3.set_LitBarColor(Color.White);
				expr_A3.set_UnlitBarColor(Color.White);
				expr_A3.set_BarSize(new Vector2(9f, 15f));
				expr_A3.set_Spacing(1.25f);
				expr_A3.set_BarSubtexture(ContentManager.Get<Subtexture>("BetterDrinking/WaterU", null));
				expr_A3.set_TextureLinearFilter(true);
				ValueBarWidget valueBarWidget2 = expr_A3;
				CanvasWidget expr_126 = new CanvasWidget();
				expr_126.set_Name("BWaterBarList");
				expr_126.set_VerticalAlignment(2);
				expr_126.set_HorizontalAlignment(1);
				expr_126.set_Margin(new Vector2(0f, 90f));
				CanvasWidget canvasWidget = expr_126;
				CanvasWidget canvasWidget2 = new CanvasWidget();
				StackPanelWidget expr_160 = new StackPanelWidget();
				expr_160.set_Direction(0);
				StackPanelWidget stackPanelWidget = expr_160;
				canvasWidget2.Children.Add(valueBarWidget);
				canvasWidget2.Children.Add(valueBarWidget2);
				WidgetsList arg_1A2_0 = stackPanelWidget.Children;
				CanvasWidget expr_18D = new CanvasWidget();
				expr_18D.set_Size(new Vector2(300f, 0f));
				arg_1A2_0.Add(expr_18D);
				WidgetsList arg_1C8_0 = stackPanelWidget.Children;
				CanvasWidget expr_1B3 = new CanvasWidget();
				expr_1B3.set_Size(new Vector2(0f, 0f));
				arg_1C8_0.Add(expr_1B3);
				stackPanelWidget.Children.Add(canvasWidget2);
				canvasWidget.Children.Add(stackPanelWidget);
				this.m_componentPlayer.get_ComponentGui().get_ControlsContainerWidget().Children.Add(canvasWidget);
			}
		}

		public override void OnEntityRemoved()
		{
			if (this.m_componentPlayer.get_ComponentGui() != null)
			{
				CanvasWidget canvasWidget = this.m_componentPlayer.get_ComponentGui().get_ControlsContainerWidget().Children.Find<CanvasWidget>("BWaterBarList", true);
				if (canvasWidget != null)
				{
					this.m_componentPlayer.get_ComponentGui().get_ControlsContainerWidget().Children.Remove(canvasWidget);
				}
			}
		}

		public void UpdateWater()
		{
			float gameTimeDelta = this.m_subsystemTime.get_GameTimeDelta();
			GameMode arg_1C_0 = this.m_subsystemGameInfo.get_WorldSettings().GameMode;
			if (this.m_subsystemGameInfo.get_WorldSettings().AreAdventureSurvivalMechanicsEnabled)
			{
				float hungerFactor = this.m_componentPlayer.get_ComponentLevel().get_HungerFactor();
				this.Water -= hungerFactor * gameTimeDelta / 1880f;
				this.Water -= hungerFactor * gameTimeDelta * (this.m_componentPlayer.get_ComponentLocomotion().get_LastWalkOrder().get_HasValue() ? this.m_componentPlayer.get_ComponentLocomotion().get_LastWalkOrder().get_Value().Length() : 0f) / 2440f;
				this.Water -= hungerFactor * this.m_componentPlayer.get_ComponentLocomotion().get_LastJumpOrder() / 1200f;
				if (this.m_componentPlayer.get_ComponentMiner().get_DigCellFace().get_HasValue())
				{
					this.Water -= hungerFactor * gameTimeDelta / 2880f;
				}
				if (!this.m_componentPlayer.get_ComponentSleep().get_IsSleeping())
				{
					if (this.Water <= 0f)
					{
						if (this.m_subsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
						{
							this.m_componentPlayer.get_ComponentHealth().Injure(0.05f, null, false, LanguageControl.Get(ComponentBetterDrinking.fName, 4));
							ComponentVitalStats expr_17B = this.m_componentPlayer.get_ComponentVitalStats();
							expr_17B.set_Stamina(expr_17B.get_Stamina() - 0.1f);
							if (this.m_subsystemGameInfo.get_WorldSettings().GameMode != 1)
							{
								ComponentVitalStats expr_1AA = this.m_componentPlayer.get_ComponentVitalStats();
								expr_1AA.set_Sleep(expr_1AA.get_Sleep() - 0.005f);
							}
							ComponentVitalStats expr_1C6 = this.m_componentPlayer.get_ComponentVitalStats();
							expr_1C6.set_Wetness(expr_1C6.get_Wetness() - 0.2f);
						}
					}
					else if (this.Water < 0.25f && this.m_lastWater >= 0.25f)
					{
						this.m_componentPlayer.get_ComponentGui().DisplaySmallMessage(LanguageControl.Get(ComponentBetterDrinking.fName, 1), Color.Red, true, true);
					}
					else if (this.Water < 0.5f && this.m_lastWater >= 0.5f)
					{
						this.m_componentPlayer.get_ComponentGui().DisplaySmallMessage(LanguageControl.Get(ComponentBetterDrinking.fName, 2), Color.White, true, true);
					}
					else if (this.Water < 0.85f && this.m_lastWater >= 0.85f)
					{
						this.m_componentPlayer.get_ComponentGui().DisplaySmallMessage(LanguageControl.Get(ComponentBetterDrinking.fName, 3), Color.White, true, true);
					}
				}
			}
			else
			{
				this.Water = 0.95f;
			}
			this.m_lastWater = this.Water;
		}

		public SubsystemGameInfo m_subsystemGameInfo;

		public SubsystemTime m_subsystemTime;

		public ComponentPlayer m_componentPlayer;

		public SubsystemAudio m_subsystemAudio;

		public ComponentCreature m_componentCreature;

		public Random m_random = new Random();

		public float m_water;

		public float m_lastWater;

		public static string fName = "ComponentBetterDrinking";
	}
}
