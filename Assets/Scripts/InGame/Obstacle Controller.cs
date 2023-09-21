using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField]
    Color[] obstacleColor;
    
    [SerializeField]
    MeshRenderer meshRenderer;
    
    void Start()
    {
        obstacleColor = GameObject.Find("Player").GetComponent<PlayerController>().playerColors;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = obstacleColor[Random.Range(0,obstacleColor.Length)];
    }
}
