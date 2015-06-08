using UnityEngine;
using System.Collections;

public class FollowCharacter : MonoBehaviour {

	public Transform followedCharacter;

	Vector3 delta;

	// Use this for initialization
	void Start () {
		delta = transform.position - followedCharacter.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPos = followedCharacter.position;
		newPos += delta;
		transform.position = newPos;

	}
}
