using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryItem
    {
        public ItemData item;
        public int quantity;
    }


    [CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
    public class Inventory : ScriptableObject
    {
        public List<InventoryItem> items;

    }
}

