using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Sphere_PlayerController : MonoBehaviour 
{
	public Text textArea;

	public float walkSpeed;
	public float snowSpeed;
	public float runMultiplicator;
	
	public float rotationSpeed;
	
	
	public GameObject target;
	public int targetIndex;
	public GameObject targetCandidate;
	
	public float targetAngle = 100f;
	ArrayList targetList;
	
	bool switched;
	
	
	float speed, angle, direction = 0f, directionSpeed, horizontal, vertical;
	
	float currentSpeed;
	
	//public Transform playerHead;
	//Vector3 cameraOffset;
	Transform cameraT;
	//float verticalLookRotation;
	
	Vector3 moveAmount;
	Vector3 moveDir;
	Vector3 smoothMoveVelocity;
	
	bool targetMode;
	bool inSnow;
	Animator anim;
	bool combatMode;
	
	// Use this for initialization
	void Start () 
	{
		
		cameraT = Camera.main.transform;
		//cameraOffset = this.transform.position - cameraT.position;
		anim = GetComponent<Animator>();
		targetList = new ArrayList();
		if(anim == null)
		{
			Debug.Log("no animator");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{	
		
		/*adjust Walkspeed*/
		if(inSnow)
		{
			currentSpeed = snowSpeed; 
		}
		else
		{
			currentSpeed = walkSpeed;
		}
		/*adjust Walkspeed*/
		
		
		/****TARGETING******/
		if(Input.GetAxis("Target") >= 0.3f)
		{
			if(targetCandidate && !targetMode)
			{
				Vector3 targetDir = targetCandidate.transform.position - transform.position;
				targetDir.y = 0f;
				if(Vector3.Angle(transform.forward, targetDir) <= targetAngle)
				{
					Debug.Log("activate Target at Angle: " + Vector3.Angle(transform.forward, targetDir));
					target = targetCandidate;
					targetIndex = targetList.IndexOf (target);
				}
			}
			
			targetMode = true;
		}
		else if(Input.GetAxis("Target") <= 0.3f)
		{
			target = null;
			targetMode = false;
		}
		
		//Switch target
		if(targetMode && Input.GetAxisRaw("Camera") != 0f)
		{
			Debug.Log("Camera");
			if(!switched)
				SwitchTarget(Input.GetAxis("Camera") > 0f);
			
			switched = true;
		}
		else
		{
			switched = false;
		}
		//SEARCH TARGET
		if(target)
		{
			Debug.DrawLine(transform.position, target.transform.position, Color.yellow);
		}
		else
		{
			Debug.Log("search new Target");
			SelectTarget();
		}
		//SEARCH TARGET
		/****TARGETING******/
		
		
		/****RUNNING*******/
		if(Input.GetButton("Run") || !targetMode)
		{
			currentSpeed *= runMultiplicator;
		}
		/****RUNNING*******/
		
		
		
		/*****COMBAT********/
		if(Input.GetButtonUp("Attack"))
		{
			if(anim.GetCurrentAnimatorStateInfo(1).IsName("Combat.Stab"))
			{
				anim.SetTrigger("Hit");
			}
			else
			{
				anim.SetTrigger("Stab");
			}
		}
		/*****COMBAT********/
		
		
		/*****MOVEMENT****************/
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
		
		//Combat movement (relative to character not turning)
		if(targetMode)
		{
			
			moveDir = horizontal * transform.right + vertical * transform.forward; 
		}
		//Normal movement relative to camera, turning
		else
		{
			/*movement calculation*/
			StickToWorldspace(this.transform, cameraT, ref angle, ref speed);
			this.transform.Rotate(Vector3.up, angle);
		}
		
		Vector3 targetMoveAmount = moveDir * currentSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
		
		
		speed = new Vector2(horizontal, vertical).sqrMagnitude;
		
		direction = Mathf.Abs(horizontal) >= 0.8? horizontal: direction;
		/*****MOVEMENT****************/
		
		textArea.text = null;
		if(targetMode)
		{
			textArea.text = Input.GetAxis("Camera").ToString();
		}
		
		//anim.SetBool ("hasSpear", true);
		anim.SetFloat("Direction", direction);
		anim.SetFloat ("Speed", speed);
		anim.SetBool ("inSnow", inSnow);
		anim.SetBool("targetMode", targetMode);
	}
	
	void LateUpdate()
	{
		if(targetMode && target)
		{
			smoothLookAt();
		}
	}
	
	void FixedUpdate()
	{
		
		/*Movement*/
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + moveAmount * Time.fixedDeltaTime);
		
	}
	
	public void StickToWorldspace(Transform root, Transform camera, ref float angleOut, ref float speedOut)
	{
		Vector3 rootDirection = root.forward;
		
		Vector3 stickDirection = new Vector3(horizontal, 0, vertical);
		
		speedOut = stickDirection.sqrMagnitude;
		if(speedOut > 0.2)
		{
			//get camera rotation
			Vector3 CameraDirection = cameraT.forward;
			CameraDirection.y = 0.0f;
			Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, CameraDirection);
			
			//Convert joystick input in Worldspace coordinates
			Vector3 moveDirection = referentialShift * stickDirection;
			Vector3 axisSign = Vector3.Cross (moveDirection, rootDirection);
			
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection*2, Color.green);
			
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign*2, Color.red);
			
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection*2, Color.magenta);
			
			Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection*2, Color.blue);
			
			float angleRootToMove = Vector3.Angle (rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f: 1f);
			angleRootToMove /= 180f;
			
			moveDir = moveDirection;
			angleOut = angleRootToMove * rotationSpeed;
		}
		else
		{
			angleOut = 0;
			moveDir = Vector3.zero;
		}
		
	}
	
	public void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.CompareTag ("Target"))
		{
			Debug.Log("target enter: " + c.name);
			targetList.Add(c.gameObject);
		}
	}
	
	public void OnTriggerExit(Collider c)
	{
		if(c.gameObject.CompareTag("Target"))
		{
			Debug.Log("target exit: " + c.name);
			targetList.Remove(c.gameObject);
			if(target == c.gameObject)
			{
				target = null;
				targetCandidate = null;
			}
		}
	}
	
	public void SelectTarget()
	{
		if(targetList.Count > 1 || !target)
		{
			/*Ray viewRay = new Ray(transform.position, transform.forward);
			Debug.DrawRay(transform.position, transform.forward, Color.black, 10f);
			
			float minDistance = Mathf.Infinity;
			foreach(GameObject g in targetList)
			{
				/*	Vector3 point = g.transform.position;
				point.y = 0;
				float distance = Vector3.Cross(viewRay.direction, point - viewRay.origin).magnitude;
				minDistance = distance < minDistance? distance: minDistance;

			}*/
			
			/********SELECT NEAREST TARGET***/
			float nearestMagnSqr = Mathf.Infinity;
			
			foreach(GameObject g in targetList)
			{
				Vector3 targetDir = g.transform.position - transform.position;
				if(Vector3.Angle(transform.forward, targetDir) <= targetAngle)
				{
					if(targetDir.sqrMagnitude < nearestMagnSqr)
					{
						nearestMagnSqr = targetDir.sqrMagnitude;
						targetCandidate = g;
					}
				}
			}
		}
	}
	
	public void SwitchTarget(bool previous)
	{
		if(targetList.Count > 1)
		{
			int i;
			if(previous)
			{
				i = targetIndex-1;
				i = i >= 0?i:targetList.Count-1;
			}
			else
			{
				i = (targetIndex+1)%targetList.Count;
				
			}
			target = (GameObject)targetList[i];
			targetIndex = i;
		}
	}
	
	public void setInSnow(bool b)
	{
		inSnow = b;
	}
	
	public void smoothLookAt()
	{
		Vector3 lookPos = target.transform.position - transform.position;
		lookPos.y = 0;
		var rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3f);
	}
}
