using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour {

	public Transform lineEffect;
	TrailRendererWith2DCollider lineCollider;
	bool isPressed = false;

	// Use this for initialization
	void Start () {
		lineCollider = lineEffect.GetComponent<TrailRendererWith2DCollider>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Input.GetMouseButton(0))
		{
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Physics2D.OverlapPoint(new Vector2(worldPos.x, worldPos.y)))
			{
				if (!isPressed) Pressed(worldPos);
				else Move(worldPos);
			}
			else Unpressed();
		}
		else Unpressed();
	}

	void Pressed(Vector3 curPos)
	{
		isPressed = true;
		Vector3 pos = lineEffect.position;
		pos.x = curPos.x;
		pos.y = curPos.y;
		lineEffect.position = pos;
		lineCollider.CutOff();
	}

	void Move(Vector3 curPos)
	{
		Vector3 pos = lineEffect.transform.position;
		pos.x = curPos.x;
		pos.y = curPos.y;
		lineEffect.transform.position = pos;
	}

	void Unpressed()
	{
		isPressed = false;
		lineCollider.ClearUndrawnTrails();
	}
}
