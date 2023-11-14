using UnityEngine;

[ExecuteInEditMode]
public class ScaleParticles : MonoBehaviour
{
	void Update () 
	{
		var particleSystem = GetComponent<ParticleSystem>().main;
		particleSystem.startSize = transform.lossyScale.magnitude;
	}
}