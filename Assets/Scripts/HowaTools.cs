using UnityEngine;
using System.Collections;
using System;

namespace Howa
{
    public class HowaTools : MonoBehaviour
    {
        static public IEnumerator FrameDelayer(Action action, int frame)
        {
            for (int i = 0; i < frame; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action?.Invoke();
        }
    }
}
