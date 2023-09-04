using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleController : MonoBehaviour
{
    private Color[] obstacleColor = { Color.red, Color.blue, Color.black};
    private MeshRenderer _meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshRenderer.material.color = obstacleColor[Random.Range(0,3)];
    }
}
