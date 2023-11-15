using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    private GameObject verdictBar;
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
    [SerializeField]
    private GameObject lastPassedObject;

    private WaitForSeconds longNoteTime;
    private InputAction.CallbackContext callback = new ();

    void Start()
    {
        bpm = audioManager.bpm;
        anim = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -9.81f, 0);
        isInvincibility = false;
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Up").canceled += OnKeyUp;
        input.actions.FindAction("Down").canceled += OnKeyUp;
        verdictBar.transform.GetChild(2).GetComponent<VerdictBar>().onTriggerExitEvent += HandleGoodVerdictExit;
        longNoteTime = new WaitForSeconds(30f / bpm);
    }
    
    void FixedUpdate()
    {
        if (GameManager.instance.isCountDown)
        {
            return;
        }
        
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
        SetTransform(verdictBar, groundGap);
        var scale = verdictBar.gameObject.transform.localScale;
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
    }

    void HandleGoodVerdictExit(Collider other)
    {
        lastPassedObject = other.gameObject;
        Obstacle obstacleInfo = GetObstacle(lastPassedObject);
        
        if (obstacleInfo != null)
        {
            if (!obstacleInfo.wasInteracted && obstacleInfo.type == NoteType.NormalNote && !isLongInteract)
            {
                if (obstacleInfo.isUp == isUp)
                {
                    Hurt(obstacleInfo, true);
                }
                else
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
        transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
    }

    void Hurt(Obstacle info, bool isMiss)
    {
        ComboReset(info);
        
        if (isInvincibility)
        {
            return;
        }
        
        if (isMiss)
        {
            GameManager.instance.ShowVerdict(3);
        }
        health -= info.damage;

        if (health <= 0)
        {
            Die();
            return;
        }
        
        isInvincibility = true;
        Invoke(nameof(ReleaseInvincibility), 0.5f);
        
    }
    
    /// <summary>
    /// 플립이 될 때
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
    
    public void OnKeyUp()
    {
        OnKeyUp(callback);
    }

    /// <summary>
    /// 상호작용 버튼을 뗐을때
    /// </summary>
    void OnKeyUp(InputAction.CallbackContext context)
    {
        isLongInteract = false;
        int i = isUp ? 1 : 0;
        GameObject target;
        
        if (verdictBar.transform.GetChild(3).GetComponent<VerdictBar>().contacts[i].Count != 0)
        {
            target = verdictBar.transform.GetChild(3).GetComponent<VerdictBar>().contacts[i].Peek().gameObject;
        }
        else
        {
            return;
        }

        Obstacle targetInfo = GetObstacle(target);
        
        if (targetInfo == null || targetInfo.isUp != isUp || !isLongInteract)
        {
            return;
        }

        int evaluation = GetVerdict(targetInfo.transform.GetChild(targetInfo.transform.childCount - 1).GetChild(0).gameObject);

        if (evaluation == -1)
        {
            evaluation = 3;
        }
        
        if (evaluation == 3)
        {
            Hurt(targetInfo, true);
        }
        else
        {
            GameManager.instance.ShowVerdict(evaluation);
        }

        targetInfo.wasInteracted = true;
                
        for (int idx = 0; idx < 4; idx++)
        {
            VerdictBar bar = verdictBar.transform.GetChild(idx).GetComponent<VerdictBar>();
            
            if (bar.contacts[i].Count != 0)
            {
                bar.contacts[i].Dequeue();
            }
        }
                
        if (playerVerdict.contacts[i].Count != 0 && targetInfo == GetObstacle(playerVerdict.contacts[i].Peek().gameObject))
        {
            playerVerdict.contacts[i].Dequeue();
        }
    }

    int GetVerdict(GameObject target)
    {
        int idx = isUp ? 1 : 0;

        for (int i = 0; i < 4; i++)
        {
            VerdictBar bar = verdictBar.transform.GetChild(i).GetComponent<VerdictBar>();

            if (bar.contacts[idx].Count != 0 && target == bar.contacts[idx].Peek().gameObject)
            {
                return i;
            }
        }

        return -1;
    }

    public void OnUp()
    {
        if (GameManager.instance.isCountDown)
        {
            return;
        }
        
        if (!isUp)
        {
            OnFlip();
        } 
        
        Interact();
    }
    
    public void OnDown()
    {
        if (GameManager.instance.isCountDown)
        {
            return;
        }
        
        if (isUp)
        {
            OnFlip();
        }
        
        Interact();
    }

    public void Interact()
    {
        int i = isUp ? 1 : 0;

        GameObject target;
        
        if (verdictBar.transform.GetChild(3).GetComponent<VerdictBar>().contacts[i].Count != 0)
        {
            target = verdictBar.transform.GetChild(3).GetComponent<VerdictBar>().contacts[i].Peek().gameObject;
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

        int evaluation = GetVerdict(target);

        if (evaluation == 3)
        {
            Hurt(targetInfo, true);
            return;
        }
        
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
                
                targetInfo.wasInteracted = true;
                GameManager.instance.ShowVerdict(evaluation);
                
                for (int idx = 0; idx < 4; idx++)
                {
                    VerdictBar bar = verdictBar.transform.GetChild(idx).GetComponent<VerdictBar>();
            
                    if (bar.contacts[i].Count != 0)
                    {
                        bar.contacts[i].Dequeue();
                    }
                }
                
                if (playerVerdict.contacts[i].Count != 0 && targetInfo == GetObstacle(playerVerdict.contacts[i].Peek().gameObject))
                {
                    playerVerdict.contacts[i].Dequeue();
                }
                
                Attack(target);
                break;
            
            default: 
                return;
        }
        
        GameManager.instance.score += targetInfo.scoreList[evaluation];
        GameManager.instance.combo++;
    }

    IEnumerator LongNoteProcess(Obstacle obstacle)
    {
        int length = obstacle.transform.childCount;

        while (isLongInteract)
        {
            GameManager.instance.score += obstacle.scoreList[0];
            GameManager.instance.combo++;
            
            if (lastPassedObject != null && lastPassedObject.transform.parent.gameObject == obstacle.transform.GetChild(length - 1).gameObject && isLongInteract)
            {
                isLongInteract = false;
                Hurt(obstacle, true);
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
        int i = obstacle.isUp ? 1 : 0;
        
        for (int idx = 0; idx < 4; idx++)
        {
            VerdictBar bar = verdictBar.transform.GetChild(idx).GetComponent<VerdictBar>();
            
            while (bar.contacts[i].Count != 0 && bar.contacts[i].Peek() != null && obstacle == GetObstacle(bar.contacts[i].Peek().gameObject))
            {
                bar.contacts[i].Dequeue();
            }
        }

        if (playerVerdict.contacts[i].Count != 0 && playerVerdict.contacts[i].Peek() != null && obstacle == GetObstacle(playerVerdict.contacts[i].Peek().gameObject))
        {
            playerVerdict.contacts[i].Dequeue();
        }
        
        GameManager.instance.combo = 0;
    }

    public void Attack(GameObject target)
    {
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);
        attackCounter %= 2;
        
        if (target.transform.parent.gameObject.name == GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject.name && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
        
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
        if (EventSystem.current.currentSelectedGameObject != null && (GameManager.instance.esc.activeSelf || GameManager.instance.set.activeSelf) && GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (GameManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Toggle":
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                            DBManager.instance.isVibration = false;
                        }
                        else if (!EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
                            DBManager.instance.isVibration = true;
                        }
                        break;
                }
            }
            else
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    public void OnCancel()
    {
        if (!GameManager.instance.isResultPanel)
        {
            if (!GameManager.instance.isPanelOpen)
            {
                GameManager.instance.Button("Esc");
            }
            else
            {
                GameManager.instance.Back();
            }
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
        Destroy(gameObject.GetComponent<Rigidbody>());
        StopAllCoroutines();
        GameManager.instance.Finish();
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle obstacleInfo = GetObstacle(other.gameObject);
        
        if (obstacleInfo != null)
        {
            int i = isUp ? 1 : 0;

            if (obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted && isUp == obstacleInfo.isUp)
            {
                Hurt(obstacleInfo, false);
            }
            else if (obstacleInfo.type == NoteType.Heart)
            {
                health++;
                Destroy(obstacleInfo.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        int i = isUp ? 1 : 0;
        GameObject targetObj = null;
        
        if (playerVerdict.contacts[i].Count != 0 && playerVerdict.contacts[i].Peek() != null)
        {
            targetObj = playerVerdict.contacts[i].Peek().gameObject;
        }
        
        Obstacle obstacleInfo = GetObstacle(targetObj);

        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted)
        {
            Hurt(obstacleInfo, false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Obstacle obstacleInfo = GetObstacle(other.gameObject);
        
        if (obstacleInfo != null && obstacleInfo.type == NoteType.Wall)
        {
            if (obstacleInfo.beatLength == 0)
            {
                GameManager.instance.score += obstacleInfo.scoreList[0];
            }
            else
            {
                int length = obstacleInfo.gameObject.transform.childCount;
                
                if (other.transform.parent == obstacleInfo.gameObject.transform.GetChild(length - 1))
                {
                    GameManager.instance.score += obstacleInfo.scoreList[0];
                }
            }
        }
        
        if (other.transform.parent.gameObject.name == GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject.name && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
    }
}