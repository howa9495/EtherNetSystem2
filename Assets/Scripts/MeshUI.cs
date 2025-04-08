using System.Timers;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MeshUI : MonoBehaviour
{
    Camera meshCamera;
    [SerializeField] Transform meshPosition;
    [SerializeField] GameObject _meshObject;
    [SerializeField] float rotateSpeed = 1;
    public GameObject meshObject
    {
        get
        {
            return _meshObject;
        }
        set
        {
            if (_meshObject != null) Destroy(_meshObject);
            _meshObject = Instantiate(value, meshPosition.transform);
        }
    }
    void Awake()
    {
        meshCamera = GetComponentInChildren<Camera>();
    }

    public void RotateSelf( Vector2 value)
    {
        Vector3 direction = new Vector3(value.y, value.x, 0);
        meshPosition.transform.Rotate(direction*Time.unscaledDeltaTime*rotateSpeed);
    }

    public void ResetRotation()
    {
        meshPosition.transform.localEulerAngles = Vector3.zero;
    }


}
