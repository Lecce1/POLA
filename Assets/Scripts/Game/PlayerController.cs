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
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField] 
    private GameObject camInfo;
    
    [FoldoutGroup("일반")] 
    public Animator anim;
    [FoldoutGroup("일반")] 
    public PlayerTrails trails;

    [FoldoutGroup("일반")] 
    public Verdict verdict;
    [FoldoutGroup("일반")]
    public LayerMask ground;

    private WaitForSeconds longNoteTime;
    private InputAction.CallbackContext callback = new ();
    
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
        
        camInfo.transform.position = new Vector3((hitInfo1.point.x + hitInfo2.point.x) / 2f, (hitInfo1.point.y + hitInfo2.point.y) / 2f, 0);
        groundGap = (hitInfo1.point - hitInfo2.point).magnitude;
        verdict.groundGap = groundGap;
    }

    public void Init()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = audioManager.bpm;
        anim = GetComponent<Animator>();
        isInvincibility = false;
        verdict = GetComponent<Verdict>();
        PlayerInput input = GetComponent<PlayerInput>();
        input.actions.FindAction("Up").canceled += OnKeyUp;
        input.actions.FindAction("Down").canceled += OnKeyUp;
        longNoteTime = new WaitForSeconds(7.5f / bpm);
        transform.position = transform.forward * Mathf.Floor(DBManager.instance.latency / 1000f * (60f / bpm)) / 100f;
    }
    
    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
    }

    public void Hurt(Obstacle info, bool isMiss)
    {
        verdict.ComboReset(info);
        GameManager.instance.hpList[health - 1].enabled = false;
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
                GameManager.instance.ShowVerdict(evaluation);
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
        if (GameManager.instance.isCountDown)
        {
            return;
        }
        
        if (!verdict.isUp)
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
        
        if (verdict.isUp)
        {
            OnFlip();
        }
        
        Interact();
    }

    void Interact()
    {
        int evaluation = verdict.KeyDown(out Obstacle targetInfo);

        if (targetInfo == null || evaluation == -1)
        {
            return;
        }
        
        if (evaluation == 3)
        {
            Hurt(targetInfo, true);
            verdict.DequeueUsedCollider(targetInfo);
            return;
        }
        
        switch (targetInfo.type)
        {
            case NoteType.MoveNote:
                break;
            
            case NoteType.NormalNote:
                if (targetInfo.beatLength != 0)
                {
                    verdict.isLongInteract = true;
                    StartCoroutine(LongNoteProcess(targetInfo, evaluation));
                    return;
                }
                
                Attack(targetInfo, evaluation);
                break;
            
            default: 
                return;
        }
        
        GameManager.instance.maxCombo++;
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
                GameManager.instance.maxCombo++;
                GameManager.instance.ShowVerdict(idx == 1 ? evaluation : 0);
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
        Debug.Log(idx);
    }
    
    void Attack(Obstacle targetInfo, int evaluation)
    {
        targetInfo.wasInteracted = true;
        GameManager.instance.ShowVerdict(evaluation);
        VibrateMobile();
        CameraManager.instance.VibrateForTime(0.1f);
        
        anim.SetInteger("AttackCounter", attackCounter++);
        anim.SetBool("isAttacking", true);
        attackCounter %= 2;
        
        if (targetInfo.gameObject.name == GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject.name && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
        
        verdict.DequeueUsedCollider(targetInfo);
        Destroy(targetInfo.gameObject);
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
        Obstacle obstacleInfo = Verdict.GetObstacle(other.gameObject);
        
        if (obstacleInfo != null)
        {
            if (obstacleInfo.type == NoteType.Wall && !obstacleInfo.wasInteracted && verdict.isUp == obstacleInfo.isUp)
            {
                Hurt(obstacleInfo, false);
            }
            else if (obstacleInfo.type == NoteType.Heart && verdict.isUp == obstacleInfo.isUp)
            {
                health++;
                Destroy(obstacleInfo.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Obstacle obstacleInfo = Verdict.GetObstacle(other.gameObject);
        
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
        
        if (obstacleInfo == Verdict.GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject) && health > 0 && !GameManager.instance.isResultPanel && obstacleInfo.wasInteracted)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
    }
}