using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class WaterSpring : MonoBehaviour
{
    public float velocity = 0f;
    private float force = 0f;
    public float height = 0f;
    private float target_height = 0f;

    public Transform springTransform;
    [SerializeField] private SpriteShapeController spriteShapeController = null;
    private int waveIndex = 0;
    private List<WaterSpring> springs = new();
    private float resistance = 40f;

    public void WaveSpringUpdate(float springSitffness, float dampening)
    {
        height = transform.localPosition.y;
        var x = height - target_height;
        var loss = -dampening * velocity;

        force = -springSitffness * x + loss;
        velocity += force;

        var y = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, y + velocity, transform.localPosition.z);
    }

    public void Init(SpriteShapeController ssc)
    {
        var index = transform.GetSiblingIndex();
        waveIndex = index + 1;
        spriteShapeController = ssc;

        velocity = 0;
        height = transform.localPosition.y;
        target_height = transform.localPosition.y;
    }

    public void WavePointUpdate()
    {
        if (spriteShapeController != null)
        {
            Spline waterSpline = spriteShapeController.spline;
            Vector3 wavePosition = waterSpline.GetPosition(waveIndex);
            waterSpline.SetPosition(waveIndex, new Vector3(wavePosition.x, transform.localPosition.y, wavePosition.z));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            AudioManager.instance.PlaySound(AudioManager.instance.waterSplash, gameObject);

            var speed = collider.gameObject.GetComponent<Rigidbody2D>().velocity;

            velocity += speed.y / resistance;
        }
    }
}
