using UnityEngine;
using System.Collections;

public class ParticlePlayer : MonoBehaviour 
{

	public ParticleSystem[] allParticles;
	public float lifetime = 1f;
    public bool destroyImmediately = true;

	void Start () 
	{
		allParticles = GetComponentsInChildren<ParticleSystem>();

        if (destroyImmediately)
        {
            Destroy(gameObject, lifetime);
        }
	}
	
	public void Play()
	{
		foreach (ParticleSystem ps in allParticles)
		{
			ps.Stop();
			ps.Play();
		}

        Destroy(gameObject, lifetime);
	}
}
