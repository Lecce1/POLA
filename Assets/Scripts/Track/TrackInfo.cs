using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

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
        switch (stageNum)
        {
            case 1:
                switch (trackNum)
                {
                    case 1:
                        track_Title_Text.text = DBManager.instance.stage1_Track1_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 2:
                        track_Title_Text.text = DBManager.instance.stage1_Track2_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 3:
                        track_Title_Text.text = DBManager.instance.stage1_Track3_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 4:
                        track_Title_Text.text = DBManager.instance.stage1_Track4_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 5:
                        track_Title_Text.text = DBManager.instance.stage1_Track5_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 6:
                        track_Title_Text.text = DBManager.instance.stage1_Track6_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                }

                break;
            
            case 2:
                switch (trackNum)
                {
                    case 1:
                        track_Title_Text.text = DBManager.instance.stage2_Track1_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 2:
                        track_Title_Text.text = DBManager.instance.stage2_Track2_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 3:
                        track_Title_Text.text = DBManager.instance.stage2_Track3_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 4:
                        track_Title_Text.text = DBManager.instance.stage2_Track4_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 5:
                        track_Title_Text.text = DBManager.instance.stage2_Track5_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                    
                    case 6:
                        track_Title_Text.text = DBManager.instance.stage2_Track6_Title;
                        track_Num_Text.text = $"트랙 {trackNum}";
                        break;
                }

                break;
        }
        
        TrackManager.instance.btn_Type = "Start";
    }
}
