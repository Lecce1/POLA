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
    public int health;
    
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    
    [FoldoutGroup("변수")] 
    public bool isDead;
    
    [FoldoutGroup("변수")] 
    public bool isSlide;
    
    [FoldoutGroup("변수")] 
    public bool wasTouched;
    
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
    [SerializeField]
    public LayerMask ground;

    [FoldoutGroup("일반")] 
    public int curEvaluation;

    [FoldoutGroup("일반")] 
    public GameObject target;
    
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

    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 15f * Time.fixedDeltaTime);
    }

    public void Hurt()
    {
        health--;
        if (health <= 0)
        {
            Die();
            return;
        }
        isInvincibility = true;
        Invoke(nameof(ReleaseInvincibility), 1f);
        
        
    }
    
    /// <summary>
    /// 플립 버튼을 눌렀을때
    /// </summary>
    void OnFlip()
    {
        Physics.gravity *= -1;
        StartCoroutine(trails.Trails());
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
        {
            if (target != null && target.CompareTag("Unbreakable"))
            {
                wasTouched = true;
            }
            transform.position = hitInfo.point;
            transform.Rotate(transform.right, 180f);
        }
    }

    /// <summary>
    /// 슬라이드 버튼을 눌렀을때
    /// </summary>
    void Slide()
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
    void AttackUp()
    {
        anim.SetBool("isSlide", false);
        originCollider.center *= 2;
        originCollider.size = new Vector3(1, 1, 1);
    }

    public void OnAttack()
    {
        if (curEvaluation == 0 || target == null)
        {
            return;
        }
        
        if (target.CompareTag("Breakable"))
        {
            anim.SetInteger("AttackCounter", attackCounter++);
            anim.SetBool("isAttacking", true);

            attackCounter %= 2;
            Destroy(target);
        }

        if (target.CompareTag("Slide"))
        {
            Slide();
        }
        
        wasTouched = true;
        curEvaluation = 0;
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
}