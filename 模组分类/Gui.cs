using Game;
using System.Xml.Linq;
using Engine;
using System;
using Engine.Media;
using Random = Game.Random;
using System.Net.Http;
using System.Threading.Tasks;

namespace HYKJ
{
    public class NewCraftingTableWidget : CanvasWidget
    {
        public GridPanelWidget m_inventoryGrid;

        public GridPanelWidget m_craftingGrid;

        public InventorySlotWidget m_craftingResultSlot;

        public InventorySlotWidget m_craftingRemainsSlot;

        public NewComponentCraftingTable m_newcomponentCraftingTable;

        public const string fName = "NewCraftingTableWidget";

        public NewCraftingTableWidget(IInventory inventory, NewComponentCraftingTable newcomponentCraftingTable)
        {
            m_newcomponentCraftingTable = newcomponentCraftingTable;
            XElement node = ContentManager.Get<XElement>("Widgets/NewCraftingTableWidget");
            LoadContents(this, node);
            m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
            m_craftingGrid = Children.Find<GridPanelWidget>("CraftingGrid");
            m_craftingResultSlot = Children.Find<InventorySlotWidget>("CraftingResultSlot");
            m_craftingRemainsSlot = Children.Find<InventorySlotWidget>("CraftingRemainsSlot");
            int num = 10;
            for (int i = 0; i < m_inventoryGrid.RowsCount; i++)
            {
                for (int j = 0; j < m_inventoryGrid.ColumnsCount; j++)
                {
                    var inventorySlotWidget = new InventorySlotWidget();
                    inventorySlotWidget.AssignInventorySlot(inventory, num++);
                    m_inventoryGrid.Children.Add(inventorySlotWidget);
                    m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
                }
            }
            num = 0;
            for (int k = 0; k < m_craftingGrid.RowsCount; k++)
            {
                for (int l = 0; l < m_craftingGrid.ColumnsCount; l++)
                {
                    var inventorySlotWidget2 = new InventorySlotWidget();
                    inventorySlotWidget2.AssignInventorySlot(m_newcomponentCraftingTable, num++);
                    m_craftingGrid.Children.Add(inventorySlotWidget2);
                    m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
                }
            }
            Children.Find<LabelWidget>("name").Text = LanguageControl.Get(fName, "2");
            Children.Find<LabelWidget>("name1").Text = LanguageControl.Get(fName, "3");
            m_craftingResultSlot.AssignInventorySlot(m_newcomponentCraftingTable, m_newcomponentCraftingTable.ResultSlotIndex);
            m_craftingRemainsSlot.AssignInventorySlot(m_newcomponentCraftingTable, m_newcomponentCraftingTable.RemainsSlotIndex);
        }

