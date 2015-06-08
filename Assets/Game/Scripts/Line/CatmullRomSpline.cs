using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour {

	public Vector3[] points =
	{
		new Vector3(-1, 1, 0),
		new Vector3(1, 1, 0)
	};
	
	public int ResolutionPerSegment
    {
        get
        {
            return 16;
        }
    }


	public bool isGridLocked = true;
	public float gridWidth = 1.0f;

	public event Action LineUpdatedEvent;
	
	static Vector3 CatmullRomSplineFormula(Vector3[] points, float t)
	{
		return 0.5f * ((2.0f * points[1]) + 
			(-points[0] + points[2]) * t + 
			(2.0f * points[0] - 5.0f * points[1] + 4.0f * points[2] - points[3]) * Mathf.Pow(t, 2) + 
			(-points[0] + 3.0f * points[1] - 3.0f * points[2] + points[3]) * Mathf.Pow(t, 3));
	}
	
	public Vector3 GetPoint(float t)
	{
		if (t < 0 || t > points.Length - 1) throw new System.Exception("Parameter out of bouns. T-value = " + t.ToString());
		
		int baseT = Mathf.FloorToInt(t);
		
		List<Vector3> pointList = new List<Vector3>();
		
		int pointIndexStart = baseT - 1;
		int pointIndexEnd = baseT + 4; 
		
		for (int i = pointIndexStart; i < pointIndexEnd; i++)
		{
			if (i < 0) pointList.Add(points[0]);
			else if (i >= points.Length) pointList.Add(points[points.Length - 1]);
			else pointList.Add(points[i]);
		}
		
		return transform.TransformPoint(CatmullRomSplineFormula(pointList.ToArray(), t - (float)baseT));
	}
	
	public Vector3 GetNormalizedPoint(float t)
	{
		if (t < 0 || t > 1) throw new System.Exception("Parameter out of bouns. T-value = " + t.ToString());
		
		float absoluteT = t * (float)(points.Length - 1);
		
		return GetPoint(absoluteT);
	}

    float GetDistancePerSegment(int index)
    {
        float distance = 0;

        for (int i = 1; i <= ResolutionPerSegment; i++)
        {
            distance += Vector3.Distance(GetPoint((float)(i - 1) / (float)ResolutionPerSegment + (float)index),
                         GetPoint((float)i / (float)ResolutionPerSegment + (float)index));
        }

        return distance;
    }

    public float GetTotalDistance()
    {
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            distance += GetDistancePerSegment(i);
        }

        return distance;
    }

    public Vector3 GetPointByDistance(float distance)
    {
        // TODO: Optimize algo
        float distanceIterated = 0;
        int pointIndex = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            float testDistance = distanceIterated + GetDistancePerSegment(i);
            pointIndex = i;
            if (testDistance >= distance) break;
            distanceIterated = testDistance;
        }

        Vector3 fromPoint = Vector3.zero;
        Vector3 toPoint = Vector3.zero;
        float distanceLeft = distance - distanceIterated;
        float segmentDistanceIterated = 0;
        for (int i = 1; i <= ResolutionPerSegment; i++)
        {
            fromPoint = GetPoint((float)(i - 1) / (float)ResolutionPerSegment + (float)pointIndex);
            toPoint = GetPoint((float)i / (float)ResolutionPerSegment + (float)pointIndex);
            segmentDistanceIterated += Vector3.Distance(fromPoint, toPoint);
            if (segmentDistanceIterated >= distanceLeft) break;
        }

        return Vector3.Lerp(fromPoint, toPoint, 1 - ((segmentDistanceIterated - distanceLeft) / Vector3.Distance(fromPoint, toPoint)));
    }

	public void TriggerUpdateEvent()
	{
		if (LineUpdatedEvent != null) LineUpdatedEvent();
	}
}
