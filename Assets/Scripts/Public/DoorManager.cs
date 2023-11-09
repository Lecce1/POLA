using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorManager : MonoBehaviour
{
    [FoldoutGroup("정보")] 
    [Title("챕터 번호")]
    public int chapterNum;
    
    [FoldoutGroup("정보")] 
    [Title("스테이지 번호")]
    public int stageNum;
    
    [FoldoutGroup("정보")] 
    [Title("문 이름")]
    public string name;
}
