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

    public readonly double MaxSlopeAngle = 100;
    public readonly double MaxVelocity = 8;

    void Start()
    {
        Count++;
        playerGO = transform.GetChild(0).gameObject;
        _rBody = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        _collisionInfo = collisionInfo;
        sNormal = collisionInfo.contacts[0].normal;

        Vector3 sVerNormal = Quaternion.AngleAxis(-90, Vector3.forward) * sNormal;
        playerDirectionOnSurface = sVerNormal;
        //angle = Vector3.Angle(sNormal, Vector3.up);
        angle = Vector2.Angle(sNormal, Vector2.up);

    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            holdTime += Time.deltaTime*5;

        if (Input.GetKeyUp(KeyCode.Space))
            jump = true;
    }

    private void FixedUpdate()
    {
        if (angle < MaxSlopeAngle && _rBody.velocity.magnitude < MaxVelocity)
        {
            _rBody.AddForce(
                (playerDirectionOnSurface * (Configs.SlidingPower + playerDirectionOnSurface.y * 500)) * Time.deltaTime,
                ForceMode2D.Impulse);
            Debug.Log("velocity: " + _rBody.velocity.magnitude);
        }

        if (jump)
        {
            _rBody.AddForce((sNormal * 4 + playerDirectionOnSurface).normalized * (Configs.JumpPower + holdTime * 10),
                ForceMode2D.Impulse);
            jump = false;
            holdTime = 0;
        }
    }

    public void Kill()
    {
        _rBody.isKinematic = true;
        Camera.main.transform.parent.GetComponent<CameraFollow>().Player = null;

    }

}

