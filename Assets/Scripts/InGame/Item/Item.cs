using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract IEnumerator RunItem();
    public Material invisible;
}

public interface ITransparency
{
    void GetOpaque();
}
