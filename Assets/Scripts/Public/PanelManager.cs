using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    [Title("InputAction")] 
    public PlayerInput playerInput;
    
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
        if (playerInput.currentControlScheme != "MOBILE")
        {
            EventSystem.current.SetSelectedGameObject(panel.selectedObject);
        }
    }
}
