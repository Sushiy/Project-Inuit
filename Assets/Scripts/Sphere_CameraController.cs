using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sphere_CameraController : MonoBehaviour 
{
	//debug:
	
	public Text textArea;
	
	public GameObject player;
	
	Transform playerT;
	
	float distanceAway = 25f;
	float distanceUp = 15f;
	float smooth;
	
	Vector3 offset = new Vector3(0, 5.5f, 0f);
	Vector3 lookDir;
	Vector3 targetPosition;
	
	Vector3 velocityCanSmooth = Vector3.zero;
	
	float dampTime;
	float baseDampTime = 0.3f;
	float targetDampTime = 0.3f;
	
	
	// Use this for initialization
	void Start () 
	{
		playerT = player.transform;
	
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		dampTime = baseDampTime;

		Vector3 characterOffset = playerT.position + playerT.TransformDirection(offset);
		/*
		lookDir = characterOffset - this.transform.position;
		lookDir.y = 0f;
		lookDir.Normalize();*/

		lookDir = playerT.forward;
		targetPosition = characterOffset + playerT.up * distanceUp - lookDir * distanceAway;

		smoothPosition(transform.position, targetPosition);
		
		//make sure the camera is looking the right way
		transform.LookAt(characterOffset, playerT.up);

	}
	
	private void smoothPosition(Vector3 fromPos, Vector3 toPos)
	{
		// making a smooth transition between cameras current position and the position it wants to be in
		this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCanSmooth, dampTime);
	}
}
