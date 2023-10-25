using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewPlayerController : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [Title("리지드바디")]
    [SerializeField]
    private Rigidbody rigid;
    
    [FoldoutGroup("일반")]
    [Title("콜라이더")]
    [SerializeField]
    private BoxCollider originCollider;
    
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
    [SerializeField]
    public AudioManager audioManager;
    
    [FoldoutGroup("변수")]
    [Title("점프")]
    [SerializeField]
    private bool isJump = false;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private float jumpForce = 500f;
    
    [FoldoutGroup("변수")]
    [Title("슬라이드")]
    [SerializeField]
    private bool isSlide = false;

    [FoldoutGroup("모드")]
    [SerializeField]
    private bool noteEditMod = false;
    
    void Start()
    {
        bpm = audioManager.bpm;
        noteEditMod = NoteMake.instance.noteEditMod;
        rigid = GetComponent<Rigidbody>();
        originCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        if (!noteEditMod)
        {
            KeyMapping();
        }
    }

    /// <summary>
    /// 키보드 입력
    /// </summary>
    void KeyMapping()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isJump)
            {
                Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isSlide)
            {
                Slide();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isSlide)
            {
                SlideOut();
            }
        }
    }

    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        if (rigid.velocity.x > bpm / 12f - bpm * Time.fixedDeltaTime)
        {
            rigid.velocity = new Vector3(bpm / 12f, rigid.velocity.y, 0);
        }
        else
        {
            rigid.AddForce(Vector3.right * bpm);
        }
    }
    
    /// <summary>
    /// 점프 버튼을 눌렀을때
    /// </summary>
    public void Jump()
    {
        isJump = true;
        rigid.AddForce(Vector3.up * jumpForce);
    }

    /// <summary>
    /// 점프 버튼을 꾹 눌렀을때
    /// </summary>
    public void Rope()
    {
        float Distance = 4f;
        float sphereScale = 15f;
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, sphereScale / 2.0f,transform.forward, out hit, Distance))
        {
            if (hit.transform.CompareTag("Rope"))
            {
                
            }
        }
    }

    /// <summary>
    /// 슬라이드 버튼을 눌렀을때
    /// </summary>
    public void Slide()
    {
        isSlide = true;
        originCollider.center = new Vector3(0, -0.25f, 0);
        originCollider.size = new Vector3(1, 0.5f, 1);
    }

    /// <summary>
    /// 슬라이드 버튼을 뗐을때
    /// </summary>
    public void SlideOut()
    {
        isSlide = false;
        originCollider.center = new Vector3(0, 0, 0);
        originCollider.size = new Vector3(1, 1, 1);
    }

    public void OnAttack()
    {
        float Distance = 5f;
        RaycastHit rayHit;
        char evaluation = 'F';
        
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, Distance))
        {
            if (rayHit.transform.CompareTag("Breakable"))
            {
                Destroy(rayHit.transform.gameObject);

                float d = rayHit.transform.position.x - transform.position.x;

                if (d < 2f)
                {
                    evaluation = 'A';
                }
                else if (d < 3f)
                {
                    evaluation = 'B';
                }
                else if (d < 3.5f)
                {
                    evaluation = 'C';
                }
                else if (d < 3.75f)
                {
                    evaluation = 'D';
                }
                else if (d < 3.875f)
                {
                    evaluation = 'E';
                }
            }
            Debug.Log(evaluation);
        }
    }

    public void OnClick()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void OnCancel()
    {
        if (!GameManager.instance.isPanelOpen)
        {
            GameManager.instance.Button("Set");
        }
        else
        {
            GameManager.instance.Back();
        }
    }

    /// <summary>
    /// 죽었을때
    /// </summary>
    void Die()
    {
        SceneManager.LoadScene("Game 1");
    }

    private void OnCollisionEnter(Collision other)
    {
        isJump = false;

        if (other.transform.CompareTag("Breakable"))
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit rayHit;
        
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, 4f))
        {
            Gizmos.DrawWireSphere(rayHit.point, 0.5f);
        }

        Gizmos.DrawRay(transform.position, transform.forward * 4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(NoteMake.instance.beatCount);
    }
}