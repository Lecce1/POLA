using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LoadingManager : MonoBehaviour
{
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
