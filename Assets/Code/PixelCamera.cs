using UnityEngine;
public class PixelCamera : MonoBehaviour
{
    Camera cam;
    float lastRenderTime = 0;
    public int frameRate = 12;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        cam.enabled = false;
    }

    void Update()
    {
        if (Time.time - lastRenderTime > 1f / frameRate)
        {
            lastRenderTime = Time.time;
            cam.Render();
        }
    }
}
