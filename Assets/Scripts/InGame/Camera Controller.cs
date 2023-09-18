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
        VCM.m_Lens.FieldOfView = 40f;
    }
    
    void Update()
    {
        PovValue();
    }

    public void PovValue()
    {
        var playerSpeed = player.currentSpeed;

        VCM.m_Lens.FieldOfView = playerSpeed + 40;
    }
}
