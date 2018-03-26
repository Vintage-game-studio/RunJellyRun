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
  public float ratio;

  private float time = 0;
  private bool jump;
  private Vector2 initialPosition2D;
  private Vector2 targetPos;
  private float rotAngle;
  private float g;
  List<KeyValuePair<Vector3,Vector3>> linePairs=new List<KeyValuePair<Vector3, Vector3>>();
  private float deltaX1;
  private float deltaX2;
  private float g1,g2;
  private float velocity1, velocity2;
  private float t1, t2, addTime;
  private bool second = false;
  Vector2 GetPosition(float t)
  { 
    float px = InitialVelocity * Mathf.Cos(Mathf.Deg2Rad * InitialAngle) * t + this.initialPosition2D.x;
    float py = 0.5f*g * Mathf.Pow(t, 2) + InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * InitialAngle) * t +this.initialPosition2D.y;
    
    return RotatePointAroundPivot(new Vector2(px, py), this.initialPosition2D, new Vector3(0, 0, rotAngle));
  }

  void Start()
  {
    this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
    Vector2 disVec = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
    this.rotAngle = Mathf.Acos(Vector2.Dot(disVec.normalized, Vector2.right))*Mathf.Rad2Deg;

    float dis = Vector3.Distance(TargetGO.transform.position, this.transform.position);
    deltaX1 = (dis * ratio)*2;
    deltaX2 = (dis - deltaX1/2)*2;
    
    float halfExDuration = ((Duration*ratio)/2);
    addTime = Duration * (1 - ratio) / 2; 
    
    Debug.Log("ddis: "+ dis);
    Debug.Log("deltaX1: "+ deltaX1);
    Debug.Log("deltaX2: "+ deltaX2);
    
    g1=CalculateGravity(deltaX1,Duration*ratio*2,this.InitialAngle);        
    g2=CalculateGravity(deltaX2,Duration * (1 - ratio),this.InitialAngle*(1+ratio));

    velocity1 = CalculateVelocity(deltaX1, Duration*ratio*2,this.InitialAngle);
    velocity2 = CalculateVelocity(deltaX2,Duration * (1 - ratio),this.InitialAngle*(1+ratio));

    g = g1;
    this.InitialVelocity = velocity1;
  }

  void Update()
  {
    if (Input.GetKey(KeyCode.Space) && !jump)
    {
      this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
      Vector2 diffVec2 = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
      this.rotAngle = Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right)) * Mathf.Rad2Deg;
      time = 0;

      jump = true;
    }
  }

  float CalculateGravity(float deltaX, float duration, float angle)
  {
    return (-2 * deltaX * Mathf.Tan(angle*Mathf.Deg2Rad)) / (Mathf.Pow(duration,2));
  }

  float CalculateVelocity(float deltaX, float duration,float angle)
  {
    return deltaX / (Mathf.Cos(angle *Mathf.Deg2Rad) * duration);
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

      t1 = time;
      if (Math.Abs(time - Duration*ratio) < 0.01 && !second)
      {
        //Vector2 preInitPos2D = this.initialPosition2D;
        this.initialPosition2D.x += ((deltaX1 / 2) - deltaX2/2);
        //this.initialPosition2D = RotatePointAroundPivot(this.initialPosition2D,preInitPos2D,new Vector3(0, 0, rotAngle));
        
        GameObject go= GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = this.initialPosition2D;
        
        Debug.Log("this.initialPosition2D.x: "+ this.initialPosition2D.x);
        g = -8 * this.transform.position.y / (float)Mathf.Pow((Duration * (1 - ratio) * 2), 2);
        Debug.Log("g: "+ g);
        this.InitialAngle = Mathf.Rad2Deg * Mathf.Atan(4 * this.transform.position.y / deltaX2);
        Debug.Log("this.InitialAngle: "+this.InitialAngle);
        this.InitialVelocity = (-g*Duration * (1 - ratio)*2)/(2*Mathf.Sin(Mathf.Deg2Rad*this.InitialAngle));
        Debug.Log("this.InitialVelocity: "+this.InitialVelocity);
        
        time = Duration * (1 - ratio);
       
        second = true;
        
      }
      
      Vector3 prevPos = this.transform.position;
      this.transform.position = GetPosition(time);
      linePairs.Add(new KeyValuePair<Vector3, Vector3>(prevPos,this.transform.position));
      
      time += Time.fixedDeltaTime;
      Debug.Log(time);
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
