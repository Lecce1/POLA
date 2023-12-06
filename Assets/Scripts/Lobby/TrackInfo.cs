using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

public class TrackInfo : SerializedMonoBehaviour
{
    [Title("트랙 제목")]
    public Text title_Text;
    [Title("스코어")]
    public Text score_Text;
    [Title("Perfect")]
    public Text perfect_Text;
    [Title("Great")]
    public Text great_Text;
    [Title("good")]
    public Text good_Text;
    [Title("miss")]
    public Text miss_Text;

    public static TrackInfo instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        int currentChapter = DBManager.instance.currentChapter;
        int currentStage = DBManager.instance.currentStage - 1;
        int currentStage2 = DBManager.instance.currentStage;
        title_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Track", $"Chapter{currentChapter}_Stage{currentStage2}_Title", LocalizationSettings.SelectedLocale);
        score_Text.text = DBManager.instance.stageArray[currentChapter].stage[currentStage].score.ToString();
        perfect_Text.text = DBManager.instance.stageArray[currentChapter].stage[currentStage].perfect.ToString();
        great_Text.text = DBManager.instance.stageArray[currentChapter].stage[currentStage].great.ToString();
        good_Text.text = DBManager.instance.stageArray[currentChapter].stage[currentStage].good.ToString();
        miss_Text.text = DBManager.instance.stageArray[currentChapter].stage[currentStage].miss.ToString();
    }
}
