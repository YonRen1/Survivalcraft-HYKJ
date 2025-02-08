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
    //加工桌
    public class New1SubsystemCraftingTable : SubsystemBlockBehavior
    {
        public NewComponentCraftingTable newcomponentCraftingTable;

        public override int[] HandledBlocks => new int[1]
        {
            BlocksManager.GetBlockIndex<Flint_processing_tableBlock>()
        };

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            DatabaseObject databaseObject = base.SubsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject("New1CraftingTable", base.SubsystemTerrain.Project.GameDatabase.EntityTemplateType, throwIfNotFound: true);
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
                componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new New1CraftingTableWidget(componentMiner.Inventory);
                AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                return true;
            }
            return false;
        }
    }
}