using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewPlayerController : MonoBehaviour
{
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
    public bool isUp;
    
    [FoldoutGroup("변수")] 
    public float groundGap;

    [FoldoutGroup("변수")] 
    public int health;
    
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    
    [FoldoutGroup("변수")] 
    public bool isDead;
    
    [FoldoutGroup("변수")] 
    public bool isLongInteract;
    
    [FoldoutGroup("변수")] 
    public int score;

    [FoldoutGroup("변수")]
    [SerializeField]
    private int Combo;
    
    [FoldoutGroup("변수")]
    public int combo
    {
        get { return Combo; }
        set
        {
            Combo = value;
            Debug.Log("Combo: " + Combo);
        }
    }
    
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
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    public LayerMask ground;
    
    [FoldoutGroup("일반")] 
    public GameObject target;

    [FoldoutGroup("일반")] 
    private GameObject lastPassedObject;
    
    void Start()
    {
        bpm = audioManager.bpm;
        anim = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -9.81f, 0);
        isInvincibility = false;
        score = 0;
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Interact").canceled += OnInteractUp;
        greatVerdict.onTriggerExitEvent += HandleGreatVerdictExit;
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
        var scale = perfectVerdict.gameObject.transform.localScale;
        scale.y = groundGap;
        SetTransform(perfectVerdict.gameObject, groundGap);
        SetTransform(greatVerdict.gameObject, groundGap);
        SetTransform(allVerdict.gameObject, groundGap);
    }

    void SetTransform(GameObject obj, float y)
    {
        var scale = obj.transform.localScale;
        scale.y = y;
        obj.transform.localScale = scale;
        obj.transform.position = transform.position + transform.forward + transform.up * (y / 2);
        obj.transform.rotation = transform.rotation;
    }

    void HandleGreatVerdictExit(Collider other)
    {
        lastPassedObject = other.gameObject;
        Obstacle obstacleInfo = GetObstacle(lastPassedObject);
        if (obstacleInfo != null)
        {
            if (obstacleInfo.wasInteracted == false && obstacleInfo.type == NoteType.NormalNote)
            {
                if (obstacleInfo.isUp == isUp  && !obstacleInfo.wasInteracted)
                {
                    Debug.Log("누르지 않고 그냥 지나침");
                    Hurt(obstacleInfo, obstacleInfo.damage);
                }
                else if (obstacleInfo.isUp != isUp  && !obstacleInfo.wasInteracted)
                {
                    ComboReset();
                }
            } 
        }
    }
    
    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 15f * Time.fixedDeltaTime);
    }

    public void Hurt(Obstacle info, int damage)
    {
        if (isInvincibility)
        {
            return;
        }

        info.wasInteracted = true;
        ComboReset();
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
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 20f, ground))
        {
            Physics.gravity *= -1;
            isUp = !isUp;
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
        isLongInteract = false;
    }
    
    public void OnInteract()
    {
        int curScore = 0;
        
        if (allVerdict.contact != null)
        {
            target = allVerdict.contact.gameObject;
        }
        else
        {
            return;
        }

        Obstacle targetInfo = GetObstacle(target);

        if (targetInfo == null || targetInfo.wasInteracted || targetInfo.isUp != isUp)
        {
            return;
        }
        
        if (perfectVerdict.contact != null && perfectVerdict.contact.gameObject == target)
        {
            curScore = targetInfo.perfectScore;
        }
        else if (greatVerdict.contact != null && greatVerdict.contact.gameObject == target)
        {
            curScore = targetInfo.greatScore;
        }
        else
        {
            Debug.Log("제대로 못누름");
            Hurt(targetInfo, targetInfo.damage);
            return;
        }
        
        score += curScore;
        targetInfo.wasInteracted = true;

        switch (targetInfo.type)
        {
            case NoteType.MoveNote:
                break;
            
            case NoteType.NormalNote:
                if (targetInfo.beatLength != 0)
                {
                    isLongInteract = true;
                    StartCoroutine(LongNoteProcess(targetInfo));
                    return;
                }
                Attack();
                break;
        }
    }

    IEnumerator LongNoteProcess(Obstacle obstacle)
    {
        int length = obstacle.transform.childCount;
        WaitForSeconds time = new WaitForSeconds(bpm / 240f);
        while (isLongInteract)
        {
            score += obstacle.perfectScore;
            combo++;
            Debug.Log(score);
            yield return time;

            if (lastPassedObject.transform.parent.gameObject == obstacle.transform.GetChild(length - 1).gameObject && isLongInteract)
            {
                isLongInteract = false;
                Debug.Log("마지막 노트를 지났는데도 안뗐음");
                Hurt(obstacle, obstacle.damage);
                yield break;
            }
        }
        
        if (perfectVerdict.contact != null && obstacle.transform.GetChild(length - 1).gameObject == perfectVerdict.contact.transform.parent.gameObject)
        {
            score += obstacle.perfectScore;
        }
        else if (greatVerdict.contact != null && obstacle.transform.GetChild(length - 1).gameObject == greatVerdict.contact.transform.parent.gameObject)
        {
            score += obstacle.greatScore;
        }
        else
        {
            Debug.Log("마지막 노트에서 제대로 못 뗐음");
            Hurt(obstacle, obstacle.damage);
        }
    }
    
    public Obstacle GetObstacle(GameObject obj)
    {
        while (obj.transform != obj.transform.parent)
        {
            Obstacle obstacle = obj.GetComponent<Obstacle>();
            
            if (obstacle != null)
            {
                return obstacle;
            }
            
            obj = obj.transform.parent.gameObject;
        }

        return null;
    }

    public void ComboReset()
    {
        combo = 0;
    }

    public void Attack()
    {
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);
        attackCounter %= 2;
        Destroy(target);
        target = null;
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
        Obstacle obstacleInfo = GetObstacle(other.gameObject);
        
        if (obstacleInfo != null && obstacleInfo.isUp == isUp && obstacleInfo.type == NoteType.Heart)
        {
            if (health < 3)
            {
                health++;
            }

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Obstacle obstacleInfo = GetObstacle(other.gameObject);

        if (obstacleInfo != null && obstacleInfo.isUp == isUp && obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted)
        {
            Debug.Log("벽에 맞음");
            Hurt(obstacleInfo, obstacleInfo.damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Obstacle obstacleInfo = GetObstacle(other.gameObject);
        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted)
        {
            score += obstacleInfo.perfectScore;
            Debug.Log(score);
        }
    }
}