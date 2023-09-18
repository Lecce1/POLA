using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract IEnumerator RunItem();
    public Material invisible;
    public GameObject player;
    public float duration = 4f;
}

public interface ITransparency
{
    void GetOpaque();
}
