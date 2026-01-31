using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    ParticleSystem particleSystem;
    float particleDuration;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleDuration = particleSystem.main.duration;
        Debug.Log("Particle Duration: " + particleDuration);
    }
    void Update()
    {
        Destroy(gameObject, 1f);
    }
}
