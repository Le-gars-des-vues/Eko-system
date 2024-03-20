using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterShapeController : MonoBehaviour
{
    [SerializeField] private float springStiffness = 0.1f;
    [SerializeField] private float springDampening = 0.3f;
    [SerializeField] private float spread = 0.006f;

    [SerializeField] private List<WaterSpring> springs = new();

    private int cornersCount = 2;
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField][Range(1, 100)] private int waveCount = 6;

    [SerializeField] private GameObject wavePointPrefab;
    [SerializeField] private GameObject wavePoints;

    void OnValidate()
    {
        // Clean waterpoints 
        StartCoroutine(CreateWaves());
    }

    IEnumerator CreateWaves()
    {
        foreach (Transform child in wavePoints.transform)
        {
            StartCoroutine(Destroy(child.gameObject));
        }
        yield return null;
        SetWaves();
        yield return null;
    }
    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }

    private void SetWaves()
    {
        Spline waterSpline = spriteShapeController.spline;
        int waterPointCount = waterSpline.GetPointCount();

        for (int i = cornersCount; i < waterPointCount - cornersCount; i++)
        {
            waterSpline.RemovePointAt(cornersCount);
        }

        Vector3 waterTopLeftCorner = waterSpline.GetPosition(1);
        Vector3 waterTopRightCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;

        float spacingPerWave = waterWidth / (waveCount + 1);

        for (int i = waveCount; i > 0; i--)
        {
            int index = cornersCount;

            float xPosition = waterTopLeftCorner.x + (spacingPerWave * i);
            Vector3 wavePoint = new Vector3(xPosition, waterTopLeftCorner.y, waterTopLeftCorner.z);
            waterSpline.InsertPointAt(index, wavePoint);
            waterSpline.SetHeight(index, 0.1f);
            waterSpline.SetCorner(index, false);
        }
    }

    private void CreateSprings(Spline waterSpline)
    {
        springs = new();
        for (int i = 0; i <= waveCount + 1; i++)
        {
            int index = i + 1;
            GameObject wavePoint = Instantiate(wavePointPrefab, wavePoints.transform, false);
            wavePoint.transform.localPosition = waterSpline.GetPosition(index);
            WaterSpring waterSpring = wavePoint.GetComponent<WaterSpring>();
            waterSpring.Init(spriteShapeController);
            springs.Add(waterSpring);
        }
    }

    private void Smoothen(Spline waterSpline, int index)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;
        if (index > 1)
        {
            positionPrev = waterSpline.GetPosition(index - 1);
        }
        if (index - 1 <= waveCount)
        {
            positionNext = waterSpline.GetPosition(index + 1);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;

        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 rightTangent = (positionNext - position).normalized * scale;

        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);

        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, rightTangent);
    }

    private void FixedUpdate()
    {
        foreach (WaterSpring spring in springs)
        {
            spring.WaveSpringUpdate(springStiffness, springDampening);
        }
        UpdateSprings();
    }

    private void UpdateSprings()
    {
        int count = springs.Count;
        float[] leftDeltas = new float[count];
        float[] rightDeltas = new float[count];
        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                leftDeltas[i] = spread * (springs[i].height - springs[i - 1].height);
                springs[i - 1].velocity += leftDeltas[i];
            }
            if (i < springs.Count - 1)
            {
                rightDeltas[i] = spread * (springs[i].height - springs[i + 1].height);
                springs[i + 1].velocity += rightDeltas[i];
            }
        }
    }

    private void Splash(int index, float speed)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].velocity += speed;
        }
    }
}
