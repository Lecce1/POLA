using Sirenix.OdinInspector;
using UnityEngine;

public class LobbyDoorManager : MonoBehaviour
{
    [FoldoutGroup("정보")] 
    [Title("문 이름")]
    public string name;
    
    [FoldoutGroup("정보")] 
    [Title("잠금 여부")] 
    [SerializeField]
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
