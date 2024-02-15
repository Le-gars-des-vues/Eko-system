using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererSmoother : MonoBehaviour
{
    public LineRenderer line;
    public Vector3[] initialState;
    public float smoothingLength = 2f;
    public int smoothingSection = 10;
}
