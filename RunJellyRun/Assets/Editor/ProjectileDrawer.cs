using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ProjectileCalculation))]
public class ProjectileDrawer : Editor
{
    private Material material;
    private List<Vector3> linePairs;
    private ProjectileCalculation _projectileCalculation;
    private bool isDrawing = false;
    
    void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
        linePairs = new List<Vector3>();
    }

    public override void OnInspectorGUI()
    {
        _projectileCalculation = (ProjectileCalculation) target;
        
        DrawDefaultInspector();
        Rect layoutRectangle = GUILayoutUtility.GetRect(10, 10000, 200, 200);
        if (GUILayout.Button("Draw Projectile"))
        {
            linePairs = _projectileCalculation.GetProjectilePoints(layoutRectangle.width,layoutRectangle.height);
            isDrawing = true;
        }
        
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
                    GL.Vertex3(layoutRectangle.width/4+point.x, -point.y+layoutRectangle.height, point.z);
                }
                GL.End();                
            }

            GL.PopMatrix();
            GUI.EndClip();
        }

        // End our horizontal 
        GUILayout.EndHorizontal();
    }
}
