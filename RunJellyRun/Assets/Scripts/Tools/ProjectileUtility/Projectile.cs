using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile
{	
	private readonly float TIME_DIVISION = 100f;
	
	private float _initialVelocity;
	private float _gravity;
	private float _height;
	private float _initialAngle;
	private float _duration;
	private float _ratio;

	private Vector2 _initialPos2D;
	private readonly Vector2 targetPos2D;

	public struct GravityDurationTargetPos
	{
		public Transform TargetTransform;
		public float Gravity;
		public float Duration;
	}

	public struct InitialVelocityDurationTargetPos
	{
		public Transform TargetTransform;
		public float InitialVelocity;
		public float Duration;
	}

	public struct HeightDurationTargetPos
	{
		public Transform TargetTransform;
		public float Height;
		public float Duration;
	}

	public struct InitialAngleGravityInitVelocity
	{
		public float InitVelocity;
		public float InitAngle;
		public float Gravity;
	}
	
	public struct InitialAngleDurationTargetPos
	{
		public Transform TargetTransform;
		public float Duration;
		public float InitAngle;
	}

	public struct ProjectilePoint
	{
		public float TimeStamp;
		public Vector2 Position2D;

		public ProjectilePoint(float timeStamp, Vector2 position2D)
		{
			TimeStamp = timeStamp;
			Position2D = position2D;
		}
	}
	
	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D, float ratio,InitialVelocityDurationTargetPos initialVelocityDurationTargetPos)
	{
		this._initialVelocity = initialVelocityDurationTargetPos.InitialVelocity;
		this._duration = initialVelocityDurationTargetPos.Duration;
		this._ratio = ratio;

		this._initialPos2D = initialPos2D;
		this.targetPos2D=targetPos2D;		
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,HeightDurationTargetPos heightDurationTargetPos)
	{
		this._height = heightDurationTargetPos.Height;
		this._duration = heightDurationTargetPos.Duration;
		this._ratio = ratio;
		
		this._initialPos2D=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,GravityDurationTargetPos gravityDurationTargetPos)
	{
		this._gravity = gravityDurationTargetPos.Gravity;
		this._duration = gravityDurationTargetPos.Duration;
		this._ratio = ratio;
		
		this._initialPos2D=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}
	
	public Projectile(Vector2 initialPos,float ratio,InitialAngleGravityInitVelocity initialAngleGravityInitVelocity)
	{		
		this._initialVelocity = initialAngleGravityInitVelocity.InitVelocity;
		this._gravity = initialAngleGravityInitVelocity.Gravity;
		this._initialAngle = initialAngleGravityInitVelocity.InitAngle;
		this._ratio = ratio;

		this._initialPos2D = initialPos;
	}

	public Projectile(Vector2 initialPos2D,float ratio,InitialAngleDurationTargetPos initialAngleDurationTargetPos)
	{
		this._initialAngle = initialAngleDurationTargetPos.InitAngle;
		this._duration = initialAngleDurationTargetPos.Duration;
		Vector3 targetPos = initialAngleDurationTargetPos.TargetTransform.position;
		this.targetPos2D=new Vector2(targetPos.x,targetPos.y);		
		this._ratio = ratio;
		
		this._initialPos2D=initialPos2D;
	}
	
	float GetRotateAngle(Vector2 initialPos2D, Vector2 targetPos2D)
	{
		Vector2 diffVec2 = targetPos2D - initialPos2D;
		return  Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right)) * Mathf.Rad2Deg;
	}

	float CalculateGravity(float deltaX, float duration, float angle)
	{
		return (-2 * deltaX * Mathf.Tan(angle*Mathf.Deg2Rad)) / (Mathf.Pow(duration,2));
	}
	
	float CalculateVelocity(float deltaX, float duration,float angle)
	{
		return deltaX / (Mathf.Cos(angle *Mathf.Deg2Rad) * duration);
	}
	
	public Vector2 GetPosition(float time)
	{
		//this.paceLength = Vector2.Distance(this.targetPos2D, this.initialPos2D);
		float px = this._initialVelocity * Mathf.Cos(Mathf.Deg2Rad * this._initialAngle) * time + this._initialPos2D.x;
		float py = 0.5f * this._gravity * Mathf.Pow(time, 2) +
		           this._initialVelocity * Mathf.Sin(Mathf.Deg2Rad * this._initialAngle) * time + this._initialPos2D.y;

		return new Vector2(px, py);
	}

	public List<ProjectilePoint> GetProjectileSamples(bool normalized=false)
	{
		float totalDistance = Vector2.Distance(this.targetPos2D, this._initialPos2D);
		float firstProjectileDistance = (totalDistance * _ratio)*2;
		float secondProjectileDistance = (totalDistance - firstProjectileDistance/2)*2;

		float firstDuration = this._duration * _ratio * 2;
		float secondDuration = this._duration * (1 - _ratio) * 2;
		
		if (this._gravity == 0)
			this._gravity = CalculateGravity(firstProjectileDistance, firstDuration, this._initialAngle);

		if (this._initialVelocity == 0)
			this._initialVelocity = CalculateVelocity(firstProjectileDistance, firstDuration, this._initialAngle);

		this._height = 0.5f * _gravity * Mathf.Pow(firstDuration/2, 2) +
		              this._initialVelocity * Mathf.Sin(Mathf.Deg2Rad * this._initialAngle) * this._duration * _ratio +
		              this._initialPos2D.y;

		float currentProjectileSpentTime = 0;
		float overalTime=0;
		
		float rotationAngle = GetRotateAngle(this._initialPos2D, this.targetPos2D);
		List<ProjectilePoint> projectilePoints = new List<ProjectilePoint>();
		Vector2 rotationPivot = this._initialPos2D;
		
		float timeInterval= this._duration / TIME_DIVISION;
		
		while (this._duration - overalTime > 0.0001)
		{
			Vector3 currentPos = GetPosition(currentProjectileSpentTime);			
			Vector2 rotatedVec2;		
			
			if (normalized)
			{
				Vector2 normCurrentPos=new Vector2(currentPos.x/totalDistance,currentPos.y/this._height);
				rotatedVec2=RotatePointAroundPivot(normCurrentPos,rotationPivot, new Vector3(0, 0, rotationAngle));
			}
			else
			{
				rotatedVec2= RotatePointAroundPivot(currentPos,rotationPivot, new Vector3(0, 0, rotationAngle));
			}
			projectilePoints.Add(new ProjectilePoint(overalTime,rotatedVec2));
			
			if (Math.Abs(overalTime - this._duration * _ratio) < 0.01)
			{
				this._initialPos2D.x += ((firstProjectileDistance / 2) - secondProjectileDistance/2);   
				_gravity = -8 * (currentPos.y-this._initialPos2D.y) / (float)Mathf.Pow(secondDuration, 2);
				this._initialAngle = Mathf.Rad2Deg * Mathf.Atan(4 * (currentPos.y-this._initialPos2D.y) / secondProjectileDistance);
				this._initialVelocity = (-_gravity * secondDuration)/(2*Mathf.Sin(Mathf.Deg2Rad * this._initialAngle));
				currentProjectileSpentTime = this._duration * (1 - _ratio);
			}
			
			currentProjectileSpentTime += timeInterval;
			overalTime += timeInterval;
			
			Debug.Log("spentTime: "+ currentProjectileSpentTime);
			Debug.Log("overalTime: "+ overalTime);
			Debug.Log("dis: "+ Vector2.Distance(currentPos, this.targetPos2D));
		}

		return projectilePoints;
	}
	
	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

}
