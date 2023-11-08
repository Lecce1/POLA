using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
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

    [FoldoutGroup("판정")] 
    [SerializeField] 
    private VerdictBar playerVerdict;
    
    [FoldoutGroup("일반")] 
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;
    
    [FoldoutGroup("일반")]
    public LayerMask ground;
    
    [FoldoutGroup("일반")] 
    public GameObject target;

    [FoldoutGroup("일반")] 
    [SerializeField]
    private GameObject lastPassedObject;

    private WaitForSeconds longNoteTime;

    void Start()
    {
        bpm = audioManager.bpm;
        anim = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -9.81f, 0);
        isInvincibility = false;
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Up").canceled += OnKeyUp;
        input.actions.FindAction("Down").canceled += OnKeyUp;
        greatVerdict.onTriggerExitEvent += HandleGreatVerdictExit;
        longNoteTime = new WaitForSeconds(15f / bpm);
    }
    
    void FixedUpdate()
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
        SetTransform(perfectVerdict.gameObject, groundGap);
        SetTransform(greatVerdict.gameObject, groundGap);
        SetTransform(allVerdict.gameObject, groundGap);
        var scale = perfectVerdict.gameObject.transform.localScale;
        scale.y = groundGap;
        playerVerdict.transform.localScale = scale;
        playerVerdict.transform.position = transform.position + transform.up * groundGap / 2;
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
                    Hurt(obstacleInfo);
                }
                else if (obstacleInfo.isUp != isUp  && !obstacleInfo.wasInteracted)
                {
                    ComboReset(obstacleInfo);
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

    public void Hurt(Obstacle info)
    {
        if (isInvincibility)
        {
            return;
        }

        info.wasInteracted = true;
        ComboReset(info);
        health -= info.damage;
        GameManager.instance.StatUpdate();

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
    void OnKeyUp(InputAction.CallbackContext context)
    {
        Obstacle obstacle = GetObstacle(lastPassedObject);
        
        if (obstacle != null && obstacle.beatLength != 0 && isLongInteract)
        {
            int length = obstacle.gameObject.transform.childCount;
            int i = isUp ? 1 : 0;
            
            if (perfectVerdict.contact[i] != null && obstacle.transform.GetChild(length - 1).gameObject == perfectVerdict.contact[i].transform.parent.gameObject)
            {
                GameManager.instance.Score += obstacle.perfectScore;
            }
            else if (greatVerdict.contact[i] != null && obstacle.transform.GetChild(length - 1).gameObject == greatVerdict.contact[i].transform.parent.gameObject)
            {
                GameManager.instance.Score += obstacle.greatScore;
            }
            else
            {
                Hurt(obstacle);
            }
        }
        
        isLongInteract = false;
    }

    public void OnUp()
    {
        if (!isUp)
        {
            OnFlip();
        } 
        
        Interact();
    }
    
    public void OnDown()
    {
        if (isUp)
        {
            OnFlip();
        }
        
        Interact();
    }

    public void Interact()
    {
        int curScore = 0;
        int i = isUp ? 1 : 0; 
        
        if (allVerdict.contact[i] != null)
        {
            target = allVerdict.contact[i].gameObject;
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
        
        if (perfectVerdict.contact[i] != null && perfectVerdict.contact[i].gameObject == target)
        {
            curScore = targetInfo.perfectScore;
        }
        else if (greatVerdict.contact[i] != null && greatVerdict.contact[i].gameObject == target)
        {
            curScore = targetInfo.greatScore;
        }
        else
        {
            Hurt(targetInfo);
            return;
        }
        
        switch (targetInfo.type)
        {
            case NoteType.MoveNote:
                break;
            
            case NoteType.NormalNote:
                targetInfo.wasInteracted = true;
                
                if (targetInfo.beatLength != 0)
                {
                    isLongInteract = true;
                    StartCoroutine(LongNoteProcess(targetInfo));
                    return;
                }
                
                Attack();
                break;
        }
        
        GameManager.instance.Score += curScore;
        GameManager.instance.Combo++;
    }

    IEnumerator LongNoteProcess(Obstacle obstacle)
    {
        int length = obstacle.transform.childCount;

        while (isLongInteract)
        {
            GameManager.instance.Score += obstacle.perfectScore;
            GameManager.instance.Combo++;
            
            if (lastPassedObject != null && lastPassedObject.transform.parent.gameObject == obstacle.transform.GetChild(length - 1).gameObject && isLongInteract)
            {
                isLongInteract = false;
                Hurt(obstacle);
                yield break;
            }
            
            yield return longNoteTime;
        }
    }
    
    public static Obstacle GetObstacle(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }
        
        while (obj.transform != obj.transform.root)
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

    void ComboReset(Obstacle obstacle)
    {
        obstacle.wasInteracted = true;
        GameManager.instance.Combo = 0;
    }

    public void Attack()
    {
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);
        attackCounter %= 2;
        Destroy(target);

        if (target != null)
        {
            target = null;
        }
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
        GetComponent<PlayerInput>().enabled = false;
        audioManager.audio.Stop();
        Invoke(nameof(Reset), 2f);
        Destroy(gameObject.GetComponent<Rigidbody>());
        StopAllCoroutines();
    }

    void Reset()
    {
        SceneManager.LoadScene(DBManager.instance.gameSceneName);
    }

    void OnTriggerEnter(Collider other)
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

    void OnTriggerStay(Collider other)
    {
        int i = isUp ? 1 : 0;
        GameObject targetObj = null;
        
        if (playerVerdict.contact[i] != null)
        {
            targetObj = playerVerdict.contact[i].gameObject;
        }
        
        Obstacle obstacleInfo = GetObstacle(targetObj);

        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted)
        {
            Hurt(obstacleInfo);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Obstacle obstacleInfo = GetObstacle(other.gameObject);
        
        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall)
        {
            if (obstacleInfo.beatLength == 0)
            {
                GameManager.instance.Score += obstacleInfo.perfectScore;
            }
            else
            {
                int length = obstacleInfo.gameObject.transform.childCount;
                
                if (other.transform.parent == obstacleInfo.gameObject.transform.GetChild(length - 1))
                {
                    GameManager.instance.Score += obstacleInfo.perfectScore;
                }
            }
        }
    }
}