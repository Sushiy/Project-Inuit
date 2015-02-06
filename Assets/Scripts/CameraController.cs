using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	//debug:

	public Text textArea;

	public GameObject player;

	Transform playerT;

	float distanceAway = 25f;
	float distanceUp = 7f;
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

		//is the targetmode button down?
		if(Input.GetAxis("Target") >= 0.5f)
		{
			textArea.text = "target";
			lookDir = playerT.forward.normalized;
			dampTime = targetDampTime;
		}
		else
		{
			//calculate the direction from camera to player, kill y and normalize
			lookDir = characterOffset - this.transform.position;
			lookDir.y = 0f;
			lookDir.Normalize();
			Debug.DrawRay(this.transform.position, lookDir, Color.green);
		}
		//setting the target position to be the correct offset
		targetPosition = characterOffset + playerT.up * distanceUp - lookDir * distanceAway;

		smoothPosition(transform.position, targetPosition);

		//make sure the camera is looking the right way
	    transform.LookAt(characterOffset);
	}

	private void smoothPosition(Vector3 fromPos, Vector3 toPos)
	{
		// making a smooth transition between cameras current position and the position it wants to be in
		this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCanSmooth, dampTime);
	}
}
