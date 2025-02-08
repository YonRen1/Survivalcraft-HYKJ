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

}