using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerWaypointScript : MonoBehaviour
{
    public Transform playerTransform;
    public float minimapHeight;
    public GameObject waypointParent;
    public GameObject minimapSolution;
    public List<Transform> waypoints;

    private Transform closestWaypoint;

    private void Start()
    {
        foreach (Transform item in waypointParent.GetComponentsInChildren<Transform>())
        {
            waypoints.Add(item);
        }
        waypoints.RemoveAt(0);
    }

    void LateUpdate()
    {
        if (waypoints.Count < 2)
            minimapSolution.SetActive(false);

        Vector3 newPosition = playerTransform.position;
        newPosition.y = minimapHeight;
        transform.position = newPosition;

        closestWaypoint = GetClosestWaypoint(waypoints);

        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, closestWaypoint.position);
    }

    Transform GetClosestWaypoint(List<Transform> waypoints)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in waypoints)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget > 100)
            {
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            else
            {
                int lenghtOf = waypoints.IndexOf(potentialTarget);
                for (int i = 0; i <= lenghtOf; i++)
                {
                    Transform temp = waypoints[0];
                    waypoints.RemoveAt(0);
                    temp.gameObject.SetActive(false);
                } 
                return waypoints[0];
            } 
        }
        int indexOfBest = waypoints.IndexOf(bestTarget);
        if (indexOfBest != 0)
        {
            for (int i = 0; i < indexOfBest; i++)
            {
                Transform temp = waypoints[0];
                waypoints.RemoveAt(0);
                temp.gameObject.SetActive(false);
            }
        }
        return bestTarget;
    }
}
