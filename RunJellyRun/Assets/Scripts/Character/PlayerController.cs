using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine.Assertions.Must;

[Serializable]
public class PlayerController : CharacterController
{
    private Collision2D _collisionInfo;
    private Vector2 sNormal;
    private Vector2 playerDirectionOnSurface;
    private float angle;
    private float holdTime;
    private bool jump;
    private GameObject playerGO;
    public static int Count;
    private bool isJump=false;
    private bool forceUpdate;
    private float lastMagnitude = 0;
    private bool enterColl = false;
    private float lastDis;
    private Vector2 lastContact;
    private Vector2 lastPos;
    private float forceDown = 2;
    private RaycastHit2D hitInfo;
    private Vector2 lastG;
    
    public readonly double MaxSlopeAngle = 100;
    public readonly double MaxVelocity=6;

    void Start()
    {
        Count++;
        playerGO = transform.GetChild(0).gameObject;
        _rBody = GetComponent<Rigidbody2D>();
    }

    public void ResetPos()
    {
        lastContact=Vector2.zero;
        lastPos=Vector2.zero;
    }
    private void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (playerDirectionOnSurface.y>0)
        {
            _rBody.gravityScale = Configs.Gravity;
        }
        isJump = true;
        enterColl = true;
        _collisionInfo = collisionInfo;
        if (collisionInfo.contacts.Length==0) return;
        sNormal = collisionInfo.contacts[0].normal;
        lastContact = collisionInfo.contacts[0].point;
        Vector3 sVerNormal;
        sVerNormal = Quaternion.AngleAxis(-90, Vector3.forward) * new Vector3(sNormal.x,sNormal.y,0);
        playerDirectionOnSurface = sVerNormal;
        angle = Vector2.Angle(sNormal, Vector2.up);
        Physics2D.gravity=Vector2.down*-4;
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        forceUpdate = true;
       // isJump = true;
        _collisionInfo = collisionInfo;
        if (collisionInfo.contacts.Length==0) return;
        sNormal = collisionInfo.contacts[0].normal;
        Vector3 sVerNormal;
        //if (sNormal.x<0) return;
           sVerNormal = Quaternion.AngleAxis(-90, Vector3.forward) * new Vector3(sNormal.x,sNormal.y,0);   
        playerDirectionOnSurface = sVerNormal;
        angle = Vector2.Angle(sNormal, Vector2.up);

    }


    public void Update()
    {
        Vector2.ClampMagnitude(_rBody.velocity, 5.0f);
        //Debug.Log(sNormal);
      

        if (Input.GetKey(KeyCode.Space))
            holdTime += Time.fixedDeltaTime * 5;

        if (enterColl && Input.GetKeyUp(KeyCode.Space))
            jump = true;
        
        if (jump && isJump && enterColl)
        {
            //Physics2D.gravity = new Vector2(0,sNormal.y-1) * -4;
            forceUpdate = false;
            isJump = false;
            
            _rBody.AddForce(
                Rotate(sNormal, -45) *
                    (Configs.JumpPower *(2-((float)Math.Pow(playerDirectionOnSurface.y,2))) *(1 + Math.Min(holdTime, 0.75f))),
                    ForceMode2D.Impulse);
                    
/*            if (playerDirectionOnSurface.y > 0)
            {
                Debug.Log(-(playerDirectionOnSurface.y * 60));
                    _rBody.AddForce(
                    Rotate(sNormal, /*-(1+((float)Math.Pow(playerDirectionOnSurface.y,2)))*#1#45) *
                    (Configs.JumpPower *(2-((float)Math.Pow(playerDirectionOnSurface.y,2))) *(1 + Math.Min(holdTime, 0.75f))),
                    ForceMode2D.Impulse);
                    _rBody.gravityScale = Configs.Gravity + playerDirectionOnSurface.y-1;
                }
            else
            {
                _rBody.AddForce(
                    Rotate(sNormal, 1+(playerDirectionOnSurface.y*60)) *
                    (Configs.JumpPower *(1+Math.Abs(playerDirectionOnSurface.y)) * (1 + Math.Min(holdTime, 0.5f))),
                    ForceMode2D.Impulse);
            }*/
            // we may want to use the abs of playerDirectionOnSurface...
            
            jump = false;
            holdTime = 0;
            enterColl = false;
        }
       
        if (playerDirectionOnSurface.y<0 && _rBody.velocity.magnitude>MaxVelocity && _rBody.gravityScale>1)
        {
            _rBody.gravityScale -= 0.2f;
        }
        else
        {
            if (_rBody.gravityScale<Configs.Gravity)
            _rBody.gravityScale += 0.5f;
        }

        if (lastG==Vector2.zero)
        hitInfo=Physics2D.Raycast(new Vector2(this.transform.position.x,this.transform.position.y),Vector2.down,50,256);
        else
        {
            hitInfo = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), lastG, 50, 256);
        }
        if (!hitInfo.collider)
        {
            Physics2D.gravity = lastG;
            return;
        }

        if (hitInfo.distance > 5)
        {
            _rBody.AddForce(Rotate(Vector2.down, 10) * forceDown, ForceMode2D.Impulse);
            forceDown += 0.1f;          
        }
        else
            forceDown = 3;

        if (hitInfo.point != Vector2.zero )
        {
            if ( lastG!=Vector2.zero && Math.Abs(Vector2.Dot(lastG,hitInfo.normal)) < 0.01)
            {
                Vector3 crossVector =Vector3.Cross(new Vector3(0, 0, 1), new Vector3(hitInfo.normal.x, hitInfo.normal.y, 0));
                Vector2 gravityVec= new Vector2(Math.Abs(crossVector.x),-Math.Abs(crossVector.y));
                Physics2D.gravity = gravityVec*4;         
            }
            else
            {
                Physics2D.gravity = new Vector2(Math.Abs(hitInfo.normal.x),-Math.Abs(hitInfo.normal.y)) * 4;
                lastG = Physics2D.gravity;
            }
        }
        else
        {
            Physics2D.gravity = lastG;
        }
     
    }

    private void FixedUpdate()
    {

        Debug.Log(Physics2D.gravity);

        if (forceUpdate)
        {
            //Debug.Log(playerDirectionOnSurface);
            if (_rBody.velocity.magnitude < this.MaxVelocity)
            {
                _rBody.AddForce(
                    (playerDirectionOnSurface * (Configs.SlidingPower)) *
                    Time.fixedDeltaTime, ForceMode2D.Impulse);
                
/*                if (playerDirectionOnSurface.y > 0.05)
                {
                    _rBody.gravityScale = Configs.Gravity;
                    _rBody.AddForce(
                        (playerDirectionOnSurface * (Configs.SlidingPower * (1 + playerDirectionOnSurface.y))) *
                        Time.fixedDeltaTime, ForceMode2D.Impulse);
                }
                else
                {
                    if (playerDirectionOnSurface.y < 0.15 && playerDirectionOnSurface.y >= 0)
                    {
                        _rBody.gravityScale = Configs.Gravity;
                        _rBody.AddForce(
                            (new Vector2(0.5f,0.5f) * (Configs.SlidingPower * (1 + playerDirectionOnSurface.y))) *
                            Time.fixedDeltaTime, ForceMode2D.Impulse);
                    }
                    else
                    {
                        _rBody.gravityScale = Configs.Gravity + playerDirectionOnSurface.y-1;
                        _rBody.AddForce(
                            (new Vector2(playerDirectionOnSurface.x, playerDirectionOnSurface.y) *
                             Configs.SlidingPower * (1 + playerDirectionOnSurface.y)) * Time.fixedDeltaTime,
                            ForceMode2D.Impulse);
                    }

                }*/
            }


            if (_rBody.drag < 2.5)
            {
                _rBody.drag += .1f;
            }

            forceUpdate = false;
        }
        else
        {
            
        }     

    }

    public void Kill()
    {
        _rBody.isKinematic = true;
        Camera.main.transform.parent.GetComponent<CameraFollow>().Player = null;

    }
    Vector2 Rotate(Vector2 aPoint, float aDegree)
    {
        return Quaternion.Euler(0,0,aDegree) * aPoint;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(hitInfo.point, lastG*5);
    }
}

