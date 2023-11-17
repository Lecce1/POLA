using Sirenix.OdinInspector;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraInfo;
    public Vector3 offset;
    
    [FoldoutGroup("카메라 흔들림")]
    [Title("횟수")]
    [SerializeField]
    private float ShakeAmount;
    [FoldoutGroup("카메라 흔들림")]
    [Title("시간")]
    [SerializeField]
    private float ShakeTime;

    public static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (ShakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * ShakeAmount + transform.position;
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            ShakeTime = 0.0f;
        }
    }
    
    void FixedUpdate()
    {
        if (GameManager.instance.isCountDown)
        {
            return;
        }

        transform.position = cameraInfo.transform.position + offset;
    }
    
    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }
}
