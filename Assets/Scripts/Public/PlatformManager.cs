using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlatformManager : MonoBehaviour
    {
        [Title("키보드 오브젝트")] 
        public List<GameObject> keyboardObjects = new List<GameObject>();
        [Title("게임패드 오브젝트")] 
        public List<GameObject> gamepadObjects = new List<GameObject>();
        [Title("모바일 오브젝트")] 
        public List<GameObject> mobileObjects = new List<GameObject>();
        [Title("PlayerInput")] 
        public PlayerInput playerInput;

        void Start()
        {
            Init();
        }
        
        void Update()
        {
            if (SceneManager.GetActiveScene().name != DBManager.instance.gameSceneName)
            {
                Switch();
            }
        }
        
        void Init()
        {
            if (playerInput.currentControlScheme == "PC")
            {
                SwitchToKeyboard();
            }
            else if (playerInput.currentControlScheme == "CONSOLE")
            { 
                SwitchToGamepad();
            }
            else if (playerInput.currentControlScheme == "MOBILE")
            {
                SwitchToMobile();
            }
        }

        void Switch()
        {
            if (playerInput.currentControlScheme == "PC" && DBManager.instance.currentPlatform != "PC")
            {
                SwitchToKeyboard();
            }
            else if (playerInput.currentControlScheme == "CONSOLE" && DBManager.instance.currentPlatform != "CONSOLE")
            { 
                SwitchToGamepad();
            }
            else if (playerInput.currentControlScheme == "MOBILE" && DBManager.instance.currentPlatform != "MOBILE")
            {
                SwitchToMobile();
            }
        }
        
        public void SwitchToKeyboard()
        {
            if (DBManager.instance != null)
            {
                DBManager.instance.currentPlatform = "PC";
            }
            
            if (LobbyManager.instance.join_Btn.activeSelf)
            {
                LobbyManager.instance.join_Btn.SetActive(false);
            }
            
            for (int i = 0; i < gamepadObjects.Count; i++)
            {
                if (gamepadObjects[i] == null)
                    continue;

                gamepadObjects[i].SetActive(false);
            }
            
            for (int i = 0; i < mobileObjects.Count; i++)
            {
                if (mobileObjects[i] == null)
                    continue;

                mobileObjects[i].SetActive(false);
            }

            for (int i = 0; i < keyboardObjects.Count; i++)
            {
                if (keyboardObjects[i] == null)
                    continue;

                keyboardObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(keyboardObjects[i].GetComponentInParent<RectTransform>());
            }
            
            Cursor.visible = true;
        }

        public void SwitchToGamepad()
        {
            if (DBManager.instance != null)
            {
                DBManager.instance.currentPlatform = "CONSOLE";
            }
            
            if (LobbyManager.instance.join_Btn.activeSelf)
            {
                LobbyManager.instance.join_Btn.SetActive(false);
            }
            
            for (int i = 0; i < keyboardObjects.Count; i++)
            {
                if (keyboardObjects[i] == null)
                    continue;
                
                keyboardObjects[i].SetActive(false);
            }
            
            for (int i = 0; i < mobileObjects.Count; i++)
            {
                if (mobileObjects[i] == null)
                    continue;

                mobileObjects[i].SetActive(false);
            }
            
            for (int i = 0; i < gamepadObjects.Count; i++)
            {
                gamepadObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(gamepadObjects[i].GetComponentInParent<RectTransform>());
            }
            
            Cursor.visible = false;
        }

        public void SwitchToMobile()
        {
            if (DBManager.instance != null)
            {
                DBManager.instance.currentPlatform = "MOBILE";
            }
            
            if (!LobbyManager.instance.join_Btn.activeSelf)
            {
                LobbyManager.instance.join_Btn.SetActive(true);
            }
            
            for (int i = 0; i < keyboardObjects.Count; i++)
            {
                if (keyboardObjects[i] == null)
                    continue;
                
                keyboardObjects[i].SetActive(false);
            }
            
            for (int i = 0; i < gamepadObjects.Count; i++)
            {
                if (gamepadObjects[i] == null)
                    continue;

                gamepadObjects[i].SetActive(false);
            }

            for (int i = 0; i < mobileObjects.Count; i++)
            {
                if (mobileObjects[i] == null)
                    continue;

                mobileObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(mobileObjects[i].GetComponentInParent<RectTransform>());
            }
        }
    }