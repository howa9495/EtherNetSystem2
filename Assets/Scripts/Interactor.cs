using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interactor : MonoBehaviour
{
    [SerializeField]GameObject canva;
    [SerializeField] TextMeshProUGUI intereactText;

    InteractableObject interactableObject;

    bool cacnShowHints = true;
    private bool _interact = false;
    public bool interact 
    {
        get
        {
            return _interact;
        }
        set
        {
            _interact = value;
        }
    }

    void Start()
    {
        if (canva)canva.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            hitInfo.transform.gameObject.TryGetComponent<InteractableObject>(out interactableObject);
            canva.SetActive(interactableObject && cacnShowHints);
            if (interactableObject == null) return;
            
            if(interactableObject) intereactText.text = interactableObject.interactAction+ " " + interactableObject.objectName;

            if (/*playerInputMapping.*/interact == true)
            {
                if(cacnShowHints) canva.SetActive(false);
                cacnShowHints = false;


                interactableObject.interactEvent?.Invoke();
                /*playerInputMapping.*/interact = false;
            }
        }
    }

}
