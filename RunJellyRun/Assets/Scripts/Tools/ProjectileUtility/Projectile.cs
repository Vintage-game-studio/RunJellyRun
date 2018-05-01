using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile
{	
	private float initialVelocity;
	private float gravity;
	private float height;
	private float initialAngle;
	private float duration;
	private float ratio;

	private Vector2 initialPos2D;
	private Vector2 targetPos2D;
	private Vector2 currentPosiion;
	private float maxWidth;
	private float maxHeight;
	private readonly float TIME_DIVISION = 100f;
	private float paceLength;
	private bool second;

	public struct Gravity_duration_targetPos
	{
		public Transform TargetTransform;
		public float Gravity;
		public float Duration;
	}

	public struct InitialVelocity_duration_targetPos
	{
		public Transform TargetTransform;
		public float InitialVelocity;
		public float Duration;
	}

	public struct Height_duration_targetPos
	{
		public Transform TargetTransform;
		public float Height;
		public float Duration;
	}

	public struct InitialAngle_gravity_initVelocity
	{
		public float InitVelocity;
		public float InitAngle;
		public float Gravity;
	}
	
	public struct InitialAngle_duration_targetPos
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
	
	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D, float ratio,InitialVelocity_duration_targetPos initialVelocityDurationTargetPos)
	{
		this.initialVelocity = initialVelocityDurationTargetPos.InitialVelocity;
		this.duration = initialVelocityDurationTargetPos.Duration;
		this.ratio = ratio;

		this.initialPos2D = initialPos2D;
		this.targetPos2D=targetPos2D;		
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,Height_duration_targetPos heightDurationTargetPos)
	{
		this.height = heightDurationTargetPos.Height;
		this.duration = heightDurationTargetPos.Duration;
		this.ratio = ratio;
		
		this.initialPos2D=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,Gravity_duration_targetPos gravityDurationTargetPos)
	{
		this.gravity = gravityDurationTargetPos.Gravity;
		this.duration = gravityDurationTargetPos.Duration;
		this.ratio = ratio;
		
		this.initialPos2D=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}
	
	public Projectile(Vector2 initialPos,float ratio,InitialAngle_gravity_initVelocity initialAngleGravityInitVelocity)
	{
		
		
		this.initialVelocity = initialAngleGravityInitVelocity.InitVelocity;
		this.gravity = initialAngleGravityInitVelocity.Gravity;
		this.initialAngle = initialAngleGravityInitVelocity.InitAngle;
		this.ratio = ratio;

		this.initialPos2D = initialPos;
	}

	public Projectile(Vector2 initialPos2D,float ratio,InitialAngle_duration_targetPos initialAngleDurationTargetPos)
	{
		this.initialAngle = initialAngleDurationTargetPos.InitAngle;
		this.duration = initialAngleDurationTargetPos.Duration;
		Vector3 targetPos = initialAngleDurationTargetPos.TargetTransform.position;
		this.targetPos2D=new Vector2(targetPos.x,targetPos.y);		
		this.ratio = ratio;
		
		this.initialPos2D=initialPos2D;
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
		this.paceLength = Vector2.Distance(this.targetPos2D, this.initialPos2D);
		float px = this.initialVelocity * Mathf.Cos(Mathf.Deg2Rad * this.initialAngle) * time + this.initialPos2D.x;
		float py = 0.5f * this.gravity * Mathf.Pow(time, 2) +
		           this.initialVelocity * Mathf.Sin(Mathf.Deg2Rad * this.initialAngle) * time + this.initialPos2D.y;

		currentPosiion = new Vector2(px, py);

		return currentPosiion;
	}

	public List<ProjectilePoint> GetProjectileSamples(bool normalized=false)
	{
		float totalDistance = Vector2.Distance(this.targetPos2D, this.initialPos2D);
		float firstProjectileDistance = (totalDistance * ratio)*2;
		float secondProjectileDistance = (totalDistance - firstProjectileDistance/2)*2;

		float firstDuration = this.duration * ratio * 2;
		float secondDuration = this.duration * (1 - ratio) * 2;
		
		if (this.gravity == 0)
			this.gravity = CalculateGravity(firstProjectileDistance, firstDuration, this.initialAngle);

		if (this.initialVelocity == 0)
			this.initialVelocity = CalculateVelocity(firstProjectileDistance, firstDuration, this.initialAngle);

		this.height = 0.5f * gravity * Mathf.Pow(firstDuration/2, 2) +
		              this.initialVelocity * Mathf.Sin(Mathf.Deg2Rad * this.initialAngle) * this.duration * ratio +
		              this.initialPos2D.y;

		float currentProjectileSpentTime = 0;
		float overalTime=0;
		
		float rotationAngle = GetRotateAngle(this.initialPos2D, this.targetPos2D);
		List<ProjectilePoint> projectilePoints = new List<ProjectilePoint>();
		Vector2 rotationPivot = this.initialPos2D;
		
		float timeInterval= this.duration / TIME_DIVISION;
		
		while (this.duration - overalTime > 0.0001)
		{
			Vector3 currentPos = GetPosition(currentProjectileSpentTime);			
			Vector2 rotatedVec2;		
			
			if (normalized)
			{
				Vector2 normCurrentPos=new Vector2(currentPos.x/totalDistance,currentPos.y/this.height);
				rotatedVec2=RotatePointAroundPivot(normCurrentPos,rotationPivot, new Vector3(0, 0, rotationAngle));
			}
			else
			{
				rotatedVec2= RotatePointAroundPivot(currentPos,rotationPivot, new Vector3(0, 0, rotationAngle));
			}
			projectilePoints.Add(new ProjectilePoint(overalTime,rotatedVec2));
			
			if (Math.Abs(currentProjectileSpentTime - this.duration * ratio) < 0.01 && !second)
			{
				this.initialPos2D.x += ((firstProjectileDistance / 2) - secondProjectileDistance/2);   
				gravity = -8 * (currentPos.y-this.initialPos2D.y) / (float)Mathf.Pow(secondDuration, 2);
				this.initialAngle = Mathf.Rad2Deg * Mathf.Atan(4 * (currentPos.y-this.initialPos2D.y) / secondProjectileDistance);
				this.initialVelocity = (-gravity * secondDuration)/(2*Mathf.Sin(Mathf.Deg2Rad * this.initialAngle));
				currentProjectileSpentTime = this.duration * (1 - ratio);
				second = true;
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
