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
    //合成桩
    public class NewSubsystemCraftingTable : SubsystemBlockBehavior
    {
        public NewComponentCraftingTable newcomponentCraftingTable;

        public override int[] HandledBlocks => new int[1]
        {
            BlocksManager.GetBlockIndex<working_pilesBlock>()
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
    /*public class Subsystemclay_kilnBlockBehavior : SubsystemEntityBlockBehavior
{
    public SubsystemParticles m_subsystemParticles;

    public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = [];

    public override int[] HandledBlocks => new int[2]
    {
        341,
        360
    };

    public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
    {
        if (Terrain.ExtractContents(oldValue) != 341 && Terrain.ExtractContents(oldValue) != 360)
        {
            base.OnBlockAdded(value, oldValue, x, y, z);
        }
        if (Terrain.ExtractContents(value) == 360)
        {
            AddFire(value, x, y, z);
        }
    }

    public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
    {
        if(Terrain.ExtractContents(newValue) != 341 && Terrain.ExtractContents(newValue) != 360)
        {
            base.OnBlockRemoved(value,newValue,x,y,z);
        }
        if (Terrain.ExtractContents(value) == 360)
        {
            RemoveFire(x, y, z);
        }
    }

    public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
    {
        if (Terrain.ExtractContents(value) == 360)
        {
            AddFire(value, x, y, z);
        }
    }

    public override void OnChunkDiscarding(TerrainChunk chunk)
    {
        var list = new List<Point3>();
        foreach (Point3 key in m_particleSystemsByCell.Keys)
        {
            if (key.X >= chunk.Origin.X && key.X < chunk.Origin.X + 16 && key.Z >= chunk.Origin.Y && key.Z < chunk.Origin.Y + 16)
            {
                list.Add(key);
            }
        }
        foreach (Point3 item in list)
        {
            RemoveFire(item.X, item.Y, item.Z);
        }
    }

    public override bool InteractBlockEntity(ComponentBlockEntity blockEntity,ComponentMiner componentMiner)
    {
        if(blockEntity != null && componentMiner.ComponentPlayer != null)
        {
            ComponentFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentFurnace>(throwOnError: true);
            componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new newFurnaceWidget(componentMiner.Inventory,componentFurnace);
            AudioManager.PlaySound("Audio/UI/ButtonClick",1f,0f,0f);
            return true;
        }
        return false;
    }

    public override void Load(ValuesDictionary valuesDictionary)
    {
        base.Load(valuesDictionary);
        m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(throwOnError: true);
        m_databaseObject = Project.GameDatabase.Database.FindDatabaseObject("Furnace",Project.GameDatabase.EntityTemplateType,throwIfNotFound: true);
    }

    public void AddFire(int value, int x, int y, int z)
    {
        reurn;

        var v = new Vector3(0.5f, 0.2f, 0.5f);
        float size = 0.15f;
        var fireParticleSystem = new FireParticleSystem(new Vector3(x, y, z) + v, size, 16f);
        m_subsystemParticles.AddParticleSystem(fireParticleSystem);
        m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;

        }

    public void RemoveFire(int x, int y, int z)
    {
        return;

        var key = new Point3(x, y, z);
        FireParticleSystem particleSystem = m_particleSystemsByCell[key];
        m_subsystemParticles.RemoveParticleSystem(particleSystem);
        m_particleSystemsByCell.Remove(key);

    }*/
}