using UnityEngine;
using UnityEngine.SceneManagement;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigid;
    private float bpm;
    private bool isJump = false;
    private float jumpForce = 500f;
    public AudioManager am;
    
    void Start()
    {
        bpm = am.bpm;
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJump)
            {
                Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }
    }

    void Move()
    {
        if (rigid.velocity.x > bpm / 15f - bpm * Time.fixedDeltaTime)
        {
            rigid.velocity = new Vector3(bpm / 15f, rigid.velocity.y, 0);
        }
        else
        {
            rigid.AddForce(Vector3.right * bpm);
        }
    }
    
    public void Jump()
    {
        isJump = true;
        rigid.AddForce(Vector3.up * jumpForce);
    }
    
    public void Attack()
    {
        float Distance = 4f;

        RaycastHit rayhit;

        char evaluation = 'F';
        
        if (Physics.Raycast(transform.position, transform.forward, out rayhit, Distance))
        {
            if (rayhit.transform.CompareTag("Breakable"))
            {
                Destroy(rayhit.transform.gameObject);

                float d = rayhit.transform.position.x - transform.position.x;

                if (d < 2f)
                {
                    evaluation = 'A';
                }
                else if (d < 3f)
                {
                    evaluation = 'B';
                }
                else if (d < 3.5f)
                {
                    evaluation = 'C';
                }
                else if (d < 3.75f)
                {
                    evaluation = 'D';
                }
                else if (d < 3.875f)
                {
                    evaluation = 'E';
                }
            }
            Debug.Log(evaluation);
        }
    }

    void Die()
    {
        Debug.Log(Time.time);
        SceneManager.LoadScene("Game 1");
    }

    private void OnCollisionEnter(Collision other)
    {
        isJump = false;

        if (other.transform.CompareTag("Breakable"))
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit rayhit;
        
        if (Physics.Raycast(transform.position, transform.forward, out rayhit, 4f))
        {
            Gizmos.DrawWireSphere(rayhit.point, 0.5f);
        }

        Gizmos.DrawRay(transform.position, transform.forward * 4f);
    }
}