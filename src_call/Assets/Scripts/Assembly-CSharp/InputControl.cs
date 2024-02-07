using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InputControl : MonoBehaviour
{
	private FPSPlayer FPSPlayerComponent;

	[HideInInspector]
	public bool fireHold;

	[HideInInspector]
	public bool firePress;

	[HideInInspector]
	public bool reloadPress;

	[HideInInspector]
	public bool fireModePress;

	[HideInInspector]
	public bool jumpHold;

	[HideInInspector]
	public bool jumpPress;

	[HideInInspector]
	public bool crouchHold;

	[HideInInspector]
	public bool proneHold;

	[HideInInspector]
	public bool sprintHold;

	[HideInInspector]
	public bool zoomHold;

	[HideInInspector]
	public bool zoomPress;

	[HideInInspector]
	public bool leanLeftHold;

	[HideInInspector]
	public bool leanRightHold;

	[HideInInspector]
	public bool useHold;

	[HideInInspector]
	public bool usePress;

	[HideInInspector]
	public bool usePressUp;

	[HideInInspector]
	public bool toggleCameraHold;

	[HideInInspector]
	public bool toggleCameraDown;

	[HideInInspector]
	public bool grenadeHold;

	[HideInInspector]
	public bool deadzonePress;

	[HideInInspector]
	public bool meleePress;

	[HideInInspector]
	public bool flashlightPress;

	[HideInInspector]
	public bool holsterPress;

	[HideInInspector]
	public bool dropPress;

	[HideInInspector]
	public bool bulletTimePress;

	[HideInInspector]
	public bool moveHold;

	[HideInInspector]
	public bool movePress;

	[HideInInspector]
	public bool throwHold;

	[HideInInspector]
	public bool helpPress;

	[HideInInspector]
	public bool menuPress;

	[HideInInspector]
	public bool pausePress;

	[HideInInspector]
	public bool selectNextPress;

	[HideInInspector]
	public bool selectPrevPress;

	public bool selectGrenPress;

	[HideInInspector]
	public bool selectWeap1Press;

	[HideInInspector]
	public bool selectWeap2Press;

	[HideInInspector]
	public bool selectWeap3Press;

	[HideInInspector]
	public bool selectWeap4Press;

	[HideInInspector]
	public bool selectWeap5Press;

	[HideInInspector]
	public bool selectWeap6Press;

	[HideInInspector]
	public bool selectWeap7Press;

	[HideInInspector]
	public bool selectWeap8Press;

	[HideInInspector]
	public bool selectWeap9Press;

	[HideInInspector]
	public bool selectWeap0Press;

	[HideInInspector]
	public float mouseWheel;

	[HideInInspector]
	public bool leftHold;

	[HideInInspector]
	public bool rightHold;

	[HideInInspector]
	public bool forwardHold;

	[HideInInspector]
	public bool backHold;

	[HideInInspector]
	public float moveXButton;

	[HideInInspector]
	public float moveYButton;

	[HideInInspector]
	public float deadzone = 0.25f;

	private Vector2 moveInput;

	private Vector2 lookInput;

	[HideInInspector]
	public float moveX;

	[HideInInspector]
	public float moveY;

	[HideInInspector]
	public float lookX;

	[HideInInspector]
	public float lookY;

	[HideInInspector]
	public bool xboxDpadLeftHold;

	[HideInInspector]
	public bool xboxDpadRightHold;

	[HideInInspector]
	public bool xboxDpadUpHold;

	[HideInInspector]
	public bool xboxDpadDownHold;

	[HideInInspector]
	public bool xboxDpadLeftPress;

	[HideInInspector]
	public bool xboxDpadRightPress;

	[HideInInspector]
	public bool xboxDpadUpPress;

	[HideInInspector]
	public bool xboxDpadDownPress;

	private bool xbdpLstate;

	private bool xbdpRstate;

	private bool xbdpUstate;

	private bool xbdpDstate;

	public NavMeshAgent navMeshAgent;

	private FPSRigidBodyWalker ref_FPSRigidBody;

	public SmoothMouseLook smoothMouseLook;

	private Transform DestPoint;

	private Vector2 myLook;

	private Transform tempTransform;

	public bool changePoint;

	public GameObject SwitchButton;

	public GameObject ReloadButton;

	public GameObject GrenadeButton;

	public GameObject ZoomButton;

	public GameObject BulletField;

	public Text BulletInfo;

	public Image BulletFiller;

	public bool AutoShoot;

	public Toggle TogglerAutoShoot;

	public GameObject ShootButton;

	public GameObject PickButton;

	public Text PickUpName;

	public Text GrenadeTxt;

	private bool CanShoot;

	private void Start()
	{
		FPSPlayerComponent = GetComponent<FPSPlayer>();
		navMeshAgent = GetComponent<NavMeshAgent>();
		ref_FPSRigidBody = GetComponent<FPSRigidBodyWalker>();
	}

	public void setPointer(Vector2 value)
	{
		myLook = value;
	}

	public void MoveToNextPos(Transform point)
	{
		if (!point.Equals(tempTransform))
		{
			tempTransform = point;
			navMeshAgent.enabled = true;
			if (zoomHold)
			{
				zoomHold = false;
			}
			DestPoint = point;
			navMeshAgent.SetDestination(DestPoint.position);
			changePoint = true;
		}
	}

	public void CharacterMove()
	{
		if (!changePoint || navMeshAgent.remainingDistance <= 0f)
		{
			return;
		}
		if (navMeshAgent.remainingDistance >= 1f)
		{
			zoomHold = false;
			reloadPress = false;
			ref_FPSRigidBody.sprintActive = true;
			sprintHold = true;
			Vector3 desiredVelocity = navMeshAgent.desiredVelocity;
			desiredVelocity = base.transform.InverseTransformDirection(desiredVelocity);
			moveX = desiredVelocity.x;
			moveY = desiredVelocity.z;
			smoothMouseLook.allowAim(true);
			smoothMouseLook.setMouseData(new Vector2(10f * desiredVelocity.x, 10f * desiredVelocity.y));
		}
		else
		{
			ref_FPSRigidBody.sprintActive = false;
			sprintHold = false;
			moveX = 0f;
			moveY = 0f;
			navMeshAgent.isStopped = true;
			navMeshAgent.enabled = false;
			changePoint = false;
			if ((bool)LevelController.instance)
			{
				LevelController.instance.AlertEnemies();
			}
		}
	}

	private void Update()
	{
		if (leftHold && !rightHold)
		{
			moveXButton = -1f;
		}
		else if (rightHold && !leftHold)
		{
			moveXButton = 1f;
		}
		else
		{
			moveXButton = 0f;
		}
		if (forwardHold && !backHold)
		{
			moveYButton = 1f;
		}
		else if (backHold && !forwardHold)
		{
			moveYButton = -1f;
		}
		else
		{
			moveYButton = 0f;
		}
		moveInput = new Vector2(Input.GetAxis("Joystick Move X"), Input.GetAxis("Joystick Move Y"));
		if (moveInput.magnitude < deadzone)
		{
			moveInput = Vector2.zero;
		}
		else
		{
			moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1f - deadzone));
		}
		lookInput = new Vector2(Input.GetAxis("Joystick Look X"), Input.GetAxis("Joystick Look Y"));
		if (lookInput.magnitude < deadzone)
		{
			lookInput = Vector2.zero;
		}
		else
		{
			lookInput = lookInput.normalized * ((lookInput.magnitude - deadzone) / (1f - deadzone));
		}
		moveX = moveXButton + moveInput.x;
		moveY = moveYButton + moveInput.y;
		if ((bool)FPSPlayerComponent && !FPSPlayerComponent.restarting)
		{
			CharacterMove();
		}
		else
		{
			fireHold = false;
		}
	}

	public void StopFire()
	{
		if (AutoShoot)
		{
			Globals.canShoot = false;
		}
		else
		{
			CanShoot = false;
		}
	}

	public void setBulletInfo(int totalInClip, int remainingInMag)
	{
		BulletInfo.text = remainingInMag + "/" + totalInClip;
		BulletFiller.fillAmount = (float)remainingInMag * 1f / (float)totalInClip;
	}

	public void WeaponOperateState(bool value)
	{
		AutoShoot = value;
		ShootButton.SetActive(!AutoShoot);
	}

	public void ShowPickUpButton(bool value, string data)
	{
		PickButton.SetActive(value);
		PickUpName.text = data;
	}

	public void SwitchWeapon(bool value)
	{
		selectNextPress = value;
	}

	public void SelectGrenade(bool value)
	{
		grenadeHold = value;
	}

	public void Zoom(bool value)
	{
		zoomHold = value;
	}

	public void Reload(bool value)
	{
		reloadPress = value;
	}

	public void PickWeapon(bool value)
	{
		useHold = (usePress = value);
	}

	private float AccelerateInput(float input)
	{
		return 0.25f * input * (Mathf.Abs(input) * 4f) * Time.smoothDeltaTime * 60f;
	}

	public void Shoot(bool value)
	{
		CanShoot = value;
	}

	public void Crouch(bool value)
	{
		crouchHold = value;
	}
}
