using Game;
using System.Xml.Linq;
using Engine;
using System;
using Engine.Media;
using Random = Game.Random;
//using System.Threading.Tasks;

namespace HYKJ {
    public class NewCraftingTableWidget : CanvasWidget {
        public GridPanelWidget m_inventoryGrid;

        public GridPanelWidget m_craftingGrid;

        public InventorySlotWidget m_craftingResultSlot;

        public InventorySlotWidget m_craftingRemainsSlot;

        public ValueBarWidget m_progress;

        public NewComponentCraftingTable m_newcomponentCraftingTable;

        public const string fName = "NewCraftingTableWidget";

        public NewCraftingTableWidget(IInventory inventory, NewComponentCraftingTable newcomponentCraftingTable) {
            m_newcomponentCraftingTable = newcomponentCraftingTable;
            XElement node = ContentManager.Get<XElement>("Widgets/NewCraftingTableWidget");
            LoadContents(this, node);
            m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
            m_craftingGrid = Children.Find<GridPanelWidget>("CraftingGrid");
            m_craftingResultSlot = Children.Find<InventorySlotWidget>("CraftingResultSlot");
            m_craftingRemainsSlot = Children.Find<InventorySlotWidget>("CraftingRemainsSlot");
            m_progress = Children.Find<ValueBarWidget>("Progress");
            int num = 10;
            for (int i = 0; i < m_inventoryGrid.RowsCount; i++) {
                for (int j = 0; j < m_inventoryGrid.ColumnsCount; j++) {
                    var inventorySlotWidget = new InventorySlotWidget();
                    inventorySlotWidget.AssignInventorySlot(inventory, num++);
                    m_inventoryGrid.Children.Add(inventorySlotWidget);
                    m_inventoryGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
                }
            }
            num = 0;
            for (int k = 0; k < m_craftingGrid.RowsCount; k++) {
                for (int l = 0; l < m_craftingGrid.ColumnsCount; l++) {
                    var inventorySlotWidget2 = new InventorySlotWidget();
                    inventorySlotWidget2.AssignInventorySlot(m_newcomponentCraftingTable, num++);
                    m_craftingGrid.Children.Add(inventorySlotWidget2);
                    m_craftingGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
                }
            }
            m_craftingResultSlot.AssignInventorySlot(m_newcomponentCraftingTable, m_newcomponentCraftingTable.ResultSlotIndex);
            m_craftingRemainsSlot.AssignInventorySlot(m_newcomponentCraftingTable, m_newcomponentCraftingTable.RemainsSlotIndex);
        }

        public override void Update() {
            m_progress.Value = m_newcomponentCraftingTable.m_craftingProgress;
            if (!m_newcomponentCraftingTable.IsAddedToProject) {
                ParentWidget.Children.Remove(this);
            }
        }
    }
}