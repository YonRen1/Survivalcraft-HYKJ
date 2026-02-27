using Game;
using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace HYKJ
{
    //图片物品
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
            BlocksManager.DrawFlatOrImageExtrusionBlock(primitivesRenderer, value, size, ref matrix, texture, Color.White, false, environmentData);
        }

        /*public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawFlatBlock(primitivesRenderer, value, size * 1f, ref matrix, texture, Color.White, isEmissive: true, environmentData);
        }*/
    }
    //镐头
    public abstract class Pickaxe1Block : Block
    {
        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public Pickaxe1Block(int headTextureSlot)
        {
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Pickaxe");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.8f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_headTextureSlot % 16 / 16f, m_headTextureSlot / 16 / 16f, 0f));
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 1.8f * size, ref matrix, environmentData);
        }
    }
    //斧头
    public abstract class Axe1Block : Block
    {
        public int m_headTextureSlot;

        public BlockMesh m_standaloneBlockMesh = new();

        public Axe1Block(int headTextureSlot)
        {
            m_headTextureSlot = headTextureSlot;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Axe");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Head").ParentBone);
            var blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Head").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.8f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_headTextureSlot % 16 / 16f, m_headTextureSlot / 16 / 16f, 0f));
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 1.8f * size, ref matrix, environmentData);
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
    //矿块
    public abstract class HYKJChunkBlock : Block
    {
        public BlockMesh m_standaloneBlockMesh = new();

        public Matrix m_transform;

        public Matrix m_tcTransform;

        public Color m_color;

        public int m_TextureSlot;

        public HYKJChunkBlock(int TextureSlot, Matrix transform, Matrix tcTransform, Color color)
        {
            m_TextureSlot = TextureSlot;//材质
            m_transform = transform;//空间
            m_tcTransform = tcTransform;//纹理
            m_color = color;//颜色
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Stone");
            Matrix matrix = BlockMesh.GetBoneAbsoluteTransform(model.Meshes[0].ParentBone) * m_transform;
            m_standaloneBlockMesh.AppendModelMeshPart(model.Meshes[0].MeshParts[0], matrix, makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, m_color);
            m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(m_TextureSlot % 16 / 16f, m_TextureSlot / 16 / 16f, 0f));
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
    //加工刀
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

    public abstract class modIngotBlock : Block
    {
        public string m_meshName;

        public Color m_modcolor;

        public BlockMesh m_standaloneBlockMesh = new();

        public modIngotBlock(string meshName, Color modcolor)
        {
            m_meshName = meshName;
            m_modcolor = modcolor;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Ingots");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(m_meshName).ParentBone);
            m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh(m_meshName).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.1f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
            base.Initialize();
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_modcolor, 2f * size, ref matrix, environmentData);
        }
    }
}