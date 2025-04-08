using UnityEngine;
using InventorySystem;
using System.Collections.Generic;
using PlayerSystems;
using Howa;
using System;
using System.ComponentModel;
namespace canvasSystem
{
    public class InventoryCanvas : BaseCanvas
    {

        [SerializeField] InventoryManager inventoryManager;
        [SerializeField] RectTransform inventortSlots;
        [SerializeField] InventorySlotUI slotPrefab;

        EventHandler eventHandler;

        public event EventHandler updateInventoryEvent;
        List<InventorySystem.InventoryItem> inventoryItems;

        int itemCcount = 0;
        [SerializeField] bool _equipItem = false;
        public bool equipItem
        {
            get
            {
                return _equipItem;
            }
            set
            {
                _equipItem = value;
            }
        }

        [SerializeField] private int _selecteditem = 0;
        public int selectedItem
        {
            get 
            {
                return _selecteditem;
            }
            set 
            {
                if (itemCcount > 0)
                {

                    if (value >= itemCcount) value -= itemCcount;
                    if (value < 0) value += itemCcount;
                    if(value!=_selecteditem)equipItem = false;
                }
                else
                { 
                    value = 0;
                }
                _selecteditem = value;
                updateInventoryEvent?.Invoke(this, null);
            }
        }

        private void Awake()
        {
            inventoryManager = PlayerManager.Instance.inventoryManager;
            if (inventoryManager == null) inventoryManager = new InventoryManager();
             SetUpEventHandler(); UpdateInventory();
        }
        void Start()
        {
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            if (eventHandler == null) SetUpEventHandler();
            inventoryManager.inventoryUpdateEvent += eventHandler;
            UpdateInventory();

        }
        void SetUpEventHandler()
        {
            eventHandler = (sender, e) =>
            {
                UpdateInventory();
            };
        }
        private void OnDisable()
        {
            inventoryManager.inventoryUpdateEvent -= eventHandler;
        }
        void CheckInventory()
        {
            if (inventoryManager == null) Debug.Log("No Inventory Manager"); return;
        }
        

        public void UpdateInventory()
        {
            CheckInventory();

            ClearSlots();

            UpdateInventoryData();
            //Debug.Log("InventoryItems: " + inventoryItems);
            //Debug.Log("InventoryItemsCount: " + inventoryItems.Count);
            if (inventoryItems == null) return;
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                InventorySystem.InventoryItem item = inventoryItems[i];

                if (item == null) return;
                InventorySlotUI slot = Instantiate(slotPrefab, inventortSlots.transform);
                slot.data = item;
                slot.gameObject.layer = 5;

            }
            SetUpEventHandler();
            UpdateInventoryData();
        }

        private void UpdateInventoryData()
        {
            inventoryItems = inventoryManager.GetItems();
            if (inventoryItems != null) itemCcount = inventoryItems.Count;
        }
        void ClearSlots()
        {
            foreach (Transform child in inventortSlots.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void ChangeSelectedItem(int changeValue)
        {
            selectedItem += changeValue;
        }

        public void SetEquipment()
        {
            equipItem = !equipItem;
        }
    }
}