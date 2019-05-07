using System;
using UnityEngine;

namespace Legacy.MarchingCube.Movable_1
{
    public class Movable : MonoBehaviour
    {
        public PolygonCollider2D cd;
        
        public Vector2 velocity;
        
        // Energy loss from collision and bumping.
        [Range(0, 1)] public float energyLossRate;
        
        // The always applied gravity.
        public float gravity;
        
        // Minimum vertical speed required to cause a bumping. 
        public float minBumpSpeed;
        
        // Reduce speed by time.
        [Range(0, 1)] public float resistanceRate;
        
        public void Start()
        {
            if(cd.pathCount != 1) Debug.LogWarning("Player collider should have a collider with only ONE path.");
        }
        
        void Update()
        {
            Vector2 move = velocity * Time.fixedDeltaTime;
            foreach(var anchor in cd.points)
            {
                var anchorPoint = anchor * transform.localScale + (Vector2)this.transform.position;
            }
        }
        
        // Physics Frame.
        void FixedUpdate()
        {
            velocity *= Mathf.Pow(resistanceRate, Time.fixedDeltaTime);
            
            // Find collision along speed.
            var hit = new RaycastHit2D();
            var finalAnchor = new Vector2();
            float minDist = 1e20f;
            
            // The move is considering the acceleration here.
            Vector2 move = velocity * Time.fixedDeltaTime + 0.5f * gravity * Time.fixedDeltaTime.Sqr() * Vector2.down;
            
            foreach(var anchor in cd.points)
            {
                var anchorPoint = anchor * transform.localScale + (Vector2)this.transform.position;
                var curHit = Physics2D.Raycast(anchorPoint, move, move.magnitude, ~(1 << this.gameObject.layer));
                Debug.DrawRay(anchorPoint - move, move * 2.0f, Color.blue);
                if(curHit.collider == null) continue;
                var dist = anchorPoint.To(curHit.point).magnitude;
                if(hit.collider == null || dist < minDist)
                {
                    minDist = dist;
                    hit = curHit;
                    finalAnchor = anchorPoint;
                }
            }
            
            if(hit.collider != null) Debug.DrawRay(finalAnchor - move, move * 2.0f, Color.red);
            
            velocity += gravity * Time.fixedDeltaTime * Vector2.down;
            
            
            // nothing collides. Just move.
            if(hit.collider == null)
            {
                this.transform.position += (Vector3)move;
                return;
            }
            
            // Something collides.
            // Get normal from this so we don't get wrong normal if ray origin of on the collider.
            var normal = Physics2D.Raycast(hit.point - move, move, move.magnitude * 2.0f, ~(1 << this.gameObject.layer)).normal.normalized;
            var tangent = normal.Rot(0.5f * Mathf.PI);
            
            var beforeSpeed = velocity.magnitude;
            var beforeTime = minDist / beforeSpeed;
            var beforeMove = minDist * move.normalized;
            
            var afterSpeed = velocity.magnitude * energyLossRate.Sqrt();
            var afterTime = Time.fixedDeltaTime - beforeTime;
            var afterMove = afterTime * afterSpeed * normal.Reflect(-velocity).normalized;
            
            // Here acceleration is taken into account.
            var afterVelocity = afterSpeed * afterMove.normalized;
            
            var vertVelocity = (-velocity).Dot(normal);
            if(vertVelocity < minBumpSpeed)
            {
                // remove vertical speed part from "after" things.
                afterSpeed *= afterMove.normalized.Dot(tangent);
                afterMove -= afterMove.Dot(normal) * normal;
                afterVelocity -= afterVelocity.Dot(normal) * normal;
            }
            
            this.transform.position += (Vector3)(beforeMove + afterMove);
            velocity = afterVelocity;
        }
        
    }
}
