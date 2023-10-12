using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerGrappling : MonoBehaviour
{
    private Collider[] overlaps = new Collider[15];
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool grappling = false;
    
    [FoldoutGroup("일반")]
    [SerializeField]
    private PlayerController player;
    
    [FoldoutGroup("일반")]
    public Transform hand;
    
    [FoldoutGroup("일반")]
    public LineRenderer lr;

    [FoldoutGroup("일반")]
    public Transform grapplePoint;
    
    [FoldoutGroup("그래플링")]
    public float maxGrappleDistance = 25f;
    
    [FoldoutGroup("그래플링")]
    public float overshootYAxis = 2f;
    
    
    void Start()
    {
        player = GetComponent<PlayerController>();
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

        for (int i = 0; i < overlap; i++)
        {
            if (overlaps[i].CompareTag("Grappling"))
            {
                if (overlaps[i].transform.position.x > player.transform.position.x)
                {
                    grapplePoint = overlaps[i].transform;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 그래플 버튼 눌렀을 때
    /// </summary>
    public void OnGrappleBtnClicked()
    {
        if (grapplePoint != null && !grappling)
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
    }
}
