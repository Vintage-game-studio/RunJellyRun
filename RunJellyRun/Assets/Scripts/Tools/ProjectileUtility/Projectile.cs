using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile
{
	private readonly float TIME_DIVISION = 100;
	
	public struct ProjectileData_gravity_duration
	{
		public Transform TargetTransform;
		public float Gravity;
		public float Duration;
	}

	public struct ProjectileData_initialVelocity_duration
	{
		public Transform TargetTransform;
		public float InitialVelocity;
		public float Duration;
	}

	public struct ProjectileData_height_duration
	{
		public Transform TargetTransform;
		public float Height;
		public float Duration;
	}

	public struct Projectile_initialAngle_gravity_initVelocity
	{
		public float InitVelocity;
		public float InitAngle;
		public float Gravity;
	}

	public Transform TargetTransform;
	public float InitialVelocity;
	public float Gravity;
	public float Height;
	public float InitialAngle;
	public float Duration;
	public float Ratio;

	private Vector2 initialPos2D;
	private Vector2 targetPos2D;
	private Vector2 currentPosiion;
	private float timeInterval;
	

	public Projectile(Transform initialTransform, float ratio,ProjectileData_initialVelocity_duration projectileDataInitialVelocityDuration)
	{
		this.timeInterval= this.Duration / TIME_DIVISION;
		
		this.initialPos2D=new Vector2(initialTransform.position.x,initialTransform.position.y);
		this.TargetTransform = projectileDataInitialVelocityDuration.TargetTransform;
		this.InitialVelocity = projectileDataInitialVelocityDuration.InitialVelocity;
		this.Duration = projectileDataInitialVelocityDuration.Duration;
		this.Ratio = ratio;
	}

	public Projectile(Transform initialTransform,float ratio,ProjectileData_height_duration projectileDataHeightDuration)
	{
		this.timeInterval= this.Duration / TIME_DIVISION;
		
		this.initialPos2D=new Vector2(initialTransform.position.x,initialTransform.position.y);
		this.TargetTransform = projectileDataHeightDuration.TargetTransform;
		this.Height = projectileDataHeightDuration.Height;
		this.Duration = projectileDataHeightDuration.Duration;
		this.Ratio = ratio;
	}

	public Projectile(Transform initialTransform,float ratio,ProjectileData_gravity_duration projectileDataGravityDuration)
	{
		this.timeInterval= this.Duration / TIME_DIVISION;
		
		this.initialPos2D=new Vector2(initialTransform.position.x,initialTransform.position.y);
		this.TargetTransform = projectileDataGravityDuration.TargetTransform;
		this.Gravity = projectileDataGravityDuration.Gravity;
		this.Duration = projectileDataGravityDuration.Duration;
		this.Ratio = ratio;
	}
	
	public Projectile(Transform initialTransform,float ratio,Projectile_initialAngle_gravity_initVelocity projectileInitialAngleGravityInitVelocity)
	{
		this.timeInterval= this.Duration / TIME_DIVISION;
		
		this.initialPos2D=new Vector2(initialTransform.position.x,initialTransform.position.y);
		this.InitialVelocity = projectileInitialAngleGravityInitVelocity.InitVelocity;
		this.Gravity = projectileInitialAngleGravityInitVelocity.Gravity;
		this.InitialAngle = projectileInitialAngleGravityInitVelocity.InitAngle;
		this.Ratio = ratio;
	}	

	float GetRotateAngle(Vector2 initialPos,Vector2 targetPos)
	{
		Vector2 diffVec2 = targetPos - initialPos;
		return  -Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right)) * Mathf.Rad2Deg;
	}

	float CalculateGravity(float deltaX, float duration, float angle)
	{
		return (-2 * deltaX * Mathf.Tan(angle*Mathf.Deg2Rad)) / (Mathf.Pow(duration,2));
	}
	
	float CalculateVelocity(float deltaX, float duration,float angle)
	{
		return deltaX / (Mathf.Cos(angle *Mathf.Deg2Rad) * duration);
	}
  
	
	public Vector2 GetCurrentPosition(float time)
	{
		float px = this.InitialVelocity * Mathf.Cos(Mathf.Deg2Rad * this.InitialAngle) * time + this.initialPos2D.x;
		float py = 0.5f * this.Gravity * Mathf.Pow(time, 2) +
		           this.InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * this.InitialAngle) * time + this.initialPos2D.y;
    
		currentPosiion=new Vector2(px,py);
		
		return currentPosiion;
	}
	

}
