using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile
{	
	private readonly float TIME_DIVISION = 100f;

	private float height;
	private float duration;
	private float ratio;

	private Vector2 targetPos2D;

	private float gravity1;
	private float gravity2;

	private float initialVelocity1;
	private float initialVelocity2;

	private float initialAngle1;
	private float initialAngle2;

	private Vector2 initialPos2D1;
	private Vector2 initialPos2D2;
	private float rotationAngle;
	private Vector2 rotationPivot;
	private float totalDistance;

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
		this.initialVelocity1 = initialVelocityDurationTargetPos.InitialVelocity;
		this.duration = initialVelocityDurationTargetPos.Duration;
		this.ratio = ratio;

		this.initialPos2D2 = initialPos2D;
		this.targetPos2D=targetPos2D;		
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,HeightDurationTargetPos heightDurationTargetPos)
	{
		this.height = heightDurationTargetPos.Height;
		this.duration = heightDurationTargetPos.Duration;
		this.ratio = ratio;
		
		this.initialPos2D1=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}

	public Projectile(Vector2 initialPos2D, Vector2 targetPos2D,float ratio,GravityDurationTargetPos gravityDurationTargetPos)
	{
		this.gravity1 = gravityDurationTargetPos.Gravity;
		this.duration = gravityDurationTargetPos.Duration;
		this.ratio = ratio;
		
		this.initialPos2D1=initialPos2D;
		this.targetPos2D=targetPos2D;	
	}
	
	public Projectile(Vector2 initialPos,float ratio,InitialAngleGravityInitVelocity initialAngleGravityInitVelocity)
	{		
		this.initialVelocity1 = initialAngleGravityInitVelocity.InitVelocity;
		this.gravity1 = initialAngleGravityInitVelocity.Gravity;
		this.initialAngle1 = initialAngleGravityInitVelocity.InitAngle;
		this.ratio = ratio;

		this.initialPos2D1 = initialPos;
	}

	public Projectile(Vector2 initialPos2D, float ratio, InitialAngleDurationTargetPos initialAngleDurationTargetPos)
	{
		this.duration = initialAngleDurationTargetPos.Duration;

		Vector3 targetPos = initialAngleDurationTargetPos.TargetTransform.position;
		this.targetPos2D = new Vector2(targetPos.x, targetPos.y);
		this.ratio = ratio;

		this.initialAngle1 = initialAngleDurationTargetPos.InitAngle;

		this.initialPos2D1 = initialPos2D;
		this.initialPos2D2 = initialPos2D;

		this.totalDistance = Vector2.Distance(this.targetPos2D, initialPos2D1);
		float projectileDistance1 = (totalDistance * this.ratio) * 2;
		float projectileDistance2 = (totalDistance - projectileDistance1 / 2) * 2;
		this.initialPos2D2.x= ((projectileDistance1 / 2) - projectileDistance2 / 2);

		float duration1 = this.duration * this.ratio * 2;
		float duration2 = this.duration * (1 - this.ratio) * 2;

		this.gravity1 = CalculateGravity(projectileDistance1, duration1, this.initialAngle1);
		this.initialVelocity1 = CalculateVelocity(projectileDistance1, duration1, this.initialAngle1);

		this.height = 0.5f * this.gravity1 * Mathf.Pow(duration1 / 2, 2) +
		               this.initialVelocity1 * Mathf.Sin(Mathf.Deg2Rad * this.initialAngle1) * duration1/2+
		               this.initialPos2D1.y;

		Vector2 midPos = GetPosition(duration1/2, this.initialVelocity1, this.initialAngle1, this.initialPos2D1, this.gravity1);
		this.gravity2 = -8 * (midPos.y - initialPos2D2.y) / (float)Mathf.Pow(duration2, 2);
		this.initialAngle2= Mathf.Rad2Deg * Mathf.Atan(4 * (midPos.y - initialPos2D2.y) / projectileDistance2);
		this.initialVelocity2 = (-gravity2 * duration2) / (2 * Mathf.Sin(Mathf.Deg2Rad * initialAngle2));

		this.rotationAngle = GetRotateAngle(this.initialPos2D1, this.targetPos2D);
		this.rotationPivot = this.initialPos2D1;
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

	private Vector2 GetPosition(float time, float initialVelocity,float initialAngle, Vector2 initialPos2D, float gravity)
	{
		float px = initialVelocity * Mathf.Cos(Mathf.Deg2Rad * initialAngle) * time + initialPos2D.x;
		float py = 0.5f * gravity * Mathf.Pow(time, 2) +
		           initialVelocity * Mathf.Sin(Mathf.Deg2Rad * initialAngle) * time + initialPos2D.y;

		return new Vector2(px, py);
	}

	public List<ProjectilePoint> GetProjectileSamples(bool normalized = false)
	{
		float currentProjectileSpentTime = 0;
		float overalTime = 0;

		float timeInterval = this.duration / TIME_DIVISION;

		List<ProjectilePoint> projectilePoints = new List<ProjectilePoint>();

		while (Math.Abs(overalTime - this.duration * ratio) > 0.001)
		{
			Vector3 currentPos = GetPosition(currentProjectileSpentTime, this.initialVelocity1, this.initialAngle1,
				this.initialPos2D1, this.gravity1);

			projectilePoints.Add(new ProjectilePoint(overalTime,
				RotatePointAroundPivot(currentPos, rotationPivot, new Vector3(0, 0, rotationAngle))));

			currentProjectileSpentTime += timeInterval;
			overalTime += timeInterval;
		}

		currentProjectileSpentTime = this.duration * (1 - ratio);

		while (this.duration - overalTime > 0.00001)
		{
			currentProjectileSpentTime += timeInterval;
			overalTime += timeInterval;

			Vector3 currentPos = GetPosition(currentProjectileSpentTime, this.initialVelocity2, this.initialAngle2,
				this.initialPos2D2, this.gravity2);

			projectilePoints.Add(new ProjectilePoint(overalTime,
				RotatePointAroundPivot(currentPos, rotationPivot, new Vector3(0, 0, rotationAngle))));

		}

		if (normalized)
			for (var index = 0; index < projectilePoints.Count; index++)
			{
				ProjectilePoint pointPair = projectilePoints[index];
				Vector2 currentPos = pointPair.Position2D;
				Vector2 normCurrentPos = new Vector2(currentPos.x / totalDistance, currentPos.y / this.height);
				pointPair.Position2D = RotatePointAroundPivot(normCurrentPos, rotationPivot, new Vector3(0, 0, rotationAngle));
				projectilePoints[index] = new ProjectilePoint(pointPair.TimeStamp, pointPair.Position2D);
			}

		return projectilePoints;
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

}
