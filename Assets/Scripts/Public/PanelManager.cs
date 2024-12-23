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
        EventSystem.current.SetSelectedGameObject(panel.selectedObject);
    }
}
