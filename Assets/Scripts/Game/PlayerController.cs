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
    public int health;
    [FoldoutGroup("변수")] 
    public bool isInvincibility;
    [FoldoutGroup("변수")] 
    public bool isDead;
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
    [FoldoutGroup("일반")] 
    [SerializeField]
    private Obstacle keyUpCheckObstacle;
    [FoldoutGroup("일반")] 
    [SerializeField]
    private PlayerParticle playerParticle;
    [FoldoutGroup("일반")] 
    [SerializeField]
    SkinnedMeshRenderer[] playerMaterialList;

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

        camInfo.transform.position = new Vector3(transform.position.x, 0, 0);
    }

    public void Init()
    {
        playerMaterialList = GetComponentsInChildren<SkinnedMeshRenderer>();
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
        foreach(var temp in playerMaterialList)
        {
            temp.material.SetColor("_BaseColor", Color.red);
        }
        
        CameraManager.instance.Vibrate(0.25f);
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
        GameManager.instance.hp.sprite = GameManager.instance.hpList[health];

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
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(new Ray(transform.position + (transform.up * -5), transform.up), out hit, 10, ground);
        verdict.isUp = !verdict.isUp;
        transform.position = hit.point;
        transform.Rotate(transform.right, 180f);
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
        if (!isLoaded || !verdict.isLongInteract)
        {
            return;
        }

        if (keyUpCheckObstacle != null && keyUpCheckObstacle.transform.childCount != 0)
        {
            Hurt(keyUpCheckObstacle, true);
        }
        
        verdict.KeyUpOnLongInteract();
        keyUpCheckObstacle = null;
    }

    public void OnUp()
    {
        if (!isLoaded)
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
        if (!isLoaded)
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
        int evaluation = verdict.KeyDown(out Obstacle obstacle);

        playerParticle.AttackParticle();
        animator.SetTrigger("Attack");
        
        if (obstacle == null || evaluation == -1 || obstacle.wasInteracted || (GameManager.instance.isCountDown && !GameManager.instance.isStart))
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
        bool isFirst = true;
        keyUpCheckObstacle = obstacle;

        while (verdict.isLongInteract)
        {
            if (obstacle.transform.childCount != 0)
            {
                var longNote = obstacle.transform.GetChild(0).GetChild(0).gameObject;
                verdict.DequeueUsedCollider(longNote.gameObject);
                Destroy(longNote.transform.parent.gameObject);
                animator.SetTrigger("Attack");
                playerParticle.AttackParticle();
                playerParticle.DestroyPartice();
                audioManager.PlayAudio("Attack");
                
                if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
                {
                    VibrateMobile();
                }
                
                GameManager.instance.ShowVerdict(isFirst ? evaluation : 0, obstacle);
                isFirst = false;
                
                while (obstacle.transform.childCount != 0 && !verdict.CheckNextObject(obstacle.transform.GetChild(0).GetChild(0).gameObject))
                {
                    yield return null;
                }
            }
            else
            {
                OnKeyUp(callback);
                yield break;
            }
        }
    }
    
    void Attack(Obstacle obstacle, int evaluation)
    {
        obstacle.wasInteracted = true;
        GameManager.instance.ShowVerdict(evaluation, obstacle);
        
        verdict.DequeueUsedCollider(obstacle);
        //animator.SetTrigger("Attack");
        //playerParticle.AttackParticle();
        audioManager.PlayAudio("Attack");
        playerParticle.DestroyPartice();
        
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
        {
            VibrateMobile();
        }
        
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
    
    public void OnBack()
    {
        if (!GameManager.instance.isResultPanel && !GameManager.instance.isCountDown)
        {
            if (GameManager.instance.isPanelOpen)
            {
                GameManager.instance.Back();
            }
        }
    }

    public void OnCancel()
    {
        if (!GameManager.instance.isResultPanel && !GameManager.instance.isCountDown)
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
        
        foreach(var temp in playerMaterialList)
        {
            temp.material.SetColor("_BaseColor", Color.white);
        }
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
    }
}