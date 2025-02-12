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
        public static Subtexture ToSubtexture(string imgpath, Vector2? TopLeft = null, Vector2? BottomRight = null)
        {
            return new Subtexture(ContentManager.Get<Texture2D>(imgpath), TopLeft ?? Vector2.Zero, BottomRight ?? Vector2.One);
        }

        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("GuiUpdate", this);
            ModsManager.RegisterHook("OnMinerHit", this);
            ModsManager.RegisterHook("OnLoadingFinished", this);
        }

        //定义新的按钮
        private BitmapButtonWidget tool = new BitmapButtonWidget
        {
            Name = "toolButton",
            Size = new Vector2(68f, 64f),
            NormalSubtexture = ToSubtexture("Textures/Button/tool"),
            ClickedSubtexture = ToSubtexture("Textures/Button/tool_Pressed"),
            Text = "",
            Margin = new Vector2(4, 0),
        };

        /// <summary>
        /// Gui组件帧更新时执行
        /// </summary>
        /// <param name="componentGui"></param>
        public override void GuiUpdate(ComponentGui componentGui)
        {
            ComponentPlayer m_componentPlayer = componentGui.m_componentPlayer;
            //获取容器
            StackPanelWidget moreContents = m_componentPlayer.GuiWidget.Children.Find<StackPanelWidget>("MoreContents");
            moreContents.AddChildren(tool);

            //点击事件
            if (tool.IsClicked)
            {
                m_componentPlayer.ComponentGui.ModalPanelWidget = new ToolWidget(m_componentPlayer);
            }
        }

        /// <summary>
        /// 加载任务结束时执行
        /// 在BlocksManager初始化之后
        /// </summary>
        /// <param name="actions"></param>
        public override void OnLoadingFinished(List<Action> actions)
        {
            actions.Add(delegate ()
            {
                ScreensManager.m_screens["MainMenu"] = new HYKJMainMenuScreen();//覆盖原版
            });
        }

        /*/// <summary>
        /// 当人物击打时执行
        /// </summary>
        public override void OnMinerHit(ComponentMiner miner, ComponentBody componentBody, Vector3 hitPoint, Vector3 hitDirection, ref float attackPower, ref float playerProbability, ref float creatureProbability, out bool Hitted)
        {
            //ComponentZelaTool tool = miner.Entity.FindComponent<ComponentZelaTool>();
            if (tool != null)
            {
                int value = miner.ActiveBlockValue;
                Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
                if (block is hammer1Block)
                {
                    hammer1Block zelaPlat = ((hammer1Block)block);
                    //tool.laset = miner.m_lastHitTime;
                    miner.m_lastHitTime -= zelaPlat.GetItemSpeed(value);
                    if (zelaPlat.GetItemHitD(value) < 2)
                    {
                        if (Vector3.Distance(hitPoint, tool.componentPlayer.ComponentCreatureModel.EyePosition) > zelaPlat.GetItemHitD(value))
                        {
                            playerProbability = 0;
                            miner.m_lastHitTime = 0;
                            //tool.m_subsystemParticles.AddParticleSystem(new HitValueParticleSystem(hitPoint, hitDirection * miner.m_random.Float(-3, 3), Color.Yellow, "太短了!"));
                        }
                    }
                    //zelaPlat.GetItemHited(miner, componentBody, hitPoint, hitDirection, attackPower, playerProbability);
                }
            }
            Hitted = false;
        }*/
    }
}