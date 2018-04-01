using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
  private float velocity;
  private bool second = false;
  private Vector2 beforRotVector2;
  private Vector2 rotationPivot;
  private float maxWidth;
  private float maxHeight;

  public List<KeyValuePair<Vector3,Vector3>> GetLines(float width, float height)
  {
    List<KeyValuePair<Vector3,Vector3>> linePairs_Norm=new List<KeyValuePair<Vector3, Vector3>>();
    foreach (KeyValuePair<Vector3,Vector3> keyValuePair in linePairs)
    {
      Vector3 key=new Vector3(width*keyValuePair.Key.x/maxWidth,height*keyValuePair.Key.y/maxHeight,0);
      Vector3 value=new Vector3(width*keyValuePair.Value.x/maxWidth,height*keyValuePair.Value.y/maxHeight,0);
      linePairs_Norm.Add(new KeyValuePair<Vector3, Vector3>(key,value));
      //Debug.Log(new KeyValuePair<Vector3, Vector3>(width*keyValuePair.Key/maxWidth,height*keyValuePair.Value/maxHeight));
    }
    
    return linePairs_Norm;
  }
  
  Vector2 GetPosition(float t,float gravity)
  { 
    float px = InitialVelocity * Mathf.Cos(Mathf.Deg2Rad * InitialAngle) * t + this.initialPosition2D.x;
    float py = 0.5f*gravity * Mathf.Pow(t, 2) + InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * InitialAngle) * t +this.initialPosition2D.y;
    
    return new Vector2(px,py);
  }

  void Start()
  {

  }

  public List<Vector3> GetProjectilePoints(float width, float height)
  {
    float timeInterval;
    List<Vector3> projectilePoints=new List<Vector3>();
    Vector3 currentPos = Vector3.zero;
    float gravity = 0;
    second = false;
    
    float dis = Vector3.Distance(TargetGO.transform.position, this.transform.position);
    deltaX1 = (dis * ratio)*2;
    deltaX2 = (dis - deltaX1/2)*2;    

    gravity = CalculateGravity(deltaX1,Duration*ratio*2,this.InitialAngle);   
    this.InitialVelocity = CalculateVelocity(deltaX1, Duration*ratio*2,this.InitialAngle);      
    this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
    Vector2 diffVec2 = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
    this.rotAngle = -Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right)) * Mathf.Rad2Deg;
    this.rotationPivot = this.initialPosition2D;

    timeInterval = this.Duration / 100f;
    float spentTime = 0;
    float overalTime=0;
    maxHeight=  0.5f*gravity * Mathf.Pow(Duration*ratio, 2) + this.InitialVelocity * Mathf.Sin(Mathf.Deg2Rad * InitialAngle) * this.Duration*ratio +this.initialPosition2D.y;
    maxWidth= 2*dis;
    
    while (Math.Abs(spentTime - Duration) > timeInterval && Vector2.Distance(currentPos, TargetGO.transform.position) > 0.1)
    {
      currentPos = GetPosition(spentTime,gravity);
      Vector2 normCurrentPos=new Vector2(width*currentPos.x/maxWidth,height*currentPos.y/maxHeight);
      Vector2 rotatedVec2= RotatePointAroundPivot(normCurrentPos, this.rotationPivot, new Vector3(0, 0, rotAngle));
      projectilePoints.Add(rotatedVec2);
      
      if (Math.Abs(spentTime - Duration * ratio) < 0.01 && !second)
      {
        this.initialPosition2D.x += ((deltaX1 / 2) - deltaX2/2);       
        gravity = -8 * (currentPos.y-this.initialPosition2D.y) / (float)Mathf.Pow((Duration * (1 - ratio) * 2), 2);
        this.InitialAngle = Mathf.Rad2Deg * Mathf.Atan(4 * (currentPos.y-this.initialPosition2D.y) / deltaX2);
        this.InitialVelocity = (-gravity * Duration * (1 - ratio)*2)/(2*Mathf.Sin(Mathf.Deg2Rad * this.InitialAngle));
        spentTime = Duration * (1 - ratio);
       
        second = true;        
      }
      spentTime += timeInterval;
      overalTime += timeInterval;
      Debug.Log(overalTime);
      Debug.Log("dis: "+Vector2.Distance(currentPos, TargetGO.transform.position));
    }
    return projectilePoints;
  }
  
  void Update()
  {
    if (Input.GetKey(KeyCode.Space) && !jump)
    {
      float dis = Vector3.Distance(TargetGO.transform.position, this.transform.position);
      deltaX1 = (dis * ratio)*2;
      deltaX2 = (dis - deltaX1/2)*2;    

      g = CalculateGravity(deltaX1,Duration*ratio*2,this.InitialAngle);
    
      this.InitialVelocity = CalculateVelocity(deltaX1, Duration*ratio*2,this.InitialAngle);
      
      this.initialPosition2D = new Vector2(this.transform.position.x,this.transform.position.y);
      Vector2 diffVec2 = new Vector2(TargetGO.transform.position.x,TargetGO.transform.position.y) - this.initialPosition2D;
      this.rotAngle = -Mathf.Acos(Vector2.Dot(diffVec2.normalized, Vector2.right)) * Mathf.Rad2Deg;
      time = 0;
      this.rotationPivot = this.initialPosition2D;
      jump = true;
      second = false;
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
          Vector2.Distance(transform.position, TargetGO.transform.position) < 0.1)
      {
        jump = false;
        //linePairs.Clear();
        return;
      }
      if (Math.Abs(time - Duration*ratio) < 0.01 && !second)
      {
        this.initialPosition2D.x += ((deltaX1 / 2) - deltaX2/2);
        
        g = -8 * (this.beforRotVector2.y-this.initialPosition2D.y) / (float)Mathf.Pow((Duration * (1 - ratio) * 2), 2);
        this.InitialAngle = Mathf.Rad2Deg * Mathf.Atan(4 * (this.beforRotVector2.y-this.initialPosition2D.y) / deltaX2);
        this.InitialVelocity = (-g*Duration * (1 - ratio)*2)/(2*Mathf.Sin(Mathf.Deg2Rad*this.InitialAngle));

        time = Duration * (1 - ratio);
       
        second = true;        
      }
      
      Vector3 prevPos = this.transform.position;
      //this.transform.position = GetPosition(time);
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
