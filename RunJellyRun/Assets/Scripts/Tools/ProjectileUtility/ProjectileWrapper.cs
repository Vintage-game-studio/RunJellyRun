using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileWrapper : MonoBehaviour {

	private float _initialVelocity;
	private float _initialAngle;
	private Vector2 _targetPos;
	private float _maxHeight;
	private float _duration;
	private float _gravity;
	
	private Projectile _projectile;
	private Vector2 initialPos2D;

	public GameObject TargetGameObject;

	public void SetInitialVelocity(float initialVelocity)
	{
		this._initialVelocity = initialVelocity;
	}

	public void SetInitialAngle(float initialAngle)
	{
		this._initialAngle = initialAngle;
	}

	public void SetTargetPos(Vector2 targetPos)
	{
		this._targetPos = targetPos;
	}

	public void SetMaxHeight(float height)
	{
		this._maxHeight = height;
	}

	public void SetDuration(float duration)
	{
		this._duration = duration;
	}

	public void SetGravity(float gravity)
	{
		this._gravity = gravity;
	}

	public List<Vector2> GetProjectile()
	{
		List<Vector2> points=new List<Vector2>();
		if (this._initialAngle>0 && this._duration>0 && this._targetPos!=Vector2.zero)
		{
			Projectile.InitialAngleDurationTargetPos initialAngleDurationTargetPos = new Projectile.InitialAngleDurationTargetPos();
			initialAngleDurationTargetPos.Duration = this._duration;
			initialAngleDurationTargetPos.InitAngle = this._initialAngle;
			this._projectile = new Projectile(this.initialPos2D, 0.6f, initialAngleDurationTargetPos);
			points = this._projectile.GetProjectileSamples().Select(s=>s.Position2D).ToList();
		}
		return points;
	}

	void Awake()
	{
		this.initialPos2D=new Vector2(this.transform.position.x,this.transform.position.y);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
