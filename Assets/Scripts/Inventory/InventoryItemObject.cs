using InventorySystem;
using PlayerSystems;
using UnityEngine;

public class InventoryItemObject : MonoBehaviour
{
    [SerializeField] ItemData data;
    [SerializeField] bool destorySelf = false;
    public void AddInventoryItem()
    {
        PlayerManager.Instance.inventoryManager.AddItem(data);
        if (destorySelf)Destroy(this.gameObject);
    }
}
