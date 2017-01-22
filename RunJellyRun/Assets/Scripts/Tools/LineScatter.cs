using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineScatter : MonoBehaviour
{
    public int MinCount = 10;
    public int MaxCount = 10;
    public int MinAngle = 10;
    public int MaxAngle = 10;
    public float ScaleFactor = 1;
    public float ScaleTolerance = 0;
    public Vector3 PosOffset ;
    public Vector3 PosTolerance ;
    public bool Uniform = true;
    public Transform ParentLine;
    public List<GameObject> PrefabsList = new List<GameObject>();
    
    private List<Vector3> _line = new List<Vector3>();
    private List<float> _distance= new List<float>();
    private float _lenght = 0;
    private int _count;
    private Transform _parentScatterdObject;

    public void Scatter()
    {
        initialize();
        _count = Random.Range(MinCount, MaxCount);
        for (int i = 0; i < _count; i++)
        {
            Transform newObject = ((GameObject) Instantiate(
                PrefabsList[Random.Range(0, PrefabsList.Count)],
                GetPos(i),
                Quaternion.AngleAxis(Random.Range(MinAngle, MaxAngle), Vector3.forward))).transform;

            newObject.parent = _parentScatterdObject;

            float s = ScaleFactor + Random.Range(-ScaleTolerance, ScaleTolerance);
            newObject.localScale = new Vector3(s, s, s);
            newObject.position = newObject.position + PosOffset + new Vector3(
                                     Random.Range(-PosTolerance.x, PosTolerance.x),
                                     Random.Range(-PosTolerance.y, PosTolerance.y),
                                     Random.Range(-PosTolerance.z, PosTolerance.z)
                                 );
        }
    }

    private Vector3 GetPos(int i)
    {
        if (!Uniform)
            return GetPosInLine(Random.Range(0, _lenght));

        return GetPosInLine(i*(_lenght/(_count - 1)));

    }

    private Vector3 GetPosInLine(float pos)
    {
        float curDis = 0;
        for (int i = 1; i < _line.Count; i++)
        {
            if (_distance[i-1]<pos && pos<_distance[i])
            {
                return Vector3.Lerp(_line[i - 1], _line[i],
                    (_distance[i] - _distance[i - 1])/(pos - _distance[i - 1]));
            }
        }
        return _line[_line.Count - 1];
    }

    private void initialize()
    {
        _line.Clear();
        _distance.Clear();
        _lenght = 0;

        Vector3 lastPos = ParentLine.transform.GetChild(0).position;
        for (int i = 0; i < ParentLine.transform.childCount; i++)
        {
            _line.Add(ParentLine.transform.GetChild(i).position);

            _lenght += Vector3.Distance(lastPos, ParentLine.transform.GetChild(i).position);

            lastPos = ParentLine.transform.GetChild(i).position;
            _distance.Add(_lenght);
        }


        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject.name == "ScatterdObjects")
                DestroyImmediate(transform.GetChild(i).gameObject);

        _parentScatterdObject = new GameObject("ScatterdObjects").transform;
        _parentScatterdObject.parent = transform;
    }

    void OnDrawGizmos()
    {
        for (int i = 1; i < _line.Count; i++)
        {
            Gizmos.DrawLine(_line[i-1],_line[i]);
        }
    }

}
