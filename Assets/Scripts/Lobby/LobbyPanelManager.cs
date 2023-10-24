using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyPanelManager : MonoBehaviour
{
    [Title("패널")] 
    public PanelItem panel = new PanelItem();
    
    [System.Serializable]
    public class PanelItem
    {
        public string name;
        public GameObject selectedObject;
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(panel.selectedObject);
    }
}
