using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallDisplay : MonoBehaviour
{
    public string backWallDefaultMsg = "Height Limit";
    public TextMeshProUGUI backWallDisplay;
    public TextMeshProUGUI weightDisplay;
    public static WallDisplay Instance;
    public float displayMsgTimer = 10f;
    public GameObject progressPanel;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(Instance);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void DisplayOnBackwall(string str)
    {
        backWallDisplay.text = str;

        StartCoroutine(DisplayBackDefault(backWallDefaultMsg));
    }

    IEnumerator DisplayBackDefault(string str)
    {
        yield return new WaitForSeconds(displayMsgTimer);
        backWallDisplay.text = str;
    }

    public static void Display(string str)
    {
        if (Instance == null)
            return;
        Instance.DisplayOnBackwall(str);
    }


    void WeightDisplay(string str)
    {
        weightDisplay.text = str;
    }
    public static void DisplayWeight(string str)
    {
        if (Instance == null)
            return;
        Instance.WeightDisplay(str);
    }


    IEnumerator DisplayUnderProgressPanelCR()
    {
        progressPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        progressPanel.SetActive(false);
    }

    void DisplayUnderProgressPanel()
    {
        StartCoroutine(DisplayUnderProgressPanelCR());
    }

    public static void DisplayUnderProgress()
    {
        if (Instance == null)
            return;
        Instance.DisplayUnderProgressPanel();
    }
}
