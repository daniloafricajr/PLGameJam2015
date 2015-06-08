using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(CatmullRomSpline))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshRenderer))]
public class LineRendererWith2DCollider : MonoBehaviour {

#if UNITY_EDITOR
	[MenuItem("Game Tools/Platform/Create Line")]
	public static void CreateLine()
	{
		GameObject gobj = new GameObject("Line");
		gobj.AddComponent<LineRendererWith2DCollider>();
		gobj.transform.position = Vector3.zero;
		gobj.layer = LayerMask.NameToLayer("Ground");
	}
#endif

	//************
	//
	// Fields
	//  
	//************

	public float changeTime = 0.5f;                 //time point when the trail begins changing its width (if widthStart != widthEnd)
	public float width = 1.0f;                 //the starting width of the trail
	public float vertexDistanceMin = 1.00f;         //the minimum distance between the center positions
	public Vector3 renderDirection = new Vector3(0, 0, -1); //the direction that the mesh of the trail will be rendered towards
	public float tilingScale = 1.0f;
	
	private CatmullRomSpline line;
	private PolygonCollider2D polyCollider; 
	
	[SerializeField][HideInInspector] private Mesh mesh;
	[SerializeField][HideInInspector] private List<Vector3> centerPositions = new List<Vector3>();
	[SerializeField][HideInInspector] private List<Vertex> leftVertices = new List<Vertex>();  
	[SerializeField][HideInInspector] private List<Vertex> rightVertices = new List<Vertex>();
	
	//************
	//
	// Private Unity Methods
	//
	//************


	private void OnEnable() {


		line = GetComponent<CatmullRomSpline> ();
		line.LineUpdatedEvent += UpdateLine;

		if (gameObject.GetComponent<MeshFilter>().sharedMesh != null)
			mesh = (Mesh)Instantiate(gameObject.GetComponent<MeshFilter>().sharedMesh);
		else
			mesh = new Mesh();
		mesh.name = "Platform";
		gameObject.GetComponent<MeshFilter>().mesh = mesh;

		polyCollider = gameObject.GetComponent<PolygonCollider2D>();
		UpdateLine();
	}

	//************
	//
	// Private Methods
	//
	//************

	private void UpdateLine()
	{
		UpdateVertices();
		SetMesh();
	}

	private void UpdateVertices()
	{
		float totalDistance = line.GetTotalDistance ();
		float currentDistance = 0;

		centerPositions.Clear();
		leftVertices.Clear();
		rightVertices.Clear();

		Vector3 centerPoint = transform.InverseTransformPoint(line.GetPointByDistance(currentDistance));
		centerPositions.Insert(0, centerPoint);

		Vector3 dirToCurrentPos;
		Vector3 cross;
		Vector3 leftPos;
		Vector3 rightPos;

		do
		{
			currentDistance = Mathf.Clamp(currentDistance + vertexDistanceMin, 0, totalDistance);
			centerPoint =  transform.InverseTransformPoint(line.GetPointByDistance(currentDistance));
			dirToCurrentPos = (centerPoint - centerPositions[0]).normalized;

			cross = Vector3.Cross(renderDirection, dirToCurrentPos);
			leftPos = centerPoint + (cross * -width * 0.5f);
			rightPos = centerPoint + (cross * width * 0.5f);

			leftVertices.Insert(0, new Vertex(leftPos, centerPoint, (leftPos - centerPoint).normalized));
			rightVertices.Insert(0, new Vertex(rightPos, centerPoint, (rightPos - centerPoint).normalized));

			centerPositions.Insert(0, centerPoint);
		}
		while (currentDistance < totalDistance);
	}

	/// <summary>
	/// Sets the mesh and the polygon collider of the mesh.
	/// </summary>
	private void SetMesh() {
		
		//create an array for the 1) trail vertices, 2) trail uvs, 3) trail triangles, and 4) vertices on the collider path
		Vector3[] vertices = new Vector3[centerPositions.Count * 2];
		Vector2[] uvs = new Vector2[centerPositions.Count * 2];
		int[] triangles = new int[ (centerPositions.Count - 1) * 6];
		Vector2[] colliderPath = new Vector2[ (centerPositions.Count - 1) * 2];

		int leftVertVal = 0;
		int rightVertVal = 0;
		
		float lastUVValue = 0;
		//iterate through all the pairs of vertices (left + right)
		for (int j = 0; j < leftVertices.Count; ++j) {
			Vertex leftVert = leftVertices[leftVertVal];
			Vertex rightVert = rightVertices[rightVertVal];
			
			//trail vertices
			int vertIndex = j * 2;
			vertices[vertIndex] = leftVert.Position;
			vertices[vertIndex + 1] = rightVert.Position;
			
			//collider vertices 
			colliderPath[j] = leftVert.Position;
			colliderPath[colliderPath.Length - (j + 1) ] = rightVert.Position;
			
			//trail uvs
			float uvValue;// = leftVert.TimeAlive / timeDelta;
			if (vertIndex == 0) uvValue = 0;
			else uvValue = lastUVValue + (Vector3.Distance(vertices[vertIndex], vertices[vertIndex - 2]) * tilingScale);
			lastUVValue = uvValue;
			
			uvs[vertIndex] = new Vector2(uvValue, 1);
			uvs[vertIndex + 1] = new Vector2(uvValue, 0);
			
			//trail triangles
			if (j > 0) {
				int triIndex = (j - 1) * 6;
				triangles[triIndex] = vertIndex -2;
				triangles[triIndex + 1] = vertIndex - 1;
				triangles[triIndex + 2] = vertIndex + 1;
				triangles[triIndex + 3] = vertIndex - 2;
				triangles[triIndex + 4] = vertIndex + 1;
				triangles[triIndex + 5] = vertIndex;
			}
			
			//increment the left and right vertex nodes
			leftVertVal++;
			rightVertVal++;
		}
		
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		polyCollider.SetPath(0, colliderPath);
	}
	
	//************
	//
	// Private Classes
	//
	//************

	[System.Serializable]
	public class Vertex {
		private Vector3 centerPosition; //the center position in the trail that this vertex was derived from
		private Vector3 derivedDirection; //the direction from the 1) center position to the 2) position of this vertex
		private float creationTime;
		
		public Vector3 Position { get; private set; }
		public float TimeAlive { get { return Time.time - creationTime; } }
		
		public void AdjustWidth(float width) {
			Position = centerPosition + (derivedDirection * width);
		}
		
		public Vertex(Vector3 position, Vector3 centerPosition, Vector3 derivedDirection) {
			this.Position = position;
			this.centerPosition = centerPosition;
			this.derivedDirection = derivedDirection;
			creationTime = Time.time;
		}
	}
}
