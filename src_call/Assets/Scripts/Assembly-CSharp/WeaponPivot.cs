using UnityEngine;

public class WeaponPivot : MonoBehaviour
{
	private GunSway GunSwayComponent;

	private FPSPlayer FPSPlayerComponent;

	private FPSRigidBodyWalker FPSWalkerComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	private SmoothMouseLook SmoothMouseLookComponent;

	private InputControl InputComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private Ironsights IronsightsComponent;

	private GameObject playerObj;

	[Tooltip("Prevent deadzone aiming with this weapon.")]
	public bool noDeadzoneAiming;

	[Tooltip("True if weapon should aim with deadzone (free/unlocked aiming).")]
	public bool deadzoneLooking;

	[Tooltip("True if weapon should zoom with deadzone (free/unlocked zooming).")]
	public bool deadzoneZooming;

	[Tooltip("True if weapon should sway towards view movement direction.")]
	public bool swayLeadingMode;

	[HideInInspector]
	public bool wasDeadzoneZooming;

	[Tooltip("True if crosshair should follow weapon when swaying.")]
	public bool swayLeadingFollowCrosshair = true;

	[Tooltip("Vertical speed of deadzone aiming.")]
	public float verticalSpeed = 1f;

	[Tooltip("Horizontal speed of deadzone aiming.")]
	public float horizontalSpeed = 1f;

	[Tooltip("Amount of weapon sway in leading mode.")]
	public float swayAmount = 1f;

	[Tooltip("Speed that weapon returns to center position after swaying or deadzone zooming.")]
	public float neutralSpeed = 0.2f;

	[Tooltip("Pivot point to rotate weapon around in deadzone aiming mode.")]
	public Transform pivot;

	[HideInInspector]
	public Transform childTransform;

	[HideInInspector]
	public Transform myTransform;

	private float cameraParentYaw;

	[HideInInspector]
	public Vector3 rotateAmtNeutral;

	[HideInInspector]
	public Vector3 mouseOffsetTarg;

	[HideInInspector]
	public Vector3 animOffsetTarg;

	public float animSpeed = 1f;

	private Vector3 velocity2;

	private float velocity3;

	private Animator PivotAnimatorComponent;

	[HideInInspector]
	public float y;

	private float yTarg;

	private float sumY;

	[HideInInspector]
	public float x;

	private float xTarg;

	private float sumX;

	private bool init;

	private bool dzSprintState;

	private bool dzProneState;

	private bool dzClimbState;

	private bool dzAimState;

	private bool dzZoomState;

	private float swayAmt;

	private Vector2 inputSmoothDampVel;

	private float leadRoll;

	private float leadPitch;

	[Tooltip("Max camera zoom FOV that weapon can free aim with (allows sniper rifles to zoom normally in free aiming mode)")]
	public float maxFreeAimFOV = 15f;

	private void Start()
	{
		GunSwayComponent = base.transform.parent.transform.GetComponent<GunSway>();
		SmoothMouseLookComponent = GunSwayComponent.cameraObj.GetComponent<SmoothMouseLook>();
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		InputComponent = playerObj.GetComponent<InputControl>();
		PlayerWeaponsComponent = FPSPlayerComponent.PlayerWeaponsComponent;
		IronsightsComponent = FPSPlayerComponent.IronsightsComponent;
		PivotAnimatorComponent = GetComponent<Animator>();
		myTransform = base.transform;
		childTransform = myTransform.GetChild(0);
		swayAmt = GunSwayComponent.swayAmount;
		if (deadzoneLooking)
		{
			SmoothMouseLookComponent.dzAiming = true;
		}
	}

