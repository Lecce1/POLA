using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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
    [Title("로딩 이미지")]
    [SerializeField]
    private Image loadingIcon;
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

    public static LoadingManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
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

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(DBManager.instance.nextScene);
        operation.allowSceneActivation = false;
        float timer = 0.0f;
        float minimumTimer = minimumLoadTime;

        while (!operation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            minimumTimer -= Time.deltaTime;

            if (operation.progress < 0.9f) 
            {
                currentProgress = Mathf.FloorToInt(operation.progress * 100f);
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

                if (!operation.allowSceneActivation)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
