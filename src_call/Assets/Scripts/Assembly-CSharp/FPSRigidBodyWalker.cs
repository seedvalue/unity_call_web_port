using UnityEngine;

public class FPSRigidBodyWalker : MonoBehaviour
{
	public enum sprintType
	{
		hold = 0,
		toggle = 1,
		both = 2
	}

	private SmoothMouseLook SmoothMouseLookComponent;

	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;

	[HideInInspector]
	public InputControl InputComponent;

	private Footsteps FootstepsComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	[HideInInspector]
	public GameObject mainObj;

	[HideInInspector]
	public GameObject weaponObj;

	[HideInInspector]
	public GameObject CameraObj;

	[HideInInspector]
	public Transform myTransform;

	private Vector3 upVec;

	private Vector3 myUpVec;

	private Transform mainCamTransform;

	[HideInInspector]
	public Animator CameraAnimatorComponent;

	[HideInInspector]
	public CameraControl CameraControlComponent;

	[HideInInspector]
	public Rigidbody RigidbodyComponent;

	[HideInInspector]
	public bool playerParented;

	[HideInInspector]
	public GameObject PlayerCharacterObj;

	[HideInInspector]
	public PlayerCharacter PlayerCharacterComponent;

	[HideInInspector]
	public float inputXSmoothed;

	[HideInInspector]
	public float inputYSmoothed;

	[HideInInspector]
	public float inputX;

	[HideInInspector]
	public float inputY;

	private float InputYLerpSpeed;

	private float InputXLerpSpeed;

	private float InputLerpSpeed = 12f;

	[Header("Movement Speeds", order = 0)]
	[Space(10f, order = 1)]
	[Tooltip("Speed that player moves when walking.")]
	public float walkSpeed = 4f;

	[Tooltip("Speed that player moves when sprinting.")]
	public float sprintSpeed = 9f;

	[Tooltip("Speed that player moves when swimming.")]
	public float swimSpeed = 4f;

	[Tooltip("Vertical speed of player jump.")]
	public float jumpSpeed = 4f;

	[Tooltip("Speed that player moves vertically when climbing.")]
	public float climbSpeed = 4f;

	[HideInInspector]
	public float moveSpeedMult = 1f;

	private float limitStrafeSpeed;

	[Tooltip("Percentage to decrease movement speed when strafing diagonally.")]
	public float diagonalStrafeAmt = 0.7071f;

	[Tooltip("Percentage to decrease movement speed when moving backwards.")]
	public float backwardSpeedPercentage = 0.6f;

	[Tooltip("Percentage to decrease movement speed when crouching.")]
	public float crouchSpeedPercentage = 0.55f;

	private float crouchSpeedAmt = 1f;

	[Tooltip("Percentage to decrease movement speed when prone.")]
	public float proneSpeedPercentage = 0.45f;

	private float proneSpeedAmt = 1f;

	[Tooltip("Percentage to decrease movement speed when strafing directly left or right.")]
	public float strafeSpeedPercentage = 0.8f;

	private float speedAmtY = 1f;

	private float speedAmtX = 1f;

	[HideInInspector]
	public bool zoomSpeed;

	[Tooltip("Percentage to decrease movement speed when zoomed.")]
	public float zoomSpeedPercentage = 0.6f;

	private float zoomSpeedAmt = 1f;

	private float speed;

	[Header("Sprinting", order = 2)]
	[Space(10f, order = 3)]
	[Tooltip("True if player should be allowed to sprint.")]
	public bool allowSprinting = true;

	[Tooltip("User may set sprint mode to toggle, hold, or both (toggle on sprint button press, hold on sprint button hold).")]
	public sprintType sprintMode = sprintType.both;

	private float sprintDelay = 0.4f;

	[Tooltip("True if player should run only while staminaForSprint > 0.")]
	public bool limitedSprint = true;

	[Tooltip("True if player should wait for stamina to fully regenerate before sprinting.")]
	public bool sprintRegenWait = true;

	[Tooltip("Time it takes to fully regenerate stamina if sprintRegenWait is true.")]
	public float sprintRegenTime = 3f;

	private bool breathFxState;

	[Tooltip("Duration allowed for sprinting when limitedSprint is true.")]
	public float staminaForSprint = 5f;

	private float staminaForSprintAmt;

	public bool catchBreathSound = true;

	private bool staminaDepleted;

	[Tooltip("True if player should cancel sprint when strafing directly left or right.")]
	public bool forwardSprintOnly = true;

	[Tooltip("True if player can reload while sprinting.")]
	public bool sprintReload;

	[Tooltip("True if player can Jump while sprinting and not cancel sprint.")]
	public bool jumpCancelsSprint;

	[Header("Movement Options", order = 4)]
	[Space(10f, order = 5)]
	[Tooltip("True if player is allowed to use prone stance.")]
	public bool allowProne = true;

	[Tooltip("True if player should be allowed to jump.")]
	public bool allowJumping = true;

	[Tooltip("True if player should be allowed to crouch.")]
	public bool allowCrouch = true;

	[Tooltip("True if player should be allowed to lean.")]
	public bool allowLeaning = true;

	[Tooltip("Distance left or right the player can lean.")]
	public float leanDistance = 0.75f;

	[Tooltip("Pecentage the player can lean while standing.")]
	public float standLeanAmt = 1f;

	[Tooltip("Pecentage the player can lean while crouching.")]
	public float crouchLeanAmt = 1f;

	private float leanFactorAmt = 1f;

	[HideInInspector]
	public GameObject leanObj;

	[HideInInspector]
	public CapsuleCollider leanCol;

	[HideInInspector]
	public float leanAmt;

	[HideInInspector]
	public float leanPos;

	private float leanVel;

	private Vector3 leanCheckPos;

	private Vector3 leanCheckPos2;

	[HideInInspector]
	public bool leanState;

	[Tooltip("If true, gun will be lowered when climbing surfaces.")]
	public bool lowerGunForClimb = true;

	[Tooltip("If true, gun will be lowered when swimming.")]
	public bool lowerGunForSwim = true;

	[Tooltip("If true, gun will be lowered when holding object.")]
	public bool lowerGunForHold = true;

	[HideInInspector]
	public bool holdingObject;

	[HideInInspector]
	public float dropTime;

	[HideInInspector]
	public bool hideWeapon;

	private bool hideState;

	[Tooltip("If true, jumping animations will be played for weapon model.")]
	public bool jumpingAnims = true;

