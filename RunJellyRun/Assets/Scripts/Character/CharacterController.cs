using System;
using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour  {

    public ControllerConfigurations Configs;

    protected Rigidbody _rBody;
    private bool onCollision;

    public CharacterController(ControllerConfigurations configs)
    {
        Configs = configs;
    }
}

[Serializable]
public class ControllerConfigurations
{
    public float JumpPower;
    public float JumpAngle;
    public float SlidingPower;
    public float WallJumpAngle;
    public float WallJumpPower=100;
    public float JumpNumber;
}