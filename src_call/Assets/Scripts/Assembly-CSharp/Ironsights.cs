using UnityEngine;

public class Ironsights : MonoBehaviour
{
	public enum zoomType
	{
		hold = 0,
		toggle = 1,
		both = 2
	}

	[HideInInspector]
	public SmoothMouseLook SmoothMouseLook;

	[HideInInspector]
	public CamAndWeapAnims CamAndWeapAnimsComponent;

	[HideInInspector]
	public Animator CamAndWeapAnimator;

	private PlayerWeapons PlayerWeaponsComponent;

	private FPSRigidBodyWalker FPSWalker;

	private FPSPlayer FPSPlayerComponent;

	private InputControl InputComponent;

	[HideInInspector]
	public WeaponBehavior WeaponBehaviorComponent;

	[HideInInspector]
	public WeaponPivot WeaponPivotComponent;

	private Animation GunAnimationComponent;

	private CameraControl CameraControlComponent;

	[HideInInspector]
	public GameObject playerObj;

	[HideInInspector]
	public GameObject weaponObj;

	[HideInInspector]
	public GameObject CameraObj;

	[HideInInspector]
	public Camera WeapCameraObj;

	[HideInInspector]
	public GameObject gunObj;

	[HideInInspector]
	public Transform gun;

	[HideInInspector]
	public float zPosRecNext;

	[HideInInspector]
	public float zPosRec;

	private float recZDamp;

	[HideInInspector]
	public Vector3 nextPos;

	[HideInInspector]
	public Vector3 newPos;

	private Vector3 bobPos;

	private Vector3 dampVel = Vector3.zero;

	[HideInInspector]
	public Vector3 tempGunPos = Vector3.zero;

	[Tooltip("Default camera field of view value.")]
	public float defaultFov = 75f;

	[Tooltip("Default camera field of view value while sprinting.")]
	public float sprintFov = 85f;

	[Tooltip("Amount to subtract from main camera FOV for weapon camera FOV.")]
	public float weaponCamFovDiff = 20f;

	[HideInInspector]
	public float nextFov = 75f;

	[HideInInspector]
	public float newFov = 75f;

	private float FovSmoothSpeed = 0.15f;

	private float dampFOV;

	[HideInInspector]
	public bool dzAiming;

	[Tooltip("User may set zoom mode to toggle, hold, or both (toggle on zoom button press, hold on zoom button hold).")]
	public zoomType zoomMode = zoomType.both;

	[Tooltip("Percentage to reduce mouse sensitivity when zoomed.")]
	public float zoomSensitivity = 0.5f;

	public AudioClip sightsUpSnd;

	public AudioClip sightsDownSnd;

	[HideInInspector]
	public bool zoomSfxState = true;

	[HideInInspector]
	public bool reloading;

	[Tooltip("Camera position bobbing amount when walking (X = horizontal, Y = vertical).")]
	[Space(10f, order = 1)]
	[Header("Bobbing Speeds and Amounts", order = 0)]
	public Vector2 walkPositionBob = Vector2.one;

	[Tooltip("Camera angle bobbing amount when walking (yaw, pitch, roll).")]
	public Vector3 walkAngleBob = Vector3.one;

	[Tooltip("Camera and weapon bobbing speed when walking.")]
	public float walkBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when sprinting (X = horizontal, Y = vertical).")]
	public Vector2 sprintPositionBob = Vector2.one;

	[Tooltip("Camera angle bobbing amount when sprinting (yaw, pitch, roll).")]
	public Vector3 sprintAngleBob = Vector3.one;

	[Tooltip("Camera and weapon bobbing speed when sprinting.")]
	public float sprintBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when crouching (X = horizontal, Y = vertical).")]
	public Vector2 crouchPositionBob = Vector2.one;

	[Tooltip("Camera angle bobbing amount when crouching (yaw, pitch, roll).")]
	public Vector3 crouchAngleBob = Vector3.one;

