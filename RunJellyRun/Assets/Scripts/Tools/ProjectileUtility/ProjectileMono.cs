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
		this.initialPos2D=new Vector2(transform.position.x,transform.position.y);
		
		Projectile.InitialAngle_duration_targetPos initialAngleDurationTargetPos = new Projectile.InitialAngle_duration_targetPos();
		initialAngleDurationTargetPos.InitAngle = this.InitialAngle;
		initialAngleDurationTargetPos.Duration = this.Duration;
		initialAngleDurationTargetPos.TargetTransform = this.TargetPos;
		
		projectile = new Projectile(initialPos2D,0.6f,initialAngleDurationTargetPos);
		List<Projectile.ProjectilePoint> projectilePoints = projectile.GetProjectileSamples();
		pointsVec2 = projectilePoints.Select(s => s.Position2D).ToList();
	}

	void Update () {
		
		
	}

	void OnDrawGizmos()
	{
		for (int i = 0; i < this.pointsVec2.Count-1; i++)
		{
			Gizmos.DrawLine(this.pointsVec2[i],this.pointsVec2[i+1]);
		}
	}
}
