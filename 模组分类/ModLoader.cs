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
using System.Threading.Tasks;
//———————————————————————荒野科技——————————————————————— 
namespace HYKJ
{
    public class HYKJModLoader : ModLoader
    {
        public static ReadOnlyList<string> Categories => new(m_categories);

        public static List<string> m_categories = [];

        public ComponentGui m_componentGui;

        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("GuiUpdate", this);
            ModsManager.RegisterHook("OnMinerHit", this);
            ModsManager.RegisterHook("OnLoadingFinished", this);
            ModsManager.RegisterHook("BlocksInitalized", this);
        }
        
        /// <summary>
        /// 在主界面初始化后执行，你可以通过这个给主界面加些你想要的按钮或者文字等
        /// 不过建议开发者使用BeforeWidgetUpdate和AfterWidgetUpdate这两个接口实现
        /// </summary>
        /// <param name="mainMenuScreen">初始化完毕后的主界面</param>
        /// <param name="leftBottomBar">主界面左下角的按钮栏，里面有着API的切换语言和资源管理按钮</param>
        /// <param name="rightBottomBar">主界面右下角的按钮栏，Mod作者们可以在这里面放入想要的按钮（例如Mod设置按钮、Mod作者介绍按钮等）</param>
        /*public virtual void OnMainMenuScreenCreated(MainMenuScreen mainMenuScreen, StackPanelWidget leftBottomBar, StackPanelWidget rightBottomBar)
		{

		}*/


        /// <summary>
        /// 方块初始化完成时执行
        /// </summary>
        /*public override void BlocksInitalized()
        {
            BlocksManager.m_categories.Clear();
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
            BlocksManager.m_categories.Add("Fireworks");
        }*/
    }
}