using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Title("레버")] 
    public RectTransform lever;
    [Title("레버 범위")] 
    [SerializeField]
    private float leverRange = 150;
    [Title("레버 방향")] 
    public Vector2 inputDir;
    [Title("RectTransform")] 
    [SerializeField]
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Focus();
        Move();
    }

    void Move()
    {
        if (TrackPlayerController.instance.isMoveAvailable)
        {
            TrackPlayerController.instance.playerVector = new Vector3(inputDir.x, 0, inputDir.y);

            if (TrackPlayerController.instance.playerVector.x == 0 && TrackPlayerController.instance.playerVector.z == 0)
            {
                TrackPlayerController.instance.isMove = false;
            }
            else
            {
                TrackPlayerController.instance.isMove = true;
            }
        }
        else
        {
            TrackPlayerController.instance.isMove = false;
        }
        
        TrackPlayerController.instance.anim.SetBool("isMove", TrackPlayerController.instance.isMove);
    }

    void Focus()
    {
        if(lever.anchoredPosition.x > 0 && lever.anchoredPosition.x < 150 && lever.anchoredPosition.y < 150 && lever.anchoredPosition.y > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (lever.anchoredPosition.x > 0 && lever.anchoredPosition.x < 150 && lever.anchoredPosition.y < 0 && lever.anchoredPosition.y > -150)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (lever.anchoredPosition.x > -150 && lever.anchoredPosition.x < 0 && lever.anchoredPosition.y < 0 && lever.anchoredPosition.y > -150)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (lever.anchoredPosition.x > -150 && lever.anchoredPosition.x < 0 && lever.anchoredPosition.y < 150 && lever.anchoredPosition.y > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        lever.anchoredPosition = Vector2.zero;
        inputDir = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
    }
}
