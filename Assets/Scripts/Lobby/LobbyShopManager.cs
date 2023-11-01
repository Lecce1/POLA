using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyShopManager : MonoBehaviour
{
    [Title("현재 패널 인덱스")] 
    public int selectedPanelNum;
    [Title("탭 리스트")] 
    public List<GameObject> tabList;
    [Title("패널 리스트")] 
    public List<GameObject> panelList;
    [Title("패널 별 선택 할 오브젝트")] 
    public List<GameObject> selectObjectList;
    [Title("선택 된 컬러")] 
    [SerializeField]
    private Color selectedColor = new Color(246 / 255f, 168 / 255f, 20 / 255f, 255 / 255f);
    [Title("선택 안 된 컬러")] 
    [SerializeField]
    private Color unSelectedColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
    
    public static LobbyShopManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < tabList.Count; i++)
        {
            if (i == 0)
            {
                tabList[i].GetComponent<Text>().color = selectedColor;
                selectedPanelNum = 0;
            }
            else
            {
                tabList[i].GetComponent<Text>().color = unSelectedColor;
            }
        }
        
        for (int i = 0; i < panelList.Count; i++)
        {
            if (i == 0)
            {
                if (!panelList[i].activeSelf)
                {
                    panelList[i].SetActive(true);
                    EventSystem.current.SetSelectedGameObject(selectObjectList[i]);
                }
            }
            else
            {
                if (panelList[i].activeSelf)
                {
                    panelList[i].SetActive(false);
                }
            }
        }
    }
    
    public void Tab()
    {
        if (selectedPanelNum < tabList.Count - 1)
        {
            selectedPanelNum++;
        }
        else if (selectedPanelNum == tabList.Count - 1)
        {
            selectedPanelNum = 0;
        }

        Change();
    }
    
    public void Prev()
    {
        if (selectedPanelNum > 0)
        {
            selectedPanelNum--;
        }

        Change();
    }

    public void Next()
    {
        if (selectedPanelNum < tabList.Count - 1)
        {
            selectedPanelNum++;
        }
        
        Change();
    }

    void Change()
    {
        for (int i = 0; i < tabList.Count; i++)
        {
            if (i == selectedPanelNum)
            {
                if (tabList[i].GetComponent<Text>().color == unSelectedColor)
                {
                    tabList[i].GetComponent<Text>().color = selectedColor;
                }

                if (!panelList[i].activeSelf)
                {
                    panelList[i].SetActive(true);
                    EventSystem.current.SetSelectedGameObject(selectObjectList[i]);
                }
            }
            else
            {
                if (tabList[i].GetComponent<Text>().color == selectedColor)
                {
                    tabList[i].GetComponent<Text>().color = unSelectedColor;
                }
                
                if (panelList[i].activeSelf)
                {
                    panelList[i].SetActive(false);
                }
            }
        }
    }
}
