using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform player;
    public float speed = 3f;

    public bool isLerp;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
    }

    private void Update()
    {
        if (isLerp)
        {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, player.transform.position.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, player.transform.position.x, interpolation);

            this.transform.position = position;
        }
        else
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
    }
}
