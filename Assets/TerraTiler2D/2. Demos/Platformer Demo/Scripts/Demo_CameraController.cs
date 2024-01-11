using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [RequireComponent(typeof(Camera))]
    public class Demo_CameraController : MonoBehaviour
    {
        private const int hardThreshold = 10;
        private const int softThreshold = 5;

        private Camera myCamera;
        private GameObject myTarget;

        private Vector3 currentPosition;

        // Start is called before the first frame update
        void Awake()
        {
            myCamera = GetComponent<Camera>();
            EventManager.GetInstance().AddListener<Demo_OnPlayerInstantiated>(SetTarget);
        }

        private void SetTarget(Demo_OnPlayerInstantiated playerEvent)
        {
            myTarget = playerEvent.player.gameObject;
        }

        private void FollowTarget()
        {
            if (myTarget == null)
            {
                return;
            }

            if (currentPosition == null || (transform.position - currentPosition).magnitude > hardThreshold * 2 || currentPosition.z != transform.position.z)
            {
                currentPosition = transform.position;
            }

            if ((myTarget.transform.position - currentPosition).magnitude > hardThreshold)
            {
                currentPosition = new Vector3(myTarget.transform.position.x + (currentPosition - myTarget.transform.position).normalized.x * hardThreshold, myTarget.transform.position.y + (currentPosition - myTarget.transform.position).normalized.y * hardThreshold, currentPosition.z);
            }
            if ((myTarget.transform.position - currentPosition).magnitude >= softThreshold)
            {
                currentPosition += (Vector3)((Vector2)myTarget.transform.position - (Vector2)currentPosition) * 0.02f;
            }

            transform.position = currentPosition;

            if (transform.eulerAngles != Vector3.zero)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            FollowTarget();
        }

        private void OnDestroy()
        {
            EventManager.GetInstance().RemoveListener<Demo_OnPlayerInstantiated>(SetTarget);
        }
    }
}
