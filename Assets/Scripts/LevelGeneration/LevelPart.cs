using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo(entrancePoint, targetSnapPoint); // Important to align before snap
        SnapTo(entrancePoint, targetSnapPoint);
    }

    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        //// Calculate the rotation offset between level part's current rotation
        //// and it's own snap point's rotation. This help in fine-tuning the alignment later
        //var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        //Debug.Log(rotationOffset);

        //// Set level part's rotation to match target snap point rotation
        //// This is initial step to align the orentation of two parts
        //transform.rotation = targetSnapPoint.transform.rotation;

        //// This is necessary because the snap points are typically facing opposite directions
        //transform.Rotate(0, 180, 0);


        //transform.Rotate(0, -rotationOffset, 0);

        // Calculate the required rotation to align own snap point's forward to target's forward
        Quaternion rotationOffset = Quaternion.FromToRotation(ownSnapPoint.transform.forward, -targetSnapPoint.transform.forward);

        // Apply the rotation offset to align the part with the target snap point
        transform.rotation = rotationOffset * transform.rotation;
    }

    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // offset to get distance (vector 3) between this and own snappoint
        var offset = transform.position - ownSnapPoint.transform.position;

        // a new position base on target snap point
        var newPosition = targetSnapPoint.transform.position + offset;

        transform.position = newPosition;
    }
    
    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Start);
    public SnapPoint GetEndPoint() => GetSnapPointOfType(SnapPointType.End);

    private SnapPoint GetSnapPointOfType(SnapPointType snapPointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        // Collect snap point by specific type
        foreach(SnapPoint snapPoint in snapPoints)
        {
            if(snapPoint.pointType == snapPointType)
            {
                filteredSnapPoints.Add(snapPoint);
            }
        }

        // If there are matching snap point, chose one ramdomly
        if(filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        return null;
    }
}
