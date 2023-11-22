using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

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

    [FoldoutGroup("스테이지")] 
    [Title("폴라로이드 머터리얼")]
    public MeshRenderer polaroid;
    [FoldoutGroup("스테이지")] 
    [Title("별 리스트")]
    public List<MeshRenderer> starList;
    [FoldoutGroup("스테이지")] 
    [Title("별 On Material")]
    public Material starOnMaterial;
    [FoldoutGroup("스테이지")] 
    [Title("별 Off Material")]
    public Material starOffMaterial;
    [FoldoutGroup("스테이지")] 
    [Title("제목")]
    public TMP_Text title_Text;
    [FoldoutGroup("스테이지")] 
    [Title("BPM")]
    public TMP_Text bpm_Text;

    void Start()
    {
        StartCoroutine("Init");
    }

    public IEnumerator Init()
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

    public void Stage_Init()
    {
        switch (name)
        {
            case "Stage":
                polaroid.material = DBManager.instance.stageArray[DBManager.instance.currentChapter]
                    .stage[stageNum - 1].polaroid;
                title_Text.text = DBManager.instance.stageArray[DBManager.instance.currentChapter]
                    .stage[stageNum - 1].name;
                bpm_Text.text = DBManager.instance.stageArray[DBManager.instance.currentChapter]
                    .stage[stageNum - 1].bpm.ToString();

                for (int i = 0; i < starList.Count; i++)
                {
                    if (i <= DBManager.instance.stageArray[DBManager.instance.currentChapter]
                            .stage[stageNum - 1].starCount - 1)
                    {
                        starList[i].material = starOnMaterial;
                    }
                    else
                    {
                        starList[i].material = starOffMaterial;
                    }
                }

                break;
        }
    }
}