	[Tooltip("Camera and weapon bobbing speed when crouching.")]
	public float crouchBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when prone (X = horizontal, Y = vertical).")]
	public Vector2 pronePositionBob = Vector2.one;

	[Tooltip("Camera angle bobbing amount when prone (yaw, pitch, roll).")]
	public Vector3 proneAngleBob = Vector3.one;

	[Tooltip("Camera and weapon bobbing speed when prone.")]
	public float proneBobSpeed = 1f;

	[Tooltip("Camera position bobbing amount when zoomed (X = horizontal, Y = vertical).")]
	public Vector2 zoomPositionBob = Vector2.one;

	[Tooltip("Camera angle bobbing amount when zoomed (yaw, pitch, roll).")]
	public Vector3 zoomAngleBob = Vector3.one;

	[Tooltip("Camera and weapon bobbing speed when zoomed.")]
	public float zoomBobSpeed = 1f;

	[Tooltip("Camera and weapon bobbing speed when zoomed and crouched.")]
	public float zoomBobSpeedCrouch = 1f;

	[Tooltip("Camera and weapon bobbing speed multiplier when swimming.")]
	public float swimBobSpeedFactor = 0.6f;

	private float swimBobSpeedAmt;

	private float moveInputAmt;

	private float moveInputSpeed;

	[HideInInspector]
	public Vector2 camPositionBobAmt = Vector2.zero;

	[HideInInspector]
	public Vector3 camAngleBobAmt = Vector3.zero;

	[HideInInspector]
	public Vector3 weapPositionBobAmt = Vector3.zero;

	[HideInInspector]
	public Vector3 weapAngleBobAmt = Vector3.zero;

	[Tooltip("Amount to roll the screen left or right when strafing and sprinting.")]
	public float sprintStrafeRoll = 2f;

	[Tooltip("Amount to roll the screen left or right when strafing and walking.")]
	public float walkStrafeRoll = 1f;

	[Tooltip("Amount to roll the screen left or right when moving view horizontally.")]
	public float lookRoll = 1f;

	[Tooltip("Amount to roll the screen left or right when moving view horizontally during bullet time.")]
	public float btLookRoll = 1f;

	[Tooltip("Amount to roll the screen left or right when moving view horizontally and underwater.")]
	public float swimLookRoll = 1f;

	[Tooltip("Speed to return to neutral roll values when above water.")]
	public float rollReturnSpeed = 4f;

	[Tooltip("Speed to return to neutral roll values when underwater.")]
	public float rollReturnSpeedSwim = 2f;

	[Tooltip("Amount the camera should bob vertically to simulate player breathing.")]
	public float idleBobAmt = 1f;

	[Tooltip("Amount the camera should bob vertically to simulate floating in water.")]
	public float swimBobAmt = 1f;

	private float strafeSideAmt;

	private float pivotBobAmt;

	private float horizontalGunPosAmt = -0.02f;

	private float sprintXPositionAmt;

	private float gunup = 0.015f;

	private float gunRunUp = 1f;

	private float yDampSpeed;

	private float yDampSpeedAmt;

	private float zDampSpeed;

	public float bobDampSpeed = 0.1f;

	private float bobMove;

	private float sideMove;

	[HideInInspector]
	public float switchMove;

	[HideInInspector]
	public float climbMove;

	private float jumpmove;

	[HideInInspector]
	public float jumpAmt;

	private float idleX;

	private float idleY;

	[HideInInspector]
	public float side;

	[HideInInspector]
	public float raise;

	private AudioSource aSource;

	[Tooltip("Point to rotate weapon models for vertical bobbing effect.")]
	public Transform pivot;

	private float pivotAmt;

	private float dampVel2;

	private float rotateAmtNeutral;

