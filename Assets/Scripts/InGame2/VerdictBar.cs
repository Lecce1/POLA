using System;
using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public Collider contact;

    private void Start()
    {
        contact = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        contact = other;
    }

    private void OnTriggerStay(Collider other)
    {
        contact = other;
    }

    private void OnTriggerExit(Collider other)
    {
        contact = null;
    }
}
