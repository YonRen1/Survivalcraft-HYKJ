using Game;
using Engine;
using System;
using Random = Game.Random;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace HYKJ
{
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

        // === 进度条系统 ===
        public float m_craftingProgress;
        public bool m_isCrafting;
        public float m_craftingTimePerUnit = 2f;

        public virtual void Update(float dt) 
        {
            if (m_recipeUpdateNeeded) 
            {
                UpdateCraftingResult(m_recipeRefindNeeded);
            }

            // === 自动合成 + 进度条 ===
            if (m_matchedRecipe != null && m_matchedRecipe.ResultValue != 0)
            {
                int currentResult = GetSlotCount(ResultSlotIndex);
                int resultContents = Terrain.ExtractContents(m_matchedRecipe.ResultValue);
                Block resultBlock = BlocksManager.Blocks[resultContents];
                int maxStack = resultBlock.GetMaxStacking(m_matchedRecipe.ResultValue);

                // 成品槽有未取走的其他配方产物 → 暂停合成，等玩家取走
                if (currentResult > 0 && m_slots[ResultSlotIndex].Value != m_matchedRecipe.ResultValue)
                {
                    m_isCrafting = false;
                    m_craftingProgress = 0f;
                }
                else if (currentResult + m_matchedRecipe.ResultCount <= maxStack)
                {
                    m_isCrafting = true;
                    // 不同工具等级不同耗时
                    float time = GetCraftingTimeForRecipe(m_matchedRecipe);
                    m_craftingProgress += dt / time;

                    if (m_craftingProgress >= 1f)
                    {
                        CraftOneUnit();
                        m_craftingProgress -= 1f;
                        UpdateCraftingResult(true);
                    }
                }
                else
                {
                    m_isCrafting = false;
                    if (m_craftingProgress > 0.99f) m_craftingProgress = 0.99f;
                }
            }
            else
            {
                m_isCrafting = false;
                m_craftingProgress = 0f;
            }

            m_recipeUpdateNeeded = false;
            m_recipeRefindNeeded = false;
        }

        /// <summary>
        /// 根据合成产物的工具等级返回耗时（可扩展）
        /// </summary>
        public float GetCraftingTimeForRecipe(CraftingRecipe recipe)
        {
            int blockIndex = Terrain.ExtractContents(recipe.ResultValue);
            // 骨器: 2s
            if (blockIndex == bone_pickBlock.Index ||
                blockIndex == bone_AxeBlock.Index ||
                blockIndex == bone_ShovelBlock.Index ||
                blockIndex == bone_MacheteBlock.Index ||
                blockIndex == bone_SpearBlock.Index ||
                blockIndex == bone_hammerBlock.Index)
                return 2f;
            // 铜器: 3s
            if (blockIndex == copper_hammerBlock.Index ||
                blockIndex == copper_sawBlock.Index ||
                blockIndex == CopperAxe1Block.Index ||
                blockIndex == CopperPickaxe1Block.Index)
                return 3f;
            // 铁器: 5s
            if (blockIndex == iron_hammerBlock.Index ||
                blockIndex == iron_sawBlock.Index)
                return 5f;
            // 燧石工具/武器: 1.5s
            if (blockIndex == flint_hammerBlock.Index ||
                blockIndex == Flint_knifeBlock.Index ||
                blockIndex == leather_knifeBlock.Index)
                return 1.5f;
            // 木/石武器: 2.5s
            if (blockIndex == WoodenClubBlock.Index ||
                blockIndex == StoneClubBlock.Index ||
                blockIndex == malletBlock.Index ||
                blockIndex == WoodenSpearBlock.Index ||
                blockIndex == RockSpearBlock.Index)
                return 2.5f;
            // 默认
            return m_craftingTimePerUnit;
        }

        /// <summary>
        /// 消耗材料、产出一份成品
        /// </summary>
        public void CraftOneUnit()
        {
            if (m_matchedRecipe == null) return;

            for (int i = 0; i < 9; i++)
            {
                if (!string.IsNullOrEmpty(m_matchedIngredients[i]))
                {
                    int index = (i % 3) + (m_craftingGridSize * (i / 3));
                    m_slots[index].Count = MathUtils.Max(m_slots[index].Count - 1, 0);
                }
            }

            m_slots[ResultSlotIndex].Value = m_matchedRecipe.ResultValue;
            m_slots[ResultSlotIndex].Count += m_matchedRecipe.ResultCount;

            if (m_matchedRecipe.RemainsValue != 0 && m_matchedRecipe.RemainsCount > 0)
            {
                m_slots[RemainsSlotIndex].Value = m_matchedRecipe.RemainsValue;
                m_slots[RemainsSlotIndex].Count += m_matchedRecipe.RemainsCount;
            }
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
            // === 合成保护：正在合成或有已合成产物时，拒绝放入会改变配方的物品 ===
            // 防止配方被中途更换导致吞掉已合成的产物
            if (m_matchedRecipe != null && (m_isCrafting || m_craftingProgress > 0f || m_slots[ResultSlotIndex].Count > 0))
            {
                if (!IsIngredientOfCurrentRecipe(slotIndex, value))
                {
                    return; // 拒绝放入
                }
            }
            int oldCount = GetSlotCount(slotIndex);
            base.AddSlotItems(slotIndex, value, count);
            if (oldCount == 0) m_recipeRefindNeeded = true;
            m_recipeUpdateNeeded = true;
            // 注意：不在这里清空成品槽/副产物槽，避免补充材料时已合成产物丢失
            m_craftingProgress = 0f;
        }

        /// <summary>
        /// 判断放入的物品是否属于当前配方在该槽位需要的材料
        /// </summary>
        public bool IsIngredientOfCurrentRecipe(int slotIndex, int value)
        {
            if (m_matchedRecipe == null) return true;
            if (slotIndex < 0 || slotIndex >= m_matchedIngredients.Length) return true;
            string required = m_matchedIngredients[slotIndex];
            // 当前配方在该位置没有材料 → 放入任何东西都会改变配方 → 拒绝
            if (string.IsNullOrEmpty(required)) return false;
            int contents = Terrain.ExtractContents(value);
            Block block = BlocksManager.Blocks[contents];
            string newId = block.GetCraftingId(value) + ":" + Terrain.ExtractData(value).ToString(CultureInfo.InvariantCulture);
            return newId == required;
        }

        public override int RemoveSlotItems(int slotIndex, int count) 
        {
            int num = 0;
            if (slotIndex == ResultSlotIndex) 
            {
                if (m_matchedRecipe != null) 
                {
                    count = count / m_matchedRecipe.ResultCount * m_matchedRecipe.ResultCount;
                    num = base.RemoveSlotItems(slotIndex, count);
                    if (num > 0) 
                    {
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
            int[] originalCount = new int[SlotsCount - 2];
            for (int i = 0; i < originalCount.Length; i++) 
            {
                originalCount[i] = GetSlotCount(i);
            }
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
                // 成品槽有未取走的产物时保留原 Value，避免与新配方产物混淆
                if (m_slots[ResultSlotIndex].Count == 0)
                {
                    m_slots[ResultSlotIndex].Value = craftingRecipe.ResultValue;
                }
                m_matchedRecipe = craftingRecipe;
            }
            else 
            {
                m_matchedRecipe = null;
                if (m_slots[ResultSlotIndex].Count == 0)
                {
                    m_slots[ResultSlotIndex].Value = 0;
                }
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