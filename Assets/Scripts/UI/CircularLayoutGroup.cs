using PlayerSystems;
using UnityEngine;
using System;
using canvasSystem;
using Howa;
using static Unity.VisualScripting.Metadata;
using InventorySystem;
using TMPro;

public class CircularLayout : MonoBehaviour
{
    public float radius = 100f;
    float startAngle = 90; // 從圓形的正下方開始生成第一個子物件
    public Vector2 circleCenter = Vector2.zero;
    public float rotationAngle = 0f;
    public float firstChildArcLength = 20f; // 第一個生成的子物件佔有的圓周角度
    GameObject selectedChildRect;
    [SerializeField]TextMeshProUGUI itemName, itemInfo;
    [SerializeField] MeshUI itemModel;

    int selectedIndex 
    {
        get
        {
            if(!inventoryCanvas) inventoryCanvas = PlayerManager.Instance.inventoryCanvas;
            return inventoryCanvas.selectedItem;
        }
    }
    InventoryCanvas inventoryCanvas;
    InventoryManager inventoryManager;
    EventHandler eventHandler;

    private void Awake()
    {
        
        inventoryCanvas = PlayerManager.Instance.inventoryCanvas;
        inventoryManager = PlayerManager.Instance.inventoryManager;
        SetUpEventHandler();
    }
    
    private void OnEnable()
    {
        inventoryCanvas.updateInventoryEvent += eventHandler;
        StartCoroutine(HowaTools.FrameDelayer(ArrangeInCircle,1));
    }
    private void OnDisable()
    {
        inventoryCanvas.updateInventoryEvent -= eventHandler;
    }
    void SetUpEventHandler()
    {
        eventHandler = (sender, e) =>
        {
            ArrangeInCircle();
        };
    }

    public void ArrangeInCircle()
    {
        Debug.Log("ArrangeInCircle");
        int childCount = transform.childCount;
        float remainingArcLength = 360f - firstChildArcLength;
        float angleStep = remainingArcLength / childCount;

        // 計算其他子物件的位置
        for (int i = 0; i < childCount; i++)
        {
            float angle;
            if (i==0) angle = startAngle + rotationAngle;
            else angle = startAngle +  + rotationAngle + i * angleStep + firstChildArcLength /2;
            float x = circleCenter.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = circleCenter.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

            int childDataOrder = (selectedIndex + i) >= childCount ? (selectedIndex + i - childCount) : (selectedIndex + i) < 0? (selectedIndex + i + childCount) : (selectedIndex + i);
            

            RectTransform childRect = transform.GetChild(childDataOrder).GetComponent<RectTransform>();
            childRect.anchoredPosition3D = new Vector3(x, 0, y);
            if (i == 0)
            {
                SetupTarget(childRect.gameObject);
            }

        }
    }
    public void SetupTarget(GameObject newTarget)
    {
        if (selectedChildRect) selectedChildRect.GetComponent<RectTransform>().localScale = Vector3.one;
        selectedChildRect = newTarget.gameObject;
        selectedChildRect.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 1.5f);
        InventorySystem.InventoryItem data = selectedChildRect.gameObject.GetComponent<InventorySlotUI>().data;
        itemName.text = data.item.name;
        itemInfo.text = data.item.info;
        Debug.Log(data.item.equipmentPrefab.gameObject);
        Debug.Log(itemModel.meshObject);
        itemModel.meshObject = data.item.equipmentPrefab.gameObject;
        Debug.Log(itemModel.meshObject);
        itemModel.meshObject.layer = 5;
        itemModel.meshObject.transform.localPosition -= itemModel.meshObject.GetComponent<Collider>().bounds.center- itemModel.meshObject.transform.position;
        itemModel.meshObject.TryGetComponent<MeshRenderer>(out MeshRenderer renderer);
        if(renderer) renderer.renderingLayerMask = 4;
        MeshRenderer[] allChildRenderers = itemModel.meshObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer child in allChildRenderers)
        {
            child.renderingLayerMask = 4;
            child.gameObject.layer = 5;

        }

        itemModel.ResetRotation();
    }

    /*
#if UNITY_EDITOR
    // 在 Unity 編輯器中更新子物件時自動重新排列
    void OnValidate()
    {
        ArrangeInCircle();
    }
#endif*/
}