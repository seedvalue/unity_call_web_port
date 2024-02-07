using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCharacter : MonoBehaviour
{
	private Animator anim;

	private Transform headBone;

	private Transform chestBone;

	private Transform spineBone;

	[HideInInspector]
	public Animator shotgunAnimComponent;

	[HideInInspector]
	public GameObject playerObj;

	private Transform playerTransform;

	[HideInInspector]
	public GameObject weaponObj;

	private InputControl InputComponent;

	private FPSRigidBodyWalker walkerComponent;

	private FPSPlayer FPSPlayerComponent;

	private SmoothMouseLook SmoothMouseLookComponent;

	private CameraControl CameraControlComponent;

	private GunSway GunSwayComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private Transform myTransform;

	[HideInInspector]
	public SkinnedMeshRenderer[] AllSkinnedMeshes;

	[HideInInspector]
	public MeshRenderer[] AllMeshRenderers;

	[HideInInspector]
	public GameObject fpBodyObj;

	private Transform fpBodyTransform;

	private Animator fpBodyAnim;

	private SkinnedMeshRenderer fpBodySkinnedMesh;

	[Tooltip("True if the visible body should be displayed in first person mode.")]
	public bool displayVisibleBody = true;

	[Tooltip("The animator controller needed to animated the first person visible body.")]
	public RuntimeAnimatorController fpBodyController;

	[Tooltip("Alternate material to use for the first person visible body.")]
	public Material alternateBodyMaterial;

	[Tooltip("Index number of character skinned mesh material to replace with alternate body material.")]
	public int materialToReplace;

	[Tooltip("Reference to the ThirdPersonWeapons.cs script for third person weapons (assigned automatically).")]
	public ThirdPersonWeapons weaponModels;

	[Tooltip("Angles of the third person weapon parent object.")]
	public Vector3 weaponModelParentAngles = new Vector3(355.03f, 276.66f, 269.42f);

	private float bodyAng;

	private float bodyAngAmt;

	private float bodyAngSpeed;

	private Vector3 tempBodyAngles;

	[Tooltip("Speed to rotate the player model's bones.")]
	public float boneSpeed = 4f;

	private float boneSpeedAmt;

	private float boneAng1;

	private float boneAng1Amt;

	[Tooltip("Speed to rotate the player model's yaw angle.")]
	public float slerpSpeed = 14f;

	private float slerpSpeedAmt;

	private float idleTime = -32f;

	private float moveTime = -32f;

	private Vector2 moveInputs;

	[Tooltip("Amount to offset the player's aiming angle.")]
	public float aimOffset;

	private float aimOffsetAmt;

	[Tooltip("Amount to offset the player's aiming angle when swimming.")]
	public float swimFireAng;

	private float swimFireAngAmt;

	private Vector3 dampvel;

	[HideInInspector]
	public Vector3 tempBodyPosition;

	private Vector3 tempSmoothedPos;

	[Tooltip("Scale of model while standing.")]
	public float modelStandScale = 1.4f;

	[Tooltip("Scale of model while crouching.")]
	public float modelCrouchScale = 1.1f;

	private float modelScaleAmt = 1.1f;

	[Tooltip("Scale of player model in third person mode.")]
	public float tpMeshScale = 0.95f;

	[Tooltip("Amount to offset the player's yaw angle when moving forward.")]
	public float forwardYawAng;

	[Tooltip("Amount to offset the player's yaw angle.")]
	public float idleYawAng;

	[Tooltip("Amount to offset the player's yaw angle when unarmed.")]
	public float idleYawAngUnarmed;

	[Tooltip("Amount to offset the player's yaw angle when unarmed and crouched.")]
	public float yawAngUnarmedCrouchStrafe = -25f;

	private float idleYawAngAmt;

	[Tooltip("Pitch angle of visible body in first person mode.")]
	public float modelPitch = -7f;

	[Tooltip("Pitch angle of visible body in first person mode when crouched.")]
	public float modelPitchCrouch = -7f;

	[Tooltip("Pitch angle of visible body when sprinting in first person mode.")]
	public float modelPitchRun = -20f;

	[HideInInspector]
	public float modelPitchAmt;

	[Tooltip("Amount to add to forward position of visible player model when standing in first person mode.")]
	public float modelForward = -0.35f;

	[Tooltip("Amount to add to forward position of visible player model when sprinting in first person mode.")]
	public float modelForwardSprint = -0.35f;

	[Tooltip("Amount to add to forward position of visible player model when crouching in first person mode.")]
	public float modelForwardCrouch = -0.3f;

	[Tooltip("Amount to add to forward position of visible player model when swimming in first person mode.")]
	public float modelForwardDown = -4.5f;

	public float sprintStrafeRight;

	private float modelForwardAmt;

	[HideInInspector]
	public float modelRightAmt;

	[Tooltip("Amount to add to upward position of visible player model when standing in first person mode.")]
	public float modelUpFP = -0.15f;

	[Tooltip("Amount to add to upward position of FP shadow and third person player model when standing.")]
	public float modelUp = 1.8f;

	[Tooltip("Amount to add to upward position of FP shadow and third person player model when crouching.")]
	public float modelUpCrouch = 1.1f;

	[Tooltip("Amount to add to upward position of FP shadow and third person player model when crouching and unarmed.")]
	public float modelUpCrouchUnarmed = 0.05f;

	[Tooltip("Amount to add to upward position of FP shadow and third person player model when sprinting.")]
	public float modelUpRun = 1.1f;

	[HideInInspector]
	public float modelUpAmt;

	[HideInInspector]
	public float verticalPos;

	private float rotAngleAmt;

	private float facingAngleDelta;

	private Vector2 moveSmoothed;

	public float inputSmoothingSpeed = 4f;

	[HideInInspector]
	public float fireTime = -32f;

	[Tooltip("Time to keep the weapon raised after firing.")]
	public float fireUpTime = 2f;

	[Tooltip("Time to keep the weapon raised when firing a single shot.")]
	public float fireShotTime = 0.4f;

	private float forwardSpeedAmt;

	private float sprintTime;

	private float proneMoveTime;

	private bool proneState;

	private float proneStanceTime;

	private int proneTransition;

	[Tooltip("Duration of prone stance change transition.")]
	public float proneTransitionLength = 0.7f;

	private float weapDownTime;

	private int moveDir;

	private int moveState;

	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down.")]
	public Vector3 mLookAng;

	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down.")]
	public Vector3 mLookAng2;

	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down when prone.")]
	public Vector3 mLookAngProne;

	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down when prone.")]
	public Vector3 mLookAngProne2;

	[Tooltip("The xyz amounts of mouse input to apply to chest angles to make the player model look up or down when aiming bow.")]
	public Vector3 mLookAngBow;

	[Tooltip("The xyz amounts of mouse input to apply to spine angles to make the player model look up or down when aiming bow.")]
	public Vector3 mLookAngBow2;

	[Tooltip("True if player model should always aim in first person mode (shadow will always aim in look direction).")]
	public bool fpTorsoAlwaysAims;

	private float torsoLayerWeight;

	private float torsoLayerWeight2;

	private Vector3 chestAngles;

	private Vector3 spineAngles;

	private Vector3 chestAnglesAmt;

	private Vector3 spineAnglesAmt;

	[Tooltip("Amount to rotate player's head to face forward when moving.")]
	public float headRotation = 0.7f;

	[Tooltip("Amount to rotate player's head to face forward when crouched.")]
	public float headRotationCrouch = 0.4f;

	private bool reloadStartTimeState;

	[HideInInspector]
	public float reloadStartTime = -32f;

	private float reloadDurationAmt;

	[Tooltip("The duration of the reloading animation for rifles.")]
	public float reloadDurationRifle = 2.25f;

	[Tooltip("The duration of the reloading animation for pistols.")]
	public float reloadDurationPistol = 1f;

	[Tooltip("The duration of the reloading animation for shotguns.")]
	public float reloadDurationShotgun = 1f;

	private float swimBlendAmt;

	private float swimBlend;

	[Tooltip("The duration of the weapon switching animation.")]
	public float switchDuration = 0.3f;

	[HideInInspector]
	public int lastWeapon;

	private float angOffset;

	private bool meleeMove;

	private float fireDelay;

	[Tooltip("The duration of the offhand melee animation.")]
	public float offhandMeleeTime = 0.14f;

	[Tooltip("Delay before making arrow object visible after firing.")]
	public float arrowVisibleDelay = 0.4f;

	private bool offhandState;

	private bool rotateHead;

	private bool tpModeState;

	private bool deadState;

	private float turnTimer;

	private Transform mainCamTransform;

	[HideInInspector]
	public float tpswitchTime;

	private float modelPosSmoothSpeed;

	private void Start()
	{
		mainCamTransform = Camera.main.transform;
		weaponModels = GetComponentInChildren<ThirdPersonWeapons>();
		weaponModels.gameObject.SetActive(true);
		anim = GetComponentInChildren<Animator>();
		if ((bool)anim.GetBoneTransform(HumanBodyBones.Head))
		{
			headBone = anim.GetBoneTransform(HumanBodyBones.Head);
			chestBone = anim.GetBoneTransform(HumanBodyBones.Chest);
			spineBone = anim.GetBoneTransform(HumanBodyBones.Spine);
		}
		else
		{
			Debug.Log("<color=red>Player model avatar error:</color> Please check your player character setup and animator avatar reference.");
		}
		anim.applyRootMotion = false;
		CameraControlComponent = mainCamTransform.GetComponent<CameraControl>();
		playerObj = CameraControlComponent.playerObj;
		weaponObj = CameraControlComponent.weaponObj;
		playerTransform = playerObj.transform;
		InputComponent = playerObj.GetComponent<InputControl>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		walkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		GunSwayComponent = weaponObj.GetComponent<GunSway>();
		SmoothMouseLookComponent = GunSwayComponent.cameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = FPSPlayerComponent.PlayerWeaponsComponent;
		myTransform = base.transform;
		AllSkinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		AllMeshRenderers = GetComponentsInChildren<MeshRenderer>(true);
		tpModeState = CameraControlComponent.thirdPersonActive;
		CameraControlComponent.PlayerCharacterComponent = this;
		CameraControlComponent.PlayerCharacterObj = base.gameObject;
		walkerComponent.PlayerCharacterComponent = this;
		walkerComponent.PlayerCharacterObj = base.gameObject;
		fpBodyObj = Object.Instantiate(myTransform.gameObject, myTransform.position, myTransform.rotation);
		Object.Destroy(fpBodyObj.GetComponent<PlayerCharacter>());
		fpBodyAnim = fpBodyObj.GetComponent<Animator>();
		GameObject obj = fpBodyObj.GetComponentInChildren<ThirdPersonWeapons>().gameObject;
		Object.Destroy(obj);
		fpBodyTransform = fpBodyObj.transform;
		fpBodyAnim.runtimeAnimatorController = fpBodyController;
		fpBodySkinnedMesh = fpBodyObj.GetComponentInChildren<SkinnedMeshRenderer>();
		fpBodySkinnedMesh.shadowCastingMode = ShadowCastingMode.Off;
		if ((bool)fpBodyAnim.GetBoneTransform(HumanBodyBones.Head))
		{
			fpBodyAnim.GetBoneTransform(HumanBodyBones.Head).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
			fpBodyAnim.GetBoneTransform(HumanBodyBones.RightUpperArm).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
			fpBodyAnim.GetBoneTransform(HumanBodyBones.LeftUpperArm).localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
		}
		else
		{
			Debug.Log("<color=red>Player model avatar error:</color> Please check your player character setup and animator avatar reference.");
		}
		if ((bool)alternateBodyMaterial)
		{
			Material[] materials = fpBodySkinnedMesh.gameObject.GetComponent<Renderer>().materials;
			materials[materialToReplace] = alternateBodyMaterial;
			fpBodySkinnedMesh.gameObject.GetComponent<Renderer>().materials = materials;
		}
		weaponModels.transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand);
		weaponModels.transform.localPosition = Vector3.zero;
		weaponModels.transform.localScale = Vector3.one;
		weaponModels.transform.localEulerAngles = weaponModelParentAngles;
		for (int i = 0; i < weaponModels.thirdPersonWeaponModels.Count; i++)
		{
			WeaponBehavior component = PlayerWeaponsComponent.weaponOrder[i].GetComponent<WeaponBehavior>();
			component.PlayerModelAnim = anim;
			if ((bool)weaponModels.thirdPersonWeaponModels[i].weaponObject)
			{
				component.thirdPersonSmokePos = weaponModels.thirdPersonWeaponModels[i].muzzleSmokePos;
				component.TPmuzzleRenderer = weaponModels.thirdPersonWeaponModels[i].muzzleFlashRenderer;
				component.shellEjectPositionTP = weaponModels.thirdPersonWeaponModels[i].shellEjectPos;
				weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(true);
				if (component.tpUseShotgunAnims)
				{
					component.tpShotgunAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject.GetComponent<Animator>();
				}
				if (component.tpWeaponAnimType == 3)
				{
					component.tpPistolAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject.GetComponent<Animator>();
				}
				weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
			}
			if (weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null)
			{
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(true);
				if (component.tpWeaponAnimType == 6)
				{
					component.tpBowAnim = weaponModels.thirdPersonWeaponModels[i].weaponObject2.GetComponentInChildren<Animator>();
				}
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(false);
				weaponModels.thirdPersonWeaponModels[i].weaponObject2.transform.parent = anim.GetBoneTransform(HumanBodyBones.LeftHand);
			}
			if (weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null)
			{
				weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(false);
				weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.transform.parent = anim.GetBoneTransform(HumanBodyBones.Chest);
			}
		}
	}

	private void Update()
	{
		if (!anim.isInitialized || !fpBodyAnim.isInitialized || !(Time.timeScale > 0f))
		{
			return;
		}
		WeaponBehaviorComponent = FPSPlayerComponent.WeaponBehaviorComponent;
		if (!CameraControlComponent.thirdPersonActive && displayVisibleBody)
		{
			if (!walkerComponent.crouched)
			{
				if (walkerComponent.prone || walkerComponent.swimming)
				{
					rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0f, 100f * Time.smoothDeltaTime);
					modelForwardAmt = Mathf.MoveTowards(modelForwardAmt, modelForwardDown, Time.smoothDeltaTime * 10f);
					modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitch, Time.smoothDeltaTime * 32f);
					modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpFP, Time.smoothDeltaTime * 8f);
					modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);
					modelRightAmt = Mathf.Lerp(modelRightAmt, 0f, Time.smoothDeltaTime * 7f);
				}
				else if (!walkerComponent.sprintActive)
				{
					if (walkerComponent.moving)
					{
						rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, forwardYawAng, 100f * Time.smoothDeltaTime);
					}
					else
					{
						if (WeaponBehaviorComponent.tpWeaponAnimType == 0)
						{
							idleYawAngAmt = idleYawAngUnarmed;
						}
						else
						{
							idleYawAngAmt = idleYawAng;
						}
						if (moveDir == 2)
						{
							rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt + 40f, 200f * Time.smoothDeltaTime);
						}
						else if (moveDir == 3)
						{
							rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt - 20f, 200f * Time.smoothDeltaTime);
						}
						else
						{
							rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, idleYawAngAmt, 100f * Time.smoothDeltaTime);
						}
					}
					modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForward, Time.smoothDeltaTime);
					modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitch, Time.smoothDeltaTime * 32f);
					modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpFP, Time.smoothDeltaTime * 8f);
					modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);
					modelRightAmt = Mathf.Lerp(modelRightAmt, 0f, Time.smoothDeltaTime * 7f);
				}
				else
				{
					rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0f, 100f * Time.smoothDeltaTime);
					if (moveDir == 2)
					{
						modelRightAmt = Mathf.Lerp(modelRightAmt, sprintStrafeRight, Time.smoothDeltaTime * 7f);
					}
					else if (moveDir == 3)
					{
						modelRightAmt = Mathf.Lerp(modelRightAmt, 0f - sprintStrafeRight, Time.smoothDeltaTime * 7f);
					}
					else
					{
						modelRightAmt = Mathf.Lerp(modelRightAmt, 0f, Time.smoothDeltaTime * 7f);
					}
					modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardSprint, Time.smoothDeltaTime);
					modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitchRun, Time.smoothDeltaTime * 32f);
					modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpRun, Time.smoothDeltaTime * 8f);
					modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelStandScale, Time.smoothDeltaTime);
				}
			}
			else
			{
				if (WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving && (moveDir == 2 || moveDir == 3))
				{
					rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, yawAngUnarmedCrouchStrafe, 100f * Time.smoothDeltaTime);
				}
				else
				{
					rotAngleAmt = Mathf.MoveTowards(rotAngleAmt, 0f, 100f * Time.smoothDeltaTime);
				}
				modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardCrouch, Time.smoothDeltaTime);
				modelPitchAmt = Mathf.MoveTowards(modelPitchAmt, modelPitchCrouch, Time.smoothDeltaTime * 32f);
				if (WeaponBehaviorComponent.tpWeaponAnimType == 0)
				{
					modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpCrouchUnarmed, Time.smoothDeltaTime * 8f);
				}
				else
				{
					modelUpAmt = Mathf.MoveTowards(modelUpAmt, modelUpCrouch, Time.smoothDeltaTime * 8f);
				}
				modelScaleAmt = Mathf.MoveTowards(modelScaleAmt, modelCrouchScale, Time.smoothDeltaTime);
				modelRightAmt = Mathf.Lerp(modelRightAmt, 0f, Time.smoothDeltaTime * 7f);
			}
			myTransform.localScale = new Vector3(modelScaleAmt, modelScaleAmt, modelScaleAmt);
			tempSmoothedPos = new Vector3(playerTransform.position.x, playerTransform.position.y - modelUpAmt, playerTransform.position.z);
			tempBodyPosition = Vector3.SmoothDamp(tempBodyPosition, tempSmoothedPos, ref dampvel, modelPosSmoothSpeed, float.PositiveInfinity, Time.smoothDeltaTime);
			myTransform.position = tempBodyPosition + playerTransform.forward * modelForwardAmt + playerTransform.right * modelRightAmt;
			tempBodyAngles = new Vector3(modelPitchAmt, bodyAngAmt + swimFireAngAmt + playerTransform.eulerAngles.y + rotAngleAmt, 0f);
			myTransform.eulerAngles = tempBodyAngles;
			if ((double)tpswitchTime + 0.001 < (double)Time.time || walkerComponent.sprintActive)
			{
				fpBodySkinnedMesh.enabled = true;
				modelPosSmoothSpeed = CameraControlComponent.lerpSpeedAmt;
			}
			else
			{
				modelPosSmoothSpeed = 0.0001f;
			}
		}
		else
		{
			myTransform.localScale = new Vector3(tpMeshScale, tpMeshScale, tpMeshScale);
			tempSmoothedPos = new Vector3(playerTransform.position.x, playerTransform.position.y - modelUp, playerTransform.position.z);
			tempBodyPosition = Vector3.SmoothDamp(tempBodyPosition, tempSmoothedPos, ref dampvel, CameraControlComponent.lerpSpeedAmt, float.PositiveInfinity, Time.smoothDeltaTime);
			myTransform.position = tempBodyPosition;
			tempBodyAngles = new Vector3(0f, bodyAngAmt + swimFireAngAmt + playerTransform.eulerAngles.y, 0f);
			myTransform.localRotation = Quaternion.Slerp(myTransform.rotation, Quaternion.Euler(tempBodyAngles), Time.smoothDeltaTime * slerpSpeedAmt);
			fpBodySkinnedMesh.enabled = false;
		}
		fpBodyTransform.position = myTransform.position;
		fpBodyTransform.rotation = myTransform.rotation;
		fpBodyTransform.localScale = myTransform.localScale;
		if (CameraControlComponent.thirdPersonActive)
		{
			if (displayVisibleBody)
			{
				if (tpModeState)
				{
					SkinnedMeshRenderer[] allSkinnedMeshes = AllSkinnedMeshes;
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in allSkinnedMeshes)
					{
						skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.On;
					}
					MeshRenderer[] allMeshRenderers = AllMeshRenderers;
					foreach (MeshRenderer meshRenderer in allMeshRenderers)
					{
						meshRenderer.shadowCastingMode = ShadowCastingMode.On;
					}
					tpModeState = false;
				}
			}
			else if (tpModeState)
			{
				SkinnedMeshRenderer[] allSkinnedMeshes2 = AllSkinnedMeshes;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in allSkinnedMeshes2)
				{
					skinnedMeshRenderer2.enabled = true;
					skinnedMeshRenderer2.shadowCastingMode = ShadowCastingMode.On;
				}
				MeshRenderer[] allMeshRenderers2 = AllMeshRenderers;
				foreach (MeshRenderer meshRenderer2 in allMeshRenderers2)
				{
					meshRenderer2.enabled = true;
					meshRenderer2.shadowCastingMode = ShadowCastingMode.On;
				}
				tpModeState = false;
			}
		}
		else if (displayVisibleBody)
		{
			if (!tpModeState)
			{
				SkinnedMeshRenderer[] allSkinnedMeshes3 = AllSkinnedMeshes;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer3 in allSkinnedMeshes3)
				{
					skinnedMeshRenderer3.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
				MeshRenderer[] allMeshRenderers3 = AllMeshRenderers;
				foreach (MeshRenderer meshRenderer3 in allMeshRenderers3)
				{
					meshRenderer3.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
				tpModeState = true;
			}
		}
		else if (!tpModeState)
		{
			SkinnedMeshRenderer[] allSkinnedMeshes4 = AllSkinnedMeshes;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer4 in allSkinnedMeshes4)
			{
				skinnedMeshRenderer4.enabled = false;
				skinnedMeshRenderer4.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
			MeshRenderer[] allMeshRenderers4 = AllMeshRenderers;
			foreach (MeshRenderer meshRenderer4 in allMeshRenderers4)
			{
				meshRenderer4.enabled = false;
				meshRenderer4.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
			tpModeState = true;
		}
		moveInputs = new Vector2(walkerComponent.inputX, walkerComponent.inputY);
		moveSmoothed = Vector2.Lerp(moveSmoothed, moveInputs, Time.smoothDeltaTime * inputSmoothingSpeed);
		if (Mathf.Abs(walkerComponent.inputY) > 0.5f)
		{
			anim.SetFloat("StrafeAmt", 0f, 0.4f, Time.deltaTime);
			fpBodyAnim.SetFloat("StrafeAmt", 0f, 0.4f, Time.deltaTime);
		}
		else
		{
			anim.SetFloat("StrafeAmt", Mathf.Abs(walkerComponent.inputX), 0.4f, Time.deltaTime);
			fpBodyAnim.SetFloat("StrafeAmt", Mathf.Abs(walkerComponent.inputX), 0.4f, Time.deltaTime);
		}
		anim.SetFloat("MoveX", walkerComponent.inputX, 0.3f, Time.deltaTime);
		anim.SetFloat("MoveY", walkerComponent.inputY, 0.4f, Time.deltaTime);
		fpBodyAnim.SetFloat("MoveX", walkerComponent.inputX, 0.3f, Time.deltaTime);
		fpBodyAnim.SetFloat("MoveY", walkerComponent.inputY, 0.4f, Time.deltaTime);
		if (walkerComponent.moving)
		{
			moveTime = Time.time;
		}
		else
		{
			idleTime = Time.time;
		}
		if (!walkerComponent.swimming && CameraControlComponent.tpPressTime + 0.15f < Time.time)
		{
			if (CameraControlComponent.thirdPersonActive)
			{
				facingAngleDelta = Mathf.DeltaAngle(myTransform.eulerAngles.y, tempBodyAngles.y);
			}
			else
			{
				facingAngleDelta = Mathf.Lerp(facingAngleDelta, SmoothMouseLookComponent.horizontalDelta, Time.smoothDeltaTime * 10f);
			}
		}
		else
		{
			facingAngleDelta = 0f;
		}
		if (Mathf.Abs(SmoothMouseLookComponent.horizontalDelta) > 0.5f && (FPSPlayerComponent.zoomed || fireTime + fireUpTime * 0.4f > Time.time))
		{
			turnTimer += Time.deltaTime;
		}
		else
		{
			turnTimer = 0f;
		}
		if (turnTimer > 0.6f)
		{
			facingAngleDelta = 16.5f;
		}
		if (!walkerComponent.prone)
		{
			forwardSpeedAmt = Mathf.Max(Mathf.Abs(moveSmoothed.y), Mathf.Abs(moveSmoothed.x));
		}
		else
		{
			forwardSpeedAmt = Mathf.Max(Mathf.Abs(moveSmoothed.y), Mathf.Abs(moveSmoothed.x)) * Mathf.Sign(moveSmoothed.y);
		}
		if (moveInputs.y > InputComponent.deadzone)
		{
			moveDir = 1;
		}
		else if (moveInputs.y < 0f - InputComponent.deadzone)
		{
			moveDir = 4;
		}
		else if (moveInputs.x > InputComponent.deadzone || (meleeMove && WeaponBehaviorComponent.swingSide && !walkerComponent.prone))
		{
			if (meleeMove)
			{
				forwardSpeedAmt = 0.5f;
			}
			moveDir = 3;
		}
		else if (moveInputs.x < 0f - InputComponent.deadzone || (meleeMove && !WeaponBehaviorComponent.swingSide && !walkerComponent.prone))
		{
			if (meleeMove)
			{
				forwardSpeedAmt = -0.4f;
			}
			moveDir = 2;
		}
		else if (moveTime + 0.35f < Time.time)
		{
			if (walkerComponent.swimming && (InputComponent.jumpHold || InputComponent.crouchHold))
			{
				moveDir = 1;
				forwardSpeedAmt = 1f;
			}
			else if ((CameraControlComponent.thirdPersonActive && facingAngleDelta > 16f) || (!CameraControlComponent.thirdPersonActive && facingAngleDelta > 0.4f))
			{
				anim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
				fpBodyAnim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
				moveDir = 3;
				forwardSpeedAmt = 1f;
				if (walkerComponent.prone)
				{
					proneMoveTime = Time.time;
				}
			}
			else if ((CameraControlComponent.thirdPersonActive && facingAngleDelta < -16f) || (!CameraControlComponent.thirdPersonActive && facingAngleDelta < -0.4f))
			{
				anim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
				fpBodyAnim.SetFloat("StrafeAmt", 1f, 0.6f, Time.deltaTime);
				moveDir = 2;
				forwardSpeedAmt = 1f;
				if (walkerComponent.prone)
				{
					proneMoveTime = Time.time;
				}
			}
			else
			{
				moveDir = 0;
			}
		}
		else
		{
			moveDir = 0;
		}
		anim.SetInteger("MoveDir", moveDir);
		fpBodyAnim.SetInteger("MoveDir", moveDir);
		if (walkerComponent.grounded)
		{
			if (walkerComponent.prone)
			{
				if (!proneState)
				{
					proneStanceTime = Time.time;
					proneState = true;
				}
				if (proneStanceTime + proneTransitionLength > Time.time)
				{
					proneTransition = 1;
				}
				else
				{
					proneTransition = 0;
				}
			}
			else
			{
				if (proneState)
				{
					proneStanceTime = Time.time;
					proneState = false;
				}
				if (proneStanceTime + proneTransitionLength > Time.time)
				{
					if (!walkerComponent.crouched)
					{
						proneTransition = 2;
					}
					else
					{
						proneTransition = 3;
					}
				}
				else
				{
					proneTransition = 0;
				}
				if (proneStanceTime + proneTransitionLength * 0.8f < Time.time && !walkerComponent.crouched)
				{
					moveState = 0;
				}
			}
		}
		else
		{
			proneTransition = 0;
		}
		if (proneTransition == 0)
		{
			WeaponBehaviorComponent.disableFiring = false;
		}
		else
		{
			WeaponBehaviorComponent.disableFiring = true;
		}
		anim.SetInteger("ProneTransition", proneTransition);
		fpBodyAnim.SetInteger("ProneTransition", proneTransition);
		if (FPSPlayerComponent.hitPoints > 0f)
		{
			if (proneTransition == 0)
			{
				if (walkerComponent.grounded)
				{
					if (walkerComponent.sprintActive && forwardSpeedAmt > InputComponent.deadzone)
					{
						if (moveInputs.y > -0.5f)
						{
							forwardSpeedAmt = Mathf.Abs(forwardSpeedAmt);
						}
						else
						{
							forwardSpeedAmt = 0f - Mathf.Abs(forwardSpeedAmt);
						}
						if (Mathf.Abs(moveInputs.y) >= InputComponent.deadzone || Mathf.Abs(moveInputs.x) >= InputComponent.deadzone)
						{
							moveState = 1;
							sprintTime = Time.time;
							modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForwardSprint, Time.smoothDeltaTime * 7f);
						}
						fireTime = -16f;
					}
					else
					{
						if (walkerComponent.crouched)
						{
							moveState = 2;
						}
						else if (walkerComponent.swimming)
						{
							if ((!FPSPlayerComponent.zoomed && fireTime + fireUpTime * 0.4f < Time.time && WeaponBehaviorComponent.tpWeaponAnimType != 6) || (WeaponBehaviorComponent.tpWeaponAnimType == 6 && !WeaponBehaviorComponent.pullAnimState))
							{
								moveState = 4;
							}
							else
							{
								moveState = 5;
							}
						}
						else if (walkerComponent.prone)
						{
							moveState = 6;
						}
						else
						{
							moveState = 0;
						}
						modelForwardAmt = Mathf.Lerp(modelForwardAmt, modelForward, Time.smoothDeltaTime * 7f);
					}
				}
				else
				{
					moveState = 3;
				}
			}
		}
		else
		{
			moveState = 7;
		}
		if (moveDir != 0)
		{
			swimBlendAmt = 1f;
		}
		else
		{
			swimBlendAmt = 0f;
		}
		swimBlend = Mathf.Lerp(swimBlend, swimBlendAmt, Time.deltaTime * 2f);
		anim.SetFloat("SwimBlend", swimBlend);
		fpBodyAnim.SetFloat("SwimBlend", swimBlend);
		anim.SetInteger("MoveState", moveState);
		fpBodyAnim.SetInteger("MoveState", moveState);
		if (WeaponBehaviorComponent.tpWeaponAnimType == 0)
		{
			if (moveInputs.y > -0.5f)
			{
				forwardSpeedAmt = Mathf.Abs(forwardSpeedAmt);
			}
			else
			{
				forwardSpeedAmt = 0f - Mathf.Abs(forwardSpeedAmt);
			}
		}
		if (FPSPlayerComponent.zoomed)
		{
			anim.SetFloat("ForwardSpeed", forwardSpeedAmt * 0.85f);
			fpBodyAnim.SetFloat("ForwardSpeed", forwardSpeedAmt * 0.85f);
		}
		else
		{
			anim.SetFloat("ForwardSpeed", forwardSpeedAmt);
			fpBodyAnim.SetFloat("ForwardSpeed", forwardSpeedAmt);
		}
		if (WeaponBehaviorComponent.tpUseShotgunAnims)
		{
			anim.SetBool("ShotgunAnim", true);
		}
		else
		{
			anim.SetBool("ShotgunAnim", false);
		}
		if (WeaponBehaviorComponent.tpOffhandMeleeIsBash)
		{
			anim.SetBool("OffhandMeleeBash", true);
		}
		else
		{
			anim.SetBool("OffhandMeleeBash", false);
		}
		if (((!FPSPlayerComponent.zoomed && fireTime + fireUpTime * fireDelay < Time.time && !WeaponBehaviorComponent.meleeActive) || (walkerComponent.sprintActive && fireTime + fireUpTime * fireDelay < Time.time) || (walkerComponent.swimming && moveState != 5) || (WeaponBehaviorComponent.tpWeaponAnimType == 0 && (walkerComponent.crouched || walkerComponent.moving))) && !PlayerWeaponsComponent.offhandThrowActive && !WeaponBehaviorComponent.pullAnimState)
		{
			if (moveInputs.y >= InputComponent.deadzone)
			{
				if (walkerComponent.sprintActive || walkerComponent.swimming || (WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving))
				{
					bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * 57.29578f;
				}
				else if (moveInputs.y > 0.5f)
				{
					bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * 57.29578f;
				}
				else
				{
					bodyAng = 0f;
				}
			}
			else if (moveInputs.y < -0.5f)
			{
				bodyAng = Mathf.Atan2(0f - moveInputs.x, 0f - moveInputs.y) * 57.29578f;
			}
			else if (walkerComponent.sprintActive || walkerComponent.swimming || (WeaponBehaviorComponent.tpWeaponAnimType == 0 && walkerComponent.moving))
			{
				bodyAng = Mathf.Atan2(moveInputs.x, moveInputs.y) * 57.29578f;
			}
			else
			{
				bodyAng = 0f;
			}
		}
		else
		{
			bodyAng = 0f;
		}
		bodyAngAmt = Mathf.LerpAngle(bodyAngAmt, bodyAng, Time.deltaTime * 5f);
		if (walkerComponent.swimming && ((moveState == 5 && WeaponBehaviorComponent.tpWeaponAnimType != 6) || (WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6)) && WeaponBehaviorComponent.tpWeaponAnimType != 1 && WeaponBehaviorComponent.tpWeaponAnimType != 2 && !WeaponBehaviorComponent.meleeActive)
		{
			swimFireAngAmt = Mathf.MoveTowards(swimFireAngAmt, swimFireAng, Time.deltaTime * 448f);
		}
		else
		{
			swimFireAngAmt = Mathf.MoveTowards(swimFireAngAmt, 0f, Time.deltaTime * 64f);
		}
		if ((Mathf.Abs(Mathf.DeltaAngle(myTransform.eulerAngles.y, tempBodyAngles.y)) > 90f || FPSPlayerComponent.zoomed || fireTime + fireUpTime > Time.time) && idleTime + 0.6f < Time.time)
		{
			slerpSpeedAmt = Mathf.MoveTowards(slerpSpeedAmt, 36f, Time.deltaTime * 128f);
		}
		else
		{
			slerpSpeedAmt = Mathf.MoveTowards(slerpSpeedAmt, slerpSpeed, Time.deltaTime * 64f);
		}
		boneAng1 = Vector3.Angle(playerTransform.forward, myTransform.forward);
		if (Vector3.Cross(playerTransform.forward, myTransform.forward).y < 0f)
		{
			boneAng1 = 0f - boneAng1;
		}
		if (walkerComponent.prone && walkerComponent.moving)
		{
			proneMoveTime = Time.time;
		}
		if (weapDownTime + 0.14f > Time.time || Mathf.Abs(Mathf.DeltaAngle(mainCamTransform.eulerAngles.y, myTransform.eulerAngles.y - swimFireAngAmt)) > 15f || sprintTime + 0.07f > Time.time || proneMoveTime + 0.06f > Time.time)
		{
			WeaponBehaviorComponent.muzzActive = false;
		}
		else
		{
			WeaponBehaviorComponent.muzzActive = true;
		}
		if (idleTime + 0.6f < Time.time && (FPSPlayerComponent.zoomed || WeaponBehaviorComponent.shooting))
		{
			boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, 64f, Time.deltaTime * 120f);
		}
		else if (Mathf.Abs(Mathf.DeltaAngle(mainCamTransform.eulerAngles.y, playerObj.transform.eulerAngles.y)) > 30f)
		{
			boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, boneSpeed, Time.deltaTime * 256f);
		}
		else
		{
			boneSpeedAmt = Mathf.MoveTowards(boneSpeedAmt, 16f, Time.deltaTime * 120f);
		}
		float num3 = ((!walkerComponent.swimming || (!FPSPlayerComponent.zoomed && !(fireTime + fireUpTime > Time.time))) ? 0f : (-35f));
		if ((FPSPlayerComponent.zoomed || fireTime + fireUpTime > Time.time) && WeaponBehaviorComponent.tpWeaponAnimType != 3)
		{
			aimOffsetAmt = aimOffset;
		}
		else
		{
			aimOffsetAmt = 0f;
		}
		if (WeaponBehaviorComponent.tpWeaponAnimType == 6)
		{
			if (WeaponBehaviorComponent.pullAnimState)
			{
				if (weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null && ((WeaponBehaviorComponent.pullTimer + arrowVisibleDelay < Time.time && !walkerComponent.prone) || (WeaponBehaviorComponent.pullTimer + arrowVisibleDelay * 0.3f < Time.time && walkerComponent.prone)))
				{
					weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(true);
				}
			}
			else if (weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null)
			{
				weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(false);
			}
		}
		boneAng1Amt = Mathf.LerpAngle(boneAng1Amt, boneAng1 + num3 + aimOffsetAmt, Time.deltaTime * boneSpeedAmt);
		boneAng1Amt = Mathf.Clamp(boneAng1Amt, -90f, 90f);
		if (WeaponBehaviorComponent.shooting)
		{
			if (proneTransition == 0)
			{
				fireTime = Time.time;
			}
		}
		else if (proneTransition != 0 || (walkerComponent.prone && walkerComponent.moving))
		{
			fireTime = -16f;
		}
		if (FPSPlayerComponent.IronsightsComponent.reloading)
		{
			if (!reloadStartTimeState)
			{
				if ((bool)WeaponBehaviorComponent.tpPistolAnim && WeaponBehaviorComponent.tpPistolAnim.isInitialized)
				{
					WeaponBehaviorComponent.tpPistolAnim.SetTrigger("Reload");
				}
				reloadStartTime = Time.time;
				reloadStartTimeState = true;
			}
		}
		else if (reloadStartTimeState)
		{
			if ((bool)WeaponBehaviorComponent.tpPistolAnim && WeaponBehaviorComponent.tpPistolAnim.isInitialized)
			{
				WeaponBehaviorComponent.tpPistolAnim.SetTrigger("Idle");
			}
			reloadStartTimeState = false;
		}
		if (WeaponBehaviorComponent.tpWeaponAnimType == 5)
		{
			if ((bool)WeaponBehaviorComponent.tpShotgunAnim)
			{
				reloadDurationAmt = reloadDurationShotgun;
			}
			else
			{
				reloadDurationAmt = reloadDurationRifle;
			}
		}
		else if (WeaponBehaviorComponent.tpWeaponAnimType == 3)
		{
			reloadDurationAmt = reloadDurationPistol;
		}
		if (!WeaponBehaviorComponent.meleeActive)
		{
			if (WeaponBehaviorComponent.tpWeaponAnimType == 1 || WeaponBehaviorComponent.tpWeaponAnimType == 2)
			{
				if (WeaponBehaviorComponent.swingSide || walkerComponent.prone)
				{
					fireDelay = WeaponBehaviorComponent.tpSwingTimeL;
				}
				else
				{
					fireDelay = WeaponBehaviorComponent.tpSwingTimeR;
				}
			}
			else
			{
				fireDelay = 1f;
			}
		}
		else
		{
			if (WeaponBehaviorComponent.tpOffhandMeleeIsBash)
			{
				fireDelay = offhandMeleeTime * 0.25f;
			}
			else
			{
				fireDelay = offhandMeleeTime;
			}
			reloadStartTime = -16f;
		}
		if (FPSPlayerComponent.hitPoints > 0f)
		{
			if (reloadStartTime + reloadDurationAmt > Time.time && PlayerWeaponsComponent.switchTime + switchDuration < Time.time && !WeaponBehaviorComponent.meleeActive)
			{
				torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 9f);
				if (CameraControlComponent.thirdPersonActive)
				{
					chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 32f);
					spineAnglesAmt = Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 32f);
				}
				fireTime = -16f;
				anim.SetInteger("ArmsState", 3);
				rotateHead = true;
			}
			else if ((FPSPlayerComponent.zoomed || (fireTime + fireUpTime * fireDelay > Time.time && !walkerComponent.swimming) || (walkerComponent.swimming && fireTime + fireUpTime * 0.5f > Time.time && WeaponBehaviorComponent.tpWeaponAnimType != 6) || WeaponBehaviorComponent.pullAnimState || (!CameraControlComponent.thirdPersonActive && lastWeapon == PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber && !PlayerWeaponsComponent.offhandThrowActive && !walkerComponent.swimming && fpTorsoAlwaysAims) || PlayerWeaponsComponent.offhandThrowActive) && proneTransition == 0 && PlayerWeaponsComponent.switchTime + switchDuration * 1.1f < Time.time)
			{
				if (walkerComponent.prone)
				{
					spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngProne.x, SmoothMouseLookComponent.inputY * mLookAngProne.y, SmoothMouseLookComponent.inputY * mLookAngProne.z);
					chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngProne2.x, SmoothMouseLookComponent.inputY * mLookAngProne2.y, SmoothMouseLookComponent.inputY * mLookAngProne2.z);
				}
				else if (WeaponBehaviorComponent.tpWeaponAnimType == 6 && WeaponBehaviorComponent.pullAnimState && !CameraControlComponent.rotating)
				{
					spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngBow.x, SmoothMouseLookComponent.inputY * mLookAngBow.y - angOffset * 0.6f, SmoothMouseLookComponent.inputY * mLookAngBow.z);
					chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAngBow2.x, SmoothMouseLookComponent.inputY * mLookAngBow2.y - angOffset * 0.4f, SmoothMouseLookComponent.inputY * mLookAngBow2.z);
				}
				else if (!CameraControlComponent.rotating)
				{
					spineAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAng.x, SmoothMouseLookComponent.inputY * mLookAng.y - angOffset * 0.6f, SmoothMouseLookComponent.inputY * mLookAng.z);
					chestAnglesAmt = new Vector3(SmoothMouseLookComponent.inputY * mLookAng2.x, SmoothMouseLookComponent.inputY * mLookAng2.y - angOffset * 0.4f, SmoothMouseLookComponent.inputY * mLookAng2.z);
				}
				if (!walkerComponent.prone || WeaponBehaviorComponent.tpWeaponAnimType != 5)
				{
					torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 9f);
				}
				else if (fireTime + fireUpTime > Time.time && !WeaponBehaviorComponent.meleeActive)
				{
					torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 9f);
				}
				else
				{
					torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 9f);
				}
				if (((WeaponBehaviorComponent.tpWeaponAnimType == 1 || WeaponBehaviorComponent.tpWeaponAnimType == 2 || WeaponBehaviorComponent.meleeActive) && fireTime + fireUpTime * fireDelay > Time.time) || WeaponBehaviorComponent.pullAnimState || PlayerWeaponsComponent.offhandThrowActive)
				{
					if ((walkerComponent.moving && !walkerComponent.swimming) || (WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6))
					{
						if (WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6 && !walkerComponent.swimming)
						{
							if (moveDir == 1 || moveDir == 4)
							{
								angOffset = Mathf.LerpAngle(angOffset, -30f, Time.deltaTime * 7f);
							}
							else if (walkerComponent.crouched)
							{
								angOffset = Mathf.LerpAngle(angOffset, -20f, Time.deltaTime * 7f);
							}
							else
							{
								angOffset = Mathf.LerpAngle(angOffset, 0f, Time.deltaTime * 7f);
							}
						}
						else if (walkerComponent.swimming && WeaponBehaviorComponent.pullAnimState && WeaponBehaviorComponent.tpWeaponAnimType == 6)
						{
							angOffset = Mathf.LerpAngle(angOffset, 10f, Time.deltaTime * 7f);
						}
						else
						{
							angOffset = Mathf.LerpAngle(angOffset, 0f, Time.deltaTime * 7f);
						}
						meleeMove = false;
					}
					else
					{
						angOffset = Mathf.LerpAngle(angOffset, 0f, Time.deltaTime * 7f);
						if (fireTime + 0.0175f > Time.time)
						{
							meleeMove = true;
						}
						else
						{
							meleeMove = false;
						}
					}
					torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 1f, Time.deltaTime * 7f);
				}
				else
				{
					torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0f, Time.deltaTime * 7f);
					angOffset = Mathf.LerpAngle(angOffset, 0f, Time.deltaTime * 9f);
					meleeMove = false;
					if (WeaponBehaviorComponent.meleeActive)
					{
						fireTime = -16f;
					}
				}
				if (fireTime + fireShotTime > Time.time)
				{
					anim.SetInteger("ArmsState", 2);
				}
				else if (WeaponBehaviorComponent.zoomIsBlock && FPSPlayerComponent.zoomed)
				{
					anim.SetInteger("ArmsState", 5);
				}
				else
				{
					anim.SetInteger("ArmsState", 1);
				}
				rotateHead = false;
			}
			else
			{
				chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24f);
				spineAnglesAmt = Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24f);
				if (PlayerWeaponsComponent.switchTime + switchDuration > Time.time)
				{
					anim.SetInteger("ArmsState", 4);
					reloadStartTime = -16f;
					fireTime = -16f;
					torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
					torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0f, Time.deltaTime * 4f);
				}
				else
				{
					if (!walkerComponent.grounded)
					{
						torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
					}
					else if (WeaponBehaviorComponent.tpWeaponAnimType == 5)
					{
						if (walkerComponent.crouched && proneTransition == 0)
						{
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
						}
						else
						{
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 3f);
						}
					}
					else if (walkerComponent.crouched && proneTransition == 0)
					{
						if (WeaponBehaviorComponent.tpWeaponAnimType == 0)
						{
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 3f);
						}
						else
						{
							torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
						}
					}
					else if ((walkerComponent.prone && moveDir != 0) || proneTransition == 1 || (walkerComponent.swimming && moveState != 5))
					{
						torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 3f);
					}
					else if (walkerComponent.prone && WeaponBehaviorComponent.tpWeaponAnimType == 0)
					{
						torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
					}
					else if (WeaponBehaviorComponent.tpWeaponAnimType == 0)
					{
						torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 3f);
					}
					else
					{
						torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 1f, Time.deltaTime * 3f);
					}
					anim.SetInteger("ArmsState", 0);
					SelectCurrentWeapon();
				}
				if (!PlayerWeaponsComponent.offhandThrowActive)
				{
					torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0f, Time.deltaTime * 4f);
					offhandState = false;
				}
				else
				{
					if (!offhandState && lastWeapon != PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber)
					{
						anim.SetTrigger("Pull");
						if (weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject != null)
						{
							weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject.SetActive(false);
						}
						if (!PlayerWeaponsComponent.offhandThrowActive)
						{
							if (weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject2 != null)
							{
								weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObject2.SetActive(false);
							}
							if (weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObjectBack != null)
							{
								weaponModels.thirdPersonWeaponModels[WeaponBehaviorComponent.weaponNumber].weaponObjectBack.SetActive(false);
							}
						}
						offhandState = true;
					}
					torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 1f, Time.deltaTime * 4f);
					lastWeapon = -lastWeapon;
				}
				anim.ResetTrigger("Fire");
				weapDownTime = Time.time;
				fireTime = -16f;
				meleeMove = false;
				angOffset = 0f;
				rotateHead = true;
			}
		}
		else
		{
			rotateHead = false;
			chestAnglesAmt = Vector3.Lerp(chestAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24f);
			spineAnglesAmt = Vector3.Lerp(spineAnglesAmt, Vector3.zero, Time.smoothDeltaTime * 24f);
			torsoLayerWeight2 = Mathf.MoveTowards(torsoLayerWeight2, 0f, Time.deltaTime * 8f);
			torsoLayerWeight = Mathf.MoveTowards(torsoLayerWeight, 0f, Time.deltaTime * 8f);
			if (!deadState)
			{
				for (int num4 = 0; num4 < weaponModels.thirdPersonWeaponModels.Count; num4++)
				{
					if ((bool)weaponModels.thirdPersonWeaponModels[num4].weaponObject)
					{
						weaponModels.thirdPersonWeaponModels[num4].weaponObject.SetActive(false);
					}
					if (weaponModels.thirdPersonWeaponModels[num4].weaponObject2 != null)
					{
						weaponModels.thirdPersonWeaponModels[num4].weaponObject2.SetActive(false);
					}
					if (weaponModels.thirdPersonWeaponModels[num4].weaponObjectBack != null)
					{
						weaponModels.thirdPersonWeaponModels[num4].weaponObjectBack.SetActive(false);
					}
				}
				deadState = true;
			}
		}
		if (!CameraControlComponent.thirdPersonActive)
		{
			CameraControlComponent.constantLooking = true;
		}
		else
		{
			CameraControlComponent.constantLooking = false;
		}
		anim.SetLayerWeight(1, torsoLayerWeight);
		anim.SetLayerWeight(2, torsoLayerWeight2);
		spineAngles.x = Mathf.LerpAngle(spineAngles.x, spineAnglesAmt.x, Time.deltaTime * 12f);
		spineAngles.y = Mathf.LerpAngle(spineAngles.y, spineAnglesAmt.y, Time.deltaTime * 24f);
		spineAngles.z = Mathf.LerpAngle(spineAngles.z, spineAnglesAmt.z, Time.deltaTime * 12f);
		chestAngles.x = Mathf.LerpAngle(chestAngles.x, chestAnglesAmt.x, Time.deltaTime * 12f);
		chestAngles.y = Mathf.LerpAngle(chestAngles.y, chestAnglesAmt.y, Time.deltaTime * 24f);
		chestAngles.z = Mathf.LerpAngle(chestAngles.z, chestAnglesAmt.z, Time.deltaTime * 12f);
	}

	private void LateUpdate()
	{
		if (rotateHead && (bool)headBone && headRotation > 0f)
		{
			if (walkerComponent.crouched)
			{
				headBone.eulerAngles += new Vector3(0f, 0f - boneAng1Amt * headRotationCrouch, 0f);
			}
			else
			{
				headBone.eulerAngles += new Vector3(0f, 0f - boneAng1Amt * headRotation, 0f);
			}
		}
		if ((bool)spineBone)
		{
			spineBone.eulerAngles += spineAngles;
		}
		if ((bool)chestBone)
		{
			chestBone.eulerAngles += chestAngles;
		}
	}

	public void SelectCurrentWeapon()
	{
		if (lastWeapon == PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.weaponNumber)
		{
			return;
		}
		for (int i = 0; i < weaponModels.thirdPersonWeaponModels.Count; i++)
		{
			if (i == WeaponBehaviorComponent.weaponNumber)
			{
				if (weaponModels.thirdPersonWeaponModels[i].weaponObject != null)
				{
					if (WeaponBehaviorComponent.tpWeaponAnimType == 6)
					{
						weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
					}
					else
					{
						weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(true);
					}
				}
				if (weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null)
				{
					weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(true);
				}
				if (weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null)
				{
					weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(true);
				}
				anim.SetInteger("WeaponType", WeaponBehaviorComponent.tpWeaponAnimType);
				fpBodyAnim.SetInteger("WeaponType", WeaponBehaviorComponent.tpWeaponAnimType);
				lastWeapon = WeaponBehaviorComponent.weaponNumber;
				continue;
			}
			if (weaponModels.thirdPersonWeaponModels[i].weaponObject != null)
			{
				weaponModels.thirdPersonWeaponModels[i].weaponObject.SetActive(false);
			}
			if (!PlayerWeaponsComponent.offhandThrowActive)
			{
				if (weaponModels.thirdPersonWeaponModels[i].weaponObject2 != null)
				{
					weaponModels.thirdPersonWeaponModels[i].weaponObject2.SetActive(false);
				}
				if (weaponModels.thirdPersonWeaponModels[i].weaponObjectBack != null)
				{
					weaponModels.thirdPersonWeaponModels[i].weaponObjectBack.SetActive(false);
				}
			}
		}
	}
}
