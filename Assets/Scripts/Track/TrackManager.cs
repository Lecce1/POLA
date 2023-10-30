using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrackManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("Info")] 
    public GameObject info;
    
    [FoldoutGroup("기타")] 
    [Title("Info 패널 온오프 여부")] 
    [SerializeField]
    public bool isInfoPanelOn = false;
    
    public static TrackManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        
    }
    
    public void Info_OnOff(bool isOn)
    {
        if (isOn == true)
        {
            info.GetComponent<Animator>().Play("InfoOn");
            isInfoPanelOn = true;
        }
        else if (isOn == false)
        {
            info.GetComponent<Animator>().Play("InfoOff");
            isInfoPanelOn = false;
        }
    }
    
    public void Button(string type)
    {
        switch (type)
        {
            case "Start":
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Game 1";
                }

                SceneManager.LoadScene("Loading");
                break;
        }
    }
}