        public override void Update()
        {
            if (!m_newcomponentCraftingTable.IsAddedToProject)
            {
                ParentWidget.Children.Remove(this);
            }
        }
    }
    public class HYKJUpdate
    {
        public static void ShowUpdate()
        {
            string title = "HYKJ";
            string text = "";
            string text2 = " ";
            DialogsManager.ShowDialog(null, new HYKJMODDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class HYKJMODDialog : Dialog
    {
        public HYKJMODDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/HYKJDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_qqButton = this.Children.Find<ButtonWidget>("QQ", true);//QQ群
            this.m_upButton = this.Children.Find<ButtonWidget>("UP", true);//UP视频      
            this.m_hmButton = this.Children.Find<ButtonWidget>("HM", true);//黑名单
            this.m_ryButton = this.Children.Find<ButtonWidget>("RY", true);//荣誉榜单
            this.m_jjButton = this.Children.Find<ButtonWidget>("JJ", true);//简介
            this.m_jqButton = this.Children.Find<ButtonWidget>("JQ", true);//剧情
            this.m_mxButton = this.Children.Find<ButtonWidget>("MX", true);//鸣谢
            this.m_zzButton = this.Children.Find<ButtonWidget>("ZZButton", true);//制作
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
            if (this.m_qqButton.IsClicked)
            {
                QQUpdate.ShowUpdate();
            }
            if (this.m_zzButton.IsClicked)
            {
                ZZUpdate.ShowUpdate();
            }
            if (this.m_upButton.IsClicked)
            {
                UPUpdate.ShowUpdate();
            }
            if (this.m_mxButton.IsClicked)
            {
                MXUpdate.ShowUpdate();
            }
            if (this.m_jqButton.IsClicked)
            {
                JQUpdate.ShowUpdate();
            }
            if (this.m_jjButton.IsClicked)
            {
                JJUpdate.ShowUpdate();
            }
            if (this.m_ryButton.IsClicked)
            {
                RYUpdate.ShowUpdate();
            }
            if (this.m_hmButton.IsClicked)
            {
                HMUpdate.ShowUpdate();
            }
        }
        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public ButtonWidget m_qqButton;
        public ButtonWidget m_zzButton;
        public ButtonWidget m_upButton;
        public ButtonWidget m_mxButton;
        public ButtonWidget m_jqButton;
        public ButtonWidget m_jjButton;
        public ButtonWidget m_ryButton;
        public ButtonWidget m_hmButton;
        public Action Action;
    }

    public class JJDialog : Dialog
    {
        public JJDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/JJDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class HMDialog : Dialog
    {
        public HMDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/HMDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class JJUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "4");
            string text = "";
            string text2 = LanguageControl.Get("ModGui", "ModDescription", "Description");

            DialogsManager.ShowDialog(null, new JJDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class HMUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "5");
            string text = "";
            string text2 = LanguageControl.Get("ModGui", "ModDescription", "blacklist");
            DialogsManager.ShowDialog(null, new HMDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class RYDialog : Dialog
    {
        public RYDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/RYDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class RYUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "6");
            string text = "";
            string text2 = LanguageControl.Get("ModGui", "ModDescription", "Honorlist");
            DialogsManager.ShowDialog(null, new RYDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class JQDialog : Dialog
    {
        public JQDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/JQDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class JQUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "7");
            string text = "";
            string text2 = " ";
            DialogsManager.ShowDialog(null, new JQDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class UPUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "8");
            string text = "";
            string text2 = LanguageControl.Get("ModGui", "9");
            DialogsManager.ShowDialog(null, new UPDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class QQDialog : Dialog
    {
        public QQDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/QQDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class MXUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "10");
            string text = "";
            string text2 = LanguageControl.Get("ModGui", "ModDescription", "thanks");
            DialogsManager.ShowDialog(null, new MXDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class MXDialog : Dialog
    {
        public MXDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/MXDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }
    public class UPDialog : Dialog
    {
        public UPDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/UPDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }

    public class QQUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "11");
            string text = "";
            string text2 = " ";
            DialogsManager.ShowDialog(null, new QQDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class ZZUpdate
    {
        public static void ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "12");
            string text = "";
            string text2 = " ";
            DialogsManager.ShowDialog(null, new ZZDialog(title, text + "\n" + text2, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }
    }
    public class ZZDialog : Dialog
    {
        public ZZDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/ZZDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_scrollPanel = this.Children.Find<ScrollPanelWidget>("ScrollPanel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
        public ScrollPanelWidget m_scrollPanel;
    }
    public class ToolWidget : CanvasWidget
    {
        public ComponentPlayer m_componentPlayer;

        public BevelledButtonWidget m_button1;

        public BevelledButtonWidget m_button2;

        public BevelledButtonWidget m_button3;

        string BenCi = LanguageControl.Get("Mod", "0.0.6Description")
                       + LanguageControl.Get("Mod", "0.0.6Day");

        string LiShi = LanguageControl.Get("Mod", "bk")
                     + LanguageControl.Get("Mod", "0.0.5Description")
                     + LanguageControl.Get("Mod", "0.0.5Day");

        public ToolWidget(ComponentPlayer componentPlayer)
        {
            m_componentPlayer = componentPlayer;
            XElement node = ContentManager.Get<XElement>("Widgets/ToolWidget");
            LoadContents(this, node);
            this.m_button1 = this.Children.Find<BevelledButtonWidget>("Button1", true);//本次更新
            this.m_button2 = this.Children.Find<BevelledButtonWidget>("Button2", true);//菜单
            this.m_button3 = this.Children.Find<BevelledButtonWidget>("Button3", true);//历史更新
        }

        public override void Update()
        {
            if (this.m_button1.IsClicked)
            {
                string title_a = LanguageControl.Get("Mod", "title_a");
                DialogsManager.ShowDialog(null, new MessageDialog(title_a, BenCi, LanguageControl.Ok, null, null));
            }
            if (this.m_button2.IsClicked)
            {
                HYKJUpdate.ShowUpdate();
            }
            if (this.m_button3.IsClicked)
            {
                string title_b = LanguageControl.Get("Mod", "title_b");
                DialogsManager.ShowDialog(null, new MessageDialog(title_b, LiShi, LanguageControl.Ok, null, null));
            }
        }
    }
    public class GxUpdate
    {
        public void Update() { }

        public static async Task ShowUpdate()
        {
            string title = LanguageControl.Get("ModGui", "1");
            string text = await GetContentFromUrl("https://gitee.com/YonRen/Survivalcraft2-HYKJ/raw/master/%E6%9B%B4%E6%96%B0%E5%86%85%E5%AE%B9.txt");

            DialogsManager.ShowDialog(null, new GXDialog(title, text, delegate ()
            {
                SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
            }));
            MotdManager.CanShowBulletin = false;
        }

        private static async Task<string> GetContentFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                string urlContent = await client.GetStringAsync(url);
                return urlContent;
            }
        }
    }
    public class GXDialog : Dialog
    {
        public GXDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/GXDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_okButton.IsVisible = true;
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }
        public LabelWidget m_titleLabel;
        public LabelWidget m_contentLabel;
        public LabelWidget m_buttonLabel;
        public ButtonWidget m_okButton;
        public Action Action;
    }

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