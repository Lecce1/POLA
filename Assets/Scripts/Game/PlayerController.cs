using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
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
    private List<VerdictBar> verdictBarList;
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
        verdictBarList[2].onTriggerExitEvent += HandleGoodVerdictExit;
        longNoteTime = new WaitForSeconds(7.5f / bpm);
        Vibration.Init();
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
        
        camInfo.transform.position = new Vector3((hitInfo1.point.x + hitInfo2.point.x) / 2f, (hitInfo1.point.y + hitInfo2.point.y) / 2f, 0);
        groundGap = (hitInfo1.point - hitInfo2.point).magnitude;
        SetTransform(verdictBar, groundGap);
        var scale = verdictBar.transform.localScale;
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
        int i = isUp ? 1 : 0;
        GameObject target;
        
        if (verdictBarList[3].contacts[i].Count != 0 && verdictBarList[3].contacts[i].Peek() != null)
        {
            target = verdictBarList[3].contacts[i].Peek().gameObject;
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

        
        isLongInteract = false;
        int evaluation = GetVerdict(targetInfo.transform.GetChild(targetInfo.transform.childCount - 1).GetChild(0).gameObject, targetInfo);
        
        if (evaluation == -1)
        {
            evaluation = 3;
        }
        
        if (evaluation == 3)
        {
            Hurt(targetInfo, true);
        }

        if (targetInfo.transform.GetChild(targetInfo.transform.childCount - 1).GetChild(0).gameObject == verdictBarList[2].contacts[i].Peek().gameObject || targetInfo.transform.GetChild(targetInfo.transform.childCount - 2).GetChild(0).gameObject ==
            verdictBarList[2].contacts[i].Peek().gameObject)
        {
            GameManager.instance.combo += verdictBarList[2].contacts[i].Count;

            for (int j = 0; j < verdictBarList[2].contacts[i].Count; j++)
            {
                GameManager.instance.ShowVerdict(0);
            }
        }

        targetInfo.wasInteracted = true;
                
        for (int idx = 0; idx < 4; idx++)
        {
            if (verdictBarList[idx].contacts[i].Count != 0)
            {
                verdictBarList[idx].contacts[i].Dequeue();
            }
        }
                
        if (playerVerdict.contacts[i].Count != 0 && targetInfo == GetObstacle(playerVerdict.contacts[i].Peek().gameObject))
        {
            playerVerdict.contacts[i].Dequeue();
        }
        
        if (targetInfo == GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).GetChild(0).gameObject) && health > 0 && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
    }

    int GetVerdict(GameObject target, Obstacle targetInfo)
    {
        int idx = isUp ? 1 : 0;

        for (int i = 0; i < 4; i++)
        {
            foreach (var item in verdictBarList[i].contacts[idx])
            {
                if (targetInfo == GetObstacle(item.gameObject))
                {
                    if (verdictBarList[i].contacts[idx].Count != 0 && item.gameObject == target)
                    {
                        return i;
                    }
                }
                else
                {
                    break;
                }
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
        
        if (verdictBarList[3].contacts[i].Count != 0)
        {
            target = verdictBarList[3].contacts[i].Peek().gameObject;
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

        int evaluation = GetVerdict(target, targetInfo);

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
                    StartCoroutine(LongNoteProcess(targetInfo, target));
                    return;
                }
                
                targetInfo.wasInteracted = true;
                GameManager.instance.ShowVerdict(evaluation);
                
                for (int idx = 0; idx < 4; idx++)
                {
                    if (verdictBarList[idx].contacts[i].Count != 0)
                    {
                        verdictBarList[idx].contacts[i].Dequeue();
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
        
        GameManager.instance.combo++;
    }

    IEnumerator LongNoteProcess(Obstacle obstacle, GameObject firstTarget)
    {
        int i = isUp ? 1 : 0;
        GameObject target = firstTarget;
        
        while (isLongInteract)
        {
            if (DBManager.instance.isVibration)
            {
                if(Application.platform == RuntimePlatform.Android)
                {
                    Vibration.VibrateAndroid(100);
                }
                else if(Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
                }
            }

            if (target != verdictBarList[2].contacts[i].Peek().gameObject)
            {
                Debug.Log("test");
                GameManager.instance.combo++;
                GameManager.instance.ShowVerdict(0);
                target = verdictBarList[2].contacts[i].Peek().gameObject;
            }
            
            yield return null;

            int length = obstacle.transform.childCount;
            if (lastPassedObject != null && lastPassedObject.transform.parent.gameObject == obstacle.transform.GetChild(length - 1).gameObject && isLongInteract)
            {
                isLongInteract = false;
                Hurt(obstacle, true);
                if (obstacle == GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).GetChild(0).gameObject) && health > 0 && !GameManager.instance.isResultPanel)
                {
                    GameManager.instance.isResultPanel = true;
                    GameManager.instance.Invoke("Finish", 2.0f);
                }
                yield break;
            }
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
            while (verdictBarList[idx].contacts[i].Count != 0 && verdictBarList[idx].contacts[i].Peek() != null && obstacle == GetObstacle(verdictBarList[idx].contacts[i].Peek().gameObject))
            {
                verdictBarList[idx].contacts[i].Dequeue();
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
        if (DBManager.instance.isVibration)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                Vibration.VibrateAndroid(100);
            }
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
            }
        }
        
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
        
        if (obstacleInfo == GetObstacle(GameManager.instance.noteFolder.transform.GetChild(GameManager.instance.noteFolder.transform.childCount - 1).gameObject) && health > 0 && !GameManager.instance.isResultPanel && obstacleInfo.wasInteracted)
        {
            GameManager.instance.isResultPanel = true;
            GameManager.instance.Invoke("Finish", 2.0f);
        }
    }
}