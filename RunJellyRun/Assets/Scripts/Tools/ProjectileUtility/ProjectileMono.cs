using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class ProjectileMono : MonoBehaviour
{
	public float InitialAngle;
	public float Duration;
	public Transform TargetPos;

	private Projectile projectile;
	private Vector2 initialPos2D;
	List<Vector2> pointsVec2=new List<Vector2>();
	
	void Start ()
	{
		Projectile.InitialAngleDuration initialAngleDuration = new Projectile.InitialAngleDuration();
		initialAngleDuration.InitAngle = this.InitialAngle;
		initialAngleDuration.Duration = this.Duration;
		
		projectile = new Projectile(0.6f,initialAngleDuration);

	}

	void FixedUpdate () {
		
		List<Projectile.ProjectilePoint> projectilePoints = projectile.GetProjectileSamples(this.transform.position,this.TargetPos.position);
		pointsVec2 = projectilePoints.Select(s => s.Position2D).ToList();
	}

	void OnDrawGizmos()
	{
		for (int i = 0; i < this.pointsVec2.Count-1; i++)
		{
			Gizmos.DrawLine(this.pointsVec2[i],this.pointsVec2[i+1]);
		}
	}
}
