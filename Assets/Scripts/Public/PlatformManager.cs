using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlatformManager : MonoBehaviour
    {
        enum Type
        {
            Lobby,
            Game,
            Latency
        }
        
        [Title("키보드 오브젝트")] 
        public List<GameObject> keyboardObjects = new List<GameObject>();
        [Title("게임패드 오브젝트")] 
        public List<GameObject> gamepadObjects = new List<GameObject>();
        [Title("모바일 오브젝트")] 
        public List<GameObject> mobileObjects = new List<GameObject>();
        [Title("PlayerInput")] 
        public PlayerInput playerInput;
        [Title("현재 씬 타입")] 
        [SerializeField]
        private Type type;
        [Title("첫 번째 키 입력 여부")] 
        public bool isFirstKey;
        
        public static PlatformManager instance;

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
        
        void Update()
        {
            if (Input.anyKey && isFirstKey == false)
            {
                isFirstKey = true;
            }
            
            Switch();
        }
        
        void Init()
        {
            if (DBManager.instance.currentPlatform == "")
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
            else
            {
                if (DBManager.instance.currentPlatform == "PC")
                {
                    SwitchToKeyboard();
                }
                else if (DBManager.instance.currentPlatform == "CONSOLE")
                {
                    SwitchToGamepad();
                }
                else if (DBManager.instance.currentPlatform == "MOBILE")
                {
                    SwitchToMobile();
                }
            }

        }

        void Switch()
        {
            if (isFirstKey == false)
            {
                return;
            }
            
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
            
            if (type == Type.Lobby && LobbyManager.instance.join_Btn.activeSelf)
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
            
            if (type == Type.Lobby && LobbyManager.instance.join_Btn.activeSelf)
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
            
            if (type == Type.Lobby && !LobbyManager.instance.join_Btn.activeSelf)
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

                if (type == Type.Lobby && mobileObjects[i].name == "Back" && DBManager.instance.currentGround == 0)
                {
                    mobileObjects[i].SetActive(false);
                }
                else
                {
                    mobileObjects[i].SetActive(true);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(mobileObjects[i].GetComponentInParent<RectTransform>());
            }
        }
    }