using Sirenix.OdinInspector;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Color[] colorOptions;
    
    [FoldoutGroup("색상 정보")]
    [SerializeField]
    private bool isRandom;
    
    [FoldoutGroup("색상 정보")]
    [SerializeField]
    private int colorIndex;
    
    [FoldoutGroup("기타")]
    [SerializeField]
    private bool isDeadZone;
    

    void Start()
    {
        colorOptions = PlayerController.instance.GetPlayerColors();
        
        ApplySelectedColor();
    }

    void ApplySelectedColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        
        if (isDeadZone)
        {
            renderer.material.color = Color.black;
            return;
        }
        
        if (isRandom)
        {
            colorIndex = Random.Range(0, colorOptions.Length);
        }
        
        if (renderer != null)
        {
            renderer.material.color = colorOptions[colorIndex];
        }
    }
}