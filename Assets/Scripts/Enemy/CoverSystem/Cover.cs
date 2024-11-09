using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Cover Points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints;
    [SerializeField] private float xOffset = 1.25f;
    [SerializeField] private float yOffset = .2f;
    [SerializeField] private float zOffset = 1.25f;

    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform;
    }
    public List<CoverPoint> GetCoverPoints() => coverPoints;

    private void GenerateCoverPoints()
    {
        Vector3[] localPositions =
        {
            new Vector3(xOffset, yOffset, 0),
            new Vector3(-xOffset, yOffset, 0),
            new Vector3(0, yOffset, zOffset),
            new Vector3(0, yOffset, -zOffset)
        };

        foreach(var position in localPositions )
        {
            Vector3 worldPosition = transform.TransformPoint(position);
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPosition, Quaternion.identity).GetComponent<CoverPoint>();
            coverPoints.Add(coverPoint);
        }
    }

    public List<CoverPoint> GetValidCoverPoints(Transform enemy)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();

        foreach(CoverPoint coverPoint in coverPoints )
        {
            if (IsValidCoverPoint(coverPoint, enemy))
            {
                validCoverPoints.Add(coverPoint);
            }
        }

        return validCoverPoints;
    }

    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemy)
    {
        if (coverPoint.isOccupied)
        {
            return false;
        }

        if(IsFutherestCoverPoint(coverPoint) == false)
        {
            return false;
        }

        if (IsCoverCloseToPlayer(coverPoint))
        {
            return false;
        }

        if (IsCoverBehindPlayer(coverPoint, enemy))
        {
            return false;
        }


        if (IsCoverCloseToLastCover(coverPoint, enemy))
        {
            return false;
        }

        return true;
    }

    private bool IsFutherestCoverPoint(CoverPoint coverPoint)
    {
        CoverPoint furtherestCoverPoint = null;
        float furtherestDistance = 0;

        foreach(CoverPoint point in coverPoints )
        {
            float currentDistance = Vector3.Distance(playerTransform.position, point.transform.position);
            if(currentDistance > furtherestDistance)
            {
                furtherestCoverPoint = point;
                furtherestDistance = currentDistance;
            }
        }

        return furtherestCoverPoint == coverPoint;
    }

    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemyTransform)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemyTransform.position);

        return distanceToPlayer < distanceToEnemy;
    }

    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.position) < 2;
    }

    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<EnemyRange>().lastCover;

        return lastCover != null &&
            Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
