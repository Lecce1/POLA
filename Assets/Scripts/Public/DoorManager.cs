using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorManager : MonoBehaviour
{
    [Title("잠금 오브젝트")]
    public GameObject lockObject;
    
    [FoldoutGroup("정보")] 
    [Title("챕터 번호")]
    public int chapterNum;
    
    [FoldoutGroup("정보")] 
    [Title("스테이지 번호")]
    public int stageNum;
    
    [FoldoutGroup("정보")] 
    [Title("문 이름")]
    public new string name;
    
    [FoldoutGroup("정보")] 
    [Title("잠금 여부")]
    public bool isLock;

    void Start()
    {
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        while (DBManager.instance.isJsonLoad == false)
        {
            yield return null;
        }
        
        switch (name)
        {
            case "Chapter":
                if (DBManager.instance.chapter >= chapterNum)
                {
                    if (lockObject.activeSelf)
                    {
                        lockObject.SetActive(false);
                    }

                    isLock = false;
                }
                else if (DBManager.instance.chapter < chapterNum)
                {
                    if (!lockObject.activeSelf)
                    {
                        lockObject.SetActive(true);
                    }
                    
                    isLock = true;
                }
                break;
        }
    }
}
