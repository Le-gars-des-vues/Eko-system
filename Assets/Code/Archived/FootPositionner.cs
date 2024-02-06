using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPositionner : MonoBehaviour
{
    // reference to player character object
    public GameObject playerObj;

    // reference to IK target
    public Transform target;

    // reference to the other foot
    public FootPositionner otherFoot;

    public bool isBalanced;

    // used to lerp the foot from its current position to target position
    public float lerp;

    // the start and end position of a step
    private Vector3 startPos;
    private Vector3 endPos;

    // how far should we anticipate a step
    public float overShootFactor = 0.5f;

    // how fast the foot moves
    public float stepSpeed = 3f;

    // the foot's displacement from body center on the X axis
    public float footDisplacementOnX;
    private float otherFootOvershoot;

    //Position du pied au milieu de la step
    private Vector3 midPos;

    //Check direction
    private bool isFacingRight;

    private void Start()
    {
        isFacingRight = true;
        startPos = midPos = endPos = target.position;
        otherFootOvershoot = otherFoot.overShootFactor;
    }

    private void Update()
    {
        UpdateBalance();

        
        if (Input.GetAxis("Horizontal") != 0)
        {
            CheckDirectionToFace(Input.GetAxis("Horizontal") > 0);
        }
        

        // this foot can only move when: (1) the other foot finishes moving, (2) the other foot made the last step
        bool thisFootCanMove = otherFoot.lerp > 1 && lerp > otherFoot.lerp;

        // if the body is not balanced AND this foot has finished its previous step (we don't want to calculate new steps in the process of moving a foot)
        if (!isBalanced && lerp > 1 && thisFootCanMove)
        {
            CalculateNewStep();
        }

        // using ease in/ease out value will make the animation look more natural
        float easedLerp = EaseInOutCubic(lerp);

        // a lerping method that draws an arc using startPos, midPos, and endPos
        target.position = Vector3.Lerp(
            Vector3.Lerp(startPos, midPos, easedLerp),
            Vector3.Lerp(midPos, endPos, easedLerp),
            easedLerp
            );
        lerp += Time.deltaTime * stepSpeed;
    }

    private void UpdateBalance()
    {
        // get center of mass in world position
        float centerOfMass = playerObj.transform.position.x;
        // if center of mass is between two feet, the body is balanced
        isBalanced = IsFloatInRange(centerOfMass, target.position.x, otherFoot.target.position.x);
    }

    /// returns true if "value" is between "bound1" and "bound2"
    bool IsFloatInRange(float value, float bound1, float bound2)
    {
        float minValue = Mathf.Min(bound1, bound2);
        float maxValue = Mathf.Max(bound1, bound2);
        return value > minValue && value < maxValue;
    }

    /// Smoothly ease in and ease out the input using sigmoid function
    private float EaseInOutCubic(float x)
    {
        return 1f / (1 + Mathf.Exp(-10 * (x - 0.5f)));
    }

    /// Calculate where the new step should be made
    private void CalculateNewStep()
    {
        // set starting position
        startPos = target.position;

        // this will make the foot start moving to its target position starting from next frame
        lerp = 0;

        // find where the foot should land without considering overshoot
        RaycastHit2D ray = Physics2D.Raycast(playerObj.transform.position + new Vector3(footDisplacementOnX, 0, 0), Vector2.down, 10);

        // consider the overshoot factor
        Vector3 posDiff = ((Vector3)ray.point - target.position) * (1 + overShootFactor);

        // find end target position
        endPos = target.position + posDiff;

        // midPos is the mid point between startPos and endPos, but lifted up a bit depending on stepSize
        float stepSize = Vector3.Distance(startPos, endPos);
        midPos = startPos + posDiff / 2f + new Vector3(0, stepSize * 0.2f);
    }

    /// This helps visualize the target position in run time
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPos, 0.1f);
    }

    private void Turn()
    {
        overShootFactor = otherFootOvershoot;
        otherFootOvershoot = otherFoot.overShootFactor;
        footDisplacementOnX *= -1;
        isFacingRight = !isFacingRight;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }
    }
}
