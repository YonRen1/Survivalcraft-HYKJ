using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Engine;
using Game;
using XmlUtilities;

namespace HYKJ
{
	public class BetterDrinkingModLoader : ModLoader
	{
		public static ReadOnlyList<WaterData> WaterDatas
		{
			get
			{
				return new ReadOnlyList<WaterData>(BetterDrinkingModLoader.m_waterDatas);
			}
		}
		
		public override void __ModInitialize()
		{
			ModsManager.RegisterHook("GuiUpdate", this);
			ModsManager.RegisterHook("ClothingProcessSlotItems", this);
			this.items = this.GetHwValue();
			ModsManager.RegisterHook("BlocksInitalized", this);
			ModsManager.RegisterHook("ClothingWidgetOpen", this);
		}
		
		public override void ClothingWidgetOpen(ComponentGui componentGui, ClothingWidget clothingWidget)
		{
			CanvasWidget expr_05 = new CanvasWidget();
			expr_05.set_Name("WaterBottonC");
			expr_05.set_Size(new Vector2(80f, 80f));
			CanvasWidget canvasWidget = expr_05;
			clothingWidget.SetWidgetPosition(canvasWidget, new Vector2?(new Vector2(24f, 64f)));
			StackPanelWidget expr_46 = new StackPanelWidget();
			expr_46.set_Direction(1);
			expr_46.set_VerticalAlignment(1);
			StackPanelWidget stackPanelWidget = expr_46;
			BevelledButtonWidget expr_5A = new BevelledButtonWidget();
			expr_5A.set_Name("WaterBotton");
			expr_5A.set_Text(string.Empty);
			expr_5A.set_Style(ContentManager.Get<XElement>("Styles/ButtonStyle_70x60", null));
			BevelledButtonWidget bevelledButtonWidget = expr_5A;
			RectangleWidget expr_87 = new RectangleWidget();
			expr_87.set_Name("WaterIcon");
			expr_87.set_Size(new Vector2(40.01f));
			expr_87.set_OutlineColor(Color.Transparent);
			expr_87.set_FillColor(Color.White);
			expr_87.set_Subtexture(ContentManager.Get<Subtexture>("BetterDrinking/Water", null));
			expr_87.set_ColorTransform(Color.InkBlue);
			expr_87.set_HorizontalAlignment(1);
			expr_87.set_VerticalAlignment(1);
			RectangleWidget rectangleWidget = expr_87;
			RectangleWidget expr_E8 = new RectangleWidget();
			expr_E8.set_Size(new Vector2(40f));
			expr_E8.set_OutlineColor(Color.Transparent);
			expr_E8.set_FillColor(Color.White);
			expr_E8.set_Subtexture(ContentManager.Get<Subtexture>("BetterDrinking/Water", null));
			expr_E8.set_ColorTransform(Color.White);
			expr_E8.set_HorizontalAlignment(1);
			expr_E8.set_VerticalAlignment(1);
			RectangleWidget rectangleWidget2 = expr_E8;
			bevelledButtonWidget.Children.Add(rectangleWidget2);
			bevelledButtonWidget.Children.Add(rectangleWidget);
			stackPanelWidget.Children.Add(bevelledButtonWidget);
			canvasWidget.Children.Add(stackPanelWidget);
			clothingWidget.Children.Add(canvasWidget);
		}

		public override void GuiUpdate(ComponentGui componentGui)
		{
			if (componentGui.m_componentPlayer.get_ComponentHealth().get_Health() > 0f)
			{
				ValueBarWidget valueBarWidget = componentGui.get_ControlsContainerWidget().Children.Find<ValueBarWidget>("WaterL", true);
				if (valueBarWidget != null)
				{
					valueBarWidget.set_Value(componentGui.get_Entity().FindComponent<ComponentBetterDrinking>().Water);
				}
				CanvasWidget canvasWidget = componentGui.get_ControlsContainerWidget().Children.Find<CanvasWidget>("BWaterBarList", true);
				if (canvasWidget != null)
				{
					GameMode arg_71_0 = componentGui.m_subsystemGameInfo.get_WorldSettings().GameMode;
					canvasWidget.set_IsVisible(true);
				}
			}
		}

		public override bool ClothingProcessSlotItems(ComponentPlayer componentPlayer, Block block, int slotIndex, int value, int count)
		{
			new Random();
			WaterData waterData = BetterDrinkingModLoader.m_waterDatas.Find((WaterData w) => w.BlockValue == value);
			ComponentBetterDrinking componentBetterDrinking = componentPlayer.get_Entity().FindComponent<ComponentBetterDrinking>();
			if (waterData != null && componentBetterDrinking != null)
			{
				if (block.GetNutritionalValue(value) == 0f)
				{
					componentPlayer.get_ComponentMiner().get_Inventory().RemoveSlotItems(slotIndex, 1);
					if (block is BucketBlock)
					{
						componentPlayer.get_ComponentMiner().get_Inventory().AddSlotItems(slotIndex, 90, 1);
					}
				}
				float num = MathUtils.Clamp((componentBetterDrinking.Water + waterData.WaterValue - 1f) / 2f, 0f, 0.25f);
				componentBetterDrinking.Water += waterData.WaterValue;
				componentPlayer.m_subsystemAudio.PlaySound("Audio/Sinking", 1f, 0f, componentPlayer.get_ComponentBody().get_Position(), 1f, true);
				if (num > 0f)
				{
					componentPlayer.get_ComponentHealth().Injure(num, null, false, "\n你.....撑死了?\nYou....? Support to death");
					if (componentPlayer.get_ComponentSickness().m_sicknessDuration == 0f && block.GetSicknessProbability(value) <= 0f)
					{
						componentPlayer.get_ComponentSickness().StartSickness();
						componentPlayer.get_ComponentSickness().m_sicknessDuration = 0.5f;
					}
					ComponentVitalStats expr_14D = componentPlayer.get_ComponentVitalStats();
					expr_14D.set_Food(expr_14D.get_Food() - (num / 2f + block.GetNutritionalValue(value) * 0.05f));
				}
			}
			return false;
		}

		public override void BlocksInitalized()
		{
			using (IEnumerator<XElement> enumerator = this.items.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement expr_19 = enumerator.get_Current();
					int blockValue = CraftingRecipesManager.DecodeResult(XmlUtils.GetAttributeValue<string>(expr_19, "Result"));
					float attributeValue = XmlUtils.GetAttributeValue<float>(expr_19, "WaterValue");
					float attributeValue2 = XmlUtils.GetAttributeValue<float>(expr_19, "SicknessProbability");
					WaterData waterData = new WaterData
					{
						BlockValue = blockValue,
						WaterValue = attributeValue,
						SicknessProbability = attributeValue2
					};
					if (attributeValue != 0f)
					{
						BetterDrinkingModLoader.m_waterDatas.Add(waterData);
					}
				}
			}
		}

		public XElement GetHwValue()
		{
			XElement items = null;
			this.Entity.GetFiles(".bwter", delegate(string filename, Stream stream)
			{
				items = XmlUtils.LoadXmlFromStream(stream, Encoding.get_UTF8(), true);
				stream.Close();
			});
			if (items == null)
			{
				items.Add(ContentManager.Get<XElement>("BetterDrinking", null));
			}
			else
			{
				items = ContentManager.Get<XElement>("BetterDrinking", null);
			}
			return items;
		}

		public static List<WaterData> m_waterDatas = new List<WaterData>();

		public XElement items;
	}
}
