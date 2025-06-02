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
            Model model = ContentManager.Get<Model>("Models/刀");
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
    //刀
    public abstract class processing_MacheteBlock : Block
    {
        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public processing_MacheteBlock(int handleTextureSlot, int headTextureSlot)
        {
            m_handleTextureSlot = handleTextureSlot;
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/加工刀");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Handle").ParentBone);
            Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("cuboid").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Handle").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_handleTextureSlot % 16 / 16f, m_handleTextureSlot / 16 / 16f, 0f));
            var blockMesh2 = new BlockMesh();
            blockMesh2.AppendModelMeshPart(model.FindMesh("cuboid").MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
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
    //锤
    public abstract class hammer1Block : Block
    {
        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public hammer1Block(int handleTextureSlot, int headTextureSlot)
        {
            m_handleTextureSlot = handleTextureSlot;
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/锤");
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
    //锯
    public abstract class sawBlock : Block
    {
        public int m_handleTextureSlot;

        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public sawBlock(int handleTextureSlot, int headTextureSlot)
        {
            m_handleTextureSlot = handleTextureSlot;
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/锯");
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
    public class FlintBlock : SFlatBlock
    {
        public static int Index = 300;
        public FlintBlock()
           : base(32)
        {
        }
    }
    //燧石片
    public class flint_flakeBlock : SFlatBlock
    {
        public static int Index = 301;
        public flint_flakeBlock()
           : base(33)
        {
        }
    }
    //植物纤维
    public class plant_fiberBlock : SFlatBlock
    {
        public static int Index = 302;
        public plant_fiberBlock()
           : base(34)
        {
        }
    }
    //草绳
    public class straw_ropeBlock : SFlatBlock
    {
        public static int Index = 303;
        public straw_ropeBlock()
           : base(35)
        {
        }
    }
    //麻绳
    public class hemp_ropeBlock : SFlatBlock
    {
        public static int Index = 304;
        public hemp_ropeBlock()
           : base(36)
        {
        }
    }
    //树枝
    public class branchesBlock : SFlatBlock
    {
        public static int Index = 305;
        public branchesBlock()
           : base(37)
        {
        }
    }
    //树皮
    public class barkBlock : SFlatBlock
    {
        public static int Index = 307;
        public barkBlock()
           : base(38)
        {
        }
    }
    //钻木取火
    public class drill_wood_make_fireBlock : SFlatBlock
    {
        public static int Index = 308;
        public drill_wood_make_fireBlock()
           : base(39)
        {
        }
    }

    //燧石锤
    public class flint_hammerBlock : Block
    {
        public static int Index = 309;

        public BlockMesh m_standaloneBlockMesh = new();

        public override void Initialize()
        {
            int num = 47;//材质
            int num2 = 19;//材质
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
    public class Wood1Block : SFlatBlock
    {
        public static int Index = 310;
        public Wood1Block()
           : base(41)
        {
        }
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
        public const int Index = 318;

        public Dictionary<Texture2D, BlockMesh> m_meshes = new Dictionary<Texture2D, BlockMesh>();

        public Dictionary<Texture2D, BlockMesh> m_meshes2 = new Dictionary<Texture2D, BlockMesh>();

        private BlockMesh m_standaloneBlockMesh = new BlockMesh();

        private BlockMesh m_drawBlockMesh = new BlockMesh();

        public Texture2D texture = ContentManager.Get<Texture2D>("HYKJTextures/CraftingTable1");

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/CraftingTable1");
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
        public static int Index = 319;

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

            Model model = ContentManager.Get<Model>("Models/Flint_processing_table");
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
        public static int Index = 320;

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

            Model model = ContentManager.Get<Model>("Models/backpack");
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
    //骨镐
    public class bone_pickBlock : PickaxeBlock
    {
        public static int Index = 321;

        public bone_pickBlock()
            : base(47, 7)
        {
        }
    }
    //骨刀
    public class bone_MacheteBlock : MacheteBlock
    {
        public static int Index = 322;

        public bone_MacheteBlock()
            : base(47, 7)
        {
        }
    }
    //骨铲
    public class bone_ShovelBlock : ShovelBlock
    {
        public static int Index = 323;

        public bone_ShovelBlock()
            : base(47, 7)
        {
        }
    }
    //骨斧
    public class bone_AxeBlock : AxeBlock
    {
        public static int Index = 324;

        public bone_AxeBlock()
            : base(47, 7)
        {
        }
    }
    //骨矛
    public class bone_SpearBlock : SpearBlock
    {
        public static int Index = 325;

        public bone_SpearBlock()
            : base(47, 7)
        {
        }
    }
    //工作桩
    public class working_piles1Block : Block
    {
        public const int Index = 326;

        public Dictionary<Texture2D, BlockMesh> m_meshes = new Dictionary<Texture2D, BlockMesh>();

        public Dictionary<Texture2D, BlockMesh> m_meshes2 = new Dictionary<Texture2D, BlockMesh>();

        private BlockMesh m_standaloneBlockMesh = new BlockMesh();

        private BlockMesh m_drawBlockMesh = new BlockMesh();

        public Texture2D texture = ContentManager.Get<Texture2D>("HYKJTextures/CraftingTable1");

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/CraftingTable1");
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
    //工作版
    public class working_piles2Block : Block
    {
        public const int Index = 327;

        public Dictionary<Texture2D, BlockMesh> m_meshes = new Dictionary<Texture2D, BlockMesh>();

        public Dictionary<Texture2D, BlockMesh> m_meshes2 = new Dictionary<Texture2D, BlockMesh>();

        private BlockMesh m_standaloneBlockMesh = new BlockMesh();

        private BlockMesh m_drawBlockMesh = new BlockMesh();

        public Texture2D texture = ContentManager.Get<Texture2D>("HYKJTextures/CraftingTable1");

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/CraftingTable1");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("CraftingTable").ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_drawBlockMesh.AppendModelMeshPart(model.FindMesh("CraftingTable").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
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
    //燧石加工刀
    public class Flint_processingBlock : processing_MacheteBlock
    {
        public static int Index = 328;

        public Flint_processingBlock()
            : base(47, 19)
        {
        }
    }
    //铜锤
    public class copper_hammerBlock : hammer1Block
    {
        public static int Index = 329;

        public copper_hammerBlock()
            : base(47, 79)
        {
        }
    }
    //铁锤
    public class iron_hammerBlock : hammer1Block
    {
        public static int Index = 330;

        public iron_hammerBlock()
            : base(47, 63)
        {
        }
    }
    //石锤
    public class malletBlock : hammer1Block
    {
        public static int Index = 331;

        public malletBlock()
            : base(47, 6)
        {
        }
    }
    //骨锤
    public class bone_hammerBlock : hammer1Block
    {
        public static int Index = 332;

        public bone_hammerBlock()
            : base(47, 7)
        {
        }
    }
    //铜粒
    public class copper_particlesBlock : SFlatBlock
    {
        public static int Index = 335;

        public copper_particlesBlock()
            : base(227)
        {
        }
    }
    //铁粒
    public class sideroblastsBlock : SFlatBlock
    {
        public static int Index = 336;
        public sideroblastsBlock()
           : base(225)
        {
        }
    }
    public class emBlock : Block
    {
        public static int Index = 337;

        public BlockMesh m_blockMesh = new();

        public BlockMesh m_standaloneBlockMesh = new();

        public int m_textureCount;

        public Texture2D texture;

        public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
        {
            return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, DestructionDebrisScale, Color.White, GetFaceTextureSlot(1, value), texture);
        }
        public override void Initialize()
        {
            texture = ContentManager.Get<Texture2D>("HYKJTextures/犀牛头颅");

            Model model = ContentManager.Get<Model>("Models/犀牛头颅");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            m_blockMesh.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
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
    //皮毛
    public class furBlock : SFlatBlock
    {
        public static int Index = 339;
        public furBlock()
           : base(46)
        {
        }
    }
    //尖木棍
    public class sharp_stickBlock : SFlatBlock
    {
        public static int Index = 340;
        public sharp_stickBlock()
           : base(62)
        {
        }
    }
    //木柴
    public class firewoodBlock : SFlatBlock
    {
        public static int Index = 341;
        public firewoodBlock()
           : base(63)
        {
        }
    }
    //烧制粘土
    public class firing_clayBlock : CubeBlock
    {
        public static int Index = 343;
    }
    //粘土
    public class clay_Block : SFlatBlock
    {
        public static int Index = 344;
        public clay_Block()
           : base(50)
        {
            CanBeBuiltIntoFurniture = true;
        }
    }
    //铁锯
    public class iron_sawBlock : sawBlock
    {
        public static int Index = 350;

        public iron_sawBlock()
            : base(47, 63)
        {
        }
    }
    //铜锯
    public class copper_sawBlock : sawBlock
    {
        public static int Index = 351;

        public copper_sawBlock()
            : base(47, 79)
        {
        }
    }
    //焦炭
    public class cokeBlock : SFlatBlock
    {
        public static int Index = 352;
        public cokeBlock()
           : base(48)
        {
        }
    }
    //废料
    public class wasteBlock : SFlatBlock
    {
        public static int Index = 353;
        public wasteBlock()
           : base(51)
        {
        }
    }
    //模版
    public class templateBlock : SFlatBlock
    {
        public static int Index = 354;
        public templateBlock()
           : base(208)
        {
        }
    }
    //金矿
    public class gold_mineBlock : CubeBlock
    {
        public static int Index = 355;
    }
    //斧头模具
    public class axe_moldBlock : SFlatBlock
    {
        public axe_moldBlock() : base(210)
        { }
        public static int Index = 356;
    }
    //板模具
    public class slab_moldBlock : SFlatBlock
    {
        public slab_moldBlock() : base(212)
        { }
        public static int Index = 357;
    }
    //粘土窑
    public class clay_kilnBlock : Block
    {
        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/黏土窑");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("cuboid", true).ParentBone);
            this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("cuboid", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
            this.m_drawBlockMesh.AppendModelMeshPart(model.FindMesh("cuboid", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
            this.m_meshes.Add(this.texture, this.m_standaloneBlockMesh);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            foreach (KeyValuePair<Texture2D, BlockMesh> keyValuePair in this.m_meshes)
            {
                generator.GenerateShadedMeshVertices(this, x, y, z, keyValuePair.Value, Color.White, null, null, geometry.GetGeometry(keyValuePair.Key).SubsetAlphaTest);
            }
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, this.texture, color, size, ref matrix, environmentData);
        }

        public static int Index = 341;

        public Dictionary<Texture2D, BlockMesh> m_meshes = new Dictionary<Texture2D, BlockMesh>();

        private BlockMesh m_standaloneBlockMesh = new BlockMesh();

        private BlockMesh m_drawBlockMesh = new BlockMesh();

        public Texture2D texture = ContentManager.Get<Texture2D>("HYKJTextures/黏土");
    }
    //铜制加工刀
    public class copper_processingBlock : processing_MacheteBlock
    {
        public copper_processingBlock() : base(47, 181)
        {
        }
        public static int Index = 359;
    }
    public class Litclay_kilnBlock : clay_kilnBlock
    {
        public static int Index = 360;
    }
}
