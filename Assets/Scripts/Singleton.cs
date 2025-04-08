using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : Component
{
    private static T _Instance;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = Object.FindFirstObjectByType(typeof(T)) as T;
                if (_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _Instance = obj.AddComponent<T>();
                }
            }
            return _Instance;
        }
    }
}

