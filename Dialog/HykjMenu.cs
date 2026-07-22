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
}