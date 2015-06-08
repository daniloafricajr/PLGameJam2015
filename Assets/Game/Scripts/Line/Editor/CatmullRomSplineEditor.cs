using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CatmullRomSpline))]
public class CatmullRomSplineEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		CatmullRomSpline spline = target as CatmullRomSpline;
		
		List<Vector3> pointList = new List<Vector3>(spline.points);

		GUILayout.BeginVertical();
		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal(EditorStyles.toolbar);
		GUILayout.Label("Points");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Add", EditorStyles.toolbarButton))
		{
			pointList.Add(spline.points[spline.points.Length - 1] + Vector3.right);
		}
		GUILayout.EndHorizontal();
		
		int removeItemIndex = -1;
		
		for (int i = 0; i < pointList.Count; i++)
		{
			GUILayout.BeginHorizontal();
			
			pointList[i] = spline.transform.InverseTransformPoint(EditorGUILayout.Vector3Field("#" + (i + 1).ToString(), spline.transform.TransformPoint(pointList[i])));
			
			if (pointList.Count > 2)
			{
				if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(20)))
				{
					removeItemIndex = i;
				}
			}
			
			GUILayout.EndHorizontal();
		}
		
		if (removeItemIndex != -1)
		{
			pointList.RemoveAt(removeItemIndex);
		}

		GUILayout.EndVertical();
		
		spline.points = pointList.ToArray();

		EditorGUILayout.LabelField("Total Distance", spline.GetTotalDistance().ToString());

		spline.isGridLocked = EditorGUILayout.Toggle("Use Grid", spline.isGridLocked);
		spline.gridWidth = EditorGUILayout.FloatField("Grid Size", spline.gridWidth);

		GUILayout.EndVertical();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			spline.TriggerUpdateEvent();
		}
	}


	Vector3 SetToGridPosition(float gridWidth, Vector3 vectIn)
	{
		vectIn.x /= gridWidth;
		vectIn.x = Mathf.Floor(vectIn.x);
		vectIn.x *= gridWidth;

		vectIn.y /= gridWidth;
		vectIn.y = Mathf.Floor(vectIn.y);
		vectIn.y *= gridWidth;
	
		vectIn.z /= gridWidth;
		vectIn.z = Mathf.Floor(vectIn.z);
		vectIn.z *= gridWidth;

		return vectIn;
	}
	
	void OnSceneGUI()
	{
		CatmullRomSpline spline = target as CatmullRomSpline;
		
		List<Vector3> pointList = new List<Vector3>(spline.points);
		
		for (int i = 0; i < pointList.Count; i++)
		{
			pointList[i] = spline.transform.InverseTransformPoint(Handles.PositionHandle(spline.transform.TransformPoint(pointList[i]), spline.transform.rotation));
			if (spline.isGridLocked && spline.gridWidth > 0) pointList[i] = SetToGridPosition(spline.gridWidth, pointList[i]);
			Handles.Label(spline.transform.TransformPoint(pointList[i]), "#" + (i+1).ToString());
		}
		
		for (int i = 0; i < pointList.Count -1 ; i++)
		{
			for (int j = 1; j <= spline.ResolutionPerSegment; j++)
			{
				Handles.DrawLine(spline.GetPoint((float)(j - 1) / (float)spline.ResolutionPerSegment + (float)i),
					spline.GetPoint((float)j / (float)spline.ResolutionPerSegment + (float)i));
			}
		}
		
		spline.points = pointList.ToArray();
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			spline.TriggerUpdateEvent();
		}
	}
}
