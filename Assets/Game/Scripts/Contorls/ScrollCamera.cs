using UnityEngine;
using System.Collections;

public class ScrollCamera : MonoBehaviour {

	public Transform cameraPos;
	public Transform minPos;
	public Transform maxPos;

	public bool isScrolling;

	void Update()
	{
		isScrolling = false;
		if (Input.touchCount > 2)
		{
			isScrolling = true;
			if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
			{
				if (Input.touches[0].phase == TouchPhase.Moved)
				{
					Vector3 pos = cameraPos.position;
					pos += new Vector3(Input.touches[0].deltaPosition.x, Input.touches[0].deltaPosition.y) * Time.deltaTime;
					pos.x = Mathf.Clamp(pos.x, minPos.position.x, maxPos.position.x);
					pos.y = Mathf.Clamp(pos.y, minPos.position.y, maxPos.position.y);
					pos.z = Mathf.Clamp(pos.z, minPos.position.z, maxPos.position.z);
					cameraPos.position = pos;
				}
				else if (Input.touches[1].phase == TouchPhase.Moved)
				{
					Vector3 pos = cameraPos.position;
					pos += new Vector3(Input.touches[1].deltaPosition.x, Input.touches[1].deltaPosition.y) * Time.deltaTime;
					pos.x = Mathf.Clamp(pos.x, minPos.position.x, maxPos.position.x);
					pos.y = Mathf.Clamp(pos.y, minPos.position.y, maxPos.position.y);
					pos.z = Mathf.Clamp(pos.z, minPos.position.z, maxPos.position.z);
					cameraPos.position = pos;
				}
			}
		}
	}
}
