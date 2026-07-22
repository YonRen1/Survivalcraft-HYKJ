using Game;
using System.Xml.Linq;
using Engine;
using System;
using Engine.Media;
using System.Threading.Tasks;

namespace HYKJ
{
    public class HYKJClothingWidget : CanvasWidget
    {
        public StackPanelWidget m_clothingStack;
        public PlayerModelWidget m_innerClothingModelWidget;
        public PlayerModelWidget m_outerClothingModelWidget;
        public ComponentPlayer m_componentPlayer;
        public ComponentPlayer componentPlayer;
        public LabelWidget m_titleLabel;
        public ValueBarWidget m_healthValueBar;
        public ValueBarWidget m_staminaValueBar;
        public ValueBarWidget m_foodValueBar;
        public ValueBarWidget m_sleepValueBar;
        public ValueBarWidget m_experienceValueBar;
        public LabelWidget m_insulationLabel;
        public ValueBarWidget m_temperatureValueBar;

        public HYKJClothingWidget(ComponentPlayer componentPlayer)
        {
            m_componentPlayer = componentPlayer;
            XElement node = ContentManager.Get<XElement>("Widgets/HYKJClothingWidget");
            LoadContents(this, node);
            m_clothingStack = Children.Find<StackPanelWidget>("ClothingStack");
            m_innerClothingModelWidget = Children.Find<PlayerModelWidget>("InnerClothingModel");
            m_outerClothingModelWidget = Children.Find<PlayerModelWidget>("OuterClothingModel");
            m_innerClothingModelWidget.PlayerClass = componentPlayer.PlayerData.PlayerClass;
            m_innerClothingModelWidget.CharacterSkinTexture = m_componentPlayer.ComponentClothing.InnerClothedTexture;
            m_outerClothingModelWidget.PlayerClass = componentPlayer.PlayerData.PlayerClass;
            m_outerClothingModelWidget.OuterClothingTexture = m_componentPlayer.ComponentClothing.OuterClothedTexture;
            m_titleLabel = Children.Find<LabelWidget>("TitleLabel");
            m_healthValueBar = Children.Find<ValueBarWidget>("HealthValueBar");
            m_staminaValueBar = Children.Find<ValueBarWidget>("StaminaValueBar");
            m_temperatureValueBar = Children.Find<ValueBarWidget>("TemperatureValueBar");
            m_foodValueBar = Children.Find<ValueBarWidget>("FoodValueBar");
            m_sleepValueBar = Children.Find<ValueBarWidget>("SleepValueBar");
            m_experienceValueBar = Children.Find<ValueBarWidget>("ExperienceValueBar");
        }

        public override void Update()
        {
            m_titleLabel.Text = $"{m_componentPlayer.PlayerData.Name}, 等级 {MathF.Floor(m_componentPlayer.PlayerData.Level)}  ";
            m_healthValueBar.Value = m_componentPlayer.ComponentHealth.Health;
            m_staminaValueBar.Value = m_componentPlayer.ComponentVitalStats.Stamina;
            m_foodValueBar.Value = m_componentPlayer.ComponentVitalStats.Food;
            m_sleepValueBar.Value = m_componentPlayer.ComponentVitalStats.Sleep;
            m_temperatureValueBar.Value = m_componentPlayer.ComponentVitalStats.Temperature / 24f;
            m_experienceValueBar.Value = m_componentPlayer.PlayerData.Level - MathF.Floor(m_componentPlayer.PlayerData.Level);
        }
    }
}