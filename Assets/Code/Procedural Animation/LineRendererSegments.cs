using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererSegments
{
    public Vector3 startingPosition;
    public Vector3 endingPosition;
    public float length;

    public void Follow(LineRendererSegments targetSegment)
    {
        Follow(targetSegment.startingPosition);
    }

    public void Follow(Vector3 targetPosition)
    {
        endingPosition = targetPosition;
        Vector3 difference = endingPosition - startingPosition;
        Vector3 normalized = difference.normalized;
        startingPosition = endingPosition - (normalized * length);
    }

    public void AnchorStartAt(Vector3 targetPosition)
    {
        Vector3 difference = endingPosition - startingPosition;
        startingPosition = targetPosition;
        endingPosition = startingPosition + difference;
    }

    #region videospecific
    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 difference = targetPosition - startingPosition;
        endingPosition = startingPosition + (difference.normalized * length);
    }
    public void AnchorEndAt(Vector3 targetPosition)
    {
        Vector3 difference = endingPosition - startingPosition;
        endingPosition = targetPosition;
        startingPosition = endingPosition - difference;
    }
    #endregion
}
