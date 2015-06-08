using UnityEngine;
using System.Collections;

public class LoadNextLevelTrigger : MonoBehaviour {

	public string nextLevel = string.Empty;

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log("Loading Level: " + nextLevel);
			Application.LoadLevel(nextLevel);
		}
	}

}
