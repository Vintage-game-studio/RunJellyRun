using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class PlayerController : CharacterController {

    private Collision _collisionInfo;
    private Vector3 sNormal;
    private Vector3 playerDirectionOnSurface;
    private float angle;
    private float holdTime;
    private bool jump;

    public readonly double MaxSlopeAngle;

    public PlayerController(ControllerConfigurations configs) : base(configs)
    {
    }

    public void Start()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        _collisionInfo = collisionInfo;
        sNormal = collisionInfo.contacts[0].normal;
        
        Vector3 sVerNormal = Quaternion.AngleAxis(-90, Vector3.forward) * sNormal;
        playerDirectionOnSurface = sVerNormal;
        angle = Vector3.Angle(sNormal, Vector3.up);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            holdTime += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.Space))
            jump = true;
    }

    private void FixedUpdate()
    {
        if (angle < MaxSlopeAngle)
        {
            _rBody.AddForce((playerDirectionOnSurface * Configs.SlidingPower) * Time.deltaTime*angle/20 ,ForceMode.VelocityChange);
            //Debug.Log("Angle: " + angle);
        }

        if (jump)
        {
            _rBody.AddForce((sNormal * 2 + playerDirectionOnSurface*angle/5).normalized * (Configs.JumpPower + holdTime*10), ForceMode.Impulse);
            jump = false;
            holdTime = 0;
        }
    }

}
