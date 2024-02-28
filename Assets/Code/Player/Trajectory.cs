using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public float power;
    Rigidbody2D rb;
    LineRenderer line;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {

    }
}
