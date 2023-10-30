using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{
    private Collider[] overlaps = new Collider[15];
    private Vector3 currentGrapplePosition;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool swinging = false;
    
    [FoldoutGroup("일반")]
    [SerializeField]
    private NewPlayerController player;
    
    [FoldoutGroup("일반")]
    public Transform hand;
    
    [FoldoutGroup("일반")]
    public LineRenderer lr;

    [FoldoutGroup("일반")]
    public Transform swingPoint;

    [FoldoutGroup("일반")] 
    public GameObject infoPlane;

    [FoldoutGroup("일반")] 
    public Coroutine coroutine = null;
    
    [FoldoutGroup("스윙")]
    public float maxSwingDistance = 45f;
    
    [FoldoutGroup("스윙")]
    [SerializeField]
    private SpringJoint joint;
    
    void Start()
    {
        player = GetComponent<NewPlayerController>();
    }
    
    void FixedUpdate()
    {
        DetectRopes();
    }
    
    private void LateUpdate()
    {
        DrawRope();
    }
    
    public void DetectRopes()
    {
        var overlap = Physics.OverlapSphereNonAlloc(player.transform.position, maxSwingDistance, overlaps);
        if (overlap == 0)
        {
            return;
        }
        
        Transform nearestGrapple = overlaps[0].transform;

        for (int i = 0; i < overlap; i++)
        {
            if (overlaps[i].CompareTag("Swing"))
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

        if (nearestGrapple.CompareTag("Swing"))
        {
            if (nearestGrapple != null && nearestGrapple != swingPoint)
            {
                swingPoint = nearestGrapple;
                infoPlane.SetActive(false);
            }
            
            if (!infoPlane.activeInHierarchy)
            {
                infoPlane.SetActive(true);
                coroutine = StartCoroutine(ShowInfo());
            }
        }
    }
    
    IEnumerator ShowInfo()
    {
        float start = Time.time;
        infoPlane.transform.position = swingPoint.position;
        var originPos = infoPlane.transform.position;
        
        while (start + 0.5f > Time.time)
        {
            var function = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuint);
            float process = function(0, 1, (Time.time - start) * 2);

            infoPlane.transform.position = originPos + Vector3.up * process;
            yield return null;
        }
    }
    
    public void OnSwingBtnClicked()
    {
        if (swingPoint != null && !swinging && GetComponent<PlayerGrappling>().grapplePoint == null && !player.isAttacking)
        {
            swingPoint.tag = "Ground";
            swinging = true;
            ExcecuteSwing();
        }
    }

    public void ExcecuteSwing()
    {
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint.position;

        float distanceFromPoint = Vector3.Distance(player.transform.position, swingPoint.position);
        
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;
        
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = hand.position;
        
    }
    
    public void OnStopSwingBtn()
    {
        if (swinging)
        {
            swinging = false;
            lr.positionCount = 0;
            swingPoint = null;
            Destroy(joint);
            infoPlane.SetActive(false);
        }
    }

    private void DrawRope()
    {
        if (!joint)
        {
            return;
        }
        
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint.position, Time.deltaTime * 8f);

        lr.SetPosition(0, hand.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}