using UnityEngine;
using InventorySystem;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconUI;
    [SerializeField] private TextMeshProUGUI quantityUI;

    [SerializeField] private InventorySystem.InventoryItem _data;
    public InventorySystem.InventoryItem data 
    {
        get
        {
            return _data;
        }
        set 
        {
            _data = value;
            iconUI.sprite = _data.item.sprite;
            quantityUI.text = _data.quantity.ToString();

        }
    }


    void Awake()
    {
        iconUI = GetComponentInChildren<Image>();
        quantityUI = GetComponentInChildren<TextMeshProUGUI>();

    }

    
}
