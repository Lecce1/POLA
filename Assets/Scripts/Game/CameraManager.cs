using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    enum Type
    {
        Game,
        Latency
    }
    
    [Title("타입")]
    [SerializeField]
    private Type type;
    
    [FoldoutGroup("카메라")]
    [Title("타겟")]
    public GameObject target;
    [FoldoutGroup("카메라")]
    [Title("오프셋")]
    public Vector3 offset;
    
    [FoldoutGroup("카메라 흔들림")]
    [Title("횟수")]
    [SerializeField]
    private float shakeAmount = 0.1f;
    [FoldoutGroup("카메라 흔들림")]
    [Title("시간")]
    [SerializeField]
    private float shakeTime;

    [FoldoutGroup("플레이어")] 
    [Title("플레이어 컨트롤러")] 
    [SerializeField]
    private PlayerController playerController;

    public static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void FixedUpdate()
    {
        if (type == Type.Game)
        {
            if (GameManager.instance.isCountDown && !GameManager.instance.isStart)
            {
                return;
            }
            
            transform.position = target.transform.position + offset;
        }
        else if (type == Type.Latency)
        {
            transform.position = target.transform.position + offset;
        }
    }
    
    public void Vibrate(float time)
    {
        shakeTime = time;
        StartCoroutine("VibrateCoroutine");
    }

    IEnumerator VibrateCoroutine()
    {
        while (shakeTime > 0 && !GameManager.instance.isPanelOpen)
        {
            transform.position = Random.insideUnitSphere * shakeAmount + transform.position;
            shakeTime -= Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator ZoomInOnLongInteract()
    {
        var offsetNormal = offset.normalized;
        var originLength = offset.magnitude;
        var zoomInfunction = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutSine);
        var zoomOutFunction = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInSine);
        float originFOV = Camera.main.fieldOfView;
        float zoomTime = 0.1f;
        float progressed = 0;
        
        while (progressed < 1 && playerController.verdict.isLongInteract)
        {
            progressed += Time.deltaTime / zoomTime;
            progressed = Mathf.Clamp(progressed, 0, 1);
            offset = offsetNormal * zoomInfunction(originLength, originLength - 1, progressed);
            Camera.main.fieldOfView = zoomInfunction(originFOV, originFOV - 5, progressed);
            yield return null;
        }

        while (playerController.verdict.isLongInteract)
        {
            yield return null;
        }

        while (progressed > 0)
        {
            progressed -= Time.deltaTime / zoomTime;
            progressed = Mathf.Clamp(progressed, 0, 1);
            offset = offsetNormal * zoomOutFunction(originLength, originLength - 1, progressed);
            Camera.main.fieldOfView = zoomOutFunction(originFOV, originFOV - 5, progressed);
            yield return null;
        }

        offset = offsetNormal * originLength;
        Camera.main.fieldOfView = originFOV;
    }
}