	private void Start()
	{
		SmoothMouseLook = CameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		InputComponent = playerObj.GetComponent<InputControl>();
		WeaponPivotComponent = FPSPlayerComponent.WeaponPivotComponent;
		CamAndWeapAnimsComponent = CameraObj.GetComponent<CamAndWeapAnims>();
		CamAndWeapAnimator = CameraObj.GetComponent<Animator>();
		CameraControlComponent = Camera.main.transform.GetComponent<CameraControl>();
		tempGunPos = Vector3.zero;
		aSource = playerObj.AddComponent<AudioSource>();
		aSource.spatialBlend = 0f;
		aSource.playOnAwake = false;
		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", true);
	}

	private void Update()
	{
		if (!(Time.timeScale > 0f) || !(Time.deltaTime > 0f) || !(Time.smoothDeltaTime > 0f))
		{
			return;
		}
		yDampSpeedAmt = Mathf.MoveTowards(yDampSpeedAmt, yDampSpeed, Time.deltaTime);
		if ((double)SmoothMouseLook.playerMovedTime + 0.1 < (double)Time.time || FPSWalker.moving)
		{
			newPos.x = Mathf.SmoothDamp(newPos.x, nextPos.x, ref dampVel.x, yDampSpeedAmt, float.PositiveInfinity, Time.deltaTime);
			newPos.y = Mathf.SmoothDamp(newPos.y, nextPos.y, ref dampVel.y, yDampSpeedAmt, float.PositiveInfinity, Time.deltaTime);
			newPos.z = Mathf.SmoothDamp(newPos.z, nextPos.z, ref dampVel.z, zDampSpeed, float.PositiveInfinity, Time.deltaTime);
			zPosRec = Mathf.SmoothDamp(zPosRec, zPosRecNext, ref recZDamp, 0.25f, float.PositiveInfinity, Time.deltaTime);
		}
		else
		{
			newPos.x = nextPos.x;
			newPos.y = nextPos.y;
			newPos.z = nextPos.z;
			zPosRec = zPosRecNext;
		}
		newFov = Mathf.SmoothDamp(Camera.main.fieldOfView, nextFov, ref dampFOV, FovSmoothSpeed, float.PositiveInfinity, Time.deltaTime);
		if ((bool)WeapCameraObj)
		{
			WeapCameraObj.fieldOfView = Camera.main.fieldOfView - weaponCamFovDiff;
		}
		Camera.main.fieldOfView = newFov;
		float inputX = FPSWalker.inputX;
		float inputY = FPSWalker.inputY;
		if (WeaponBehaviorComponent.shootStartTime + 0.1f > Time.time)
		{
			if (FPSPlayerComponent.zoomed)
			{
				zPosRecNext = WeaponBehaviorComponent.kickBackAmtZoom;
			}
			else
			{
				zPosRecNext = WeaponBehaviorComponent.kickBackAmtUnzoom;
			}
		}
		else if (WeaponBehaviorComponent.pullBackAmt != 0f)
		{
			zPosRecNext = WeaponBehaviorComponent.pullBackAmt * WeaponBehaviorComponent.fireHoldMult;
		}
		else
		{
			zPosRecNext = 0f;
		}
		if (FPSWalker.moving)
		{
			idleY = 0f;
			idleX = 0f;
			if (((FPSWalker.sprintActive && !FPSWalker.crouched && !FPSWalker.cancelSprint && FPSWalker.midPos >= FPSWalker.standingCamHeight && FPSWalker.proneRisen) || (!reloading && !FPSWalker.proneRisen && !FPSWalker.crouched && (FPSWalker.prone || FPSWalker.sprintActive))) && FPSWalker.fallingDistance < 0.75f && !FPSPlayerComponent.zoomed && !FPSWalker.jumping)
			{
				if (!FPSWalker.cancelSprint && (!reloading || FPSWalker.sprintReload) && FPSWalker.fallingDistance < 0.75f && !FPSWalker.jumping)
				{
					if (FPSWalker.grounded)
					{
						PlaySprintingAnim();
					}
					else
					{
						PlayIdleAnim();
					}
					if (((FPSWalker.inputY != 0f && FPSWalker.forwardSprintOnly) || (!FPSWalker.forwardSprintOnly && FPSWalker.moving)) && !FPSWalker.prone)
					{
						nextFov = sprintFov;
					}
					else
					{
						nextFov = defaultFov;
					}
					if (!reloading)
					{
						sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, WeaponBehaviorComponent.sprintXPosition, Time.deltaTime * 16f);
						horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
						if (gunRunUp < 1.4f)
						{
							gunRunUp += Time.deltaTime / 4f;
						}
						bobMove = gunup + WeaponBehaviorComponent.sprintYPosition;
						sideMove = 0f;
					}
					else
					{
						gunRunUp = 1f;
						bobMove = 0f;
						sideMove = 0f;
						sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, 0f, Time.deltaTime * 16f);
						horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
					}
				}
				else
				{
					nextFov = defaultFov;
					gunRunUp = 1f;
					bobMove = -0.01f;
					if (!FPSWalker.prone)
					{
						switchMove = 0f;
					}
				}
			}
			else
			{
				if (FPSWalker.grounded)
				{
					PlayWalkingAnim();
				}
				else
				{
					PlayIdleAnim();
				}
				gunRunUp = 1f;
				sprintXPositionAmt = Mathf.MoveTowards(sprintXPositionAmt, 0f, Time.deltaTime * 16f);
				horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition + sprintXPositionAmt;
				if (reloading)
				{
					nextFov = defaultFov;
					bobMove = 0f;
					sideMove = 0f;
				}
				else
				{
					nextFov = defaultFov;
					if (FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0f)
					{
						bobMove = 0f;
					}
					else if (FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f)
					{
						bobMove = WeaponBehaviorComponent.crouchWalkYPosition;
						sideMove = WeaponBehaviorComponent.crouchXPosition;
					}
					else
					{
						bobMove = WeaponBehaviorComponent.walkYPosition;
						sideMove = 0f;
					}
				}
			}
		}
		else
		{
			PlayIdleAnim();
			nextFov = defaultFov;
			horizontalGunPosAmt = WeaponBehaviorComponent.unzoomXPosition;
			if (sprintXPositionAmt > 0f)
			{
				sprintXPositionAmt -= Time.deltaTime / 4f;
			}
			if (reloading)
			{
				nextFov = defaultFov;
				bobMove = 0f;
				sideMove = 0f;
			}
			else if ((FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) && !FPSPlayerComponent.zoomed)
			{
				bobMove = WeaponBehaviorComponent.crouchYPosition;
				sideMove = WeaponBehaviorComponent.crouchXPosition;
			}
			else
			{
				bobMove = 0f;
				sideMove = 0f;
			}
			if (FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0f)
			{
				idleX = Mathf.Sin(Time.time * 1.25f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
				idleY = Mathf.Sin(Time.time * 1.5f) * 0.0005f * WeaponBehaviorComponent.zoomIdleSwayAmt;
			}
			else if (!FPSWalker.swimming)
			{
				idleX = Mathf.Sin(Time.time * 1.25f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
				idleY = Mathf.Sin(Time.time * 1.5f) * 0.0012f * WeaponBehaviorComponent.idleSwayAmt;
			}
			else
			{
				idleX = Mathf.Sin(Time.time * 1.25f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;
				idleY = Mathf.Sin(Time.time * 1.5f) * 0.003f * WeaponBehaviorComponent.swimIdleSwayAmt;
			}
		}
		tempGunPos.x = newPos.x + CamAndWeapAnimsComponent.weapPosAnim.x * weapPositionBobAmt.x;
		tempGunPos.y = newPos.y + CamAndWeapAnimsComponent.weapPosAnim.y * weapPositionBobAmt.y;
		tempGunPos.z = newPos.z + zPosRec - CamAndWeapAnimsComponent.weapPosAnim.z * weapPositionBobAmt.z;
		if (!WeaponBehaviorComponent.unarmed && Time.timeSinceLevelLoad > 0.3f)
		{
			gun.localPosition = tempGunPos;
		}
		gun.transform.parent.transform.localPosition = Vector3.MoveTowards(gun.transform.parent.transform.localPosition, Vector3.zero, 0.005f * Time.smoothDeltaTime);
		pivotAmt = Mathf.SmoothDampAngle(pivotAmt, WeaponPivotComponent.animOffsetTarg.x * 6f + CamAndWeapAnimsComponent.weapAngleAnim.y * weapAngleBobAmt.y * 6.5f, ref dampVel2, 0.04f, float.PositiveInfinity, Time.smoothDeltaTime);
		rotateAmtNeutral = Mathf.DeltaAngle(gun.transform.parent.transform.localEulerAngles.x, pivotAmt);
		gun.transform.parent.transform.RotateAround(pivot.position, gun.transform.parent.transform.right, rotateAmtNeutral);
		if (FPSWalker.jumping || FPSWalker.fallingDistance > 1.25f)
		{
			if (!FPSPlayerComponent.zoomed)
			{
				if (FPSWalker.airTime + 0.175f > Time.time)
				{
					jumpmove = 0.015f;
				}
				else
				{
					jumpmove = -0.025f;
				}
			}
			else
			{
				jumpmove = -0.01f;
			}
		}
		else
		{
			jumpmove = 0f;
		}
		if (!FPSWalker.swimming)
		{
			swimBobSpeedAmt = 1f;
		}
		else
		{
			swimBobSpeedAmt = Mathf.Max(0.01f, swimBobSpeedFactor);
		}
		if (!FPSWalker.sprintActive)
		{
			moveInputSpeed = Mathf.Clamp01(Mathf.Max(Mathf.Abs(FPSWalker.inputX), Mathf.Abs(FPSWalker.inputY) + InputComponent.deadzone));
		}
		else
		{
			moveInputSpeed = 1f;
		}
		moveInputAmt = Mathf.MoveTowards(moveInputAmt, moveInputSpeed, Time.deltaTime * 3.5f);
		if ((FPSPlayerComponent.zoomed || (FPSPlayerComponent.canBackstab && !WeaponBehaviorComponent.shooting)) && FPSPlayerComponent.hitPoints > 1f && PlayerWeaponsComponent.switchTime + WeaponBehaviorComponent.readyTimeAmt < Time.time && !reloading && PlayerWeaponsComponent.currentWeapon != 0 && (FPSPlayerComponent.canBackstab || (WeaponBehaviorComponent.zoomIsBlock && ((WeaponBehaviorComponent.shootStartTime + WeaponBehaviorComponent.fireRate < Time.time && !WeaponBehaviorComponent.shootFromBlock) || WeaponBehaviorComponent.shootFromBlock)) || !WeaponBehaviorComponent.zoomIsBlock) && WeaponBehaviorComponent.reloadLastStartTime + WeaponBehaviorComponent.reloadLastTime < Time.time)
		{
			if (!dzAiming)
			{
				if (!reloading)
				{
					strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16f);
				}
				else
				{
					strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16f);
				}
				if (!FPSPlayerComponent.canBackstab)
				{
					nextPos.x = WeaponBehaviorComponent.zoomXPosition + side / 1.5f + idleX + FPSWalker.inputX * 0.1f * strafeSideAmt;
					nextPos.y = WeaponBehaviorComponent.zoomYPosition + raise / 1.5f + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove);
					nextPos.z = WeaponBehaviorComponent.zoomZPosition;
				}
				else
				{
					nextPos.x = WeaponBehaviorComponent.backstabXPosition + side / 1.5f + idleX + FPSWalker.inputX * 0.1f * strafeSideAmt;
					nextPos.y = WeaponBehaviorComponent.backstabYPosition + raise / 1.5f + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove);
					nextPos.z = WeaponBehaviorComponent.backstabZPosition;
				}
				if (WeaponBehaviorComponent.zoomFOVTp > 0f && CameraControlComponent.thirdPersonActive)
				{
					nextFov = WeaponBehaviorComponent.zoomFOVTp;
				}
				else
				{
					nextFov = WeaponBehaviorComponent.zoomFOV;
				}
				if (zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0f && !WeaponBehaviorComponent.unarmed)
				{
					aSource.clip = sightsUpSnd;
					aSource.volume = 1f;
					aSource.pitch = 1f * Time.timeScale;
					aSource.Play();
					zoomSfxState = false;
				}
			}
			else
			{
				if (!reloading)
				{
					strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideZoom, Time.deltaTime * 16f);
				}
				else
				{
					strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16f);
				}
				nextPos.x = side + idleX + sideMove + horizontalGunPosAmt + FPSWalker.leanAmt / 60f + FPSWalker.inputX * 0.1f * strafeSideAmt;
				nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove) + WeaponBehaviorComponent.unzoomYPosition;
				nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
				nextFov = WeaponBehaviorComponent.zoomFOVDz;
			}
			FovSmoothSpeed = 0.09f;
			yDampSpeed = 0.09f;
			zDampSpeed = 0.15f;
			if (!FPSPlayerComponent.canBackstab)
			{
				FPSWalker.zoomSpeed = true;
				if (!WeaponBehaviorComponent.zoomIsBlock)
				{
					SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity * WeaponBehaviorComponent.zoomSensitivity;
				}
				camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, zoomPositionBob, Time.smoothDeltaTime * 24f);
				camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, zoomAngleBob, Time.smoothDeltaTime * 24f);
				weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.zoomBobPosition, Time.smoothDeltaTime * 24f);
				weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.zoomBobAngles, Time.smoothDeltaTime * 24f);
				if (FPSWalker.crouched)
				{
					CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, zoomBobSpeedCrouch * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);
				}
				else
				{
					CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, zoomBobSpeed * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);
				}
			}
			return;
		}
		FovSmoothSpeed = 0.18f;
		if (!PlayerWeaponsComponent.switching)
		{
			yDampSpeed = 0.18f;
		}
		else
		{
			yDampSpeed = 0.2f;
		}
		zDampSpeed = 0.1f;
		nextPos.x = side + idleX + sideMove + horizontalGunPosAmt + FPSWalker.leanAmt / 60f + FPSWalker.inputX * 0.1f * strafeSideAmt;
		nextPos.y = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove) + WeaponBehaviorComponent.unzoomYPosition;
		if (!FPSWalker.prone)
		{
			nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
		}
		else
		{
			nextPos.z = WeaponBehaviorComponent.unzoomZPosition;
		}
		FPSWalker.zoomSpeed = false;
		if (!zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0f && !WeaponBehaviorComponent.unarmed)
		{
			aSource.clip = sightsDownSnd;
			aSource.volume = 1f;
			aSource.pitch = 1f * Time.timeScale;
			aSource.Play();
			zoomSfxState = true;
		}
		SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity;
		if (FPSWalker.sprintActive && (!FPSWalker.forwardSprintOnly || Mathf.Abs(inputX) == 0f || !(Mathf.Abs(inputY) < 0.75f)) && (Mathf.Abs(inputY) != 0f || (!FPSWalker.forwardSprintOnly && FPSWalker.moving)) && !FPSWalker.cancelSprint && !FPSWalker.crouched && !FPSWalker.prone && FPSWalker.midPos >= FPSWalker.standingCamHeight && !FPSPlayerComponent.zoomed && !InputComponent.fireHold)
		{
			if (!reloading)
			{
				strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideSprint, Time.deltaTime * 16f);
			}
			else
			{
				strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16f);
			}
			camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, sprintPositionBob, Time.smoothDeltaTime * 16f);
			camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, sprintAngleBob, Time.smoothDeltaTime * 16f);
			weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.sprintBobPosition, Time.smoothDeltaTime * 24f);
			weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.sprintBobAngles, Time.smoothDeltaTime * 24f);
			CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, sprintBobSpeed, Time.smoothDeltaTime * 16f);
			if (!reloading)
			{
				nextPos.z = WeaponBehaviorComponent.sprintZPosition;
			}
			return;
		}
		if (!reloading)
		{
			strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, WeaponBehaviorComponent.strafeSideUnzoom, Time.deltaTime * 16f);
		}
		else
		{
			strafeSideAmt = Mathf.MoveTowards(strafeSideAmt, 0f, Time.deltaTime * 16f);
		}
		if (!FPSWalker.crouched && !FPSWalker.prone && FPSWalker.midPos >= FPSWalker.standingCamHeight)
		{
			camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, walkPositionBob, Time.smoothDeltaTime * 16f);
			camAngleBobAmt.x = Mathf.MoveTowards(camAngleBobAmt.x, walkAngleBob.x, Time.smoothDeltaTime * 16f);
			camAngleBobAmt.y = Mathf.MoveTowards(camAngleBobAmt.y, walkAngleBob.y, Time.smoothDeltaTime * 16f);
			weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.walkBobPosition, Time.smoothDeltaTime * 24f);
			weapAngleBobAmt.x = Mathf.MoveTowards(weapAngleBobAmt.x, WeaponBehaviorComponent.walkBobAngles.x, Time.smoothDeltaTime * 24f);
			weapAngleBobAmt.y = Mathf.MoveTowards(weapAngleBobAmt.y, WeaponBehaviorComponent.walkBobAngles.y, Time.smoothDeltaTime * 24f);
			camAngleBobAmt.z = Mathf.MoveTowards(camAngleBobAmt.z, walkAngleBob.z / swimBobSpeedAmt, Time.smoothDeltaTime * 16f);
			weapAngleBobAmt.z = Mathf.MoveTowards(weapAngleBobAmt.z, WeaponBehaviorComponent.walkBobAngles.z / swimBobSpeedAmt, Time.smoothDeltaTime * 24f);
			CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed, walkBobSpeed * swimBobSpeedAmt * moveInputAmt, Time.smoothDeltaTime * 16f);
		}
		else if (FPSWalker.crouched)
		{
			camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, crouchPositionBob, Time.smoothDeltaTime * 16f);
			camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, crouchAngleBob, Time.smoothDeltaTime * 16f);
			weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.crouchBobPosition, Time.smoothDeltaTime * 24f);
			weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.crouchBobAngles, Time.smoothDeltaTime * 24f);
			CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, crouchBobSpeed, Time.smoothDeltaTime * 16f);
		}
		else if (FPSWalker.prone)
		{
			camPositionBobAmt = Vector2.MoveTowards(camPositionBobAmt, pronePositionBob, Time.smoothDeltaTime * 16f);
			camAngleBobAmt = Vector3.MoveTowards(camAngleBobAmt, proneAngleBob, Time.smoothDeltaTime * 16f);
			weapPositionBobAmt = Vector3.MoveTowards(weapPositionBobAmt, WeaponBehaviorComponent.proneBobPosition, Time.smoothDeltaTime * 24f);
			weapAngleBobAmt = Vector3.MoveTowards(weapAngleBobAmt, WeaponBehaviorComponent.proneBobAngles, Time.smoothDeltaTime * 24f);
			CamAndWeapAnimator.speed = Mathf.MoveTowards(CamAndWeapAnimator.speed * moveInputAmt, proneBobSpeed, Time.smoothDeltaTime * 16f);
		}
	}

	private void PlayWalkingAnim()
	{
		CamAndWeapAnimator.SetBool("Walking", true);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", false);
	}

	private void PlaySprintingAnim()
	{
		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", true);
		CamAndWeapAnimator.SetBool("Idle", false);
	}

	private void PlayIdleAnim()
	{
		CamAndWeapAnimator.SetBool("Walking", false);
		CamAndWeapAnimator.SetBool("Sprinting", false);
		CamAndWeapAnimator.SetBool("Idle", true);
	}
}
