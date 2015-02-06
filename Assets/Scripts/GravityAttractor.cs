using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour 
{
	public float gravity = -10f;

	public void Attract(Transform body)
	{
		Vector3 targetDir = (body.position - transform.position).normalized;
		Vector3 bodyUp = body.up;

		body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;

		body.rigidbody.AddForce(targetDir * gravity);

	}

}
