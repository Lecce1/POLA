using UnityEngine;
using Sirenix.OdinInspector;

public class LobbyPlayerController : MonoBehaviour
{
    [Title("방향")] 
    [SerializeField] 
    private int direction;
    
    [Title("속도")] 
    [SerializeField] 
    private float speed = 5.0f;
    
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(0, 0, direction * speed * Time.deltaTime);
    }
    
    public void OnLeftDown()
    {
        direction = -1;
    }
    
    public void OnRightDown()
    {
        direction = 1;
    }

    public void OnBtnUp()
    {
        direction = 0;
    }
}