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
    public float groundGap;
    [FoldoutGroup("변수")] 
    public int health;
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    [FoldoutGroup("변수")] 
    public bool isDead;
    [FoldoutGroup("변수")]
    [SerializeField]
    private int attackCounter;
    [FoldoutGroup("변수")] 
    [SerializeField]
    private bool isLoaded = false;
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField] 
    private GameObject camInfo;
    
    [FoldoutGroup("일반")] 
    public Animator animator;
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;
    [FoldoutGroup("일반")] 
    public Verdict verdict;
    [FoldoutGroup("일반")]
    public LayerMask ground;

    private InputAction.CallbackContext callback = new ();
    
    void FixedUpdate()
    {
        if (!isLoaded)
        {
            return;
        }
        
        if (GameManager.instance.isCountDown && !GameManager.instance.isStart)
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
        
        camInfo.transform.position = new Vector3((hitInfo1.point.x + hitInfo2.point.x) / 2f, (hitInfo1.point.y + hitInfo2.point.y) / 2f, 0);
        groundGap = (hitInfo1.point - hitInfo2.point).magnitude;
        verdict.groundGap = groundGap;
    }

    public void Init()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = audioManager.bpm;
        animator = GetComponent<Animator>();
        isInvincibility = false;
        verdict = GetComponent<Verdict>();
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Up").canceled += OnKeyUp;
        input.actions.FindAction("Down").canceled += OnKeyUp;
        transform.position -= transform.forward * (DBManager.instance.latency * bpm / 7500f);
        isLoaded = true;
    }
    
    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
    }

    public void Hurt(Obstacle obstacle, bool isMiss)
    {
        verdict.ComboReset(obstacle);

        if (isInvincibility)
        {
            return;
        }
        
        if (isMiss)
        {
            GameManager.instance.ShowVerdict(3, obstacle);
        }
        
        health -= obstacle.damage;
        
        for (int i = 0; i < 3; i++)
        {
            if (i <= health - 1)
            {
                GameManager.instance.hpList[i].enabled = true;
            }
            else
            {
                GameManager.instance.hpList[i].enabled = false;
            }
        }

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
            if (verdict.isLongInteract)
            {
                OnKeyUp(callback);
            }
            
            Physics.gravity *= -1;
            verdict.isUp = !verdict.isUp;
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
        if (!verdict.isLongInteract)
        {
            return;
        }
        
        int evaluation = verdict.KeyUpOnLongInteract(out Obstacle obstacle);
        
        if (obstacle != null)
        {
            if (evaluation == 3)
            {
                Hurt(obstacle, true);
            }
            else
            {
                GameManager.instance.ShowVerdict(evaluation, obstacle);
            }
        }
        
        if (obstacle == Verdict.GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).GetChild(0).gameObject) && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke(nameof(GameManager.Finish), 2.0f);
        }
        
        verdict.DequeueUsedCollider(obstacle);
    }

    public void OnUp()
    {
        if (!verdict.isUp)
        {
            OnFlip();
        } 
        
        Interact();
    }
    
    public void OnDown()
    {
        if (verdict.isUp)
        {
            OnFlip();
        }
        
        Interact();
    }

    void Interact()
    {
        int evaluation = verdict.KeyDown(out Obstacle obstacle);

        if (obstacle == null || evaluation == -1)
        {
            return;
        }
        
        if (evaluation == 3)
        {
            Hurt(obstacle, true);
            return;
        }
        
        switch (obstacle.type)
        {
            case NoteType.MoveNote:
                break;
            
            case NoteType.NormalNote:
                if (obstacle.beatLength != 0)
                {
                    verdict.isLongInteract = true;
                    StartCoroutine(LongNoteProcess(obstacle, evaluation));
                    return;
                }
                
                Attack(obstacle, evaluation);
                break;
            
            default: 
                return;
        }
    }

    IEnumerator LongNoteProcess(Obstacle obstacle, int evaluation)
    {
        int idx = 0;
        GameObject longNote = obstacle.transform.GetChild(idx).GetChild(0).gameObject;
        
        while (verdict.isLongInteract)
        {
            if (idx < obstacle.transform.childCount)
            {
                longNote = obstacle.transform.GetChild(idx).GetChild(0).gameObject;
                VibrateMobile();
                GameManager.instance.ShowVerdict(idx == 1 ? evaluation : 0, obstacle);
            }
            else
            {
                Hurt(obstacle, true);
                OnKeyUp(callback);
                yield break;
            }
            
            while (!verdict.CheckNextObject(longNote) && verdict.isLongInteract)
            {
                yield return null;
            }

            idx++;
        }
    }
    
    void Attack(Obstacle obstacle, int evaluation)
    {
        obstacle.wasInteracted = true;
        GameManager.instance.ShowVerdict(evaluation, obstacle);
        VibrateMobile();
        animator.SetInteger("AttackCounter", attackCounter++);
        animator.SetBool("isAttacking", true);
        attackCounter %= 2;
        
        if (obstacle.gameObject.name == GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject.name && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
        
        verdict.DequeueUsedCollider(obstacle);
        Destroy(obstacle.gameObject);
    }

    void VibrateMobile()
    {
        if (DBManager.instance.isVibration)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                
            }
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                
            }
        }
    }
    
    /// <summary>
    /// 어택 애니메이션 종료 시 발생하는 메서드
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        animator.SetBool("isAttacking", false);
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
        
        animator.SetTrigger("Die");
        isDead = true;
        GetComponent<PlayerInput>().enabled = false;
        audioManager.audio.Stop();
        Destroy(gameObject.GetComponent<Rigidbody>());
        StopAllCoroutines();
        GameManager.instance.Finish();
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = Verdict.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            if (obstacle.type == NoteType.Wall && !obstacle.wasInteracted && verdict.isUp == obstacle.isUp)
            {
                Hurt(obstacle, false);
            }
            else if (obstacle.type == NoteType.Heart && verdict.isUp == obstacle.isUp)
            {
                health++;
                Destroy(obstacle.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Obstacle obstacle = Verdict.GetObstacle(other.gameObject);
        
        if (obstacle != null && obstacle.type == NoteType.Wall)
        {
            if (obstacle.beatLength == 0)
            {
                GameManager.instance.rankScore += obstacle.scoreList[0];
            }
            else
            {
                int length = obstacle.gameObject.transform.childCount;
                
                if (other.transform.parent == obstacle.gameObject.transform.GetChild(length - 1))
                {
                    GameManager.instance.rankScore += obstacle.scoreList[0];
                }
            }
        }
        
        if (obstacle == Verdict.GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject) && health > 0 && !GameManager.instance.isResultPanel && obstacle.wasInteracted)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke(nameof(GameManager.Finish), 2.0f);
        }
    }
}