using UnityEngine;
using System.Collections;

public class TrimParticlesOnCollision : SpellEffect
{

    private ParticleSystem particleSystem;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        particleSystem = GetComponent<ParticleSystem>();
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Debug.DrawRay(args.collPoints, Vector3.up * 5f, Color.red);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        particleSystem.GetParticles(particles);

            float distanceToCollPoint = Vector3.Distance(args.collPoints, transform.position);
        for (int i = 0; i < particles.Length; i++)
        {
            if (Vector3.Distance(transform.TransformPoint(particles[i].position), transform.position) > distanceToCollPoint)
                particles[i].lifetime = 0f;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }

    //public void OnParticleCollision(GameObject other)
    //{
    //    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
    //    particleSystem.GetParticles(particles);

    //    Debug.DrawRay(other.transform.position, Vector3.up * 5f, Color.cyan, 2f);

    //    ParticleCollisionEvent[] collEvent = new ParticleCollisionEvent[16];
    //    int events = particleSystem.GetCollisionEvents(other, collEvent);

    //    for (int i = 0; i < collEvent.Length; i++)
    //    {
    //        Color c = i == 0 ? Color.blue : Color.green;
    //        Debug.DrawRay(collEvent[i].intersection, Vector3.up * 5f, c, 2f);
    //    }
    //    float distanceToCollPoint = Vector3.Distance(collEvent[collEvent.Length - 1].intersection, transform.position);
    //    for (int i = 0; i < particles.Length; i++)
    //    {
    //        if (Vector3.Distance(transform.TransformPoint(particles[i].position), transform.position) > distanceToCollPoint)
    //            particles[i].lifetime = 0f;
    //    }

    //    particleSystem.SetParticles(particles, particles.Length);
    //}


}
