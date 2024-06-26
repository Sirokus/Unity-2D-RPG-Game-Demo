using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    public static WayPointManager ins;

    public Dictionary<string, WayPoint> wayPoints = new Dictionary<string, WayPoint>();

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this);
            return;
        }
        ins = this;

        WayPoint[] points = FindObjectsByType<WayPoint>(FindObjectsSortMode.None);
        foreach (var point in points)
        {
            wayPoints.Add(point.WayPointName, point);
        }
    }

    public static void ResetWayPoint(string name)
    {
        ins.wayPoints[name].used = false;
    }
}
