using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewPlayerController : MonoBehaviour
{
    private Rigidbody rigid;
    private float bpm = 80f;
    private bool isJump = false;
    private float jumpForce = 500f;
    public bool noteWriteMod;
    public List<float> noteTime = new();
    public GameObject note;
    private string notes;
    private string url;
    public AudioManager am;
    
    // Start is called before the first frame update
    void Start()
    {
        noteTime.Clear();

        if (System.IO.File.Exists(url))
        {
            string text = System.IO.File.ReadAllText(url);
            string[] lines = text.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                noteTime.Add( float.Parse(lines[i]));
            }

            for (int i = 0; i < noteTime.Count; i++)
            {
                Instantiate(note, new Vector3(1f + bpm * 0.005f + (noteTime[i] - 0.1f) * bpm * 0.1f, 0, 0), Quaternion.identity);
            }
        }
        
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!noteWriteMod)
        {
            Move();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJump && !noteWriteMod)
            {
                Jump();
            }

            if (noteWriteMod)
            {
                notes += Time.time + "\n";
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.E) && noteWriteMod)
        {
            notes = notes.TrimEnd('\n');
            System.IO.File.WriteAllText(url, notes, Encoding.Default);
        }
    }

    void Move()
    {
        foreach (Intervals interval in am.intervals)
        {
            float sampledTime = (am.audio.timeSamples / (am.audio.clip.frequency * interval.GetIntervalLength(bpm)));
            
            float beatsPerMinute = bpm;
            float secondsPerBeat = 60f / beatsPerMinute;
            float distancePerMove = 4f;
            float moveSpeed = distancePerMove / (secondsPerBeat / 4);

            if (rigid.velocity.x > moveSpeed - secondsPerBeat * Time.fixedDeltaTime)
            {
                rigid.velocity = new Vector3(moveSpeed, rigid.velocity.y, 0);
            }
            else
            {
                rigid.AddForce(Vector3.right * moveSpeed);
            }

            interval.CheckForNewInterval(sampledTime);
        }
    }
    
    public void Jump()
    {
        isJump = true;
        rigid.AddForce(Vector3.up * jumpForce);
    }
    
    public void Attack()
    {
        float Distance = 10f;

        RaycastHit rayhit;
        
        if (Physics.Raycast(transform.position, transform.forward, out rayhit, Distance))
        {
            if (rayhit.transform.CompareTag("Breakable"))
            {
                Destroy(rayhit.transform.gameObject);
            }
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
        
        if (Physics.Raycast(transform.position, transform.forward, out rayhit, 10f))
        {
            Gizmos.DrawWireSphere(rayhit.transform.position, 0.5f);
        }
    }
}