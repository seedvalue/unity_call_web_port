using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[HideInInspector]
	public GameObject gun;

	public GameObject playerObj;

	[HideInInspector]
	public GameObject weaponObj;

	private Transform myTransform;

	[HideInInspector]
	public FPSRigidBodyWalker FPSWalkerComponent;

	private Ironsights IronsightsComponent;

	[HideInInspector]
	public SmoothMouseLook MouseLookComponent;

	private WorldRecenter WorldRecenterComponent;

	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;

	private InputControl InputComponent;

	[HideInInspector]
	public GunSway GunSwayComponent;

	[HideInInspector]
	public CamAndWeapAnims CamAndWeapAnimsComponent;

	private Animation AnimationComponent;

	private Animator AnimatorComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private WeaponBehavior CurrentWeaponBehaviorComponent;

	[HideInInspector]
	public GameObject PlayerCharacterObj;

	[HideInInspector]
	public PlayerCharacter PlayerCharacterComponent;

	[HideInInspector]
	public bool constantLooking;

	[HideInInspector]
	public Vector3 CameraAnglesAnim = Vector3.zero;

	[HideInInspector]
	public Vector3 bobAngles = Vector3.zero;

	private Quaternion tempCamAngles;

	private float deltaAmt;

	private float returnSpeedAmt = 4f;

	private bool landState;

	private float landStartTime;

	private float landTime = 0.35f;

	private float landAmt = 1f;

	private float landValue;

	private float gunDown;

	[HideInInspector]
	public float dampOriginX;

	[HideInInspector]
	public float dampOriginY;

	[Tooltip("True if third person mode is allowed.")]
	public bool allowThirdPerson = true;

	[Tooltip("True if first person mode is allowed.")]
	public bool allowFirstPerson = true;

	[Tooltip("True third person mode is active, otherwise, first person mode is active.")]
	public bool thirdPersonActive;

	private Vector3 dampVel;

	[Tooltip("Speed to smooth the camera angles in third person mode.")]
	public float camSmoothSpeed = 0.075f;

	[HideInInspector]
	public float lerpSpeedAmt;

	private Transform playerObjTransform;

	[HideInInspector]
	public Transform mainCameraTransform;

	[HideInInspector]
	public float movingTime;

	[HideInInspector]
	public Vector3 targetPos = Vector3.one;

	private Vector3 targetPos2;

	[HideInInspector]
	public Vector3 camPos;

	[HideInInspector]
	public Vector3 tempLerpPos = Vector3.one;

	private float rollAmt;

	private float lookRollAmt;

	[Tooltip("Horizontal input speed for rotating the camera in third person mode.")]
	public float horizontalRotateSpeed = 5f;

	[Tooltip("Vertical input speed for rotating the camera in third person mode.")]
	public float verticalRotateSpeed = 5f;

	[Tooltip("Amount to offset the camera from the player in third person mode (horizontal, vertical).")]
	public Vector2 offset;

	private Vector2 offsetAmt;

	[Tooltip("Minumum allowed distance to zoom the camera in third person mode.")]
	public float minZoom = 1.45f;

	[Tooltip("Maximum allowed distance to zoom the camera in third person mode.")]
	public float maxZoom = 10f;

	private float vertical;

	private float horizontal;

	[Tooltip("Minumum allowed vertical camera angle in third person mode.")]
	public float verticalMin = -25f;

	[Tooltip("Maximum allowed vertical camera angle in third person mode.")]
	public float verticalMax = 85f;

	private bool angleState;

	private bool tpState;

	[HideInInspector]
	public float tpPressTime;

	private bool tpPressState = true;

	[HideInInspector]
	public bool rotating;

	private bool idleRotating;

	private bool rotated;

	[HideInInspector]
	public float zoomDistance;

	private float previousZoomDist;

	private bool zoomState;

	[Tooltip("Radius of sphere collider for detecting collisions with camera in third person mode.")]
	public float sphereSizeTpCol = 0.3f;

	[Tooltip("Distance behind camera to check for obstacles in third person mode (for fine tuning to preventing clipping into scene geometry).")]
	public float rayTestPadding = -0.5f;

	private float smoothedDistance;

	private float targetDistance;

	[HideInInspector]
	public float currentDistance;

	private RaycastHit hit;

	private Vector3 direction;

	private Quaternion camRotation;

	private bool tpSwitching;

	private bool tpScopeState;

	[Tooltip("Height to zoom to and from in transition to third person mode.")]
	public float fpZoomHeadHeight = 1.27f;

	[Tooltip("Distance to zoom to and from in transition to third person mode.")]
	public float tpModeSwitchZoomDist = 0.65f;

	private float distanceVelocity;

	private float dampVelocity;

	private float dampOrg;

	private float stancePos;

	private float stancePosAmt;

	[HideInInspector]
	public bool viewUnderwater;

	[Tooltip("Layers that the camera will collide with in third person mode.")]
	public LayerMask clipMask;

	private void Start()
	{
		myTransform = base.transform;
		playerObjTransform = playerObj.transform;
		mainCameraTransform = Camera.main.transform;
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		weaponObj = FPSPlayerComponent.weaponObj;
		MouseLookComponent = base.transform.parent.transform.GetComponent<SmoothMouseLook>();
		GunSwayComponent = weaponObj.GetComponent<GunSway>();
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		IronsightsComponent = playerObj.GetComponent<Ironsights>();
		CamAndWeapAnimsComponent = base.transform.parent.transform.GetComponent<CamAndWeapAnims>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		InputComponent = playerObj.GetComponent<InputControl>();
		if ((bool)playerObj.GetComponent<WorldRecenter>())
		{
			WorldRecenterComponent = playerObj.GetComponent<WorldRecenter>();
		}
		AnimatorComponent = GetComponent<Animator>();
		offsetAmt = offset;
		currentDistance = 0f;
		zoomDistance = 0f;
	}

	private void Update()
	{
		if (Time.timeScale > 0f && Time.deltaTime > 0f && Time.smoothDeltaTime > 0f && Time.time > 0f)
		{
			CurrentWeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
			if (allowThirdPerson)
			{
				if (InputComponent.toggleCameraDown)
				{
					tpPressTime = Time.time;
					tpPressState = false;
					if (!thirdPersonActive)
					{
						thirdPersonActive = true;
						tpPressState = true;
					}
				}
				if (!InputComponent.toggleCameraHold)
				{
					if (tpPressTime + Time.deltaTime * 30f > Time.time && !tpPressState)
					{
						if (!thirdPersonActive)
						{
							thirdPersonActive = true;
						}
						else if (allowFirstPerson && !tpSwitching)
						{
							tpSwitching = true;
						}
					}
					rotating = false;
					if (idleRotating)
					{
						rotated = false;
					}
					tpPressState = true;
				}
				else if (tpPressTime + 0.2f < Time.time && !FPSPlayerComponent.zoomed)
				{
					rotating = true;
				}
			}
			if (CurrentWeaponBehaviorComponent.fpZoomForTp)
			{
				if (FPSPlayerComponent.zoomed && thirdPersonActive && PlayerWeaponsComponent.switchTime + CurrentWeaponBehaviorComponent.readyTimeAmt < Time.time)
				{
					tpScopeState = true;
					thirdPersonActive = false;
				}
				if (tpScopeState && !FPSPlayerComponent.zoomed)
				{
					thirdPersonActive = true;
					tpScopeState = false;
				}
			}
			else
			{
				tpScopeState = false;
			}
			if (AnimatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
			{
				CameraAnglesAnim = Vector3.zero;
			}
			if (!thirdPersonActive)
			{
				targetPos = playerObjTransform.position + playerObjTransform.right * FPSWalkerComponent.leanPos;
			}
			else
			{
				targetPos = playerObjTransform.position;
			}
			if ((bool)WorldRecenterComponent && WorldRecenterComponent.worldRecenterTime + 0.1f * Time.timeScale > Time.time)
			{
				tempLerpPos = playerObjTransform.position;
			}
			else if (movingTime + 0.75f > Time.time)
			{
				lerpSpeedAmt = 0f;
			}
			else if ((FPSPlayerComponent.removePrefabRoot && playerObj.transform.parent == null) || (!FPSPlayerComponent.removePrefabRoot && playerObj.transform.parent == FPSWalkerComponent.mainObj.transform))
			{
				lerpSpeedAmt = Mathf.MoveTowards(lerpSpeedAmt, camSmoothSpeed, Time.deltaTime);
			}
			else
			{
				lerpSpeedAmt = Mathf.MoveTowards(lerpSpeedAmt, 0.01f, Time.deltaTime);
			}
			tempLerpPos = Vector3.SmoothDamp(tempLerpPos, targetPos, ref dampVel, lerpSpeedAmt, float.PositiveInfinity, Time.smoothDeltaTime);
			dampOrg = Mathf.SmoothDamp(dampOrg, FPSWalkerComponent.midPos, ref dampVelocity, FPSWalkerComponent.camDampSpeed, float.PositiveInfinity, Time.smoothDeltaTime);
			if (FPSWalkerComponent.crouched && FPSWalkerComponent.moving)
			{
				stancePos = offsetAmt.y * 0.5f;
			}
			else if (FPSWalkerComponent.prone)
			{
				stancePos = offsetAmt.y * -0.7f;
			}
			else if (FPSWalkerComponent.swimming && FPSPlayerComponent.zoomed)
			{
				stancePos = offsetAmt.y * -0.3f;
			}
			else
			{
				stancePos = 0f;
			}
			stancePosAmt = Mathf.Lerp(stancePosAmt, stancePos, Time.deltaTime * 4f);
			if (!thirdPersonActive)
			{
				camPos = tempLerpPos + playerObjTransform.right * (CamAndWeapAnimsComponent.camPosAnim.x * IronsightsComponent.camPositionBobAmt.x) + new Vector3(0f, dampOrg + CamAndWeapAnimsComponent.camPosAnim.y * IronsightsComponent.camPositionBobAmt.y, 0f);
			}
			else
			{
				camPos = tempLerpPos + playerObjTransform.up * stancePosAmt;
			}
			if (thirdPersonActive)
			{
				if (!tpState)
				{
					MouseLookComponent.playerMovedTime = Time.time;
					MouseLookComponent.rotationX = mainCameraTransform.parent.transform.eulerAngles.y;
					camRotation = mainCameraTransform.parent.transform.rotation;
					currentDistance = 0f;
					if (FPSWalkerComponent.sprintActive)
					{
						smoothedDistance = tpModeSwitchZoomDist - 0.35f;
					}
					else
					{
						smoothedDistance = tpModeSwitchZoomDist - 0.2f;
					}
					zoomDistance = maxZoom * 0.65f;
					GunSwayComponent.enabled = false;
					tpState = true;
				}
				weaponObj.transform.rotation = mainCameraTransform.rotation;
				MouseLookComponent.thirdPerson = true;
				horizontal = MouseLookComponent.rotationX;
				vertical = MouseLookComponent.rotationY + MouseLookComponent.recoilY;
				vertical = ClampAngle(vertical, verticalMin, verticalMax);
				horizontal = ClampAngle(horizontal, -360f, 360f);
				smoothedDistance = Mathf.Lerp(smoothedDistance, zoomDistance, Time.smoothDeltaTime * 9f);
				targetPos2 = camPos + playerObjTransform.up * offsetAmt.y * 1.25f;
				camRotation = Quaternion.Slerp(camRotation, Quaternion.Euler(0f - vertical, horizontal, 0f), 0.35f * Time.smoothDeltaTime * 60f / Time.timeScale);
				if (smoothedDistance != 0f)
				{
					direction = camRotation * (-Vector3.forward - Vector3.right * (offsetAmt.x / smoothedDistance));
				}
				if (!tpSwitching)
				{
					targetDistance = AdjustLineOfSight(targetPos2, direction);
					currentDistance = Mathf.Min(targetDistance + 0.5f, Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, 0.15f));
					offsetAmt.x = Mathf.Lerp(offsetAmt.x, offset.x, Time.smoothDeltaTime * 6f);
					offsetAmt.y = Mathf.Lerp(offsetAmt.y, offset.y, Time.smoothDeltaTime * 6f);
					if (!FPSPlayerComponent.zoomed)
					{
						if (zoomState)
						{
							zoomDistance = previousZoomDist;
							zoomState = false;
						}
						if (rotating)
						{
							zoomDistance = Mathf.Clamp(zoomDistance - Input.GetAxis("Mouse Scroll Wheel") * 5f, minZoom, maxZoom);
						}
					}
					else
					{
						if (!zoomState)
						{
							previousZoomDist = zoomDistance;
							zoomState = true;
						}
						if (PlayerWeaponsComponent.switchTime + CurrentWeaponBehaviorComponent.readyTimeAmt < Time.time && CurrentWeaponBehaviorComponent.meleeSwingDelay == 0f)
						{
							zoomDistance = minZoom;
						}
					}
				}
				else
				{
					offsetAmt.x = Mathf.Lerp(offsetAmt.x, 0f, Time.smoothDeltaTime * 6f);
					offsetAmt.y = Mathf.Lerp(offsetAmt.y, fpZoomHeadHeight, Time.smoothDeltaTime * 6f);
					zoomDistance = tpModeSwitchZoomDist;
					currentDistance = Mathf.SmoothDamp(currentDistance, 0f, ref distanceVelocity, 0.075f);
					if (smoothedDistance < tpModeSwitchZoomDist + 0.2f)
					{
						smoothedDistance = 0f;
						currentDistance = 0f;
						targetDistance = 0f;
						zoomDistance = 0f;
						if ((bool)PlayerCharacterComponent)
						{
							PlayerCharacterComponent.tpswitchTime = Time.time;
						}
						targetPos2 = camPos + playerObjTransform.up * offsetAmt.y * (FPSWalkerComponent.capsule.height * 0.5f);
						thirdPersonActive = false;
						tpSwitching = false;
					}
				}
				if (!rotating && (InputComponent.moveX != 0f || InputComponent.moveY != 0f || InputComponent.fireHold || InputComponent.grenadeHold || InputComponent.meleePress || CurrentWeaponBehaviorComponent.releaseTimer + 0.1f > Time.time || PlayerWeaponsComponent.offhandThrowActive || constantLooking || FPSPlayerComponent.zoomed))
				{
					if (!angleState)
					{
						if (rotated)
						{
							MouseLookComponent.rotationX = mainCameraTransform.parent.transform.eulerAngles.y;
							rotated = false;
						}
						mainCameraTransform.position = mainCameraTransform.parent.transform.position;
						mainCameraTransform.rotation = mainCameraTransform.parent.transform.rotation;
						angleState = true;
					}
					idleRotating = false;
					MouseLookComponent.tpIdleCamRotate = false;
					mainCameraTransform.parent.transform.rotation = camRotation;
					mainCameraTransform.parent.transform.position = camPos + direction * (currentDistance + smoothedDistance) + playerObjTransform.up * offsetAmt.y;
				}
				else
				{
					if (angleState)
					{
						mainCameraTransform.parent.transform.rotation = mainCameraTransform.rotation;
						angleState = false;
					}
					if (rotating && (InputComponent.moveX != 0f || InputComponent.moveY != 0f))
					{
						rotated = true;
						idleRotating = false;
					}
					else
					{
						idleRotating = true;
					}
					MouseLookComponent.tpIdleCamRotate = true;
					mainCameraTransform.rotation = camRotation;
					mainCameraTransform.position = camPos + direction * (currentDistance + smoothedDistance) + playerObjTransform.up * offsetAmt.y;
				}
				if (!FPSWalkerComponent.moving)
				{
					IronsightsComponent.side = 0f;
					IronsightsComponent.raise = 0f;
				}
			}
			else
			{
				if (tpState)
				{
					MouseLookComponent.playerMovedTime = Time.time;
					MouseLookComponent.enabled = false;
					mainCameraTransform.parent.transform.rotation = Quaternion.Euler(0f - vertical, horizontal, 0f);
					mainCameraTransform.rotation = mainCameraTransform.parent.transform.rotation;
					MouseLookComponent.enabled = true;
					MouseLookComponent.rotationX = horizontal;
					MouseLookComponent.rotationY = vertical;
					MouseLookComponent.horizontalDelta = 0f;
					MouseLookComponent.recoilY = 0f;
					MouseLookComponent.recoilX = 0f;
					MouseLookComponent.xQuaternion = Quaternion.Euler(0f, 0f, 0f);
					MouseLookComponent.yQuaternion = Quaternion.Euler(0f, 0f, 0f);
					MouseLookComponent.originalRotation = Quaternion.Euler(0f, 0f, 0f);
					GunSwayComponent.enabled = true;
					GunSwayComponent.localRaise = 0f;
					GunSwayComponent.localSide = 0f;
					tpState = false;
				}
				mainCameraTransform.parent.transform.position = camPos;
				mainCameraTransform.position = camPos;
				MouseLookComponent.thirdPerson = false;
				if (Time.timeSinceLevelLoad < 1f)
				{
					returnSpeedAmt = 32f;
				}
				else if (!viewUnderwater)
				{
					returnSpeedAmt = IronsightsComponent.rollReturnSpeed;
				}
				else
				{
					returnSpeedAmt = IronsightsComponent.rollReturnSpeedSwim;
				}
				if (FPSWalkerComponent.sprintActive)
				{
					rollAmt = IronsightsComponent.sprintStrafeRoll;
					lookRollAmt = -1000f * (1f - Time.timeScale) * IronsightsComponent.btLookRoll;
				}
				else
				{
					rollAmt = IronsightsComponent.walkStrafeRoll;
					if (!viewUnderwater)
					{
						if (Time.timeScale < 1f)
						{
							lookRollAmt = -500f * (1f - Time.timeScale) * IronsightsComponent.btLookRoll;
						}
						else
						{
							lookRollAmt = -100f * IronsightsComponent.lookRoll;
						}
					}
					else
					{
						lookRollAmt = -100f * IronsightsComponent.swimLookRoll;
					}
				}
				myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeedAmt);
				deltaAmt = Mathf.Clamp01(Time.deltaTime) * 75f;
				tempCamAngles = Quaternion.Euler(mainCameraTransform.localEulerAngles.x - CamAndWeapAnimsComponent.camAngleAnim.y * IronsightsComponent.camAngleBobAmt.y * deltaAmt + CameraAnglesAnim.x * deltaAmt, mainCameraTransform.localEulerAngles.y - CamAndWeapAnimsComponent.camAngleAnim.x * IronsightsComponent.camAngleBobAmt.x * deltaAmt + CameraAnglesAnim.y * deltaAmt, mainCameraTransform.localEulerAngles.z - CamAndWeapAnimsComponent.camAngleAnim.z * IronsightsComponent.camAngleBobAmt.z * deltaAmt + CameraAnglesAnim.z * deltaAmt - FPSWalkerComponent.leanAmt * 3f * Time.deltaTime * returnSpeedAmt - FPSWalkerComponent.inputX * rollAmt * Time.deltaTime * returnSpeedAmt - IronsightsComponent.side * lookRollAmt * Time.deltaTime * returnSpeedAmt);
				myTransform.localRotation = tempCamAngles;
			}
		}
		if (FPSWalkerComponent.fallingDistance < 1.25f && !FPSWalkerComponent.jumping)
		{
			if (!landState)
			{
				landStartTime = Time.time;
				landState = true;
			}
		}
		else if (landState && landStartTime + landTime < Time.time)
		{
			landState = false;
		}
		if (landStartTime + landTime > Time.time)
		{
			if (landStartTime + landTime * 0.5f < Time.time)
			{
				landValue += landAmt * Mathf.Min(0.1f, Time.deltaTime);
			}
			else
			{
				landValue -= landAmt * Mathf.Min(0.1f, Time.deltaTime);
			}
		}
		else
		{
			landValue = 0f;
		}
		if (!FPSPlayerComponent.zoomed)
		{
			gunDown = landValue * 0.3f;
		}
		else
		{
			gunDown = landValue * 0.1f;
		}
		if (movingTime + 0.75f < Time.time)
		{
			IronsightsComponent.jumpAmt = gunDown;
		}
		else
		{
			IronsightsComponent.jumpAmt = 0f;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360f;
		if (angle >= -360f && angle <= 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	private float AdjustLineOfSight(Vector3 target, Vector3 direction)
	{
		RaycastHit hitInfo;
		if (Physics.SphereCast(target, sphereSizeTpCol, direction, out hitInfo, zoomDistance, clipMask))
		{
			if (hitInfo.collider.gameObject.tag != "Usable" && hitInfo.collider.gameObject.tag != "NoHitMark")
			{
				return hitInfo.distance - zoomDistance + rayTestPadding;
			}
			return 0f;
		}
		return 0f;
	}
}
