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

    public abstract class spear : Block//长矛
    {
        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public spear(int handleTextureSlot, int headTextureSlot)
        {
            m_handleTextureSlot = handleTextureSlot;
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("HYKJModels/spear");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("b").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("t").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("b").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_handleTextureSlot % 16 / 16f, m_handleTextureSlot / 16 / 16f, 0f));
            var blockMesh2 = new BlockMesh();
            blockMesh2.AppendModelMeshPart(model.FindMesh("t").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation(m_headTextureSlot % 16 / 16f, m_headTextureSlot / 16 / 16f, 0f));
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
        }
    }

    //燧石
    public class FlintBlock : FlatBlock
    {
        public static int Index = 300;
    }
    //燧石片
    public class flint_flakeBlock : FlatBlock
    {
        public static int Index = 301;
    }
    //植物纤维
    public class plant_fiberBlock : FlatBlock
    {
        public static int Index = 302;
    }
    //草绳
    public class straw_ropeBlock : FlatBlock
    {
        public static int Index = 303;
    }

    //石长矛
    public class stone_spear : spear
    {
        public static int Index = 304;

        public stone_spear()
            : base(47, 1)
        {
        }
    }
    public class bone_spear : spear
    {
        public static int Index = 305;

        public bone_spear()
            : base(47, 7)
        {
        }
    }
    //麻绳
    public class hemp_ropeBlock : FlatBlock
    {
        public static int Index = 304;
    }
    //树枝
    public class branchesBlock : FlatBlock
    {
        public static int Index = 305;
    }
    //草绳筛网
    /*public class straw_rope_screenBlock : FlatBlock
    {
     public static int Index = 306;
    }*/
    //树皮
    public class barkBlock : FlatBlock
    {
        public static int Index = 307;
    }
    //钻木取火

    public class drill_wood_make_fireBlock : FlatBlock
    {
        public static int Index = 308;
    }

    /*
     //燧石手斧
      public class flint_hand_axeBlock : FlatBlock
    {
       public static int Index = 309;
    }*/
    //燧石锤
    public class flint_hammerBlock : Block
    {
        public static int Index = 309;

        public BlockMesh m_standaloneBlockMesh = new();

        public override void Initialize()
        {
            int num = 47;
            int num2 = 49;
            Model model = ContentManager.Get<Model>("Models/StoneAxe");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Handle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(num % 16 / 16f, num / 16 / 16f, 0f));
            var blockMesh2 = new BlockMesh();
            blockMesh2.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh2.TransformTextureCoordinates(Matrix.CreateTranslation(num2 % 16 / 16f, num2 / 16 / 16f, 0f));
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh2);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
        }
    }
    //燧石加工刀
    /*  public class flint_processing_knifeBlock : FlatBlock
    {
      public static int Index = 311;
    }



     //皮革布
      public class Leather_clothBlock : FlatBlock
    {
      public static int Index = 312;
    }
     //燧石小刀
      public class Flint_knifeBlock : FlatBlock
    {
      public static int Index = 313;
    }
    //燧石皮革刀
      public class Flint_leather_knifeBlock : FlatBlock
    {
      public static int Index = 314;
    }

    */

    public class bone_2_spear : SpearBlock
    {
        public static int Index = 329;

        public bone_2_spear()
           : base(47, 7)
        {
        }
    }
    //———————————————————————矿物——————————————————
    //金锭
    /*public class 金锭 : Ingot1Block
    {
        public static int Index = 344;

        public 金锭()
            : base("IronIngot")
        {//new Color(199, 191, 8, 255)
        }
    }
    //银锭
    public class 银锭 : Ingot2Block
    {
        public static int Index = 345;

        public 银锭()
            : base("IronIngot")
        {//(194, 194, 194, 255
        }
    }
    //钛合金锭
    public class 钛合金锭 : Ingot3Block
    {
        public static int Index = 346;

        public 钛合金锭()
            : base("IronIngot")
        {//140, 140, 140, 255
        }
    }*/
    //————————————————————————子系统——————————————————————    


    #region 开局添加物品
    //暂时没想加啥
    #endregion

    #region HYKJModLoader

    public class HYKJModLoader : ModLoader
    {
        public static Subtexture ToSubtexture(string imgpath, Vector2? TopLeft = null, Vector2? BottomRight = null)
        {
            return new Subtexture(ContentManager.Get<Texture2D>(imgpath), TopLeft ?? Vector2.Zero, BottomRight ?? Vector2.One);
        }

        public override void __ModInitialize()
        {
            //ModsManager.RegisterHook("OnLoadingFinished", this);//加载时
            ModsManager.RegisterHook("GuiUpdate", this);
        }

        private BitmapButtonWidget tool = new BitmapButtonWidget
        {
            Name = "toolButton",
            Size = new Vector2(68f, 64f),
            NormalSubtexture = ToSubtexture("Textures/Button/tool"),
            ClickedSubtexture = ToSubtexture("Textures/Button/tool_Pressed"),
            Margin = new Vector2(4, 0),
        };

        public override void GuiUpdate(ComponentGui componentGui)
        {
            ComponentPlayer m_componentPlayer = componentGui.m_componentPlayer;
            CanvasWidget controlsContainer = m_componentPlayer.GuiWidget.Children.Find<CanvasWidget>("ControlsContainer");
            StackPanelWidget moreContents = m_componentPlayer.GuiWidget.Children.Find<StackPanelWidget>("MoreContents");
            moreContents.AddChildren(tool);

            if (tool.IsClicked)
            {
                m_componentPlayer.ComponentGui.ModalPanelWidget = new ToolWidget(m_componentPlayer);
            }
        }
    }
    #endregion

    public class ToolWidget : CanvasWidget
    {
        public ComponentPlayer m_componentPlayer;//玩家界面

        public ToolWidget(ComponentPlayer componentPlayer)
        {
            m_componentPlayer = componentPlayer;
            XElement node = ContentManager.Get<XElement>("Widgets/ToolWidget");
            LoadContents(this, node);
        }

        public override void Update()//检测
        {
        }
    }

    /* public virtual void OnEatPickable(ComponentEatPickableBehavior eatPickableBehavior, Pickable EatPickable, out bool Dealed)
    {
        Dealed = false;

      //回水分
      另，外部调用water的方式如下，由玩家对象componentPlayer出发：
      ComponentThirst componentThirst = componentPlayer.Entity.FindComponent<ComponentThirst>(true);
      然后即可获取或修改componentThirst.Water

    }*/





    //————————————————————————原版修改—————————————————————

    //碎石
    public class GravelBlock : CubeBlock
    {
        public static int Index = 6;

        public GravelBlock()
        {
            IsCollapsable = true;
        }

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            if (toolLevel < RequiredToolLevel)
            {
                return;
            }
            if (Random.Float(0f, 1f) < 0.73f)
            {
                base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
                return;
            }
            int num = Random.Int(1, 2);
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(300),
                    Count = 1
                });
            }
        }
    }
}