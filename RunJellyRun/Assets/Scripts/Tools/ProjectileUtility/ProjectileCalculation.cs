using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Vuforia;

public class ProjectileCalculation: MonoBehaviour
{

  public float InitialVelocity;
  //public Transform InitialTranstform3D;
  public float InitialAngle;
  public GameObject TargetGO;
  public float Duration;

  private float time = 0;
  private bool jump;
  private Vector2 initialPosition2D;
  private Vector2 targetPos;
  private float rotAngle;
  private float g;
  List<KeyValuePair<Vector3,Vector3>> linePairs=new List<KeyValuePair<Vector3, Vector3>>();
  
  Vector2 GetPosition(float t)
  { 
    float px = InitialVelocity * Mathf.Cos(Mathf.Deg2Rad * InitialAngle) * t + this.initialPosition2D.x;
    float py = 0.5f*g * Mathf.Pow(t, 2) + InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * InitialAngle) * t +this.initialPosition2D.y;

   return RotatePointAroundPivot(new Vector2(px, py), this.initialPosition2D, new Vector3(0, 0, rotAngle));
  }

  void Start()
  {
    this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
    Vector2 diffVec2 = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
    this.rotAngle = Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right))*Mathf.Rad2Deg;
 }

  void Update()
  {
    if (Input.GetKey(KeyCode.Space) && !jump)
    {
      this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
      Vector2 diffVec2 = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
      this.rotAngle = Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right))*Mathf.Rad2Deg;
      time = 0;
      
      float dis = Vector3.Distance(TargetGO.transform.position, this.transform.position);
      InitialVelocity = dis / (Mathf.Cos(this.InitialAngle *Mathf.Deg2Rad) * Duration);
      g = (-2 * dis* Mathf.Tan(this.InitialAngle*Mathf.Deg2Rad)) / (Mathf.Pow(Duration, 2));
      jump = true;
    }
  }

  void FixedUpdate()
  {
    if (jump)
    {
      if (Math.Abs(time - Duration) < Time.deltaTime ||
          Vector2.Distance(transform.position, TargetGO.transform.position) < 0.01)
      {
        jump = false;
        //linePairs.Clear();
        return;
      }

      Vector3 prevPos = this.transform.position;
      this.transform.position = GetPosition(time);
      linePairs.Add(new KeyValuePair<Vector3, Vector3>(prevPos,this.transform.position));
      
      time += Time.fixedDeltaTime;
    }
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    foreach (KeyValuePair<Vector3, Vector3> keyValuePair in linePairs)
    {
      Gizmos.DrawLine(keyValuePair.Key, keyValuePair.Value);
/*      Gizmos.color=Color.red;
      Gizmos.DrawSphere(keyValuePair.Value,1);*/
    }
  }
  
  Vector2 RotatePoint(Vector2 aPoint, float aDegree)
  {
    return Quaternion.Euler(0,0,aDegree) * aPoint;
  }
  
  public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
    return Quaternion.Euler(angles) * (point - pivot) + pivot;
  }
}
