using Game;
using Engine;
using System;
using Random = Game.Random;
using GameEntitySystem;
using System.Globalization;
using TemplatesDatabase;

namespace HYKJ {
    // 陶土窑的冶炼组件。继承自原版 ComponentFurnace，复用其全部冶炼逻辑，
    public class ComponentClayKiln : ComponentFurnace {
        public override void Update(float dt) {
            m_fuelEndTime = (float)(m_subsystemGameInfo.TotalElapsedGameTime + m_fireTimeRemaining);
            if (m_heatLevel > 0f) {
                m_fireTimeRemaining = MathUtils.Max(0f, m_fireTimeRemaining - dt);
                if (m_fireTimeRemaining == 0f) {
                    m_heatLevel = 0f;
                }
            }
            if (m_updateSmeltingRecipe) {
                UpdateSmeltingRecipe();
            }
            if (m_smeltingRecipe == null) //没有配方，处理空烧
            {
                if (StopFuelWhenNoRecipeIsActive) {
                    StopSmelting(true);
                }
            }
            if (m_smeltingRecipe != null
                && m_fireTimeRemaining <= 0f) {
                UseFuel();
            }
            if (m_fireTimeRemaining <= 0f) {
                m_smeltingRecipe = null;
                m_smeltingProgress = MathUtils.Max(0, m_smeltingProgress - dt * SmeltProgressReductionSpeed);
            }
            if (m_smeltingRecipe != null) {
                m_smeltingProgress = MathUtils.Min(m_smeltingProgress + SmeltSpeed * dt, 1f);
                if (m_smeltingProgress >= 1f) {
                    for (int i = 0; i < m_furnaceSize; i++) {
                        if (m_slots[i].Count > 0) {
                            m_slots[i].Count--;
                        }
                    }
                    m_slots[ResultSlotIndex].Value = m_smeltingRecipe.ResultValue;
                    m_slots[ResultSlotIndex].Count += m_smeltingRecipe.ResultCount;
                    if (m_smeltingRecipe.RemainsValue != 0
                        && m_smeltingRecipe.RemainsCount > 0) {
                        m_slots[RemainsSlotIndex].Value = m_smeltingRecipe.RemainsValue;
                        m_slots[RemainsSlotIndex].Count += m_smeltingRecipe.RemainsCount;
                    }
                    m_smeltingRecipe = null;
                    m_smeltingProgress = 0f;
                    m_updateSmeltingRecipe = true;
                }
            }
            //根据燃烧状态调整方块值
            int cellValue = m_componentBlockEntity.BlockValue;
            if (m_heatLevel > 0f) {
                m_fireParticleSystem.m_position = m_componentBlockEntity.Position + new Vector3(0.5f, 0.2f, 0.5f);
                if (Terrain.ExtractContents(cellValue) == ClayKilnBlock.Index) {
                    m_subsystemParticles.AddParticleSystem(m_fireParticleSystem);
                    m_componentBlockEntity.BlockValue = Terrain.ReplaceContents(cellValue, LitClayKilnBlock.Index);
                }
            }
            else {
                if (Terrain.ExtractContents(cellValue) == LitClayKilnBlock.Index) {
                    m_subsystemParticles.RemoveParticleSystem(m_fireParticleSystem);
                    m_componentBlockEntity.BlockValue = Terrain.ReplaceContents(cellValue, ClayKilnBlock.Index);
                }
            }
        }
 
        // 加载时找到 ClayKiln 实体模板（由 ClayKilnDatabase.xdb 注册）。
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
        }
    }
}