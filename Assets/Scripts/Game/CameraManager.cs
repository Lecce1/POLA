using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
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
        if (GameManager.instance.isCountDown && !GameManager.instance.isStart)
        {
            return;
        }

        transform.position = target.transform.position + offset;
    }
    
    public void Vibrate(float time)
    {
        shakeTime = time;
        StartCoroutine("VibrateCoroutine");
    }

    IEnumerator VibrateCoroutine()
    {
        while (shakeTime > 0)
        {
            transform.position = Random.insideUnitSphere * shakeAmount + transform.position;
            shakeTime -= Time.deltaTime;

            yield return null;
        }
    }
}
