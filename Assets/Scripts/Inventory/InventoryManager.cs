using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventorySystem;
using PlayerSystems;
using System;



public class InventoryManager : MonoBehaviour
    {

    [SerializeField]private Inventory _inventory;
    public Inventory inventory 
    {
        get
        {
            return _inventory;
        }
        set
        {
            _inventory = value;
            PlayerManager.Instance.inventoryCanvas.UpdateInventory();
            inventoryUpdateEvent?.Invoke(this,null);
        }
    }
    public event EventHandler inventoryUpdateEvent;
        private void Start()
        {
            inventory ??= new Inventory();
        inventory.items ??= new List<InventorySystem.InventoryItem>();
        }

        public InventorySystem.InventoryItem SearchItem(ItemData item)
        {
        return inventory.items.Find(x => x.item.id == item.id);
        }

        public void AddItem(ItemData item)
        {
        AddItem(item, 1);
        }

        public void AddItem(ItemData item, int quantity = 1)
        {
        InventorySystem.InventoryItem inventoryItem = SearchItem(item);

            if (inventoryItem != null)
            {
                inventoryItem.quantity += quantity;
            }
            else
            {
                inventoryItem = new InventorySystem.InventoryItem();
                inventoryItem.item = item;
                inventoryItem.quantity = quantity;

                inventory.items.Add(inventoryItem);
                inventoryItem = inventory.items.Last();

            }

            if (inventoryItem.quantity <= 0) RemoveItem(inventoryItem);
            //if(PlayerManager.Instance.inventoryCanvas.isActiveAndEnabled)PlayerManager.Instance.inventoryCanvas.UpdateInventory();
        }
        public void RemoveItem(InventorySystem.InventoryItem item)
        {
            inventory.items.Remove(item);
        }

        public void RemoveItem(ItemData item)
        {
        InventorySystem.InventoryItem inventoryItem = SearchItem(item);
            RemoveItem(inventoryItem);
        }

        public void ResetInventory()
        {
            inventory.items.Clear();
            Debug.Log("Inventory reset.");
        }

        public List<InventorySystem.InventoryItem> GetItems()
        {
            return inventory.items;
        }
    }


