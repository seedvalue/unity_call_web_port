using UnityEngine;

public class GunSway : MonoBehaviour
{
	private FPSRigidBodyWalker FPSWalkerComponent;

	private Ironsights IronsightsComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	private FPSPlayer FPSPlayerComponent;

	private InputControl InputComponent;

	private CameraControl CameraControlComponent;

	[HideInInspector]
	public GameObject cameraObj;

	[HideInInspector]
	public GameObject playerObj;

	private Transform myTransform;

	private float dampSpeed = 0.001f;

	private float dampVelocity1;

	private float dampVelocity2;

	private float dampVelocity6;

	private float dampVelocity7;

	[HideInInspector]
	public Vector3 targetRotation;

	[HideInInspector]
	public Vector3 targetRotation2;

	[HideInInspector]
	public float targetRotationRoll;

	[HideInInspector]
	public float targetRotationPitch;

	[HideInInspector]
	public Vector3 tempEulerAngles;

	[HideInInspector]
	public float localSide;

	[HideInInspector]
	public float localRaise;

	private float swingAmt = 0.035f;

	private float swingSpeed = 9f;

	[HideInInspector]
	public float localRoll;

	[HideInInspector]
	public float localPitch;

	private float rollSpeed;

	private float zAxis1;

	private float zAxis2;

	private float gunBobRoll = 10f;

	private float gunBobYaw = 16f;

	[Tooltip("Amount to sway weapon with mouse movement.")]
	public float swayAmount = 1f;

	private float swayVal = 1f;

	[Tooltip("Amount to sway weapon roll angle with mouse movement.")]
	public float rollSwayAmount = 1f;

	[Tooltip("Amount to bob weapon yaw angles when player is walking.")]
	public float walkBobYawAmount = 1f;

	[Tooltip("Amount to bob weapon roll angles when player is walking.")]
	public float walkBobRollAmount = 1f;

	[Tooltip("Amount to bob weapon yaw angles when player is sprinting.")]
	public float sprintBobYawAmount = 1f;

	[Tooltip("Amount to bob weapon roll angles when player is sprinting.")]
	public float sprintBobRollAmount = 1f;

	[HideInInspector]
	public bool dzAiming;

	private float zDampVel;

	private float zDamp;

	private void Start()
	{
		myTransform = base.transform;
		CameraControlComponent = Camera.main.GetComponent<CameraControl>();
		playerObj = CameraControlComponent.playerObj;
		cameraObj = CameraControlComponent.transform.parent.transform.gameObject;
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		IronsightsComponent = playerObj.GetComponent<Ironsights>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		InputComponent = playerObj.GetComponent<InputControl>();
		sprintBobYawAmount = Mathf.Clamp01(sprintBobYawAmount);
		sprintBobRollAmount = Mathf.Clamp01(sprintBobRollAmount);
		gunBobRoll *= walkBobRollAmount;
		gunBobYaw *= walkBobYawAmount;
	}

