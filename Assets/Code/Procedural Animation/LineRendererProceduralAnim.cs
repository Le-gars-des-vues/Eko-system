using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererProceduralAnim : MonoBehaviour
{
    public Vector3 targetPosition;
    public bool isFixed;
    public Transform anchoredStart;
    public int segmentAmount = 10;
    public float segmentsLength = 1;
    public bool followMouse;

    LineRendererSegments[] segments;
    LineRenderer lineRenderer;
    [SerializeField] Transform plantTransform;

    public Renderer MyRenderer;
    public string MySortingLayer;
    public int MySortingOrderInLayer;

    void Awake()
    {
        InitializeSegments();
    }

    // Use this for initialization
    void Start()
    {
        if (MyRenderer == null)
            MyRenderer = this.GetComponent<Renderer>();
    }

    void InitializeSegments()
    {
        anchoredStart = transform;
        lineRenderer = this.GetComponent<LineRenderer>();
        segments = new LineRendererSegments[segmentAmount];
        for (int i = 0; i < segmentAmount; i++)
        {
            LineRendererSegments segment = new LineRendererSegments();
            segment.startingPosition = Vector3.zero + (Vector3.up * segmentsLength * (i));
            segment.endingPosition = segment.startingPosition + (Vector3.up * segmentsLength);
            segment.length = segmentsLength;
            segments[i] = segment;
        }
    }

    void Update()
    {
        if (segmentAmount != segments.Length)
        {
            InitializeSegments(); //reinitialize if amount changed
        }
        if (followMouse)
        {
            targetPosition = GetWorldPositionFromMouse();
        }
        else
            targetPosition = plantTransform.position;

        Follow();
        DrawSegments(segments);

        if (MyRenderer == null)
            MyRenderer = this.GetComponent<Renderer>();
        MyRenderer.sortingLayerName = MySortingLayer;
        MyRenderer.sortingOrder = MySortingOrderInLayer;
    }

    void Follow()
    {
        segments[segmentAmount - 1].Follow(targetPosition);
        for (int i = segmentAmount - 2; i >= (segmentAmount / 5); i--)
        {
            segments[i].Follow(segments[i + 1]);
        }

        if (isFixed)
        {
            segments[0].AnchorStartAt(anchoredStart.position);
            for (int i = 1; i < segmentAmount; i++)
            {
                segments[i].AnchorStartAt(segments[i - 1].endingPosition);
            }
        }
    }

    void DrawSegments(LineRendererSegments[] segments)
    {
        lineRenderer.positionCount = segmentAmount + 1;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < segmentAmount; i++)
        {
            points.Add(segments[i].startingPosition);
        }
        points.Add(segments[segmentAmount - 1].endingPosition);
        //lineRenderer.sortingLayerName = "PixelateBackground";
        lineRenderer.SetPositions(points.ToArray());
    }

    Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}
