using Sirenix.OdinInspector;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [FoldoutGroup("정보")] 
    [Title("스테이지 번호")]
    public int stageNum;
    
    [FoldoutGroup("정보")] 
    [Title("트랙 번호")]
    public int trackNum;
    
    [FoldoutGroup("정보")] 
    [Title("문 이름")]
    public string name;
}
