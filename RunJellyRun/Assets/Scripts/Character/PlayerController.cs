using System;
using UnityEngine;
using System.Collections;

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
    
    public readonly double MaxSlopeAngle = 100;
    public readonly double MaxVelocity = 5;

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
        isJump = true;
        enterColl = true;
        // isJump = true;
        _collisionInfo = collisionInfo;
        if (collisionInfo.contacts.Length==0) return;
        sNormal = collisionInfo.contacts[0].normal;
        lastContact = collisionInfo.contacts[0].point;
        Vector3 sVerNormal;
        //if (sNormal.x<0) return;
        sVerNormal = Quaternion.AngleAxis(-90, Vector3.forward) * new Vector3(sNormal.x,sNormal.y,0);
        playerDirectionOnSurface = sVerNormal;
        //Debug.Log(playerDirectionOnSurface);
        //angle = Vector3.Angle(sNormal, Vector3.up);
        angle = Vector2.Angle(sNormal, Vector2.up);
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
        //Debug.Log(playerDirectionOnSurface);
        //angle = Vector3.Angle(sNormal, Vector3.up);
        angle = Vector2.Angle(sNormal, Vector2.up);

    }


    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            holdTime += Time.deltaTime * 5;

        if (enterColl && Input.GetKeyUp(KeyCode.Space))
            jump = true;
        
        if (jump && isJump && enterColl)
        {
            isJump = false;
            _rBody.AddForce(
                new Vector2(0.5f,1)*
                (Configs.JumpPower * (1+Math.Min(holdTime,0.5f))*(playerDirectionOnSurface.y+1)),
                ForceMode2D.Impulse);

            jump = false;
            holdTime = 0;
            enterColl = false;
        }
        
    }

    private void FixedUpdate()
    {
        if (!forceUpdate) return;

        if (lastPos == lastContact)
            this.transform.Translate(0.1f, 0.1f, 0);

        float dis = Vector2.Distance(lastContact, this.transform.position);
        Vector2 currentPos = new Vector2(this.transform.position.x, this.transform.position.y);
        
        if (lastContact != null && dis > 12 && Vector2.Distance(currentPos,lastPos)>0.5)
            this.transform.position = lastPos;
        else
            lastPos = this.transform.position;
        
        if (_rBody.velocity.magnitude < MaxVelocity || _rBody.velocity.x<0)
        {
            _rBody.gravityScale = Configs.Gravity;
            _rBody.drag = 0;

            if (isJump)
                if (playerDirectionOnSurface.x>0)
                    _rBody.AddForce(
                        (playerDirectionOnSurface * (Configs.SlidingPower)) * Time.deltaTime,
                        ForceMode2D.Impulse);
                else
                {                  
                    _rBody.AddForce(
                        (new Vector2(Math.Max(-playerDirectionOnSurface.x,0),playerDirectionOnSurface.y) * (-Configs.SlidingPower)) * 20 *Time.deltaTime,
                        ForceMode2D.Impulse);
                }         
            else
                _rBody.AddForce(
                    (new Vector2(1, 0) * (Configs.SlidingPower)) * Time.deltaTime,
                    ForceMode2D.Impulse);

            if (_rBody.velocity.x<0.01)
            {
                Debug.Log("_rBody.velocity.x<0.1");
                this.transform.Translate(10*Time.deltaTime,0,0);
            }
        }
        else
        {
            if (_rBody.gravityScale > 2) _rBody.gravityScale -= 0.03f;
            if (_rBody.drag < 2.5) _rBody.drag += .1f;
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
}

