using System;
using UnityEngine;

namespace MarchingCube
{
    public class SkillMove : Skill
    {
        public float moveSpeed;
        
        public float jumpSpeed;
        
        // In whitch contact angle it is considered as "ground".
        public float groundAngle;
        
        public float accRate;
        
        float gravityScale;
        
        // Use default gravity.
        Vector2 gravity => Physics2D.gravity * gravityScale;
        
        // The default gravity direction is down.
        // Unity uses left-hand coordinates.
        // Reverse the angle here.
        Vector2 downDir => Vector2.down.Rot(-this.transform.eulerAngles.z);
        
        CoordSys localCoord => new CoordSys(downDir.Rot(0.5f * Mathf.PI), -downDir);
        
        Rigidbody2D rd => this.GetComponent<Rigidbody2D>();
        
        /// Used for post-command jumping.
        /// When player pressed JumpKey and the protagonist does not touching the ground,
        /// There's a count down that will make protagonist jump after reaching the ground.
        public float jumpCooldown;
        [SerializeField] float jumpCooldownTimer;
        
        ContactPoint2D[] recentContacts;
        public int contactCount;
        
        public string curStateDisplay;
        public string velocityDispaly;
        
        /// The protagonist is on the fly.
        bool standingStable
        {
            // Criterion: there *exists* a collision with angle between normal and gravity is less than groundAngle.
            get
            {
                for(int i=0; i<contactCount; i++)
                {
                    var pt = recentContacts[i].normal;
                    if(Vector2.Angle(pt, -gravity).LE(groundAngle)) return true; 
                }
                return false;
            }
        }
        
        /// The protagonist is standing on a surface, stable or not.
        /// This case should completely cover standingStable case.
        bool standing
        {
            // Criterion: there *exists* a collision with angles between normal and gravity is less than or equal to 90 degrees.
            get
            {
                for(int i=0; i<contactCount; i++)
                {
                    var pt = recentContacts[i].normal;
                    if(Vector2.Angle(pt, -gravity).LE(90)) return true; 
                }
                return false;
            }
        }
        
        bool flying => !standing;
        
        void Start()
        {
            gravityScale = 1.0f;
        }
        
        void Update()
        {
            // if(standingStable) gravityScale = 0.0f;
            // else gravityScale = 1.0f;
            
            const int contactLimit = 20;
            if(recentContacts == null) recentContacts = new ContactPoint2D[contactLimit];
            for(int i=0; i<recentContacts.Length; i++) recentContacts[i] = new ContactPoint2D();
            contactCount = rd.GetContacts(recentContacts);
            
            Debug.DrawRay(this.transform.position, -gravity, Color.black);
            foreach(var i in recentContacts) if(i.collider != null)
                Debug.DrawRay(this.transform.position, i.normal, Color.red);
            
            if(standingStable) curStateDisplay = "stable";
            else if(standing) curStateDisplay = "unstable";
            else curStateDisplay = "flying";
            
            velocityDispaly = rd.velocity.ToString();
            
            
            // Whatever player's behaviour is, the gravity applies.
            // The Rigidbody2D will handle collision, constraints and so on.
            // (if the speed is not too large...)
            rd.velocity += gravity * Time.deltaTime;
            
            if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) ActionStop();
            else if(Input.GetKey(KeyCode.A)) ActionLeft();
            else if(Input.GetKey(KeyCode.D)) ActionRight();
            else ActionStop();
            
            jumpCooldownTimer = Mathf.Max(0, jumpCooldownTimer - Time.deltaTime);
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) jumpCooldownTimer = jumpCooldown;
            ActionJump();
        }
        
        void ActionLeft()
        {
            ActionMove(new CoordSys(-localCoord.x, localCoord.y));
        }
        
        void ActionRight()
        {
            ActionMove(localCoord);
        }
        
        void ActionMove(CoordSys coord)
        {
            var v = coord.WorldToLocal(rd.velocity);
            if(flying)
            {
                var deltaX = moveSpeed - v.x;
                v.x += deltaX * (1.0f - accRate);
            }
            else if(standingStable)
            {
                var deltaX = moveSpeed - v.x;
                deltaX = Mathf.Max(deltaX, 0.0f);
                v.x += deltaX * (1.0f - accRate);
                v.y = 0;
            }
            else
            {
                var res = 1.0f;
                for(int i=0; i<contactCount; i++) if(recentContacts[i].collider != null)
                {
                    if(coord.WorldToLocal(recentContacts[i].normal).Dot(Vector2.right).GEZ()) continue;
                    res = Mathf.Min(res, coord.WorldToLocal(recentContacts[i].normal).Dot(Vector2.left));
                }
                v.x = res * moveSpeed;
            }
            
            rd.velocity = coord.LocalToWorld(v);
        }
        
        void ActionStop()
        {
            if(flying)
            {
                var v = localCoord.WorldToLocal(rd.velocity);
                v.x = 0;
                rd.velocity = localCoord.LocalToWorld(v);
            }
            else if(standingStable)
            {
                rd.velocity = Vector2.zero;
            }
            else
            {
                // do nothing...
            }
        }
        
        void ActionJump()
        {
            
            if(standingStable && jumpCooldownTimer != 0.0f)
            {
                rd.velocity -= downDir * jumpSpeed;
            }
        }
    }
    
}
