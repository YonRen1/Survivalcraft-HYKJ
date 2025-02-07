using System.Linq;
using Game;
using Engine.Serialization;
using System.Xml.Linq;
using Engine;
using System;
using Engine.Media;
using Random = Game.Random;
using Engine.Graphics;
using GameEntitySystem;
using System.Collections.Generic;
using System.Globalization;
using TemplatesDatabase;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using XmlUtilities;
using Engine.Input;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
//———————————————————————荒野科技——————————————————————— 
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
            XElement node = ContentManager.Get<XElement>("HYKJWidgets/NewCraftingTableWidget");
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

    public class New1CraftingTableWidget : CanvasWidget
    {
        public GridPanelWidget m_inventoryGrid;

        public GridPanelWidget m_craftingGrid;

        public InventorySlotWidget m_craftingResultSlot;

        public InventorySlotWidget m_craftingRemainsSlot;

        public NewComponentCraftingTable m_newcomponentCraftingTable;

        public const string fName = "New1CraftingTableWidget";

        public New1CraftingTableWidget(IInventory inventory)
        {
            XElement node = ContentManager.Get<XElement>("HYKJWidgets/New1CraftingTableWidget");
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
    //覆盖原版界面
    public class HYKJMainMenuScreen : Screen
    {
        public string m_versionString = string.Empty;

        public bool m_versionStringTrial;

        public ButtonWidget m_showBulletinButton;

        public StackPanelWidget m_bulletinStackPanel;

        public LabelWidget m_copyrightLabel;

        public ButtonWidget m_languageSwitchButton;

        public const string fName = "MainMenuScreen";

        public HYKJMainMenuScreen()
        {
            XElement node = ContentManager.Get<XElement>("HYKJScreens/HYKJMainMenuScreen");
            LoadContents(this, node);
            m_showBulletinButton = Children.Find<ButtonWidget>("BulletinButton");
            m_bulletinStackPanel = Children.Find<StackPanelWidget>("BulletinStackPanel");
            m_copyrightLabel = Children.Find<LabelWidget>("CopyrightLabel");
            m_languageSwitchButton = Children.Find<ButtonWidget>("LanguageSwitchButton");
            string languageType = ModsManager.Configs.GetValueOrDefault("Language", "zh-CN");
            m_bulletinStackPanel.IsVisible = languageType == "zh-CN";
            m_copyrightLabel.IsVisible = languageType != "zh-CN";
        }

        public override void Enter(object[] parameters)
        {
            MusicManager.CurrentMix = MusicManager.Mix.Menu;
            Children.Find<MotdWidget>().Restart();
            if (SettingsManager.IsolatedStorageMigrationCounter < 3)
            {
                SettingsManager.IsolatedStorageMigrationCounter++;
                VersionConverter126To127.MigrateDataFromIsolatedStorageWithDialog();
            }
            if (MotdManager.CanShowBulletin) MotdManager.ShowBulletin();
        }

        public override void Leave()
        {
            Keyboard.BackButtonQuitsApp = false;
        }

        public override void Update()
        {
            Keyboard.BackButtonQuitsApp = !MarketplaceManager.IsTrialMode;
            if (string.IsNullOrEmpty(m_versionString) || MarketplaceManager.IsTrialMode != m_versionStringTrial)
            {
                m_versionString = string.Format("Version {0}{1}", VersionsManager.Version, MarketplaceManager.IsTrialMode ? " (Day One)" : string.Empty);
                m_versionStringTrial = MarketplaceManager.IsTrialMode;
            }
            Children.Find("Buy").IsVisible = MarketplaceManager.IsTrialMode;
            Children.Find<LabelWidget>("Version").Text = m_versionString + " -  API " + ModsManager.ApiVersionString;
            RectangleWidget rectangleWidget = Children.Find<RectangleWidget>("Logo");
            float num = 1f + (0.02f * MathF.Sin(1.5f * (float)MathUtils.Remainder(Time.FrameStartTime, 10000.0)));
            rectangleWidget.RenderTransform = Matrix.CreateTranslation((0f - rectangleWidget.ActualSize.X) / 2f, (0f - rectangleWidget.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(rectangleWidget.ActualSize.X / 2f, rectangleWidget.ActualSize.Y / 2f, 0f);
            if (m_languageSwitchButton.IsClicked)
            {
                DialogsManager.ShowDialog(null, new ListSelectionDialog(null, LanguageControl.LanguageTypes, 70f, (object item) => ((KeyValuePair<string, CultureInfo>)item).Value.NativeName, delegate (object item)
                {
                    LanguageControl.ChangeLanguage(((KeyValuePair<string, CultureInfo>)item).Key);
                }));
            }
            if (Children.Find<ButtonWidget>("Play").IsClicked)
            {
                ScreensManager.SwitchScreen("Play");
            }
            if (Children.Find<ButtonWidget>("Help").IsClicked)
            {
                ScreensManager.SwitchScreen("Help");
            }
            if (Children.Find<ButtonWidget>("Content").IsClicked)
            {
                ScreensManager.SwitchScreen("Content");
            }
            if (Children.Find<ButtonWidget>("Settings").IsClicked)
            {
                ScreensManager.SwitchScreen("Settings");
            }
            if (Children.Find<ButtonWidget>("Buy").IsClicked)
            {
                MarketplaceManager.ShowMarketplace();
            }
            // 点击HYKJButton按钮
            if (this.Children.Find<ButtonWidget>("HYKJButton", true).IsClicked)
            {
                HYKJUpdate.ShowUpdate();
            }
            if (this.Children.Find<ButtonWidget>("GXButton", true).IsClicked)
            {
                GxUpdate.ShowUpdate();
            }

            if (Children.Find<BevelledButtonWidget>("Manage").IsClicked)
            {
                ScreensManager.m_screens.TryGetValue("Content", out Screen screen);
                ContentScreen contentScreen = screen as ContentScreen;
                contentScreen.OpenManageSelectDialog();
            }
            if (m_showBulletinButton.IsClicked)
            {
                if (MotdManager.m_bulletin != null && !MotdManager.m_bulletin.Title.Equals("null", StringComparison.CurrentCultureIgnoreCase))
                {
                    MotdManager.ShowBulletin();
                }
                else
                {
                    DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(fName, "1"), LanguageControl.Get(fName, "2"), LanguageControl.Ok, null, null));
                }
            }
            if ((Input.Back && !Keyboard.BackButtonQuitsApp) || Input.IsKeyDownOnce(Key.Escape))
            {
                if (MarketplaceManager.IsTrialMode)
                {
                    ScreensManager.SwitchScreen("Nag");
                }
                else
                {
                    Window.Close();
                }
            }
            if (!String.IsNullOrEmpty(ExternalContentManager.openFilePath))
            {
                ScreensManager.SwitchScreen("ExternalContent");
            }
        }
    }
    //当界面更新时
    public class HYKJUpdate
    {
        public static void ShowUpdate()
        {
            try
            {
                string title = "荒野科技";
                string text = "";
                string text2 = " ";
                DialogsManager.ShowDialog(null, new HYKJMODDialog(title, text + "\n" + text2, delegate ()
                {
                    SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
                }));
                MotdManager.CanShowBulletin = false;
            }
            catch (Exception ex)
            {
                Log.Warning("ShowBulletin失败。原因: " + ex.Message);
            }
        }
    }
    //HYKJMODDialog界面
    public class HYKJMODDialog : Dialog
    {
        public HYKJMODDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/HYKJDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            /*this.m_qqButton = this.Children.Find<ButtonWidget>("QQButton", true);//QQ群
            this.m_gxButton = this.Children.Find<ButtonWidget>("GXButton", true);//贡献
            this.m_hmButton = this.Children.Find<ButtonWidget>("HMButton", true);//黑名单
            this.m_ryButton = this.Children.Find<ButtonWidget>("RYButton", true);//荣誉
            this.m_jjButton = this.Children.Find<ButtonWidget>("JJButton", true);//简介
            this.m_jqButton = this.Children.Find<ButtonWidget>("JQButton", true);//剧情
            this.m_mxButton = this.Children.Find<ButtonWidget>("MXButton", true);//鸣谢
            this.m_zzButton = this.Children.Find<ButtonWidget>("ZZButton", true);//制作*/
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_buttonLabel = this.Children.Find<LabelWidget>("ButtonLabel", true);
            this.m_buttonLabel.Text = LanguageControl.Ok;
            this.m_okButton.IsVisible = true;//显示按钮
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                this.Action.Invoke();
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel; // 标题标签
        public LabelWidget m_contentLabel; // 内容标签
        public LabelWidget m_buttonLabel; // 按钮标签
        public ButtonWidget m_okButton; // Ok按钮
        public Action Action; // 动作
    }

    //工具界面   
    public class ToolWidget : CanvasWidget
    {
        public ComponentPlayer m_componentPlayer;//玩家界面

        public ToolWidget(ComponentPlayer componentPlayer)
        {
            m_componentPlayer = componentPlayer;
            XElement node = ContentManager.Get<XElement>("HYKJWidgets/ToolWidget");
            LoadContents(this, node);
        }

        public override void Update()//检测
        {
        }
    }
    //这段出问题找找Style
    public class GxUpdate
    {
        public void Update()
        {
        }

        public static async Task ShowUpdate()
        {
            try
            {
                string title = "HYKJ更新公告";
                string text = await GetContentFromUrl("https://gitee.com/YonRen/Survivalcraft2-HYKJ/raw/master/%E6%9B%B4%E6%96%B0%E5%86%85%E5%AE%B9.txt");

                DialogsManager.ShowDialog(null, new GXDialog(title, text, delegate ()
                {
                    SettingsManager.BulletinTime = MotdManager.m_bulletin.Time;
                }));

                MotdManager.CanShowBulletin = false;
            }
            catch (Exception ex)
            {
                Log.Warning("ShowBulletin失败。原因: " + ex.Message);
            }
        }

        private static async Task<string> GetContentFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string urlContent = await client.GetStringAsync(url);
                    return urlContent;
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"Request error: {e.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred: {e.Message}");
                    throw;
                }
            }
        }
    }
    //界面
    public class GXDialog : Dialog
    {
        public GXDialog(string title, string content, Action action)
        {
            XElement xelement = ContentManager.Get<XElement>("HYKJDialogs/GXDialog", null);
            base.LoadContents(this, xelement);
            this.m_okButton = this.Children.Find<ButtonWidget>("OkButton", true);
            this.m_titleLabel = this.Children.Find<LabelWidget>("TitleLabel", true);
            this.m_contentLabel = this.Children.Find<LabelWidget>("ContentLabel", true);
            this.m_buttonLabel = this.Children.Find<LabelWidget>("ButtonLabel", true);
            this.m_buttonLabel.Text = LanguageControl.Ok;
            this.m_okButton.IsVisible = true;//显示按钮
            this.m_titleLabel.Text = title;
            this.m_contentLabel.Text = content;
            this.Action = action;
        }

        public override void Update()
        {
            if (this.m_okButton.IsClicked)
            {
                this.Action.Invoke();
                DialogsManager.HideDialog(this);
            }
        }

        public LabelWidget m_titleLabel; // 标题标签
        public LabelWidget m_contentLabel; // 内容标签
        public LabelWidget m_buttonLabel; // 按钮标签
        public ButtonWidget m_okButton; // Ok按钮
        public Action Action; // 动作
    }

}