using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    public float groundGap;

    [FoldoutGroup("변수")] 
    public int health;
    
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    
    [FoldoutGroup("변수")] 
    public bool isDead;
    
    [FoldoutGroup("변수")] 
    public int score;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private int attackCounter;

    [FoldoutGroup("변수")] 
    [SerializeField] 
    private GameObject camInfo;
    
    [FoldoutGroup("판정")] 
    [SerializeField] 
    private VerdictBar perfectVerdict;
    
    [FoldoutGroup("판정")] 
    [SerializeField] 
    private VerdictBar greatVerdict;
    
    [FoldoutGroup("판정")] 
    [SerializeField]
    private VerdictBar allVerdict;

    [FoldoutGroup("일반")] 
    [SerializeField]
    private GameObject verdictObject;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    private Vector3 verdictOriginPos;
    
    [FoldoutGroup("일반")] 
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    public LayerMask ground;
    
    [FoldoutGroup("일반")] 
    public GameObject target;
    
    void Start()
    {
        bpm = audioManager.bpm;
        rigid = GetComponent<Rigidbody>();
        originCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -9.81f, 0);
        isInvincibility = false;
        score = 0;
        verdictOriginPos = verdictObject.transform.position;
        
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Interact").canceled += OnInteractUp;
    }
    
    private void FixedUpdate()
    {
        if (!isDead)
        {
            Move();
        }
        
        RaycastHit hitInfo1, hitInfo2;
        
        if (!Physics.Raycast(new Ray(transform.position + transform.up, transform.up), out hitInfo1, 10, ground))
        {
            hitInfo1.point = transform.position + transform.up * 10f;
        }
        
        if(!Physics.Raycast(new Ray(transform.position + transform.up, -transform.up), out hitInfo2, 10, ground))
        {
            hitInfo2.point = transform.position - transform.up * 10f;
        }
        
        camInfo.transform.position = (hitInfo1.point + hitInfo2.point) / 2;
        groundGap = (hitInfo1.point - hitInfo2.point).magnitude;
        var scale = verdictObject.transform.localScale;
        scale.y = groundGap;
        verdictObject.transform.localScale = scale;
        verdictObject.transform.position = transform.position + transform.rotation * Vector3.forward + Vector3.up * (groundGap / 2);
    }

    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 15f * Time.fixedDeltaTime);
    }

    public void Hurt(int damage)
    {
        health -= damage;
        Debug.LogError(health);
        
        if (health <= 0)
        {
            Die();
            return;
        }
        
        isInvincibility = true;
        Invoke(nameof(ReleaseInvincibility), 0.5f);
    }
    
    /// <summary>
    /// 플립 버튼을 눌렀을때
    /// </summary>
    public void OnFlip()
    {
        Ray ray = new Ray(transform.position, transform.up);
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
        {
            Physics.gravity *= -1;
            StartCoroutine(trails.Trails());
            transform.position = hitInfo.point;
            transform.Rotate(transform.right, 180f);
        }
    }

    /// <summary>
    /// 상호작용 버튼을 뗐을때
    /// </summary>
    void OnInteractUp(InputAction.CallbackContext context)
    {
        Debug.Log("뗐다");
    }
    
    public void OnInteract()
    {
        if (allVerdict.contact != null)
        {
            target = allVerdict.contact.gameObject;
        }
        else
        {
            return;
        }

        Obstacle targetInfo = target.GetComponent<Obstacle>();

        switch (targetInfo.type)
        {
            case NoteType.MoveNote:
                break;
            
            case NoteType.NormalNote:
                Attack();
                break;
        }
    }

    public void Attack()
    {
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);
        attackCounter %= 2;
        Destroy(target);
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

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacleInfo = other.GetComponent<Obstacle>();
        
        if (obstacleInfo != null)
        {
            switch (obstacleInfo.type)
            {
                case NoteType.Heart:
                    if (health < 3)
                    {
                        health++;
                    }
                    
                    Destroy(other.gameObject);
                    break;
                
                case NoteType.Wall:
                    if (!obstacleInfo.isInteracted)
                    {
                        Hurt(obstacleInfo.damage);
                        obstacleInfo.isInteracted = true;
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Obstacle obstacleInfo = other.GetComponent<Obstacle>();
        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall && !obstacleInfo.isInteracted)
        {
            score += obstacleInfo.perfectScore;
        }
    }
}