using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public void Awake()
    {
#if UNITY_STANDALONE
        Application.targetFrameRate = 40;
#elif UNITY_ANDROID || UNITY_WEBGL
    Application.targetFrameRate = 30;
#else
    Application.targetFrameRate = 40;
#endif
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