	[Tooltip("Amount of time before player starts drowning.")]
	public float holdBreathDuration = 15f;

	[Tooltip("Rate of damage to player while drowning.")]
	public float drownDamage = 7f;

	[Tooltip("Angle of ground surface that player won't be allowed to move over.")]
	[Range(0f, 90f)]
	public int slopeLimit = 40;

	[Tooltip("Maximum height of step that will be climbed.")]
	public float maxStepHeight = 0.8f;

	[Tooltip("Amount to move player vertically over stairs for each step.")]
	public float stepHeight = 0.17f;

	[Tooltip("Delay between step detections.")]
	public float stepDelay = 0.1f;

	private float lastStepTime = -64f;

	private float stepDist;

	private float cameraSmoothSpeedAmt;

	private RaycastHit stepHit2;

	private RaycastHit stepHit;

	[Tooltip("True if player capsule friction should be reduced when moving to allow easier climbing up stairs. If false, player will jump over obstacles less, but will need invisible ramp colliders to go up stairs.")]
	public bool lowerMoveFriction = true;

	private bool frictionState;

	[Tooltip("Maximum allowed upward vertical speed of player.")]
	public float verticalSpeedLimit = 9f;

	[HideInInspector]
	public bool grounded;

	[HideInInspector]
	public bool rayTooSteep;

	[HideInInspector]
	public bool capsuleTooSteep;

	[HideInInspector]
	public Vector3 velocity = Vector3.zero;

	[HideInInspector]
	public CapsuleCollider capsule;

	[HideInInspector]
	public SphereCollider sphereCol;

	private Vector3 sweepHeight;

	private bool parentState;

	[HideInInspector]
	public bool inWater;

	[HideInInspector]
	public bool holdingBreath;

	[HideInInspector]
	public bool belowWater;

	[HideInInspector]
	public bool swimming;

	[HideInInspector]
	public bool canWaterJump = true;

	private float swimmingVerticalSpeed;

	[HideInInspector]
	public float swimStartTime;

	[HideInInspector]
	public float diveStartTime;

	[HideInInspector]
	public bool drowning;

	private float drownStartTime;

	private Vector3 sweepBase;

	[HideInInspector]
	public float airTime;

	private bool airTimeState;

	[Tooltip("Number of units the player can fall before taking damage.")]
	public float fallDamageThreshold = 5.5f;

	[Tooltip("Multiplier of unit distance fallen to convert to hitpoint damage for the player.")]
	public float fallDamageMultiplier = 2f;

	private float fallStartLevel;

	[HideInInspector]
	public float fallingDistance;

	private bool falling;

	[HideInInspector]
	public bool climbing;

	[HideInInspector]
	public bool noClimbingSfx;

	[HideInInspector]
	public float verticalSpeedAmt = 4f;

	[Tooltip("Time in seconds allowed between player jumps.")]
	public float antiBunnyHopFactor = 0.35f;

	[HideInInspector]
	public bool jumping;

	private float jumpTimer;

	[HideInInspector]
	public bool jumpfxstate = true;

	[HideInInspector]
	public bool jumpBtn = true;

	[HideInInspector]
	public float landStartTime;

	[HideInInspector]
	public bool canRun = true;

	[HideInInspector]
	public bool sprintActive;

	private bool sprintBtnState;

	private float sprintStartTime;

	private float sprintStart = -2f;

	[HideInInspector]
	public float sprintEnd;

	private bool sprintEndState;

	private bool sprintStartState;

	[HideInInspector]
	public bool cancelSprint;

	[HideInInspector]
	public float sprintStopTime;

	private bool sprintStopState = true;

	[HideInInspector]
	public float midPos = 0.9f;

	[HideInInspector]
	public bool crouched;

	[HideInInspector]
	public bool crouchRisen;

	[HideInInspector]
	public bool crouchState;

	private float lastCrouchTime;

	public float crouchDelay = 0.57f;

	[HideInInspector]
	public bool prone;

	[HideInInspector]
	public bool proneRisen;

	[HideInInspector]
	public bool proneState;

	[HideInInspector]
	public bool proneMove;

	private float lastProneTime;

	public float proneDelay = 0.8f;

	[HideInInspector]
	public bool moving;

	[HideInInspector]
	public float camDampSpeed;

	private float lowpos;

	[HideInInspector]
	public AudioClip landfx;

	[HideInInspector]
	public bool landState;

	[Tooltip("Mask of layers that player will detect collisions with.")]
	public LayerMask clipMask;

	[HideInInspector]
	public PhysicMaterial playerPhysicMaterial;

	private RaycastHit rayHit;

	private RaycastHit capHit;

	private RaycastHit hit2;

	private Vector3 p1;

	private Vector3 p2;

	[Header("Camera and Capsule Collider Heights", order = 6)]
	[Space(10f, order = 7)]
	[Tooltip("Vertical height of player eyes/face (used for player swimming/holding breath detection).")]
	public float eyeHeight = -0.4f;

	[HideInInspector]
	public Vector3 eyePos;

	[Tooltip("Y position/height of camera when standing.")]
	public float standingCamHeight = 0.89f;

	[Tooltip("Y position/height of camera when crouched.")]
	public float crouchingCamHeight = 0.45f;

	[Tooltip("Y position/height of camera when prone.")]
	public float proneCamHeight = -0.2f;

	[Tooltip("Height of player capsule while standing.")]
	public float standingCapsuleheight = 2f;

	[Tooltip("Height of player capsule while crouching.")]
	public float crouchingCapsuleHeight = 1.25f;

	public float capsuleCastHeight = 0.75f;

	private float capsuleCastDist = 0.4f;

	private float rayCastHeight = 2.6f;

	[HideInInspector]
	public float playerHeightMod;

	private float capsuleCheckRadius = 0.5f;

	private Vector3 checkDirection;

	private int maxVelocityChange = 5;

	[HideInInspector]
	public Vector3 moveDirection = Vector3.zero;

	private Vector3 velocityChange = Vector3.zero;

