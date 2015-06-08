using UnityEngine;
using System.Collections;

public class Character2DController : MonoBehaviour {

	[SerializeField] float speed = 1.0f;
	[SerializeField] float jumpForce = 5.0f;
	[SerializeField] Transform colliderOrigin;
	[SerializeField] Transform colliderTarget;
	[SerializeField] LayerMask layerMask;

	Rigidbody2D body2d;
	Vector2 vectorisedJumpForce;
	float direction = 1;

	void Awake()
	{
		//layerMask = LayerMask.NameToLayer("Ground");
		body2d = GetComponent<Rigidbody2D>();

		vectorisedJumpForce = new Vector2(0, jumpForce);
	}

	void MoveForward()
	{
		body2d.velocity = new Vector2(speed * direction, body2d.velocity.y);
	}

	void Jump()
	{
		body2d.AddForce(vectorisedJumpForce);
	}

	void FlipDirection()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1f;
		localScale.z *= -1f;
		direction *= -1f;
		transform.localScale = localScale;
	}

	void LateUpdate()
	{
		RaycastHit2D hit = Physics2D.Raycast(colliderOrigin.position, (colliderTarget.position - colliderOrigin.position).normalized, Vector2.Distance(colliderTarget.position, colliderOrigin.position), layerMask);
		if (hit.collider != null && !hit.collider.isTrigger)
		{
			FlipDirection();
		}
	}

	void FixedUpdate()
	{
		MoveForward();
	}
}
