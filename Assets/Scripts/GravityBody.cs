using UnityEngine;
using System.Collections;

[RequireComponent ( typeof (Rigidbody))]
public class GravityBody : MonoBehaviour 
{
	 GravityAttractor planet;

	void Awake()
	{
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

	}

	void FixedUpdate()
	{
		planet.Attract(transform);
	}
}
