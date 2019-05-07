using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.MarchingCube.Movable_2
{
    [ExecuteAlways]
    public class Movable : MonoBehaviour
    {
        public bool reset;
        
        public PolygonCollider2D cd;
        
        public float mass;
        public Vector2 force;
        public Vector2 velocity;
        
        // Energy loss from collision and bumping.
        [Range(0, 1)] public float energyLossRate;
        
        // Minimum vertical speed required to cause a bumping. 
        public float minBumpSpeed;
        
        // Reduce speed by time.
        [Range(0, 1)] public float resistanceRate;
        
        public float detecitonRange;
        
        /// The preserve range does not affected by the scale.
        public float preserveRange;
        
        public int maxSimulateTimes;
        
        public List<Ray2D> rays;
        
        public void Start()
        {
            
        }
        
        void Update()
        {
        }
        
        // Physics Frame.
        void FixedUpdate()
        {
            Debug.Log("upd!");
            if(reset || rays == null) SetupRays();
            
            // The time we really simulated.
            float simulatedDuration = 0.0f;
            
            // Cache hits info for later computation.
            var hits = new RaycastHit2D[rays.Count];
            for(int i=0; i<rays.Count; i++)
            {
                hits[i] = Physics2D.Raycast(
                    (Vector2)this.transform.position + rays[i].origin - rays[i].direction.normalized * preserveRange,
                    rays[i].direction.normalized,
                    detecitonRange + preserveRange,
                    ~(1 << this.gameObject.layer)
                );
            }
            
            // Object moving phase.
            // If velocity is 0, we don't move this object further more.
            int simulatedTimes = 0;
            if(velocity.magnitude.NZ()) while(true)
            {
                simulatedTimes += 1;
                if(simulatedTimes == maxSimulateTimes + 1) break;
                
                // Notice even if step time is 0 and the position of this object doesn't change, the velocity may change.
                float stepTime = Time.fixedDeltaTime - simulatedDuration;
                
                Debug.DrawRay(
                    (Vector2)this.transform.position,
                    velocity,
                    Color.yellow
                );
                
                Debug.Log("simt " + simulatedTimes + " st " + stepTime + " v " + velocity);
                // get the minumum linear moving time span, during this time there's no collision and so on.
                for(int i=0; i<rays.Count; i++)
                {
                    var dir = rays[i].direction.normalized;
                    
                    // Ignore this ray if object's moving on its back direction.
                    if(dir.Dot(velocity).LEZ())
                    {
                        Debug.DrawRay(
                            (Vector2)this.transform.position + rays[i].origin - rays[i].direction.normalized * preserveRange,
                            rays[i].direction.normalized * (detecitonRange + preserveRange),
                            Color.green
                        );
                        continue;
                    }
                    
                    var limitTime = hits[i].collider == null ? stepTime : (hits[i].distance - preserveRange) / dir.Dot(velocity);
                    
                    Debug.Log(hits[i].distance + " " + preserveRange + " " + dir.Dot(velocity));
                    Debug.Log((hits[i].collider == null) + " dt " + stepTime + " v " + dir);
                    if(limitTime.LEZ())
                    {
                        Debug.Log(hits[i].point + " " + hits[i].distance);
                        Debug.DrawRay(
                            (Vector2)this.transform.position + rays[i].origin - rays[i].direction.normalized * preserveRange,
                            rays[i].direction.normalized * (detecitonRange + preserveRange),
                            Color.red
                        );
                    }
                    
                    stepTime = Mathf.Min(stepTime, limitTime);
                }
                
                Debug.Log("final " + stepTime);
                stepTime = Mathf.Max(0.0f, stepTime);
                
                if(stepTime != 0.0f)
                {
                    // Move the object by the step time.
                    this.transform.position += (Vector3)(stepTime * velocity);
                    
                    // hit information is also updated due to this move.
                    // May not be negative though...
                    for(int i=0; i<hits.Length; i++) hits[i].distance -= rays[i].direction.Dot(stepTime * velocity);
                }
                
                // Find a collsion if there's any.
                for(int i=0; i<rays.Count; i++)
                {
                    // Ray not hit...
                    if(hits[i].collider == null) continue;
                    
                    // Ray is not along the velocity direction. Maybe the object is leaving the collision point of this ray.
                    if(hits[i].normal.Dot(velocity).GEZ()) continue;
                    
                    // Ray hits but it is too far to be considered as collided.
                    if(hits[i].distance.G(preserveRange)) continue;
                    
                    // Setup a bouncy.
                    var normal = hits[i].normal;
                    var tangent = normal.Rot(0.5f * Mathf.PI);
                    float vertSpeed = -normal.Dot(velocity);
                    float horiSpeed = tangent.Dot(velocity);
                    velocity = vertSpeed * normal * energyLossRate.Sqrt() + horiSpeed * tangent;
                    
                    // Only one collision point will be considered.
                    break;
                }
                
                // The air resistance.
                velocity *= Mathf.Pow(resistanceRate, stepTime);
                simulatedDuration += stepTime;
                if(simulatedDuration.GE(Time.fixedDeltaTime)) break; 
            }
            
            if(simulatedTimes <= maxSimulateTimes)
            {
                // Acceleration adds here.
                velocity += (force / mass) * simulatedDuration;
            }
            else
            {
                // The object is stuck. Get it stuck still.
                velocity = Vector2.zero;
            }
        }
        
        void SetupRays()
        {
            rays = new List<Ray2D>();
            if(cd.pathCount != 1) Debug.LogWarning("Player collider should have a collider with only ONE path.");
            var pts = cd.GetPath(0);
            for(int i=0; i < pts.Length; i++)
            {
                var left = pts[i].To(pts[Util.ModSys(i - 1, pts.Length)]);
                var right = pts[i].To(pts[Util.ModSys(i + 1, pts.Length)]);
                var dir = (left.normalized + right.normalized) * 0.5f;
                rays.Add(new Ray2D((pts[i] * transform.localScale), -dir));
            }
        }
        
    }
}
