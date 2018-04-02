using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile 
{
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
	public Transform InitialTransform;
	public float InitialVelocity;
	public float Gravity;
	public float Height;
	public float InitialAngle;
	public float Duration;
	public float Ratio;

	private Vector2 currentPosiion;

	public Projectile(Transform initialTransform, float ratio,ProjectileData_initialVelocity_duration projectileDataInitialVelocityDuration)
	{
		this.InitialTransform = initialTransform;
		this.TargetTransform = projectileDataInitialVelocityDuration.TargetTransform;
		this.InitialVelocity = projectileDataInitialVelocityDuration.InitialVelocity;
		this.Duration = projectileDataInitialVelocityDuration.Duration;
		this.Ratio = ratio;
	}

	public Projectile(Transform initialTransform,float ratio,ProjectileData_height_duration projectileDataHeightDuration)
	{
		this.InitialTransform = initialTransform;
		this.TargetTransform = projectileDataHeightDuration.TargetTransform;
		this.Height = projectileDataHeightDuration.Height;
		this.Duration = projectileDataHeightDuration.Duration;
		this.Ratio = ratio;
	}

	public Projectile(Transform initialTransform,float ratio,ProjectileData_gravity_duration projectileDataGravityDuration)
	{
		this.InitialTransform = initialTransform;
		this.TargetTransform = projectileDataGravityDuration.TargetTransform;
		this.Gravity = projectileDataGravityDuration.Gravity;
		this.Duration = projectileDataGravityDuration.Duration;
		this.Ratio = ratio;
	}
	
	public Projectile(Transform initialTransform,float ratio,Projectile_initialAngle_gravity_initVelocity projectileInitialAngleGravityInitVelocity)
	{
		this.InitialTransform = initialTransform;
		this.InitialVelocity = projectileInitialAngleGravityInitVelocity.InitVelocity;
		this.Gravity = projectileInitialAngleGravityInitVelocity.Gravity;
		this.InitialAngle = projectileInitialAngleGravityInitVelocity.InitAngle;
		this.Ratio = ratio;
	}	

	float GetRotateAngle()
	{
		
		return 0;
	}

	public Vector2 GetCurrentPosition(float time)
	{
		float px = this.InitialVelocity * Mathf.Cos(Mathf.Deg2Rad * this.InitialAngle) * time + this.InitialTransform.position.x;
		float py = 0.5f * this.Gravity * Mathf.Pow(time, 2) + this.InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * this.InitialAngle) * time +this.InitialTransform.position.y;
    
		currentPosiion=new Vector2(px,py);
		
		return currentPosiion;
	}
	

}
