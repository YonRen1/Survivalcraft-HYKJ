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
namespace HYKJ//命名空间HYKJ
{
    //新的Blocks
    public abstract class SFlatBlock : Block
    {
        public Texture2D texture;

        public int m_face;

        public SFlatBlock(int facenum)
        {
            m_face = facenum;
        }
        public override void Initialize()
        {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("HYKJTextures/HYKJBlocks");
        }
        public override int GetTextureSlotCount(int value)
        {
            return 16;
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (face == -1) return m_face;
            return m_face;
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawFlatBlock(primitivesRenderer, value, size * 1f, ref matrix, texture, Color.White, isEmissive: true, environmentData);
        }
    }
    //刀
    public abstract class XiaoMacheteBlock : Block
    {
        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public XiaoMacheteBlock(int handleTextureSlot, int headTextureSlot)
        {
            m_handleTextureSlot = handleTextureSlot;
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("HYKJModels/Machete");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Handle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_handleTextureSlot % 16 / 16f, m_handleTextureSlot / 16 / 16f, 0f));
            var blockMesh2 = new BlockMesh();
            blockMesh2.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
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

    //燧石锤
    public class flint_hammerBlock : Block
    {
        public static int Index = 309;

        public BlockMesh m_standaloneBlockMesh = new();

        public override void Initialize()
        {
            int num = 47;//材质
            int num2 = 49;//材质
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
    //木材
    public class Wood1Block : FlatBlock
    {
        public static int Index = 310;
    }

    //皮革布
    public class Leather_clothBlock : SFlatBlock
    {
        public const int Index = 311;
        public Leather_clothBlock()
           : base(40)
        {
        }
    }
    //粘合剂
    public class adhesiveBlock : SFlatBlock
    {
        public const int Index = 312;
        public adhesiveBlock()
           : base(44)
        {
        }
    }
    //大骨
    public class big_boneBlock : SFlatBlock
    {
        public const int Index = 313;
        public big_boneBlock()
           : base(42)
        {
        }
    }
    //小骨
    public class boneBlock : SFlatBlock
    {
        public const int Index = 314;
        public boneBlock()
           : base(43)
        {
        }
    }
    //燧石小刀
    public class Flint_knifeBlock : MacheteBlock
    {
        public static int Index = 315;

        public Flint_knifeBlock()
            : base(47, 19)
        {
            DefaultMeleeHitProbability = 3f;//攻击力
            DefaultMeleePower = 0.6f;//命中率
        }
    }
    //燧石小刀
    public class leather_knifeBlock : MacheteBlock
    {
        public static int Index = 316;

        public leather_knifeBlock()
            : base(77, 19)
        {
            DefaultMeleeHitProbability = 3f;//攻击力
            DefaultMeleePower = 0.9f;//命中率
        }
    }
    //铁砍刀
    public class mBlock : XiaoMacheteBlock
    {
        public static int Index = 317;

        public mBlock()
            : base(47, 63)
        {
        }
    }
    //工作桩
    public class working_pilesBlock : Block
    {
        public const int Index = 330;

        public Dictionary<Texture2D, BlockMesh> m_meshes = new Dictionary<Texture2D, BlockMesh>();

        public Dictionary<Texture2D, BlockMesh> m_meshes2 = new Dictionary<Texture2D, BlockMesh>();

        private BlockMesh m_standaloneBlockMesh = new BlockMesh();

        private BlockMesh m_drawBlockMesh = new BlockMesh();

        public Texture2D texture = ContentManager.Get<Texture2D>("HYKJTextures/CraftingTable1");

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("HYKJModels/CraftingTable1");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CraftingTable").ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_drawBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CraftingTable1").ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable1").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_drawBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable1").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_meshes.Add(texture, m_standaloneBlockMesh);
            m_meshes2.Add(texture, m_drawBlockMesh);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            foreach (var m in m_meshes)
            {
                generator.GenerateShadedMeshVertices(this, x, y, z, m.Value, Color.White, null, null, geometry.GetGeometry(m.Key).SubsetAlphaTest);
            }
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            foreach (var m in m_meshes2)
            {
                BlocksManager.DrawMeshBlock(primitivesRenderer, m.Value, m.Key, Color.White, size, ref matrix, environmentData);
            }
        }
    }
    //燧石加工桌 
    public class Flint_processing_tableBlock : Block
    {
        public static int Index = 331;

        public BlockMesh m_blockMesh = new();

        public BlockMesh m_standaloneBlockMesh = new();

        public BlockMesh m_blockMesh2 = new();

        public BlockMesh m_standaloneBlockMesh2 = new();

        public int m_textureCount;

        public Texture2D texture;

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, Color.White, GetFaceTextureSlot(1, value), texture);
        }
        public override void Initialize()
        {
            texture = ContentManager.Get<Texture2D>("HYKJTextures/Flint_processing_table");

            Model model = ContentManager.Get<Model>("HYKJModels/Flint_processing_table");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Table").ParentBone);
            m_blockMesh.AppendModelMeshPart(model.FindMesh("Table").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Table").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Axe").ParentBone);
            m_blockMesh.AppendModelMeshPart(model.FindMesh("Axe").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Axe").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateShadedMeshVertices(this, x, y, z, m_blockMesh, Color.White, null, null, geometry.GetGeometry(texture).SubsetOpaque);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, size, ref matrix, environmentData);
        }

    }

    //皮革背包 
    public class nBlock : Block
    {
        public static int Index = 334;

        public BlockMesh m_blockMesh = new();

        public BlockMesh m_standaloneBlockMesh = new();

        public BlockMesh m_blockMesh2 = new();

        public BlockMesh m_standaloneBlockMesh2 = new();

        public int m_textureCount;

        public Texture2D texture;

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, Color.White, GetFaceTextureSlot(1, value), texture);
        }
        public override void Initialize()
        {
            texture = ContentManager.Get<Texture2D>("Textures/Clothing/leather_backpack");

            Model model = ContentManager.Get<Model>("HYKJModels/backpack");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("cuboid").ParentBone);
            m_blockMesh.AppendModelMeshPart(model.FindMesh("cuboid").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("cuboid").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("cube").ParentBone);
            m_blockMesh.AppendModelMeshPart(model.FindMesh("cube").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.3f, 0.5f, 1.3f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("cube").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(-0.5f, -0.1f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            IsCollidable = false;
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateShadedMeshVertices(this, x, y, z, m_blockMesh, Color.White, null, null, geometry.GetGeometry(texture).SubsetOpaque);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, size, ref matrix, environmentData);
        }

    }

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
            ModsManager.RegisterHook("GuiUpdate", this);
            ModsManager.RegisterHook("OnLoadingFinished", this);
        }

        private BitmapButtonWidget tool = new BitmapButtonWidget
        {
            Name = "toolButton",
            Size = new Vector2(68f, 64f),
            NormalSubtexture = ToSubtexture("Textures/Button/tool"),
            ClickedSubtexture = ToSubtexture("Textures/Button/tool_Pressed"),
            Text = "";
            Margin = new Vector2(4, 0),
        };

        public override void GuiUpdate(ComponentGui componentGui)
        {
            ComponentPlayer m_componentPlayer = componentGui.m_componentPlayer;
            StackPanelWidget moreContents = m_componentPlayer.GuiWidget.Children.Find<StackPanelWidget>("MoreContents");
            moreContents.AddChildren(tool);

            if (tool.IsClicked)
            {
                m_componentPlayer.ComponentGui.ModalPanelWidget = new ToolWidget(m_componentPlayer);
            }
        }
        public override void OnLoadingFinished(List<Action> actions)
        {
            actions.Add(delegate ()
            {
                ScreensManager.m_screens["MainMenu"] = new HYKJMainMenuScreen();
            });
        }
    }
    #endregion

    //合成桩
    public class NewSubsystemCraftingTableBlockBehavior : SubsystemBlockBehavior
    {
        public NewComponentCraftingTable newcomponentCraftingTable;

        public override int[] HandledBlocks => new int[1]
        {
            working_pilesBlock.Index,
        };

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("NewCraftingTable", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, throwIfNotFound: true);
            ValuesDictionary valuesDictionary = new ValuesDictionary();
            valuesDictionary.PopulateFromDatabaseObject(databaseObject);
            valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
            Entity entity = base.SubsystemTerrain.Project.CreateEntity(valuesDictionary);
            base.SubsystemTerrain.Project.AddEntity(entity);
        }

        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
        {
            ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true).GetBlockEntity(x, y, z);
            if (blockEntity == null)
            {
                return;
            }
            Vector3 position = new Vector3(x, y, z) + new Vector3(0.5f);
            foreach (IInventory item in blockEntity.Entity.FindComponents<IInventory>())
            {
                item.DropAllItems(position);
            }
            base.SubsystemTerrain.Project.RemoveEntity(blockEntity.Entity, disposeEntity: true);
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            ComponentBlockEntity blockEntity = base.SubsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true).GetBlockEntity(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
            if (blockEntity != null && componentMiner.ComponentPlayer != null)
            {
                NewComponentCraftingTable newcomponentCraftingTable = blockEntity.Entity.FindComponent<NewComponentCraftingTable>(throwOnError: true);
                componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new NewCraftingTableWidget(componentMiner.Inventory, newcomponentCraftingTable);
                AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                return true;
            }
            return false;
        }
    }
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
            if (Random.Float(0f, 1f) < 0.85f)
            {
                base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
                return;
            }
            int num = Random.Int(1, 1);
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(FlintBlock.Index),
                    Count = 1
                });
            }
        }
    }
    //橡木OakWoodBlock
    public class OakWoodBlock : WoodBlock
    {
        public static int Index = 9;

        public OakWoodBlock()
            : base(21, 20)
        {
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(1, 2);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(Wood1Block.Index),//动态ID不怕冲突，掉落木材
                    Count = 1
                });
            }
        }
    }
    public class BirchWoodBlock : WoodBlock
    {
        public static int Index = 10;

        public BirchWoodBlock()
            : base(21, 117)
        {
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(1, 2);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(Wood1Block.Index),//动态ID不怕冲突，掉落木材
                    Count = 1
                });
            }
        }
    }
    public class SpruceWoodBlock : WoodBlock
    {
        public static int Index = 11;

        public SpruceWoodBlock()
            : base(21, 116)
        {
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(1, 2);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(Wood1Block.Index),//动态ID不怕冲突，掉落木材
                    Count = 1
                });
            }
        }
    }
    public class MimosaWoodBlock : WoodBlock
    {
        public static int Index = 255;

        public MimosaWoodBlock()
            : base(21, 116)
        {
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(1, 2);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(Wood1Block.Index),//动态ID不怕冲突，掉落木材
                    Count = 1
                });
            }
        }
    }
    public class PoplarWoodBlock : WoodBlock
    {
        public const int Index = 262;

        public PoplarWoodBlock()
            : base(21, 109)
        {
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(1, 2);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(Wood1Block.Index),//动态ID不怕冲突，掉落木材
                    Count = 1
                });
            }
        }
    }
    //高草掉落植物纤维
    public class TallGrassBlock : CrossBlock
    {
        public static int Index = 19;

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            int data = Terrain.ExtractData(oldValue);
            if (!GetIsSmall(data))
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(302, 0, data),
                    Count = 1
                });
            }
            showDebris = true;
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (!GetIsSmall(Terrain.ExtractData(value)))
            {
                return 85;
            }
            return 84;
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawFlatOrImageExtrusionBlock(primitivesRenderer, value, size, ref matrix, null, color * BlockColorsMap.Grass.Lookup(environmentData.Temperature, environmentData.Humidity), isEmissive: false, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateCrossfaceVertices(this, value, x, y, z, BlockColorsMap.Grass.Lookup(generator.Terrain, x, y, z), GetFaceTextureSlot(0, value), geometry.SubsetAlphaTest);
        }

        public override int GetShadowStrength(int value)
        {
            if (!GetIsSmall(Terrain.ExtractData(value)))
            {
                return DefaultShadowStrength;
            }
            return DefaultShadowStrength / 2;
        }

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            Color color = BlockColorsMap.Grass.Lookup(subsystemTerrain.Terrain, Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, color, GetFaceTextureSlot(4, value));
        }

        public static bool GetIsSmall(int data)
        {
            return (data & 8) != 0;
        }

        public static int SetIsSmall(int data, bool isSmall)
        {
            if (!isSmall)
            {
                return data & -9;
            }
            return data | 8;
        }
    }
    //空手挖掘
    public class AirBlock : Block
    {
        public const string fName = "AirBlock";

        public static int Index = 0;

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            if (Terrain.ExtractContents(value) != 0)
                BlocksManager.DrawFlatOrImageExtrusionBlock(primitivesRenderer, 111, size, ref matrix, null, color, isEmissive: false, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(this, 111, x, y, z, Color.Magenta, geometry.OpaqueSubsetsByFace);
        }

        public override IEnumerable<int> GetCreativeValues()
        {
            yield break;
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            int content = Terrain.ExtractContents(value);
            if (content == 0) return base.GetDisplayName(subsystemTerrain, value);
            return ($"{LanguageControl.Get(fName, "1")}({value})");
        }

        public override void Initialize()
        {
            ShovelPower = 0.4f;//铲子
            QuarryPower = 0.1f;//镐子
            HackPower = 0.2f;//斧子
        }
    }
    //石
    public class GraniteBlock : PaintedCubeBlock
    {
        public static int Index = 3;

        public GraniteBlock()
            : base(24)
        {
            CanBeBuiltIntoFurniture = true;
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            int num = Random.Int(3, 4);//随机掉落1-2个
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(StoneChunkBlock.Index),//动态ID不怕冲突
                    Count = 3
                });
            }
        }
    }
    //橡树叶
    public class OakLeavesBlock : DeciduousLeavesBlock
    {
        public static int Index = 12;

        public OakLeavesBlock()
        : base(0f, 0.25f, 0.54f, 0.85f, BlockColorsMap.OakLeaves, new Color(230, 80, 0), new Color(255, 130, 20), 2f)
        {
        }

        public override int GetFaceTextureSlot(int face, int value)
        {
            return DeciduousLeavesBlock.GetSeason(Terrain.ExtractData(value)) switch
            {
                Season.Winter => 106,
                Season.Spring => 107,
                _ => base.GetFaceTextureSlot(face, value),
            };
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
        {
            showDebris = true;
            if (toolLevel < RequiredToolLevel)
            {
                return;
            }
            if (Random.Float(0f, 1f) < 0.5f)
            {
                base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
                return;
            }
            int num = Random.Int(1, 1);
            for (int i = 0; i < num; i++)
            {
                dropValues.Add(new BlockDropValue
                {
                    Value = Terrain.MakeBlockValue(branchesBlock.Index),
                    Count = 1
                });
            }
        }
    }
    //——————————————————————界面—————————————————————
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

            /*if (this.Children.Find<ButtonWidget>("GXButton", true).IsClicked)
            {
                DialogsManager.ShowDialog(null, new Message, LanguageControl.Ok, null, null));
            }*/

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
    //————————————————————————Component—————————————————————

    public class NewComponentCraftingTable : ComponentInventoryBase, IUpdateable
    {
        public int m_craftingGridSize;

        public string[] m_matchedIngredients = new string[9];

        public CraftingRecipe m_matchedRecipe;
        public int RemainsSlotIndex => SlotsCount - 1;

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public bool m_recipeUpdateNeeded = false;

        public bool m_recipeRefindNeeded = false;
        public int ResultSlotIndex => SlotsCount - 2;

        public bool m_resetWhenSlotItemsRemoved;

        public virtual void Update(float dt)
        {
            if (m_recipeUpdateNeeded)
            {
                UpdateCraftingResult(m_recipeRefindNeeded);
            }
            m_recipeUpdateNeeded = false;
            m_recipeRefindNeeded = false;
        }

        public override int GetSlotCapacity(int slotIndex, int value)
        {
            if (slotIndex < SlotsCount - 2)
            {
                return base.GetSlotCapacity(slotIndex, value);
            }
            return 0;
        }

        public override void AddSlotItems(int slotIndex, int value, int count)
        {
            int oldCount = GetSlotCount(slotIndex);
            base.AddSlotItems(slotIndex, value, count);
            if (oldCount == 0) m_recipeRefindNeeded = true;
            m_recipeUpdateNeeded = true;
            m_slots[RemainsSlotIndex].Count = 0;
            m_slots[ResultSlotIndex].Count = 0;
        }

        public override int RemoveSlotItems(int slotIndex, int count)
        {
            int num = 0;
            int[] originalCount = new int[SlotsCount - 2];
            for (int i = 0; i < originalCount.Length; i++)
            {
                originalCount[i] = GetSlotCount(i);
            }
            if (slotIndex == ResultSlotIndex)
            {
                if (m_matchedRecipe != null)
                {
                    if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
                    {
                        if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == m_matchedRecipe.RemainsValue)
                        {
                            int num2 = BlocksManager.Blocks[Terrain.ExtractContents(m_matchedRecipe.RemainsValue)].GetMaxStacking(m_matchedRecipe.RemainsValue) - m_slots[RemainsSlotIndex].Count;
                            count = MathUtils.Min(count, num2 / m_matchedRecipe.RemainsCount * m_matchedRecipe.ResultCount);
                        }
                        else
                        {
                            count = 0;
                        }
                    }
                    count = count / m_matchedRecipe.ResultCount * m_matchedRecipe.ResultCount;
                    num = base.RemoveSlotItems(slotIndex, count);
                    if (num > 0)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (!string.IsNullOrEmpty(m_matchedIngredients[i]))
                            {
                                int index = (i % 3) + (m_craftingGridSize * (i / 3));
                                m_slots[index].Count = MathUtils.Max(m_slots[index].Count - (num / m_matchedRecipe.ResultCount), 0);
                            }
                        }
                        if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
                        {
                            m_slots[RemainsSlotIndex].Value = m_matchedRecipe.RemainsValue;
                            m_slots[RemainsSlotIndex].Count += num / m_matchedRecipe.ResultCount * m_matchedRecipe.RemainsCount;
                        }
                        ComponentPlayer componentPlayer = FindInteractingPlayer();
                        if (componentPlayer != null && componentPlayer.PlayerStats != null)
                        {
                            componentPlayer.PlayerStats.ItemsCrafted += num;
                        }
                    }
                }
            }
            else
            {
                num = base.RemoveSlotItems(slotIndex, count);
            }
            m_recipeUpdateNeeded = true;
            if (m_resetWhenSlotItemsRemoved) m_slots[ResultSlotIndex].Count = 0;
            for (int i = 0; i < originalCount.Length; i++)
            {
                if (originalCount[i] > 0 && GetSlotCount(i) == 0)
                    m_recipeRefindNeeded = true;
            }
            return num;
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            m_craftingGridSize = (int)MathF.Sqrt(SlotsCount - 2);
            UpdateCraftingResult(true);
        }

        public virtual void UpdateCraftingResult(bool recipeRefindNeeded)
        {
            int num = int.MaxValue;
            for (int i = 0; i < m_craftingGridSize; i++)
            {
                for (int j = 0; j < m_craftingGridSize; j++)
                {
                    int num2 = i + (j * 3);
                    int slotIndex = i + (j * m_craftingGridSize);
                    int slotValue = GetSlotValue(slotIndex);
                    int num3 = Terrain.ExtractContents(slotValue);
                    int num4 = Terrain.ExtractData(slotValue);
                    int slotCount = GetSlotCount(slotIndex);
                    if (slotCount > 0)
                    {
                        Block block = BlocksManager.Blocks[num3];
                        m_matchedIngredients[num2] = block.GetCraftingId(slotValue) + ":" + num4.ToString(CultureInfo.InvariantCulture);
                        num = MathUtils.Min(num, slotCount);
                    }
                    else
                    {
                        m_matchedIngredients[num2] = null;
                    }
                }
            }
            ComponentPlayer componentPlayer = FindInteractingPlayer();
            float playerLevel = componentPlayer?.PlayerData.Level ?? 1f;
            CraftingRecipe craftingRecipe;
            if (recipeRefindNeeded)
                craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(Project.FindSubsystem<SubsystemTerrain>(throwOnError: true), m_matchedIngredients, 0f, playerLevel);
            else craftingRecipe = m_matchedRecipe;
            if (craftingRecipe != null && craftingRecipe.ResultValue != 0)
            {
                m_matchedRecipe = craftingRecipe;
                m_slots[ResultSlotIndex].Value = craftingRecipe.ResultValue;
                m_slots[ResultSlotIndex].Count = craftingRecipe.ResultCount * num;
            }
            else
            {
                m_matchedRecipe = null;
                m_slots[ResultSlotIndex].Value = 0;
                m_slots[ResultSlotIndex].Count = 0;
            }
            if (craftingRecipe != null && !string.IsNullOrEmpty(craftingRecipe.Message))
            {
                string message = craftingRecipe.Message;
                if (message.StartsWith("[") && message.EndsWith("]"))
                {
                    message = LanguageControl.Get("CRMessage", message.Substring(1, message.Length - 2));
                }
                componentPlayer?.ComponentGui.DisplaySmallMessage(message, Color.White, blinking: true, playNotificationSound: true);
            }
        }
    }


}
