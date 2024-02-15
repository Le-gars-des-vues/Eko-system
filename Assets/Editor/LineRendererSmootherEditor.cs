using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LineRendererSmootherEditor : Editor
{
    private LineRendererSmoother smoother;

    private SerializedProperty line;
    private SerializedProperty initialState;
    private SerializedProperty smoothingLength;
    private SerializedProperty smoothingSection;

    private GUIContent UpdateInitialStateGUIContent = new GUIContent("Set Initial State");
    private GUIContent SmoothButtonGUIContent = new GUIContent("Smooth Path");
    private GUIContent RestoreDefaultGUIContent = new GUIContent("Restore Default Path");

    private bool expandCurves = false;
    private BezierCurve[] curves;

    private void OnEnable()
    {
        smoother = (LineRendererSmoother)target;

        if (smoother.line == null)
        {
            smoother.line = smoother.GetComponent<LineRenderer>();
        }

        line = serializedObject.FindProperty("line");
        initialState = serializedObject.FindProperty("initialState");
        smoothingLength = serializedObject.FindProperty("smoothingLength");
        smoothingSection = serializedObject.FindProperty("smoothingSection");

        EnsureCurveMatchLineLineRendererPositions();
    }

    public override void OnInspectorGUI()
    {
        if (smoother == null)
        {
            return;
        }
        EnsureCurveMatchLineLineRendererPositions();

        EditorGUILayout.PropertyField(line);
        EditorGUILayout.PropertyField(initialState);
        EditorGUILayout.PropertyField(smoothingLength);
        EditorGUILayout.PropertyField(smoothingSection);

        if (GUILayout.Button(UpdateInitialStateGUIContent))
        {
            smoother.initialState = new Vector3[smoother.line.positionCount];
            smoother.line.GetPositions(smoother.initialState);
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUI.enabled = smoother.line.positionCount >= 1;
            if (GUILayout.Button(SmoothButtonGUIContent))
            {
                SmoothPath();
            }

            bool lineRendererPathAndInitialStateAreSame = smoother.line.positionCount == smoother.initialState.Length;

            if (lineRendererPathAndInitialStateAreSame)
            {
                Vector3[] positions = new Vector3[smoother.line.positionCount];
                smoother.line.GetPositions(positions);

                lineRendererPathAndInitialStateAreSame = positions.SequenceEqual(smoother.initialState);
            }
            GUI.enabled = !lineRendererPathAndInitialStateAreSame;
            if (GUILayout.Button(RestoreDefaultGUIContent))
            {
                smoother.line.positionCount = smoother.initialState.Length;
                smoother.line.SetPositions(smoother.initialState);

                if (curves.Length != smoother.line.positionCount - 1)
                {
                    curves = new BezierCurve[smoother.line.positionCount - 1];
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void SmoothPath()
    {
        smoother.line.positionCount = curves.Length * smoothingSection.intValue;
        int index = 0;
        for (int i = 0; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSection.intValue);
            for (int j = 0; j < segments.Length; j++)
            {
                smoother.line.SetPosition(index, segments[i]);
                index++;
            }
        }

        smoothingSection.intValue = 1;
        smoothingLength.floatValue = 0;
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (smoother.line.positionCount < 3)
        {
            return;
        }
        EnsureCurveMatchLineLineRendererPositions();

        for (int i = 0; i < curves.Length; i++)
        {
            Vector3 position = smoother.line.GetPosition(i);
            Vector3 lastPosition = i == 0 ? smoother.line.GetPosition(0) : smoother.line.GetPosition(i - 1);
            Vector3 nextPosition = smoother.line.GetPosition(i + 1);

            Vector3 lastDirection = (position - lastPosition).normalized;
            Vector3 nextDirection = (nextPosition - position).normalized;

            Vector3 startTangent = (lastDirection + nextDirection) * smoothingLength.floatValue;
            Vector3 endTangent = (nextDirection + lastDirection) * -1 * smoothingLength.floatValue;

            Handles.color = Color.green;
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), position + startTangent, Quaternion.identity, 0.25f, EventType.Repaint);

            if (i != 0)
            {
                Handles.color = Color.blue;
                Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), nextPosition + endTangent, Quaternion.identity, 0.25f, EventType.Repaint);
            }

            curves[i].points[0] = position;
            curves[i].points[1] = position + startTangent;
            curves[i].points[2] = nextPosition + endTangent;
            curves[i].points[3] = nextPosition;
        }

        {
            Vector3 nextDirection = (curves[1].EndPosition - curves[1].StartPosition).normalized;
            Vector3 lastDirection = (curves[0].EndPosition - curves[0].StartPosition).normalized;

            curves[0].points[2] = curves[0].points[3] +
                (nextDirection + lastDirection) * -1 * smoothingLength.floatValue;

            Handles.color = Color.blue;
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), curves[0].points[2], Quaternion.identity, 0.25f, EventType.Repaint);
        }

        DrawSegments();
    }

    private void DrawSegments()
    {
        for (int i = 0; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSection.intValue);
            for (int j = 0; j < segments.Length; j++)
            {
                Handles.color = Color.white;
                Handles.DrawLine(segments[j], segments[j + 1]);

                float color = (float)j / segments.Length;
                Handles.color = new Color(color, color, color);
                Handles.Label(segments[j], $"C{i} S{j}");
                Handles.color = Color.green;
                Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[j], Quaternion.identity, 0.05f, EventType.Repaint);
            }

            Handles.color = Color.white;
            Handles.Label(segments[segments.Length - 1], $"C{i} S{segments.Length - 1}");
            Handles.DotHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), segments[segments.Length - 1], Quaternion.identity, 0.05f, EventType.Repaint);

            Handles.DrawLine(segments[segments.Length - 1], curves[i].EndPosition);
        }
    }

    private void EnsureCurveMatchLineLineRendererPositions()
    {
        if (curves.Length != smoother.line.positionCount - 1)
        {
            curves = new BezierCurve[smoother.line.positionCount - 1];
            for (int i = 0; i < curves.Length; i++)
            {
                curves[i] = new BezierCurve();
            }
        }
    }
}
    
