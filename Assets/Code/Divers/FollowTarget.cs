using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform followTarget;

    // Update is called once per frame
    void Update()
    {
        transform.position = followTarget.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