	private void Update()
	{
		WeaponBehaviorComponent = FPSPlayerComponent.WeaponBehaviorComponent;
		if (FPSPlayerComponent.zoomed)
		{
			swayVal = swayAmount * 0.5f;
		}
		else
		{
			swayVal = swayAmount;
		}
		if (!(Time.timeScale > 0f) || !(Time.deltaTime > 0f))
		{
			return;
		}
		if (FPSWalkerComponent.canRun)
		{
			swingAmt = 0.02f * swayVal * WeaponBehaviorComponent.swayAmountUnzoomed;
			swingSpeed = 2.5E-05f * swayVal * WeaponBehaviorComponent.swayAmountUnzoomed;
			rollSpeed = 0f * rollSwayAmount * WeaponBehaviorComponent.swayAmountUnzoomed;
			if (gunBobYaw < 12f * sprintBobYawAmount)
			{
				gunBobYaw += 60f * Time.deltaTime;
			}
			if (gunBobRoll < 15f * sprintBobRollAmount)
			{
				gunBobRoll += 60f * Time.deltaTime;
			}
		}
		else
		{
			if (gunBobYaw > -16f * walkBobYawAmount)
			{
				gunBobYaw -= 60f * Time.deltaTime;
			}
			if (gunBobRoll > 15f * walkBobRollAmount)
			{
				gunBobRoll -= 60f * Time.deltaTime;
			}
			if (!FPSPlayerComponent.zoomed || IronsightsComponent.reloading || WeaponBehaviorComponent.meleeSwingDelay != 0f)
			{
				swingAmt = 0.035f * swayVal * WeaponBehaviorComponent.swayAmountUnzoomed;
				swingSpeed = 0.00015f * swayVal * WeaponBehaviorComponent.swayAmountUnzoomed;
				rollSpeed = 0.025f * rollSwayAmount * WeaponBehaviorComponent.swayAmountUnzoomed;
			}
			else
			{
				swingAmt = 0.025f * swayVal * WeaponBehaviorComponent.swayAmountZoomed;
				swingSpeed = 0.0001f * swayVal * WeaponBehaviorComponent.swayAmountZoomed;
				rollSpeed = 0.075f * rollSwayAmount * WeaponBehaviorComponent.swayAmountZoomed;
			}
		}
		zAxis1 = Camera.main.transform.localEulerAngles.z;
		zAxis2 = cameraObj.transform.localEulerAngles.z;
		targetRotation.x = cameraObj.transform.eulerAngles.x;
		if (FPSPlayerComponent.removePrefabRoot)
		{
			targetRotation.y = cameraObj.transform.eulerAngles.y;
		}
		else
		{
			targetRotation.y = cameraObj.transform.localEulerAngles.y;
		}
		targetRotation.z = Mathf.MoveTowardsAngle(zAxis1, zAxis2, Time.deltaTime / 16f);
		if (CameraControlComponent.MouseLookComponent.playerMovedTime + 0.2f * Time.timeScale < Time.time)
		{
			if (!dzAiming)
			{
				localRaise = Mathf.DeltaAngle(targetRotation2.x, targetRotation.x) * (swingSpeed / Time.deltaTime);
				localSide = Mathf.DeltaAngle(targetRotation2.y, targetRotation.y) * (0f - swingSpeed / Time.deltaTime);
			}
			else
			{
				localRaise = (0f - InputComponent.lookY) * 0.002f;
				localSide = (0f - InputComponent.lookX) * 0.002f;
			}
		}
		IronsightsComponent.side = Mathf.Clamp(localSide, 0f - swingAmt, swingAmt);
		IronsightsComponent.raise = Mathf.Clamp(localRaise, 0f - swingAmt, swingAmt);
		localRoll = Mathf.LerpAngle(localRoll, Mathf.DeltaAngle(targetRotationRoll, targetRotation.y) * (0f - rollSpeed) * 3f, Time.deltaTime * 5f);
		localPitch = Mathf.LerpAngle(localPitch, Mathf.DeltaAngle(targetRotationPitch, targetRotation.x) * (0f - rollSpeed) * 3f, Time.deltaTime * 5f);
		targetRotation2.x = Mathf.SmoothDampAngle(targetRotation2.x, targetRotation.x, ref dampVelocity1, dampSpeed, float.PositiveInfinity, Time.deltaTime);
		targetRotation2.y = Mathf.SmoothDampAngle(targetRotation2.y, targetRotation.y, ref dampVelocity2, dampSpeed, float.PositiveInfinity, Time.deltaTime);
		targetRotationRoll = Mathf.SmoothDampAngle(targetRotationRoll, targetRotation.y, ref dampVelocity6, 0.075f, float.PositiveInfinity, Time.deltaTime);
		targetRotationPitch = Mathf.SmoothDampAngle(targetRotationPitch, targetRotation.x, ref dampVelocity7, 0.075f, float.PositiveInfinity, Time.deltaTime);
		if (WeaponBehaviorComponent.shooting)
		{
			zDamp = Mathf.SmoothDampAngle(zDamp, WeaponBehaviorComponent.randZkick, ref zDampVel, 0.075f, float.PositiveInfinity, Time.deltaTime);
		}
		else
		{
			zDamp = Mathf.SmoothDampAngle(zDamp, 0f, ref zDampVel, 0.075f, float.PositiveInfinity, Time.deltaTime);
		}
		tempEulerAngles = new Vector3(targetRotation.x, targetRotation.y, targetRotation.z - FPSWalkerComponent.leanPos * 10f + zDamp);
		myTransform.localRotation = Quaternion.Euler(tempEulerAngles);
	}
}
