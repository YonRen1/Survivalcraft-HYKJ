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
    
    	public class NewComponentFurnace : ComponentInventoryBase, IUpdateable
	{
		public SubsystemTerrain m_subsystemTerrain;

		public SubsystemExplosions m_subsystemExplosions;

		public ComponentBlockEntity m_componentBlockEntity;

		public SubsystemGameInfo m_subsystemGameInfo;

		public SubsystemTime m_subsystemTime;

		public FireParticleSystem m_fireParticleSystem;

		public SubsystemParticles m_subsystemParticles;

		public bool StopFuelWhenNoRecipeIsActive = true;

		public float SmeltSpeed = 0.15f;

		public float SmeltProgressReductionSpeed = float.PositiveInfinity;

		public float FuelTimeEfficiency = 1f;

		public int m_furnaceSize;

		public string[] m_matchedIngredients = new string[9];

		public float m_fuelEndTime;

		public float m_heatLevel;

		public bool m_updateSmeltingRecipe;

		public CraftingRecipe m_smeltingRecipe;

		public float m_smeltingProgress;
		protected virtual float epsilon => Math.Min(m_subsystemTime.GameTimeDelta, 0.1f);
		public virtual int RemainsSlotIndex => SlotsCount - 1;
		public virtual int ResultSlotIndex => SlotsCount - 2;
		public virtual int FuelSlotIndex => SlotsCount - 3;
		public virtual float HeatLevel => m_heatLevel;

		public virtual float SmeltingProgress => m_smeltingProgress;

		public UpdateOrder UpdateOrder => UpdateOrder.Default;
		public virtual float m_fireTimeRemaining
		{
			get
			{
				float ans = m_fuelEndTime - (float)m_subsystemGameInfo.TotalElapsedGameTime;
				return ans > -epsilon ? ans + epsilon : 0f;
			}
			set => m_fuelEndTime = (float)m_subsystemGameInfo.TotalElapsedGameTime + value;
		}
		public virtual float FireTimeRemaining => m_fireTimeRemaining;

		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex == FuelSlotIndex)
			{
				if (BlocksManager.Blocks[Terrain.ExtractContents(value)].GetFuelHeatLevel(value) > 0f)
				{
					return base.GetSlotCapacity(slotIndex, value);
				}
				return 0;
			}
			return base.GetSlotCapacity(slotIndex, value);
		}

		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			m_updateSmeltingRecipe = true;
			base.AddSlotItems(slotIndex, value, count);
		}

		public override int RemoveSlotItems(int slotIndex, int count)
		{
			m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		public virtual bool UseFuel()
        {
            Point3 coordinates = m_componentBlockEntity.Coordinates;
            Slot slot2 = m_slots[FuelSlotIndex];
            if (slot2.Count > 0)
            {
                int num2 = Terrain.ExtractContents(slot2.Value);
                Block block = BlocksManager.Blocks[num2];
                if (block.GetExplosionPressure(slot2.Value) > 0f)
                {
                    slot2.Count = 0;
                    m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
                }
                else if (block.GetFuelHeatLevel(slot2.Value) > 0f)
                {
                    slot2.Count--;
					if (m_heatLevel == 0f) m_fuelEndTime = (float)m_subsystemGameInfo.TotalElapsedGameTime;
                    m_fuelEndTime = m_fuelEndTime + block.GetFuelFireDuration(slot2.Value) * FuelTimeEfficiency;
                    m_heatLevel = block.GetFuelHeatLevel(slot2.Value);
					return true;
                }
            }
			return false;
        }

		public virtual void UpdateSmeltingRecipe()
		{
            m_updateSmeltingRecipe = false;
            float heatLevel = 0f;
            if (m_heatLevel > 0f)
            {
                heatLevel = m_heatLevel;
            }
            else
            {
                Slot slot = m_slots[FuelSlotIndex];
                if (slot.Count > 0)
                {
                    int num = Terrain.ExtractContents(slot.Value);
                    heatLevel = BlocksManager.Blocks[num].GetFuelHeatLevel(slot.Value);
                }
            }
            CraftingRecipe craftingRecipe = FindSmeltingRecipe(heatLevel);
            if (craftingRecipe != m_smeltingRecipe)
            {
                m_smeltingRecipe = (craftingRecipe != null && craftingRecipe.ResultValue != 0) ? craftingRecipe : null;
                m_smeltingProgress = 0f;
                if (FireTimeRemaining <= 0 && m_smeltingRecipe != null) UseFuel();
            }
        }

		public virtual void StopSmelting(bool resetProgress)
		{
            m_heatLevel = 0f;
			m_fuelEndTime = 0f;
            m_smeltingRecipe = null;
            if(resetProgress) m_smeltingProgress = 0f;
        }

		public void Update(float dt)
		{
			if (m_heatLevel > 0f)
			{
				int fuelAdded = 0;
				while (m_fuelEndTime + epsilon < (float)m_subsystemGameInfo.TotalElapsedGameTime)
				{
					if (m_smeltingRecipe != null && UseFuel()){
						fuelAdded++;
						if (fuelAdded == 100) break;
					}
					else
					{
                        StopSmelting(false);
						break;
                    }
				}
			}
			if (m_updateSmeltingRecipe)
				UpdateSmeltingRecipe();
			if (m_smeltingRecipe == null)
			{
				if (StopFuelWhenNoRecipeIsActive)
					StopSmelting(true);
			}
			if(FireTimeRemaining <= 0)
			{
                m_smeltingProgress = MathUtils.Max(m_smeltingProgress - (SmeltProgressReductionSpeed * dt), 0f);
            }
			if (m_smeltingRecipe != null && FireTimeRemaining > 0)
			{
				m_smeltingProgress = MathUtils.Min(m_smeltingProgress + (SmeltSpeed * dt), 1f);
				if (m_smeltingProgress >= 1f)
				{
					for (int i = 0; i < m_furnaceSize; i++)
					{
						if (m_slots[i].Count > 0)
						{
							m_slots[i].Count--;
						}
					}
					m_slots[ResultSlotIndex].Value = m_smeltingRecipe.ResultValue;
					m_slots[ResultSlotIndex].Count += m_smeltingRecipe.ResultCount;
					if (m_smeltingRecipe.RemainsValue != 0 && m_smeltingRecipe.RemainsCount > 0)
					{
						m_slots[RemainsSlotIndex].Value = m_smeltingRecipe.RemainsValue;
						m_slots[RemainsSlotIndex].Count += m_smeltingRecipe.RemainsCount;
					}
					m_smeltingRecipe = null;
					m_smeltingProgress = 0f;
					m_updateSmeltingRecipe = true;
				}
			}

			int cellValue = m_componentBlockEntity.BlockValue;
			if(m_heatLevel > 0f)
			{
				m_fireParticleSystem.m_position = m_componentBlockEntity.Position + new Vector3(0.5f,0.2f,0.5f);
				if(Terrain.ExtractContents(cellValue) == 341)
					m_subsystemParticles.AddParticleSystem(m_fireParticleSystem);
				m_componentBlockEntity.BlockValue = Terrain.ReplaceContents(cellValue,360);
			}
			else
			{
				if(Terrain.ExtractContents(cellValue) == 360)
					m_subsystemParticles.RemoveParticleSystem(m_fireParticleSystem);
				m_componentBlockEntity.BlockValue = Terrain.ReplaceContents(cellValue,341);
			}
		}

		public override void OnEntityRemoved()
		{
			m_subsystemParticles.RemoveParticleSystem(m_fireParticleSystem);
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			m_subsystemExplosions = Project.FindSubsystem<SubsystemExplosions>(throwOnError: true);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(throwOnError: true);
			m_furnaceSize = SlotsCount - 3;
			m_subsystemGameInfo = Project.FindSubsystem<SubsystemGameInfo>(throwOnError: true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(throwOnError: true);
			m_fireParticleSystem = new FireParticleSystem(m_componentBlockEntity.Position + new Vector3(0.5f,0.2f,0.5f), 0.15f, 16f);
			if (m_furnaceSize < 1 || m_furnaceSize > 3)
			{
				throw new InvalidOperationException("Invalid furnace size.");
			}
			float fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			m_fuelEndTime = (float)m_subsystemGameInfo.TotalElapsedGameTime + fireTimeRemaining;
			m_heatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			m_updateSmeltingRecipe = true;
			if(m_heatLevel > 0f)
				m_subsystemParticles.AddParticleSystem(m_fireParticleSystem);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			float fireTimeRemaining = m_fuelEndTime - (float)m_subsystemGameInfo.TotalElapsedGameTime;
			if(fireTimeRemaining < 0) fireTimeRemaining = 0 ;
            valuesDictionary.SetValue("FireTimeRemaining", fireTimeRemaining);
			valuesDictionary.SetValue("HeatLevel", m_heatLevel);
		}
		public virtual CraftingRecipe FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel > 0f)
			{
				for (int i = 0; i < m_furnaceSize; i++)
				{
					int slotValue = GetSlotValue(i);
					int num = Terrain.ExtractContents(slotValue);
					int num2 = Terrain.ExtractData(slotValue);
					if (GetSlotCount(i) > 0)
					{
						Block block = BlocksManager.Blocks[num];
						m_matchedIngredients[i] = block.GetCraftingId(slotValue) + ":" + num2.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						m_matchedIngredients[i] = null;
					}
				}
				ComponentPlayer componentPlayer = FindInteractingPlayer();
				float playerLevel = componentPlayer?.PlayerData.Level ?? 1f;
				CraftingRecipe craftingRecipe = null;
				craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(m_subsystemTerrain, m_matchedIngredients, heatLevel, playerLevel);
				if (craftingRecipe != null && craftingRecipe.ResultValue != 0)
				{
					if (craftingRecipe.RequiredHeatLevel <= 0f)
					{
						craftingRecipe = null;
					}
					if (craftingRecipe != null)
					{
						Slot slot = m_slots[ResultSlotIndex];
						int num3 = Terrain.ExtractContents(craftingRecipe.ResultValue);
						if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value || craftingRecipe.ResultCount + slot.Count > BlocksManager.Blocks[num3].GetMaxStacking(craftingRecipe.ResultValue)))
						{
							craftingRecipe = null;
						}
					}
					if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
					{
						if (m_slots[RemainsSlotIndex].Count == 0 || m_slots[RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
						{
							if (BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].GetMaxStacking(craftingRecipe.RemainsValue) - m_slots[RemainsSlotIndex].Count < craftingRecipe.RemainsCount)
							{
								craftingRecipe = null;
							}
						}
						else
						{
							craftingRecipe = null;
						}
					}
				}
				if (craftingRecipe != null && !string.IsNullOrEmpty(craftingRecipe.Message))
				{
					componentPlayer?.ComponentGui.DisplaySmallMessage(craftingRecipe.Message, Color.White, blinking: true, playNotificationSound: true);
				}
				return craftingRecipe;
			}
			return null;
		}
	}
}