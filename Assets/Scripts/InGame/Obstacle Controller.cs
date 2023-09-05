using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleController : MonoBehaviour
{
    private Color[] obstacleColor;
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        obstacleColor = GameObject.Find("Player").GetComponent<PlayerController>().playerColors;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = obstacleColor[Random.Range(0,obstacleColor.Length)];
    }
}
