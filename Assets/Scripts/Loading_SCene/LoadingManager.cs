using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;

public class LoadingManager : MonoBehaviour
{
    [FoldoutGroup("배경화면")]
    [Title("배경화면")]
    public Image background;
    [FoldoutGroup("배경화면")]
    [Title("리스트")]
    public List<Sprite> changeImage = new List<Sprite>();
    
    [FoldoutGroup("팁")]
    [Title("텍스트")]
    [SerializeField] 
    private Text tipText;
    [FoldoutGroup("팁")]
    [Title("리스트")]
    [SerializeField] 
    private List<string> tipList;
    
    [FoldoutGroup("기타")]
    [Title("씬")]
    public string sceneName;
    [FoldoutGroup("기타")]
    [Title("로딩 텍스트")]
    [SerializeField]
    private Text loadingText;
    [FoldoutGroup("기타")]
    [Title("최소 로딩 시간")]
    [SerializeField]
    private float minimumLoadTime = 3f;
    [FoldoutGroup("기타")]
    [Title("현재 로딩 진행률")]
    [SerializeField]
    private int currentProgress;

    private void Start()
    {
        StartCoroutine(LoadScene());
        BackGround();
        Tip();
    }

    void BackGround()
    {
        int num = Random.Range(0, changeImage.Count);
        background.sprite = changeImage[num];
    }

    void Tip()
    {
        tipText.text = tipList[Random.Range(0, tipList.Count)];
    }
    
    public void LoadScene(string name)
    {
        sceneName = name;
        SceneManager.LoadScene("Loading_Scene");
    }

    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        float minimumTimer = minimumLoadTime;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            minimumTimer -= Time.deltaTime;

            if (op.progress < 0.9f) 
            {
                currentProgress = Mathf.FloorToInt(op.progress * 100f);
                loadingText.text = string.Format("{0}%", currentProgress);
            }
            else
            {
                if(currentProgress < 100)
                {
                    currentProgress = Mathf.FloorToInt(Mathf.Lerp(currentProgress, 100, (minimumLoadTime - minimumTimer) / minimumLoadTime));

                    if (minimumTimer >= 0f && currentProgress >= 99)
                    {
                        loadingText.text = string.Format("99%");
                        continue;
                    }
                    else
                    {
                        loadingText.text = string.Format("{0}%", currentProgress);
                    }
                }

                if (minimumTimer >= 0f)
                    continue;

                if (!op.allowSceneActivation)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