	private void Update()
	{
		WeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
		if (!(Time.timeScale > 0f) || !(Time.smoothDeltaTime > 0f) || !(Time.deltaTime > 0f))
		{
			return;
		}
		if (!noDeadzoneAiming && !SmoothMouseLookComponent.thirdPerson)
		{
			if (!FPSPlayerComponent.zoomed && InputComponent.deadzonePress)
			{
				if (deadzoneLooking)
				{
					if (wasDeadzoneZooming)
					{
						deadzoneZooming = true;
						FPSPlayerComponent.reticleColor.a = 0f;
						FPSPlayerComponent.crosshairUiImage.color = FPSPlayerComponent.reticleColor;
					}
					deadzoneLooking = false;
				}
				else
				{
					deadzoneLooking = true;
					deadzoneZooming = false;
				}
			}
			if (deadzoneLooking && FPSWalkerComponent.sprintActive)
			{
				deadzoneLooking = false;
				dzSprintState = true;
			}
			if (!FPSWalkerComponent.sprintActive && WeaponBehaviorComponent.recoveryTime + WeaponBehaviorComponent.recoveryTimeAmt + 0.1f < Time.time && dzSprintState)
			{
				deadzoneLooking = true;
				dzSprintState = false;
			}
			if (deadzoneLooking && FPSWalkerComponent.moving && FPSWalkerComponent.prone)
			{
				deadzoneLooking = false;
				dzProneState = true;
			}
			if (((!FPSWalkerComponent.moving && FPSWalkerComponent.prone && WeaponBehaviorComponent.recoveryTime + WeaponBehaviorComponent.recoveryTimeAmt + 0.1f < Time.time) || !FPSWalkerComponent.prone) && dzProneState)
			{
				deadzoneLooking = true;
				dzProneState = false;
			}
			if (deadzoneLooking && FPSWalkerComponent.climbing)
			{
				deadzoneLooking = false;
				dzClimbState = true;
			}
			if (!FPSWalkerComponent.climbing && dzClimbState)
			{
				deadzoneLooking = true;
				dzClimbState = false;
			}
			if (!deadzoneZooming)
			{
				IronsightsComponent.dzAiming = false;
				FPSPlayerComponent.dzAiming = false;
				if (deadzoneLooking && FPSPlayerComponent.zoomed)
				{
					deadzoneLooking = false;
					dzAimState = true;
				}
				if (!FPSPlayerComponent.zoomed && dzAimState)
				{
					if (!dzZoomState)
					{
						deadzoneLooking = true;
						dzAimState = false;
					}
					else
					{
						dzZoomState = false;
						dzAimState = false;
					}
				}
			}
			else
			{
				if (FPSPlayerComponent.zoomed && !WeaponBehaviorComponent.zoomIsBlock && WeaponBehaviorComponent.zoomFOV > maxFreeAimFOV)
				{
					deadzoneLooking = true;
					IronsightsComponent.dzAiming = true;
					FPSPlayerComponent.dzAiming = true;
				}
				else
				{
					deadzoneLooking = false;
					IronsightsComponent.dzAiming = false;
					FPSPlayerComponent.dzAiming = false;
				}
				dzZoomState = true;
				wasDeadzoneZooming = true;
			}
		}
		else
		{
			deadzoneLooking = false;
		}
		if (myTransform.localEulerAngles.y < 180f)
		{
			rotateAmtNeutral.y = 0f - myTransform.localEulerAngles.y;
		}
		else
		{
			rotateAmtNeutral.y = 360f - myTransform.localEulerAngles.y;
		}
		if (childTransform.localEulerAngles.x < 180f)
		{
			rotateAmtNeutral.x = 0f - childTransform.localEulerAngles.x;
		}
		else
		{
			rotateAmtNeutral.x = 360f - childTransform.localEulerAngles.x;
		}
		if (childTransform.localEulerAngles.x > 180f)
		{
			cameraParentYaw = (360f - childTransform.localEulerAngles.x) * -1f;
		}
		else
		{
			cameraParentYaw = childTransform.localEulerAngles.x;
		}
		if (deadzoneLooking && !WeaponBehaviorComponent.unarmed && !SmoothMouseLookComponent.thirdPerson)
		{
			if (init)
			{
				SmoothMouseLookComponent.dzAiming = true;
				GunSwayComponent.swayAmount = swayAmt;
				GunSwayComponent.dzAiming = true;
				init = false;
			}
			x = InputComponent.lookX * horizontalSpeed;
			y = (0f - InputComponent.lookY) * verticalSpeed;
		}
		else
		{
			if (!init)
			{
				if (FPSPlayerComponent.zoomed)
				{
					SmoothMouseLookComponent.rotationY += 0f - cameraParentYaw;
					SmoothMouseLookComponent.rotationX += myTransform.localEulerAngles.y;
				}
				SmoothMouseLookComponent.dzAiming = false;
				GunSwayComponent.dzAiming = false;
				init = true;
			}
			myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, Vector3.zero, 0.005f * Time.smoothDeltaTime);
			childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, Vector3.zero, 0.005f * Time.smoothDeltaTime);
			if (swayLeadingMode)
			{
				if (swayLeadingFollowCrosshair && !SmoothMouseLookComponent.thirdPerson)
				{
					FPSPlayerComponent.raycastCrosshair = true;
				}
				else
				{
					FPSPlayerComponent.raycastCrosshair = false;
				}
				if (!FPSPlayerComponent.zoomed && !FPSWalkerComponent.sprintActive)
				{
					GunSwayComponent.swayAmount = 0f;
					leadRoll = Mathf.SmoothDampAngle(leadRoll, InputComponent.lookX, ref inputSmoothDampVel.x, Time.smoothDeltaTime);
					leadPitch = Mathf.SmoothDampAngle(leadPitch, 0f - InputComponent.lookY, ref inputSmoothDampVel.y, Time.smoothDeltaTime);
					x = Mathf.SmoothDampAngle(x, Mathf.DeltaAngle(x, InputComponent.lookX * swayAmount + rotateAmtNeutral.y * neutralSpeed), ref velocity2.y, Time.smoothDeltaTime);
					y = Mathf.SmoothDampAngle(y, Mathf.DeltaAngle(y, (0f - InputComponent.lookY) * swayAmount + rotateAmtNeutral.x * neutralSpeed), ref velocity3, Time.smoothDeltaTime);
				}
				else
				{
					GunSwayComponent.swayAmount = swayAmt;
					x = Mathf.SmoothDampAngle(x, rotateAmtNeutral.y * neutralSpeed, ref velocity2.y, Time.smoothDeltaTime);
					y = Mathf.SmoothDampAngle(y, rotateAmtNeutral.x * neutralSpeed, ref velocity3, Time.smoothDeltaTime);
				}
			}
			else
			{
				FPSPlayerComponent.raycastCrosshair = false;
				GunSwayComponent.swayAmount = swayAmt;
				if (!FPSPlayerComponent.zoomed)
				{
					x = Mathf.SmoothDampAngle(x, rotateAmtNeutral.y * neutralSpeed - IronsightsComponent.CamAndWeapAnimsComponent.weapAngleAnim.x * IronsightsComponent.weapAngleBobAmt.x, ref velocity2.y, Time.smoothDeltaTime);
					y = Mathf.SmoothDampAngle(y, rotateAmtNeutral.x * neutralSpeed, ref velocity3, Time.smoothDeltaTime);
				}
				else
				{
					x = Mathf.SmoothDampAngle(x, rotateAmtNeutral.y * neutralSpeed, ref velocity2.y, Time.smoothDeltaTime);
					y = Mathf.SmoothDampAngle(y, rotateAmtNeutral.x * neutralSpeed, ref velocity3, Time.smoothDeltaTime);
				}
			}
		}
		if (sumX + x < 0f - WeaponBehaviorComponent.horizontalDZ)
		{
			x = 0f - WeaponBehaviorComponent.horizontalDZ - sumX;
			sumX = 0f - WeaponBehaviorComponent.horizontalDZ;
			HorizontalLook();
		}
		else if (sumX + x > WeaponBehaviorComponent.horizontalDZ)
		{
			x = WeaponBehaviorComponent.horizontalDZ - sumX;
			sumX = WeaponBehaviorComponent.horizontalDZ;
			HorizontalLook();
		}
		else
		{
			sumX += x;
		}
		if (sumY + y < 0f - WeaponBehaviorComponent.verticalDZUpper)
		{
			y = 0f - WeaponBehaviorComponent.verticalDZUpper - sumY;
			sumY = 0f - WeaponBehaviorComponent.verticalDZUpper;
			VerticalLook();
		}
		else if (sumY + y > WeaponBehaviorComponent.verticalDZLower)
		{
			y = WeaponBehaviorComponent.verticalDZLower - sumY;
			sumY = WeaponBehaviorComponent.verticalDZLower;
			VerticalLook();
		}
		else
		{
			sumY += y;
		}
		myTransform.RotateAround(pivot.position, myTransform.up, x);
		childTransform.RotateAround(pivot.position, myTransform.right, y);
	}

	private void HorizontalLook()
	{
		SmoothMouseLookComponent.horizontalDelta = SmoothMouseLookComponent.rotationX;
		SmoothMouseLookComponent.rotationX += InputComponent.lookX * SmoothMouseLookComponent.sensitivityAmt * Time.timeScale;
		SmoothMouseLookComponent.horizontalDelta = Mathf.DeltaAngle(SmoothMouseLookComponent.horizontalDelta, SmoothMouseLookComponent.rotationX);
	}

	private void VerticalLook()
	{
		SmoothMouseLookComponent.rotationY += InputComponent.lookY * SmoothMouseLookComponent.sensitivityAmt * Time.timeScale;
	}

	public void PlayJumpAnim()
	{
		if (FPSWalkerComponent.jumpingAnims)
		{
			PivotAnimatorComponent.SetTrigger("WeaponJump");
		}
	}

	public void PlayLandAnim()
	{
		if (FPSWalkerComponent.jumpingAnims)
		{
			PivotAnimatorComponent.SetTrigger("WeaponLand");
		}
	}

	public void ToggleDeadzoneZooming()
	{
		if (deadzoneZooming)
		{
			wasDeadzoneZooming = false;
			deadzoneZooming = false;
			return;
		}
		wasDeadzoneZooming = true;
		deadzoneZooming = true;
		FPSPlayerComponent.reticleColor.a = 0f;
		FPSPlayerComponent.crosshairUiImage.color = FPSPlayerComponent.reticleColor;
		deadzoneLooking = false;
	}

	public void ToggleSwayLeadingMode()
	{
		if (swayLeadingMode)
		{
			swayLeadingMode = false;
		}
		else
		{
			swayLeadingMode = true;
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
}
