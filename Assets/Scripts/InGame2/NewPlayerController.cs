using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    public bool isGrounded;

    [FoldoutGroup("변수")] 
    public int Health;
    
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    
    [FoldoutGroup("변수")] 
    public bool isDead;
    
    [FoldoutGroup("변수")] 
    public bool isSlide;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private int attackCounter;
    
    [FoldoutGroup("변수")]
    [Title("점프")]
    public bool isFlip;
    
    [FoldoutGroup("일반")] 
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;
    
    [FoldoutGroup("일반")] 
    public PlayerParticle particle;

    [FoldoutGroup("일반")] 
    public GameObject cameraInfo;

    [FoldoutGroup("일반")] 
    private Ray ray;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    private LayerMask ground;
    
    [FoldoutGroup("일반")] 
    public GameObject verdictCube;
    
    void Start()
    {
        bpm = audioManager.bpm;
        rigid = GetComponent<Rigidbody>();
        originCollider = GetComponent<BoxCollider>();
        particle = GetComponent<PlayerParticle>();
        anim = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -9.81f, 0);
        isInvincibility = false;
        //transform.GetComponent<PlayerInput>().SwitchCurrentControlScheme("CONSOLE");
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Move();
        }
    }

    void Update()
    {
        KeyMapping();
    }

    /// <summary>
    /// 키보드 입력
    /// </summary>
    void KeyMapping()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isFlip)
            {
                OnFlip();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SlideOut();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.up * 10;
        }
    }

    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 15f * Time.fixedDeltaTime);
        ray = new Ray(transform.position, transform.up);
        RaycastHit hitInfo1, hitInfo2;
        
        if (!Physics.Raycast(new Ray(transform.position + transform.up, transform.up), out hitInfo1, 20, ground))
        {
            hitInfo1.point = transform.position + transform.up * 20f;
        }
        
        if(!Physics.Raycast(new Ray(transform.position + transform.up, -transform.up), out hitInfo2, 20, ground))
        {
            hitInfo2.point = transform.position - transform.up * 20f;
        }
        
        cameraInfo.transform.position = (hitInfo1.point + hitInfo2.point) / 2;

        if (transform.position.y < -5)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 점프 버튼을 눌렀을때
    /// </summary>
    public void OnFlip()
    {
        Physics.gravity *= -1;
        StartCoroutine(trails.Trails());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
        {
            transform.position = hitInfo.point;
            transform.Rotate(transform.right, 180f);
        }
    }

    /// <summary>
    /// 슬라이드 버튼을 눌렀을때
    /// </summary>
    public void Slide()
    {
        if (isDead)
        {
            return;
        }
        
        anim.SetBool("isSlide", true);
        originCollider.center /= 2;
        originCollider.size = new Vector3(1, 0.5f, 1);
    }

    /// <summary>
    /// 슬라이드 버튼을 뗐을때
    /// </summary>
    public void SlideOut()
    {
        anim.SetBool("isSlide", false);
        originCollider.center *= 2;
        originCollider.size = new Vector3(1, 1, 1);
    }

    public void OnAttack()
    {
        
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);

        attackCounter %= 2;
        float Distance = 5f;
        RaycastHit rayHit;
        char evaluation = 'F';

        Vector3 halfExtents = verdictCube.transform.localScale;
        halfExtents.x *= 2f;

        // if (Physics.BoxCast(transform.position, halfExtents))
        // {
        //     
        // }

        
    }
    
    /// <summary>
    /// 어택 애니메이션 종료 시 발생하는 메서드
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        anim.SetBool("isAttacking", false);
    }

    public void OnClick()
    {
        if (EventSystem.current.currentSelectedGameObject != null && GameManager.instance.set.activeSelf && GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
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

    void ReleaseInvincibility()
    {
        isInvincibility = false;
    }

    /// <summary>
    /// 죽었을 때 호출되는 메서드
    /// </summary>
    void Die()
    {
        if (isDead)
        {
            return;
        }
        
        anim.SetTrigger("Die");
        isDead = true;
        rigid.useGravity = false;
        Invoke(nameof(Reset), 2f);
        Destroy(gameObject.GetComponent<Rigidbody>());
        StopAllCoroutines();
    }

    private void Reset()
    {
        SceneManager.LoadScene(DBManager.instance.gameSceneName);
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
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(ray);
        Gizmos.color = Color.green;
        
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isDead)
        {
            isFlip = false;
            anim.SetBool("IsGrounded", true);
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        anim.SetBool("IsGrounded", false);
    }
}