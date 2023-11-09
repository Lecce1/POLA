using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class TrackInfo : SerializedMonoBehaviour
{
    [Title("트랙 제목")]
    public Text track_Title_Text;
    
    [Title("트랙 넘버")]
    public Text track_Num_Text;

    [FoldoutGroup("미션 1")] 
    [Title("아이콘")]
    public GameObject misson1_Icon;
    
    [FoldoutGroup("미션 1")] 
    [Title("성공 아이콘")]
    public GameObject misson1_Icon2;
    
    [FoldoutGroup("미션 1")] 
    [Title("내용")]
    public Text misson1_Content_Text;
    
    [FoldoutGroup("미션 1")] 
    [Title("성공 내용")]
    public GameObject misson1_Content_Success;
    
    [FoldoutGroup("미션 2")] 
    [Title("아이콘")]
    public GameObject misson2_Icon;
    
    [FoldoutGroup("미션 2")] 
    [Title("성공 아이콘")]
    public GameObject misson2_Icon2;
    
    [FoldoutGroup("미션 2")] 
    [Title("내용")]
    public Text misson2_Content_Text;
    
    [FoldoutGroup("미션 2")] 
    [Title("성공 내용")]
    public GameObject misson2_Content_Success;
    
    [FoldoutGroup("미션 3")] 
    [Title("아이콘")]
    public GameObject misson3_Icon;
    
    [FoldoutGroup("미션 3")] 
    [Title("성공 아이콘")]
    public GameObject misson3_Icon2;
    
    [FoldoutGroup("미션 3")] 
    [Title("내용")]
    public Text misson3_Content_Text;
    
    [FoldoutGroup("미션 3")] 
    [Title("성공 내용")]
    public GameObject misson3_Content_Success;
    
    [Title("시작 버튼")]
    public GameObject startBtn;
    
    public static TrackInfo instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Init(int stageNum, int trackNum)
    {
        track_Title_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Track", $"Stage{stageNum}_Track{trackNum}_Title", LocalizationSettings.SelectedLocale);
        track_Num_Text.text = trackNum.ToString();
        misson1_Content_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Track", $"Stage{stageNum}_Track{trackNum}_Mission_1", LocalizationSettings.SelectedLocale);
        misson2_Content_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Track", $"Stage{stageNum}_Track{trackNum}_Mission_2", LocalizationSettings.SelectedLocale);
        misson3_Content_Text.text = LocalizationSettings.StringDatabase.GetLocalizedString("Track", $"Stage{stageNum}_Track{trackNum}_Mission_3", LocalizationSettings.SelectedLocale);
    }
}
