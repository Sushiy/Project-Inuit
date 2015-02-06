using UnityEngine;
using System.Collections;

public class MeshDistort : MonoBehaviour 
{
	public float deformRadius = 1f;
	public float maxDeform = 0.2f;

	Mesh mesh;

	Vector3 latestCollision;
	bool isColliding;

	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.MarkDynamic();
	}

	void Update() 
	{
		if(isColliding)
		{	
			Vector3[] vertices = mesh.vertices;
			int i = 0;
			while (i < vertices.Length) 
			{
				Vector3 currentPoint = transform.TransformPoint(vertices[i]);
				float distance = Vector3.Distance(currentPoint, latestCollision);
				if(distance <= deformRadius)
				{
					float deform = Mathf.Min(maxDeform, maxDeform * (distance/deformRadius) * (distance/deformRadius));
					deform *= Random.Range(0.66f, 1.33f);
					vertices[i] += new Vector3(0f, 0f, -deform);
				}
				i++;
			}

			mesh.vertices = vertices;
			
			mesh.RecalculateNormals();
		}

	}

	void OnCollisionStay(Collision collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{   
			Vector3 averageContact = Vector3.zero;
			foreach (ContactPoint contact in collision.contacts) 
			{
				Debug.DrawRay(contact.point, contact.normal, Color.red);
				averageContact += contact.point;
			}

			latestCollision = averageContact/collision.contacts.Length;

			isColliding = true;
			collision.gameObject.GetComponent<PlayerController>().setInSnow(true);
		}

	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.GetComponent<PlayerController>().setInSnow(false);
		}
		Debug.Log ("collision over with " + collision.gameObject.name);
		isColliding = false;
	}
}