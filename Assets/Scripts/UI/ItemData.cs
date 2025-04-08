using UnityEngine;
using static Enums;
using UnityEngine.UI;


namespace InventorySystem 
{
    [CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string _id;
        public string id
        {
             get { return _id; }
        }
        public string name;
        public Sprite sprite;
        public ItemType itemType;
        public string info;

        [Header("If the item can be equipped")]
        public GameObject equipmentPrefab;
    }
}



