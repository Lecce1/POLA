using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerGrappling : MonoBehaviour
{
    private Collider[] overlaps = new Collider[15];
    
    [FoldoutGroup("변수")]
    [SerializeField]
    public bool grappling = false;
    
    [FoldoutGroup("일반")]
    [SerializeField]
    private NewPlayerController player;
    
    [FoldoutGroup("일반")]
    public Transform hand;
    
    [FoldoutGroup("일반")]
    public LineRenderer lr;

    [FoldoutGroup("일반")]
    public Transform grapplePoint;

    [FoldoutGroup("일반")] 
    public GameObject infoPlane;

    [FoldoutGroup("일반")] 
    public Coroutine coroutine = null;
    
    [FoldoutGroup("그래플링")]
    public float maxGrappleDistance = 45f;
    
    [FoldoutGroup("그래플링")]
    public float overshootYAxis = 2f;
    
    
    void Start()
    {
        player = GetComponent<NewPlayerController>();
    }
    
    void FixedUpdate()
    {
        DetectRopes();
    }

    /// <summary>
    /// 라인렌더러 실행
    /// </summary>
    void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, hand.position);
        }
    }

    /// <summary>
    /// 그래플 감지
    /// </summary>
    public void DetectRopes()
    {
        var overlap = Physics.OverlapSphereNonAlloc(player.transform.position, maxGrappleDistance, overlaps);
        if (overlap == 0)
        {
            return;
        }
        
        Transform nearestGrapple = overlaps[0].transform;

        for (int i = 0; i < overlap; i++)
        {
            if (overlaps[i].CompareTag("Grappling"))
            {
                if (overlaps[i].transform.position.x > player.transform.position.x)
                {
                    if ((overlaps[i].transform.position - player.transform.position).magnitude < (nearestGrapple.position - player.transform.position).magnitude)
                    {
                        nearestGrapple = overlaps[i].transform;
                    }
                }
            }
        }

        if (nearestGrapple.CompareTag("Grappling"))
        {
            if (nearestGrapple != null && nearestGrapple != grapplePoint)
            {
                grapplePoint = nearestGrapple;
                infoPlane.SetActive(false);
            }
            
            if (!infoPlane.activeInHierarchy)
            {
                infoPlane.SetActive(true);
                coroutine = StartCoroutine(ShowInfo());
            }
        }
    }
    
    /// <summary>
    /// 그래플링 감지 시 알림을 띄워주는 기능
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowInfo()
    {
        float start = Time.time;
        infoPlane.transform.position = grapplePoint.position;
        var originPos = infoPlane.transform.position;
        
        while (start + 0.5f > Time.time)
        {
            var function = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuint);
            float process = function(0, 1, (Time.time - start) * 2);

            infoPlane.transform.position = originPos + Vector3.up * process;
            yield return null;
        }
    }

    /// <summary>
    /// 그래플 버튼 눌렀을 때
    /// </summary>
    public void OnGrappleBtnClicked()
    {
        if (grapplePoint != null && !grappling && GetComponent<PlayerSwing>().swingPoint == null && !player.isAttacking)
        {
            grapplePoint.tag = "Ground";
            grappling = true;
            ExcecuteGrapple();
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint.position);
        }
    }
    
    /// <summary>
    /// 그래플 실행
    /// </summary>
    public void ExcecuteGrapple()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.position.y - lowestPoint.y;
        float highestPointOfArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOfArc = overshootYAxis;
        }

        player.JumpToPosition(grapplePoint.position, highestPointOfArc);

        Invoke(nameof(StopGrapple), 1f);
    }
    
    /// <summary>
    /// 그래플 정지
    /// </summary>
    public void StopGrapple()
    {
        grappling = false;
        lr.enabled = false;
        grapplePoint = null;
        infoPlane.SetActive(false);
    }
}
