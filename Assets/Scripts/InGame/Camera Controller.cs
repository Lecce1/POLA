using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] 
    private PlayerController player;
    
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
