using UnityEngine;
using System.Collections;

public class RestartLevelTrigger : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
