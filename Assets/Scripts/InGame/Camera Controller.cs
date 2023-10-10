using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [SerializeField] 
    private PlayerController player;
    
    [FoldoutGroup("일반")]
    [SerializeField] 
    private CinemachineVirtualCamera VCM;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        VCM = gameObject.GetComponent<CinemachineVirtualCamera>();
        VCM.m_Lens.FieldOfView = 60f;
    }
    
    void Update()
    {
        SpeedEffect();
    }

    public void SpeedEffect()
    {
        var playerSpeed = Mathf.Lerp(VCM.m_Lens.FieldOfView - 60f, player.currentSpeed, 0.02f);

        VCM.m_Lens.FieldOfView = (playerSpeed * 0.8f) + 60f;

        
    }
}
