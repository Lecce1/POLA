using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
            Switch();
        }

        void Update()
        {
            Switch();
        }

        void Switch()
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

        public void SwitchToGamepad()
        {
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
        }

        public void SwitchToKeyboard()
        {
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
        }
        
        public void SwitchToMobile()
        {
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