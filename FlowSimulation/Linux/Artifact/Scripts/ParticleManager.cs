using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ParticlePool))]
public class ParticleManager : MonoBehaviour
{
    const int hardParticleLimit = (int)2e6;
    
    ParticlePool pool;
    List<GameObject> particles = new List<GameObject>();
    
    public Vector2 emitVelocity;
    public Rect emitArea;
    [Range(0, 1)] public float generateDelay = 0.01f;
    [Range(0, 1)] public float generateDelayRandom;
    
    public Rect particlesValidRange;
    
    void Start()
    {
        pool = this.gameObject.GetComponent<ParticlePool>();
    }
    
    float t = 0; // time accumulator.
    void Update()
    {
        GenerateParticles();
        UpdateParticles();
    }
    
    
    void GenerateParticles()
    {
        if(generateDelay < 1e-5)
        {
            throw new Exception("Generating delay should not less than 1e-5.");
        }
        
        if(generateDelayRandom > generateDelay)
        {
            throw new Exception("Generating random delay should not greater than generating delay.");
        }
        
        t += Time.deltaTime;
        
        while(true)
        {
            float dt = generateDelay + (UnityEngine.Random.value * 2.0f - 1) * generateDelayRandom;
            if(dt > t) break;
            t -= dt;
            float rt = Time.deltaTime - t; // the accurate time of particle generation.
            
            var x = pool.Aquire();
            particles.Add(x);
            
            if(particles.Count > 2000000)
            {
                throw new Exception("You've allocated too many particles!");
            }
            
            var r = x.GetComponent<Rigidbody2D>();
            r.velocity = emitVelocity;
            x.transform.position = new Vector3(
                emitArea.xMin + UnityEngine.Random.value * emitArea.width + rt * r.velocity.x,
                emitArea.yMin + UnityEngine.Random.value * emitArea.height + rt * r.velocity.y,
                0 );
        }
    }
    
    void UpdateParticles()
    {
        for(int i=0; i<particles.Count; i++)
        {
            if(!particlesValidRange.Contains(particles[i].transform.position))
            {
                pool.Retire(particles[i]);
                particles[i] = particles[particles.Count - 1];
                particles.RemoveAt(particles.Count - 1);
            }
        }
    }
    
    
}
