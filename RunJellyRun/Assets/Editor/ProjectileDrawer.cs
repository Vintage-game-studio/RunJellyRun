using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ProjectileWrapper))]
public class ProjectileDrawer : Editor
{
    public GameObject TargetObj;

    private Material material;
    private List<Vector3> linePairs;
    private ProjectileWrapper _projectileWrapper;
    private bool isDrawing = false;

    private static bool enableInitialVelocity;
    private static bool enableInitialAngle;
    private static bool enableTargetPos;
    private static bool enableMaxHeight;
    private static bool enableDuration;
    private static bool enableGravity;

    private static float _initialVelocity;
    private static float _initialAngle;
    private Vector2 _targetPos;
    private static float _maxHeight;
    private static float _duration;
    private static float _gravity;

    void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        linePairs = new List<Vector3>();
    }

    public override void OnInspectorGUI()
    {     
        DrawDefaultInspector();
        
        EditorGUIUtility.wideMode = true;
        _projectileWrapper = (ProjectileWrapper) target;

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableInitialVelocity =
            EditorGUILayout.Toggle("InitialVelocity", enableInitialVelocity, new GUILayoutOption[] {});
        if (enableInitialVelocity)
        {
            _initialVelocity = EditorGUILayout.FloatField(_initialVelocity);
            _projectileWrapper.SetInitialVelocity(_initialVelocity);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableInitialAngle = EditorGUILayout.Toggle("InitialAngle", enableInitialAngle, new GUILayoutOption[] {});
        if (enableInitialAngle)
        {
            _initialAngle = EditorGUILayout.FloatField(_initialAngle);
            _projectileWrapper.SetInitialAngle(_initialAngle);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableTargetPos = EditorGUILayout.Toggle("TargetPos", enableTargetPos, new GUILayoutOption[] {});
        if (enableTargetPos)
        {
            if (TargetObj != null)
                _projectileWrapper.SetTargetPos(TargetObj.transform.position);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableMaxHeight = EditorGUILayout.Toggle("MaxHeight", enableMaxHeight, new GUILayoutOption[] {});
        if (enableMaxHeight)
        {
            _maxHeight = EditorGUILayout.FloatField(_maxHeight);
            _projectileWrapper.SetMaxHeight(_maxHeight);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableDuration = EditorGUILayout.Toggle("Duration", enableDuration, new GUILayoutOption[] {});
        if (enableDuration)
        {
            _duration = EditorGUILayout.FloatField(_duration);
            _projectileWrapper.SetDuration(_duration);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        enableGravity = EditorGUILayout.Toggle("Gravity", enableGravity, new GUILayoutOption[] {});
        if (enableGravity)
        {
            _gravity = EditorGUILayout.FloatField(_gravity);
            _projectileWrapper.SetDuration(_gravity);
        }
        GUILayout.EndHorizontal();

        Rect layoutRectangle = GUILayoutUtility.GetRect(10, 0, EditorGUIUtility.currentViewWidth / 2,
            EditorGUIUtility.currentViewWidth / 8);

        linePairs = _projectileWrapper.GetProjectile().Select(s => new Vector3(s.x, s.y, 0)).ToList();
        
        GUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(layoutRectangle);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(layoutRectangle.width, 0, 0);
            GL.Vertex3(layoutRectangle.width, layoutRectangle.height, 0);
            GL.Vertex3(0, layoutRectangle.height, 0);
            GL.End();

            if (isDrawing)
            {
                GL.Color(new Color(0.5f, 0.5f, 0.9f));
                GL.Begin(GL.LINE_STRIP);
                foreach (Vector3 point in linePairs)
                {
                    GL.Vertex3(layoutRectangle.width / 4 + layoutRectangle.width * point.x,
                        -point.y * layoutRectangle.height + layoutRectangle.height, point.z);
                }
                GL.End();
            }

            GL.PopMatrix();
            GUI.EndClip();
        }
        // End our horizontal 
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Draw Projectile"))
        {
            //linePairs = _projectileWrapper.GetProjectileSamples();
            isDrawing = true;
        }
    }
}
