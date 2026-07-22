using Engine;
using GameEntitySystem;
using TemplatesDatabase;
using Game;

namespace HYKJ
{
    // 陶土窑方块行为子系统。继承自 SubsystemEntityBlockBehavior（而非 SubsystemFurnaceBlockBehavior），
    // 这样基类的 OnBlockAdded/OnBlockRemoved 会直接创建/移除方块实体，不会使用原版熔炉写死的 64/65 索引。
    public class SubsystemClayKilnBlockBehavior : Game.SubsystemEntityBlockBehavior
    {
        public Game.SubsystemParticles m_subsystemParticles;

        // 处理陶土窑的未点燃与点燃两种方块。HandledBlocks 在运行时读取，此时索引已完成动态分配。
        public override int[] HandledBlocks => [ClayKilnBlock.Index, LitClayKilnBlock.Index];

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            // 在未点燃 / 点燃两种陶土窑之间切换时不重建实体，仅真正新放置时才创建实体（与原版熔炉一致）。
            if (Terrain.ExtractContents(oldValue) != ClayKilnBlock.Index
                && Terrain.ExtractContents(oldValue) != LitClayKilnBlock.Index)
            {
                base.OnBlockAdded(value, oldValue, x, y, z);
            }
        }

        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
        {
            // 在未点燃 / 点燃两种陶土窑之间切换时不移除实体，仅真正被替换为其他方块时才移除并掉落物品。
            if (Terrain.ExtractContents(newValue) != ClayKilnBlock.Index
                && Terrain.ExtractContents(newValue) != LitClayKilnBlock.Index)
            {
                base.OnBlockRemoved(value, newValue, x, y, z);
            }
        }

        // 与陶土窑方块交互时打开熔炉界面（复用原版 FurnaceWidget，ComponentClayKiln 继承自 ComponentFurnace）。
        public override bool InteractBlockEntity(ComponentBlockEntity blockEntity, ComponentMiner componentMiner)
        {
            if (blockEntity != null
                && componentMiner.ComponentPlayer != null)
            {
                ComponentFurnace componentFurnace = blockEntity.Entity.FindComponent<ComponentFurnace>(true);
                componentMiner.ComponentPlayer.ComponentGui.ModalPanelWidget = new FurnaceWidget(componentMiner.Inventory, componentFurnace);
                AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
                return true;
            }
            return false;
        }

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_subsystemParticles = Project.FindSubsystem<SubsystemParticles>(true);
            // 找到由 ClayKilnDatabase.xdb 注册的 ClayKiln 实体模板，用于在放置方块时创建对应实体。
            m_databaseObject = Project.GameDatabase.Database.FindDatabaseObject("ClayKiln", Project.GameDatabase.EntityTemplateType, true);
        }
    }
}