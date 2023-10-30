using Sirenix.OdinInspector;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    enum Type
    {
        UI,
        STAGE,
        TRACK,
    }

    [FoldoutGroup("정보")] 
    [Title("타입")]
    [SerializeField]
    private Type type;
    
    [FoldoutGroup("정보")] 
    [Title("트랙 번호")]
    public int trackNum;
    
    [FoldoutGroup("정보")] 
    [Title("문 이름")]
    public string name;
    
    [FoldoutGroup("정보")] 
    [Title("잠금 여부")]
    public bool isLock;
    
    [FoldoutGroup("리소스")] 
    [Title("잠금 이미지")] 
    [SerializeField]
    private Material lockMaterial;
    
    [FoldoutGroup("기타")] 
    [Title("머터리얼")] 
    [SerializeField]
    private MeshRenderer material;
    
    void Start()
    {
        Init();
    }

    void Init()
    {
        if (type == Type.STAGE)
        {
            material = transform.GetChild(0).GetComponent<MeshRenderer>();
        
            if (isLock)
            {
                material.material = lockMaterial;
            }
            else
            {
                material.material = null;
            }
        }
    }
}