	private Vector3 lastPosition;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(eyePos, 0.1f);
	}

	private void OnEnable()
	{
		RigidbodyComponent = GetComponent<Rigidbody>();
		RigidbodyComponent.freezeRotation = true;
		RigidbodyComponent.useGravity = true;
		capsule = GetComponent<CapsuleCollider>();
		leanCol = leanObj.GetComponent<CapsuleCollider>();
		sphereCol = GetComponent<SphereCollider>();
	}

	private void Start()
	{
		SmoothMouseLookComponent = CameraObj.GetComponent<SmoothMouseLook>();
		FPSPlayerComponent = GetComponent<FPSPlayer>();
		InputComponent = GetComponent<InputControl>();
		FootstepsComponent = CameraObj.GetComponent<Footsteps>();
		CameraControlComponent = Camera.main.transform.GetComponent<CameraControl>();
		CameraAnimatorComponent = Camera.main.GetComponent<Animator>();
		cameraSmoothSpeedAmt = CameraControlComponent.camSmoothSpeed;
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		upVec = Vector3.up;
		myUpVec = myTransform.up;
		Mathf.Clamp01(backwardSpeedPercentage);
		Mathf.Clamp01(crouchSpeedPercentage);
		Mathf.Clamp01(proneSpeedPercentage);
		Mathf.Clamp01(strafeSpeedPercentage);
		Mathf.Clamp01(zoomSpeedPercentage);
		staminaForSprintAmt = staminaForSprint;
		moveSpeedMult = 1f;
		capsule.height = standingCapsuleheight;
		playerHeightMod = standingCapsuleheight - 2.24f;
		capsuleCheckRadius = capsule.radius * 0.9f;
		leanCol.radius = capsuleCheckRadius;
		capsuleCastHeight = capsule.height * 0.45f;
		rayCastHeight = capsule.height * 1.3f;
		jumpSpeed /= 1f - playerHeightMod / capsule.height;
		lowpos = myTransform.position.y;
		playerPhysicMaterial = capsule.material;
		switch (sprintMode)
		{
		case sprintType.both:
			sprintDelay = 0.4f;
			break;
		case sprintType.hold:
			sprintDelay = 0f;
			break;
		case sprintType.toggle:
			sprintDelay = 999f;
			break;
		}
	}

	private void Update()
	{
		capsuleCheckRadius = capsule.radius * 0.9f;
		Vector3 vector = myTransform.position - upVec * (myTransform.position.y - 0.1f - (capsule.bounds.min.y + capsuleCheckRadius));
		Vector3 end = vector + upVec * (standingCapsuleheight - 0.1f - capsuleCheckRadius * 2f);
		Vector3 end2 = vector + upVec * (crouchingCapsuleHeight - 0.1f - capsuleCheckRadius * 2f);
		eyePos = myTransform.position - myUpVec * (capsule.bounds.min.y - myTransform.position.y) + myUpVec * eyeHeight;
		Vector3 localEulerAngles = new Vector3(0f, CameraObj.transform.localEulerAngles.y, 0f);
		myTransform.localEulerAngles = localEulerAngles;
		Vector3 eulerAngles = new Vector3(0f, CameraObj.transform.eulerAngles.y, 0f);
		myTransform.eulerAngles = eulerAngles;
		if (allowCrouch)
		{
			if (InputComponent.crouchHold && !swimming && !climbing)
			{
				if (!crouchState && lastCrouchTime + crouchDelay < Time.time)
				{
					if (!crouched && crouchRisen)
					{
						if (prone)
						{
							if (!Physics.CheckCapsule(vector, end2, capsuleCheckRadius, clipMask.value, QueryTriggerInteraction.Ignore))
							{
								camDampSpeed = 0.15f;
								crouchRisen = false;
								crouched = true;
								prone = false;
							}
						}
						else
						{
							camDampSpeed = 0.1f;
							crouched = true;
						}
						lastCrouchTime = Time.time;
						lastProneTime = Time.time;
						sprintActive = false;
					}
					else if (!Physics.CheckCapsule(myTransform.position + myUpVec * 0.75f, end, capsuleCheckRadius * 0.9f, clipMask.value, QueryTriggerInteraction.Ignore))
					{
						camDampSpeed = 0.2f;
						crouched = false;
						lastCrouchTime = Time.time;
					}
					crouchState = true;
				}
			}
			else
			{
				crouchState = false;
				if ((sprintActive || climbing || swimming) && !Physics.CheckCapsule(myTransform.position + myUpVec * 0.75f, end, capsuleCheckRadius * 0.9f, clipMask.value, QueryTriggerInteraction.Ignore))
				{
					camDampSpeed = 0.2f;
					crouched = false;
				}
			}
		}
		if (allowProne)
		{
			if (InputComponent.proneHold && !swimming && !climbing && grounded)
			{
				if (!proneState && lastProneTime + proneDelay < Time.time)
				{
					if (!prone)
					{
						camDampSpeed = 0.1f;
						prone = true;
						crouched = false;
						sprintActive = false;
					}
					else if (!Physics.CheckCapsule(vector, end, capsuleCheckRadius * 0.9f, clipMask.value, QueryTriggerInteraction.Ignore))
					{
						camDampSpeed = 0.2f;
						prone = false;
					}
					lastProneTime = Time.time;
					lastCrouchTime = Time.time;
					crouchRisen = false;
					proneRisen = false;
					proneState = true;
				}
			}
			else
			{
				proneState = false;
				if ((sprintActive || climbing || swimming) && !Physics.CheckCapsule(vector, end, capsuleCheckRadius * 0.9f, clipMask.value, QueryTriggerInteraction.Ignore))
				{
					camDampSpeed = 0.2f;
					prone = false;
				}
			}
			if (InputComponent.jumpHold && (crouched || prone) && !Physics.CheckCapsule(vector, end, capsuleCheckRadius * 0.9f, clipMask.value, QueryTriggerInteraction.Ignore))
			{
				if (crouched)
				{
					camDampSpeed = 0.2f;
				}
				else
				{
					camDampSpeed = 0.2f;
				}
				crouched = false;
				prone = false;
				lastProneTime = Time.time;
				lastCrouchTime = Time.time;
				landStartTime = Time.time;
			}
			if (mainCamTransform.position.y - myTransform.position.y > standingCamHeight * 0.95f)
			{
				camDampSpeed = 0.1f;
				if (!proneRisen && !prone)
				{
					proneRisen = true;
				}
			}
			if (moving && prone)
			{
				proneMove = true;
			}
			else
			{
				proneMove = false;
			}
		}
		if (mainCamTransform.position.y - myTransform.position.y > crouchingCamHeight * 0.5f && !crouchRisen)
		{
			camDampSpeed = 0.1f;
			crouchRisen = true;
		}
		if (Mathf.Abs(inputY) > 0.15f || Mathf.Abs(inputX) > 0.15f)
		{
			moving = true;
			if (lowerMoveFriction && frictionState)
			{
				capsule.material.frictionCombine = PhysicMaterialCombine.Minimum;
				capsule.material.dynamicFriction = 0.01f;
				capsule.material.staticFriction = 0.01f;
				capsule.material = capsule.material;
				frictionState = false;
			}
		}
		else
		{
			moving = false;
			if (lowerMoveFriction && !frictionState)
			{
				capsule.material.frictionCombine = PhysicMaterialCombine.Maximum;
				capsule.material.dynamicFriction = 1f;
				capsule.material.staticFriction = 1f;
				capsule.material = capsule.material;
				frictionState = true;
			}
		}
		if (grounded)
		{
			if (jumping)
			{
				if (jumpTimer + 0.1f < Time.time)
				{
					if (!inWater && (bool)landfx && !landState)
					{
						PlayAudioAtPos.PlayClipAt(landfx, mainCamTransform.position, 1f, 0f);
						FPSPlayerComponent.IronsightsComponent.WeaponPivotComponent.PlayLandAnim();
						landState = true;
					}
					CameraAnimatorComponent.speed = 1f;
					CameraAnimatorComponent.SetTrigger("CamLand");
					jumpfxstate = true;
					jumpBtn = false;
					jumping = false;
				}
			}
			else
			{
				if (!allowJumping)
				{
					return;
				}
				if (InputComponent.jumpPress && ((!FPSPlayerComponent.zoomed && !WeaponBehaviorComponent.canJumpZoom) || WeaponBehaviorComponent.canJumpZoom) && jumpBtn && !crouched && !prone && !belowWater && canWaterJump && !climbing && landStartTime + antiBunnyHopFactor < Time.time && (!rayTooSteep || inWater))
				{
					jumpTimer = Time.time;
					if (jumpfxstate)
					{
						PlayAudioAtPos.PlayClipAt(FPSPlayerComponent.jumpfx, mainCamTransform.position, 1f, 0f);
						jumpfxstate = false;
					}
					FPSPlayerComponent.IronsightsComponent.WeaponPivotComponent.PlayJumpAnim();
					CameraAnimatorComponent.speed = 1f;
					CameraAnimatorComponent.SetTrigger("CamJump");
					RigidbodyComponent.AddForce(new Vector3(0f, Mathf.Sqrt(2f * jumpSpeed * (0f - Physics.gravity.y)), 0f), ForceMode.VelocityChange);
					jumping = true;
				}
				if (!InputComponent.jumpHold && landStartTime + antiBunnyHopFactor < Time.time)
				{
					jumpBtn = true;
				}
			}
		}
		else if (airTime + 0.2f < Time.time)
		{
			landState = false;
		}
	}

	private void FixedUpdate()
	{
		if (!(Time.timeScale > 0f))
		{
			return;
		}
		WeaponBehaviorComponent = FPSPlayerComponent.WeaponBehaviorComponent;
		if (!prone)
		{
			p1 = myTransform.position;
			p2 = p1 + myUpVec * (capsule.height * 0.45f);
			if (!crouched)
			{
				capsuleCastDist = 0.5f + playerHeightMod * 0.2f;
				sweepBase = myTransform.position - upVec * (capsuleCheckRadius * 0.3f);
				sweepHeight = myTransform.position + upVec * capsuleCheckRadius * 0.75f;
			}
			else
			{
				capsuleCastDist = 0.5f + playerHeightMod * 0.2f;
				sweepBase = myTransform.position - upVec * (capsuleCheckRadius * 0.3f);
				sweepHeight = myTransform.position + upVec * capsuleCheckRadius * 0.5f;
			}
		}
		else
		{
			p1 = myTransform.position;
			p2 = p1 + upVec * (capsule.height * 0.4f);
			capsuleCastDist = capsuleCheckRadius * 0.25f;
			sweepBase = myTransform.position;
			sweepHeight = myTransform.position + upVec * capsuleCheckRadius * 0.25f;
		}
		velocity = RigidbodyComponent.velocity;
		if (FPSPlayerComponent.hitPoints > 1f)
		{
			checkDirection = new Vector3(InputComponent.moveX * limitStrafeSpeed, 0f, InputComponent.moveY * limitStrafeSpeed);
			checkDirection = myTransform.TransformDirection(checkDirection);
			if (!Physics.CapsuleCast(sweepBase, sweepHeight, capsuleCheckRadius * 0.45f, checkDirection, out hit2, capsuleCastDist, clipMask.value, QueryTriggerInteraction.Ignore) || climbing)
			{
				if (sprintActive)
				{
					if (forwardSprintOnly)
					{
						inputY = 1f;
					}
					else if (Mathf.Abs(InputComponent.moveY) > InputComponent.deadzone)
					{
						inputY = Mathf.Lerp(inputY, Mathf.Sign(InputComponent.moveY), Time.deltaTime * InputYLerpSpeed);
					}
					else
					{
						inputY = 0f;
					}
				}
				else if (prone && SmoothMouseLookComponent.inputY > 25f)
				{
					inputY = 0f;
					inputYSmoothed = 0f;
				}
				else if (Mathf.Abs(InputComponent.moveY) > InputComponent.deadzone)
				{
					inputY = InputComponent.moveY;
				}
				else
				{
					inputY = 0f;
				}
				if (!swimming)
				{
					InputYLerpSpeed = InputLerpSpeed;
				}
				else
				{
					InputYLerpSpeed = 3f;
				}
			}
			else if (((!hit2.rigidbody && grounded) || airTime + 0.5f > Time.time) && !hit2.collider.isTrigger)
			{
				if (InputComponent.moveY < 0f - InputComponent.deadzone)
				{
					if (prone && SmoothMouseLookComponent.inputY > 25f)
					{
						inputY = 0f;
					}
					else
					{
						inputY = InputComponent.moveY;
					}
				}
				else
				{
					inputY = 0f;
				}
				InputYLerpSpeed = 128f;
			}
			else if (prone && SmoothMouseLookComponent.inputY > 25f)
			{
				inputY = 0f;
			}
			else if (Mathf.Abs(InputComponent.moveY) > InputComponent.deadzone)
			{
				inputY = InputComponent.moveY;
			}
			else
			{
				inputY = 0f;
			}
			if (!swimming)
			{
				InputXLerpSpeed = InputLerpSpeed;
			}
			else
			{
				InputXLerpSpeed = 3f;
			}
			if (sprintActive)
			{
				if (Mathf.Abs(InputComponent.moveX) > 0.4f)
				{
					inputX = Mathf.Sign(InputComponent.moveX);
				}
				else
				{
					inputX = 0f;
				}
			}
			else if (prone && SmoothMouseLookComponent.inputY > 25f)
			{
				inputX = 0f;
				inputXSmoothed = 0f;
			}
			else
			{
				inputX = InputComponent.moveX;
			}
			if (FPSPlayerComponent.restarting)
			{
				inputX = 0f;
				inputY = 0f;
			}
			inputXSmoothed = Mathf.Lerp(inputXSmoothed, inputX, Time.deltaTime * InputXLerpSpeed);
			inputYSmoothed = Mathf.Lerp(inputYSmoothed, inputY, Time.deltaTime * InputYLerpSpeed);
		}
		if ((holdingBreath && lowerGunForSwim) || (climbing && lowerGunForClimb) || (holdingObject && lowerGunForHold))
		{
			hideWeapon = true;
			if (!hideState)
			{
				FPSPlayerComponent.PlayerWeaponsComponent.pullGrenadeState = false;
				FPSPlayerComponent.PlayerWeaponsComponent.offhandThrowActive = false;
				FPSPlayerComponent.PlayerWeaponsComponent.grenadeThrownState = false;
				WeaponBehaviorComponent.CancelWeaponPull(true);
				hideState = true;
			}
		}
		else
		{
			if (hideState)
			{
				hideState = false;
			}
			hideWeapon = false;
		}
		if (grounded)
		{
			stepDist = capsule.radius * 1.6f;
			Vector3 origin = myTransform.position - upVec * (myTransform.position.y - capsule.bounds.min.y - capsule.radius * 0.7f);
			Vector3 vector = myTransform.position - upVec * (myTransform.position.y - capsule.bounds.min.y - maxStepHeight);
			if (!crouched)
			{
				if (Physics.Raycast(origin, checkDirection.normalized, out stepHit, stepDist, clipMask.value, QueryTriggerInteraction.Ignore) && lastStepTime + stepDelay < Time.time && !stepHit.rigidbody && !Physics.Raycast(vector, checkDirection.normalized, stepDist, clipMask.value, QueryTriggerInteraction.Ignore) && Vector3.Angle(stepHit.normal, myUpVec) > 85f)
				{
					Physics.Raycast(vector + checkDirection.normalized * stepDist, -myUpVec, out stepHit2, maxStepHeight, clipMask.value, QueryTriggerInteraction.Ignore);
					float num = maxStepHeight - stepHit2.distance;
					if (num > capsule.radius * 0.9f)
					{
						Vector3 position = myTransform.position;
						position += upVec * ((maxStepHeight - stepHit2.distance) * 0.5f);
						myTransform.position = position;
						lastStepTime = Time.time;
					}
				}
			}
			else
			{
				lastStepTime = Time.time - 0.4f;
			}
			if (lastStepTime + 0.3f < Time.time)
			{
				FPSPlayerComponent.CameraControlComponent.camSmoothSpeed = cameraSmoothSpeedAmt;
			}
			else
			{
				FPSPlayerComponent.CameraControlComponent.camSmoothSpeed = 0.12f;
			}
			airTimeState = true;
			if (falling)
			{
				fallingDistance = 0f;
				landStartTime = Time.time;
				if (fallStartLevel - myTransform.position.y > 2f)
				{
					if (!inWater && (bool)landfx && !landState)
					{
						PlayAudioAtPos.PlayClipAt(landfx, mainCamTransform.position, 1f, 0f);
						FPSPlayerComponent.IronsightsComponent.WeaponPivotComponent.PlayLandAnim();
						landState = true;
					}
					CameraAnimatorComponent.speed = 1f;
					CameraAnimatorComponent.SetTrigger("CamLand");
				}
				if (myTransform.position.y < fallStartLevel - fallDamageThreshold && !inWater)
				{
					CalculateFallingDamage(fallStartLevel - myTransform.position.y);
				}
				falling = false;
			}
			if (allowSprinting)
			{
				if ((Mathf.Abs(inputY) > 0f && forwardSprintOnly) || (!forwardSprintOnly && Mathf.Abs(inputX) > 0f) || Mathf.Abs(inputY) > 0f)
				{
					if (InputComponent.sprintHold)
					{
						if (!sprintStartState)
						{
							sprintStart = Time.time;
							sprintStartState = true;
							sprintEndState = false;
							if (sprintEnd - sprintStart < sprintDelay * Time.timeScale)
							{
								if (!sprintActive)
								{
									if (!sprintActive)
									{
										sprintActive = true;
									}
									else
									{
										sprintActive = false;
									}
								}
								else
								{
									sprintActive = false;
								}
							}
						}
					}
					else if (!sprintEndState)
					{
						sprintEnd = Time.time;
						sprintEndState = true;
						sprintStartState = false;
						if (sprintEnd - sprintStart > sprintDelay * Time.timeScale)
						{
							sprintActive = false;
						}
					}
				}
				else if (!InputComponent.sprintHold)
				{
					sprintActive = false;
				}
			}
			if ((sprintActive && InputComponent.fireHold) || (sprintActive && InputComponent.zoomHold && WeaponBehaviorComponent.canZoom) || (FPSPlayerComponent.zoomed && InputComponent.fireHold) || (inputY <= 0f && Mathf.Abs(inputX) > 0f && forwardSprintOnly) || (InputComponent.moveY <= 0f && forwardSprintOnly) || (inputY < 0f && forwardSprintOnly) || (jumping && jumpCancelsSprint) || swimming || climbing || WeaponBehaviorComponent.meleeActive || (limitedSprint && staminaForSprintAmt <= 0f))
			{
				cancelSprint = true;
				sprintActive = false;
			}
			if (sprintActive && InputComponent.reloadPress && !sprintReload && WeaponBehaviorComponent.CheckForReload())
			{
				cancelSprint = true;
				sprintActive = false;
				WeaponBehaviorComponent.CheckForReload();
			}
			if (limitedSprint && staminaForSprintAmt <= 0f && !breathFxState && (bool)FPSPlayerComponent.catchBreath && catchBreathSound)
			{
				PlayAudioAtPos.PlayClipAt(FPSPlayerComponent.catchBreath, mainCamTransform.position, 1f, 0f);
				breathFxState = true;
			}
			if (!sprintActive && cancelSprint && !InputComponent.zoomHold)
			{
				cancelSprint = false;
			}
			if (limitedSprint && staminaForSprintAmt <= 0f)
			{
				staminaDepleted = true;
			}
			if (((inputY > 0f && forwardSprintOnly) || (moving && !forwardSprintOnly)) && sprintActive && !crouched && !prone && !cancelSprint && grounded)
			{
				canRun = true;
				FPSPlayerComponent.zoomed = false;
				sprintStopState = true;
				if (staminaForSprintAmt > 0f && limitedSprint)
				{
					staminaForSprintAmt -= Time.deltaTime;
				}
			}
			else
			{
				if (sprintStopState)
				{
					sprintStopTime = Time.time;
					sprintStopState = false;
				}
				canRun = false;
				if (limitedSprint)
				{
					if (sprintRegenWait)
					{
						if (!staminaDepleted)
						{
							if (staminaForSprintAmt < staminaForSprint)
							{
								staminaForSprintAmt += Time.deltaTime;
							}
						}
						else if (sprintStopTime + sprintRegenTime < Time.time)
						{
							staminaForSprintAmt = staminaForSprint;
							staminaDepleted = false;
							breathFxState = false;
						}
					}
					else if (staminaForSprintAmt < staminaForSprint)
					{
						staminaForSprintAmt += Time.deltaTime;
						breathFxState = false;
					}
				}
			}
			if (canRun)
			{
				if (speed < sprintSpeed - 0.1f)
				{
					speed += 12f * Time.deltaTime;
				}
				else if (speed > sprintSpeed + 0.1f)
				{
					speed -= 12f * Time.deltaTime;
				}
			}
			else if (!swimming)
			{
				if (speed > walkSpeed * moveSpeedMult + 0.1f)
				{
					speed -= 16f * Time.deltaTime;
				}
				else if (speed < walkSpeed * moveSpeedMult - 0.1f)
				{
					speed += 16f * Time.deltaTime;
				}
			}
			else if (speed > swimSpeed + 0.1f)
			{
				speed -= 16f * Time.deltaTime;
			}
			else if (speed < swimSpeed - 0.1f)
			{
				speed += 16f * Time.deltaTime;
			}
			if (zoomSpeed)
			{
				if (zoomSpeedAmt > zoomSpeedPercentage)
				{
					zoomSpeedAmt -= Time.deltaTime;
				}
			}
			else if (zoomSpeedAmt < 1f)
			{
				zoomSpeedAmt += Time.deltaTime;
			}
			if (crouched || midPos < standingCamHeight)
			{
				if (crouchSpeedAmt > crouchSpeedPercentage)
				{
					crouchSpeedAmt -= Time.deltaTime;
				}
			}
			else if (crouchSpeedAmt < 1f)
			{
				crouchSpeedAmt += Time.deltaTime;
			}
			if (prone && midPos < standingCamHeight)
			{
				if (proneSpeedAmt > proneSpeedPercentage)
				{
					proneSpeedAmt -= Time.deltaTime;
				}
			}
			else if (proneSpeedAmt < 1f)
			{
				proneSpeedAmt += Time.deltaTime;
			}
			if (inputY >= 0f)
			{
				if (speedAmtY < 1f)
				{
					speedAmtY += Time.deltaTime;
				}
			}
			else if (speedAmtY > backwardSpeedPercentage)
			{
				speedAmtY -= Time.deltaTime;
			}
			if (inputX == 0f || inputY != 0f)
			{
				if (speedAmtX < 1f)
				{
					speedAmtX += Time.deltaTime;
				}
			}
			else if (speedAmtX > strafeSpeedPercentage)
			{
				speedAmtX -= Time.deltaTime;
			}
			float num2 = playerHeightMod / 2.24f + 1f;
			if (Time.timeSinceLevelLoad > 0.5f)
			{
				if (prone || crouched || FPSPlayerComponent.hitPoints < 1f)
				{
					if (crouched && !prone)
					{
						if (FPSPlayerComponent.hitPoints < 1f)
						{
							midPos = proneCamHeight;
							capsule.height = crouchingCapsuleHeight / 1.15f;
						}
						else
						{
							midPos = crouchingCamHeight;
							capsule.height = Mathf.Max(capsule.height - 4f * (Time.deltaTime * num2), crouchingCapsuleHeight);
						}
					}
					else if (prone || FPSPlayerComponent.hitPoints < 1f)
					{
						midPos = proneCamHeight;
						capsule.height = crouchingCapsuleHeight / 1.15f;
					}
				}
				else
				{
					midPos = standingCamHeight;
					if (!CameraControlComponent.thirdPersonActive)
					{
						capsule.height = Mathf.Min(capsule.height + 4f * (Time.deltaTime * (0.9f * num2)), standingCapsuleheight);
					}
					else
					{
						capsule.height = Mathf.Min(capsule.height + 4f * (Time.deltaTime * (2f * num2)), standingCapsuleheight);
					}
				}
			}
			if (allowLeaning)
			{
				if (leanDistance > 0f)
				{
					if (Time.timeSinceLevelLoad > 0.5f && FPSPlayerComponent.hitPoints > 0f && !sprintActive && !prone && grounded)
					{
						if (InputComponent.leanLeftHold && !InputComponent.leanRightHold)
						{
							if (!crouched)
							{
								leanCheckPos = myTransform.position + upVec * capsuleCheckRadius;
								leanFactorAmt = standLeanAmt;
							}
							else
							{
								leanCheckPos = myTransform.position;
								leanFactorAmt = crouchLeanAmt;
							}
							leanCheckPos2 = myTransform.position + upVec * (capsule.height / 2f - capsuleCheckRadius);
							if (!Physics.CapsuleCast(leanCheckPos, leanCheckPos2, capsuleCheckRadius * 0.8f, -myTransform.right, out capHit, leanDistance * leanFactorAmt, clipMask.value))
							{
								leanAmt = (0f - leanDistance) * leanFactorAmt;
							}
							else
							{
								leanAmt = 0f;
							}
						}
						else if (InputComponent.leanRightHold && !InputComponent.leanLeftHold)
						{
							if (!crouched)
							{
								leanCheckPos = myTransform.position + upVec * capsuleCheckRadius;
								leanFactorAmt = standLeanAmt;
							}
							else
							{
								leanCheckPos = myTransform.position;
								leanFactorAmt = crouchLeanAmt;
							}
							leanCheckPos2 = myTransform.position + upVec * (capsule.height / 2f - capsuleCheckRadius);
							if (!Physics.CapsuleCast(leanCheckPos, leanCheckPos2, capsuleCheckRadius * 0.8f, myTransform.right, out capHit, leanDistance * leanFactorAmt, clipMask.value))
							{
								leanAmt = leanDistance * leanFactorAmt;
							}
							else
							{
								leanAmt = 0f;
							}
						}
						else
						{
							leanAmt = 0f;
						}
					}
					else
					{
						leanAmt = 0f;
					}
					if (sphereCol.enabled)
					{
						Vector3 center = new Vector3(leanAmt, sphereCol.center.y, sphereCol.center.z);
						sphereCol.center = center;
					}
					leanPos = Mathf.SmoothDamp(leanPos, leanAmt, ref leanVel, 0.1f, float.PositiveInfinity, Time.deltaTime);
					if (Mathf.Abs(leanAmt) > 0.1f)
					{
						leanCol.enabled = true;
					}
					else
					{
						leanCol.enabled = false;
					}
					if (crouched)
					{
						leanCol.height = 0.75f;
					}
					else
					{
						leanCol.height = 1.5f;
					}
				}
			}
			else
			{
				leanCol.enabled = false;
				leanAmt = 0f;
			}
		}
		else
		{
			if (airTimeState)
			{
				airTime = Time.time;
				airTimeState = false;
			}
			fallingDistance = fallStartLevel - myTransform.position.y;
			if (!falling)
			{
				falling = true;
				fallStartLevel = myTransform.position.y;
			}
		}
		if ((climbing || swimming) && !jumping)
		{
			if (InputComponent.moveY > 0.1f)
			{
				if (climbing)
				{
					verticalSpeedAmt = 0.2f + climbSpeed / 4f * (SmoothMouseLookComponent.inputY / 48f);
					verticalSpeedAmt = Mathf.Clamp(verticalSpeedAmt, 0f - climbSpeed, climbSpeed);
				}
				else if (swimming)
				{
					verticalSpeedAmt = Mathf.Clamp(verticalSpeedAmt, 0f - swimSpeed, swimSpeed);
					if (!belowWater)
					{
						if (SmoothMouseLookComponent.inputY < -55f)
						{
							verticalSpeedAmt = 1f + swimSpeed * (0f - Mathf.Abs(SmoothMouseLookComponent.inputY / 48f));
						}
						else
						{
							verticalSpeedAmt = 0f;
						}
					}
					else if (SmoothMouseLookComponent.inputY > -15f)
					{
						verticalSpeedAmt = 1f + swimSpeed * (SmoothMouseLookComponent.inputY / 48f);
					}
					else if (!Physics.CapsuleCast(p1, p2, capsuleCheckRadius * 0.9f, -myUpVec, out capHit, capsuleCastHeight, clipMask.value))
					{
						verticalSpeedAmt = 1f + swimSpeed * (SmoothMouseLookComponent.inputY / 48f);
					}
					else
					{
						verticalSpeedAmt = 0f;
					}
				}
				RigidbodyComponent.velocity = new Vector3(velocity.x, verticalSpeedAmt, velocity.z);
			}
			else
			{
				RigidbodyComponent.velocity = new Vector3(velocity.x, 0f, velocity.z);
			}
		}
		if (!FPSPlayerComponent.invulnerable)
		{
			if (holdingBreath)
			{
				if (Time.time - diveStartTime > holdBreathDuration / 1.5f)
				{
					drowning = true;
				}
				if (Time.time - diveStartTime > holdBreathDuration && drownStartTime < Time.time)
				{
					FPSPlayerComponent.ApplyDamage(drownDamage);
					drownStartTime = Time.time + 1.75f;
				}
			}
			else if (drowning)
			{
				PlayAudioAtPos.PlayClipAt(FPSPlayerComponent.gasp, mainCamTransform.position, 0.75f, 0f);
				drowning = false;
			}
		}
		if (Physics.CapsuleCast(p1, p1 + myUpVec * (capsule.radius * 0.8f), capsuleCheckRadius * 0.9f, -myUpVec, out capHit, capsuleCastHeight * 1.2f, clipMask.value) || climbing || swimming)
		{
			grounded = true;
			if (!climbing)
			{
				if (!inWater && !swimming)
				{
					switch (capHit.collider.gameObject.tag)
					{
					case "Water":
						if ((bool)FootstepsComponent.waterLand)
						{
							landfx = FootstepsComponent.waterLand;
						}
						break;
					case "Dirt":
						if ((bool)FootstepsComponent.dirtLand)
						{
							landfx = FootstepsComponent.dirtLand;
						}
						break;
					case "Wood":
						if ((bool)FootstepsComponent.woodLand)
						{
							landfx = FootstepsComponent.woodLand;
						}
						break;
					case "Metal":
						if ((bool)FootstepsComponent.metalLand)
						{
							landfx = FootstepsComponent.metalLand;
						}
						break;
					case "Stone":
						if ((bool)FootstepsComponent.stoneLand)
						{
							landfx = FootstepsComponent.stoneLand;
						}
						break;
					default:
						if ((bool)FootstepsComponent.dirtLand)
						{
							landfx = FootstepsComponent.dirtLand;
						}
						break;
					}
				}
			}
			else if ((bool)FootstepsComponent.dirtLand)
			{
				landfx = FootstepsComponent.dirtLand;
			}
		}
		else if (lastStepTime + 0.3f < Time.time)
		{
			grounded = false;
		}
		if (Physics.Raycast(myTransform.position, -myUpVec, out rayHit, rayCastHeight, clipMask.value))
		{
			if (Vector3.Angle(rayHit.normal, upVec) > (float)slopeLimit && !inWater)
			{
				RaycastHit hitInfo;
				Physics.Raycast(myTransform.position + moveDirection.normalized * capsuleCheckRadius, -myUpVec, out hitInfo, rayCastHeight, clipMask.value);
				if (hitInfo.point.y - rayHit.point.y >= 0f)
				{
					rayTooSteep = true;
				}
				else
				{
					rayTooSteep = false;
				}
			}
			else
			{
				rayTooSteep = false;
			}
			if (!Terrain.activeTerrain || ((bool)Terrain.activeTerrain && rayHit.collider.gameObject != Terrain.activeTerrain.gameObject))
			{
				FootstepsComponent.materialType = rayHit.collider.gameObject.tag;
				FootstepsComponent.onTerrain = false;
			}
			else
			{
				FootstepsComponent.onTerrain = true;
			}
		}
		limitStrafeSpeed = ((!(Mathf.Abs(inputX) > 0.5f) || !(Mathf.Abs(inputY) > 0.5f)) ? 1f : diagonalStrafeAmt);
		if ((grounded || climbing || swimming || airTime + 0.45f > Time.time) && FPSPlayerComponent.hitPoints > 1f && !FPSPlayerComponent.restarting)
		{
			moveDirection = new Vector3(inputXSmoothed * limitStrafeSpeed, 0f, inputYSmoothed * limitStrafeSpeed);
			moveDirection = myTransform.TransformDirection(moveDirection);
			moveDirection = moveDirection * speed * speedAmtX * speedAmtY * crouchSpeedAmt * proneSpeedAmt * zoomSpeedAmt;
			if (!capsuleTooSteep || climbing || swimming || (capsuleTooSteep && !rayTooSteep))
			{
				velocityChange = moveDirection - velocity;
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = Mathf.Clamp(velocityChange.y, 0f - jumpSpeed, jumpSpeed);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				if (climbing)
				{
					if (InputComponent.moveY > 0.1f)
					{
						velocityChange.y = verticalSpeedAmt;
					}
					else if (InputComponent.jumpHold)
					{
						inputY = 1f;
						velocityChange.y = climbSpeed * 0.75f;
					}
					else if (InputComponent.crouchHold)
					{
						inputY = -1f;
						velocityChange.y = (0f - climbSpeed) * 0.75f;
					}
					else
					{
						velocityChange.y = 0f;
					}
				}
				else
				{
					velocityChange.y = 0f;
				}
				RigidbodyComponent.AddForce(velocityChange, ForceMode.VelocityChange);
			}
		}
		else if (FPSPlayerComponent.hitPoints < 1f || FPSPlayerComponent.restarting)
		{
			SmoothMouseLookComponent.enabled = false;
		}
		if (!climbing)
		{
			if (!swimming)
			{
				if ((!prone && !crouched) || !grounded)
				{
					RigidbodyComponent.AddForce(new Vector3(0f, Physics.gravity.y * RigidbodyComponent.mass, 0f));
				}
				RigidbodyComponent.useGravity = true;
			}
			else
			{
				if (swimStartTime + 0.2f > Time.time)
				{
					if (landStartTime + 0.3f > Time.time && !Physics.CapsuleCast(p1, p2, capsuleCheckRadius * 0.9f, -myUpVec, out capHit, capsuleCastHeight, clipMask.value))
					{
						RigidbodyComponent.AddForce(new Vector3(0f, swimmingVerticalSpeed, 0f), ForceMode.VelocityChange);
					}
				}
				else
				{
					if (InputComponent.jumpHold)
					{
						if (belowWater)
						{
							swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, 3f, Time.deltaTime * 4f);
							if (holdingBreath)
							{
								canWaterJump = false;
							}
						}
						else
						{
							swimmingVerticalSpeed = 0f;
						}
					}
					else if (InputComponent.crouchHold)
					{
						swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, -3f, Time.deltaTime * 4f);
					}
					else if (belowWater)
					{
						swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, -0.2f, Time.deltaTime * 4f);
					}
					else
					{
						swimmingVerticalSpeed = 0f;
					}
					if (!belowWater && !InputComponent.jumpHold)
					{
						canWaterJump = true;
					}
					RigidbodyComponent.AddForce(new Vector3(0f, swimmingVerticalSpeed, 0f), ForceMode.VelocityChange);
				}
				RigidbodyComponent.useGravity = false;
			}
		}
		else
		{
			RigidbodyComponent.useGravity = false;
		}
		if (RigidbodyComponent.velocity.y > verticalSpeedLimit)
		{
			Vector3 vector2 = RigidbodyComponent.velocity;
			vector2.y = Mathf.Clamp(vector2.y, -500f, verticalSpeedLimit);
			RigidbodyComponent.velocity = vector2;
		}
	}

	private void OnCollisionExit(Collision col)
	{
		if (playerParented && ((bool)col.gameObject.GetComponent<MovingPlatform>() || (bool)col.gameObject.GetComponent<MovingElevator>()))
		{
			if (FPSPlayerComponent.removePrefabRoot)
			{
				myTransform.parent = null;
			}
			else if (!FPSPlayerComponent.removePrefabRoot)
			{
				myTransform.parent = mainObj.transform;
			}
			CameraObj.transform.parent = null;
			if ((bool)PlayerCharacterComponent)
			{
				PlayerCharacterComponent.fpBodyObj.transform.parent = null;
			}
			CameraObj.transform.localScale = Vector3.one;
			playerParented = false;
		}
		parentState = false;
		capsuleTooSteep = false;
		inWater = false;
		lowpos = myTransform.position.y;
	}

	private void OnCollisionEnter(Collision col)
	{
		float num = capsule.bounds.min.y + capsuleCheckRadius;
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.point.y < num && !parentState && Vector3.Angle(contactPoint.normal, upVec) < 70f && ((bool)col.gameObject.GetComponent<MovingPlatform>() || (bool)col.gameObject.GetComponent<MovingElevator>()))
			{
				myTransform.parent = col.transform;
				CameraObj.transform.parent = col.transform;
				if ((bool)PlayerCharacterComponent)
				{
					PlayerCharacterComponent.fpBodyObj.transform.parent = col.transform;
				}
				CameraObj.transform.localScale = Vector3.one;
				playerParented = true;
				parentState = true;
			}
		}
	}

	private void OnCollisionStay(Collision col)
	{
		float num = capsule.bounds.min.y + capsuleCheckRadius;
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.point.y < num)
			{
				if (contactPoint.point.y < lowpos)
				{
					lowpos = contactPoint.point.y;
				}
				if (Vector3.Angle(contactPoint.normal, upVec) > (float)slopeLimit && !inWater && lowpos < capsule.bounds.min.y - 0.2f)
				{
					capsuleTooSteep = true;
				}
				else
				{
					capsuleTooSteep = false;
				}
			}
		}
	}

	private void CalculateFallingDamage(float fallDistance)
	{
		if (fallDamageMultiplier > 0f)
		{
			FPSPlayerComponent.ApplyDamage(fallDistance * fallDamageMultiplier);
		}
	}
}
