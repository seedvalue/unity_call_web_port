using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
	[HideInInspector]
	public Animator PlayerModelAnim;

	[HideInInspector]
	public Animator tpPistolAnim;

	[HideInInspector]
	public Animator tpShotgunAnim;

	[HideInInspector]
	public Animator tpBowAnim;

	[HideInInspector]
	public GameObject playerObj;

	[HideInInspector]
	public GameObject weaponObj;

	[HideInInspector]
	public GameObject ammoUiObj;

	[HideInInspector]
	public GameObject ammoUiObjShadow;

	private AmmoText AmmoText1;

	private AmmoText AmmoText2;

	private Transform myTransform;

	private Transform mainCamTransform;

	private FPSRigidBodyWalker FPSWalkerComponent;

	[HideInInspector]
	public PlayerWeapons PlayerWeaponsComponent;

	private Ironsights IronsightsComponent;

	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;

	private InputControl InputComponent;

	[HideInInspector]
	public SmoothMouseLook MouseLookComponent;

	private WeaponEffects WeaponEffectsComponent;

	private CameraControl CameraControlComponent;

	private WeaponPivot WeaponPivotComponent;

	private CamAndWeapAnims CamAndWeapAnimsComponent;

	[HideInInspector]
	public PlayerCharacter PlayerCharacterComponent;

	[Tooltip("Reference to weapon pickup object to drop for this weapon.")]
	[Header("Inventory and Ammo", order = 0)]
	[Space(10f, order = 1)]
	public GameObject weaponDropObj;

	[Tooltip("Weapon Mesh object which will be animated and positioned by this script.")]
	public GameObject weaponMesh;

	private Vector3 initialWeaponMeshScale;

	private bool thirdpersonState;

	private Vector3 weaponMeshInitalPos;

	[HideInInspector]
	public Animator AnimatorComponent;

	[HideInInspector]
	public Animator WeaponAnimatorComponent;

	[HideInInspector]
	public Animator CameraAnimatorComponent;

	[Tooltip("True if player has this weapon in their inventory.")]
	public bool haveWeapon;

	[Tooltip("Should this weapon be selected in normal wepaon selection cycle? (false for grenades and offhand weapons).")]
	public bool cycleSelect = true;

	[Tooltip("Can this weapon be dropped? False for un-droppable weapons like fists or sidearm.")]
	public bool droppable = true;

	[Tooltip("Does this weapon count toward weapon total? False for weapons like fists or sidearm.")]
	public bool addsToTotalWeaps = true;

	[HideInInspector]
	public int weaponNumber;

	[Tooltip("Ammo amount for this weapon in player's inventory.")]
	public int ammo = 150;

	[Tooltip("Bullets left in magazine.")]
	public int bulletsLeft;

	[Tooltip("Maximum amount of bullets per magazine.")]
	public int bulletsPerClip = 30;

	[Tooltip("Number of bullets to reload per reload cycle (when < bulletsPerClip, allows reloading one or more bullets at a time).")]
	public int bulletsToReload = 50;

	[Tooltip("Maximum ammo amount player's inventory can hold for this weapon.")]
	public int maxAmmo = 999;

	[Space(10f, order = 3)]
	[Header("Damage and Firing", order = 2)]
	[Tooltip("/Damage to inflict on objects with ApplyDamage(); function.")]
	public int damage = 10;

	private float damageAmt;

	[Tooltip("Amount of physics push to apply to rigidbodies on contact.")]
	public int force = 200;

	[Tooltip("Range that weapon can hit targets.")]
	public float range = 100f;

	private float rangeAmt;

	[Tooltip("Time between shots.")]
	public float fireRate = 0.097f;

	[Tooltip("Amount of projectiles to be fired per shot ( > 1 for shotguns).")]
	public int projectileCount = 1;

	private int hitCount;

	[Tooltip("If > 0, projectile from object pool with this index will be fired from weapon, instead of raycast-based firing.")]
	public int projectilePoolIndex;

	[Tooltip("Amount forward of camera to spawn projectile.")]
	public float projSpawnForward = 0.5f;

	private GameObject projectile;

	private Rigidbody projBody;

	[Tooltip("Force to apply to projectile after firing (shot velocity).")]
	public float projectileForce;

	[Tooltip("True if forward velocity of the projectile should be tied to how long fire button is held.")]
	public bool pullProjectileForce;

	public float minimumProjForce = 1.5f;

	private float projectileForceAmt;

	[Tooltip("Vertical rotation to add to fired projectile.")]
	public float projRotUp;

	[Tooltip("Horizontal rotation to add to fired projectile.")]
	public float projRotSide;

	[Tooltip("True if weapon should fire after releasing fire button.")]
	public bool fireOnRelease;

	[HideInInspector]
	public bool fireOnReleaseState;

	[HideInInspector]
	public bool doReleaseFire;

	[Tooltip("Play optional camera animation 1 when firing after release of fire button.")]
	public bool doCamReleaseAnim1;

	public float CamReleaseAnim1speed = 1f;

	[Tooltip("Play optional camera animation 2 when firing after release of fire button.")]
	public bool doCamReleaseAnim2;

	public float CamReleaseAnim2speed = 1f;

	[Tooltip("Play optional camera animation 1 when holding fire button before release fire.")]
	public bool doCamPullAnim1;

	public float CamPullAnim1speed = 1f;

	[Tooltip("Play optional camera animation 2 when holding fire button before release fire.")]
	public bool doCamPullAnim2;

	public float CamPullAnim2speed = 1f;

	[Tooltip("Make view kick when release firing.")]
	public bool useWeaponKick = true;

	[Tooltip("Time needed to pull weapon back for release fire (holding fire button).")]
	public float pullTime = 0.5f;

	[HideInInspector]
	public float pullTimer = 0.5f;

	[Tooltip("Time after fire button release to spawned projectile.")]
	public float releaseTime = 0.5f;

	[HideInInspector]
	public float releaseTimer = 0.5f;

	[Tooltip("Maximum time needed to hold release fire (pulling weapon back) for maximum shot charge.")]
	public float maxHoldTime = 3f;

	[HideInInspector]
	public float fireHoldTimer;

	[HideInInspector]
	public float fireHoldMult;

	public float pullBackAmt;

	[HideInInspector]
	public bool pullAnimState;

	[HideInInspector]
	public bool releaseAnimState;

	[HideInInspector]
	public float fuseTime = 2f;

	[Tooltip("True if weapon can switch between burst and semi-auto.")]
	public bool fireModeSelectable;

	private bool fireModeState;

	[Tooltip("True when weapon is in semi-auto mode.")]
	public bool semiAuto;

	private bool semiState;

	[Tooltip("True when weapon is in burst mode.")]
	public bool burstFire;

	[Tooltip("If true, weapon will cycle all three burst modes.")]
	public bool burstAndAuto;

	[Tooltip("Amount of bullets to fire per burst.")]
	public int burstShots = 3;

	private bool burstState;

	private int burstShotsFired;

	private bool burstHold;

	[Tooltip("If true, enemies will not hear weapon shooting.")]
	public bool silentShots;

	private bool initialSilentShots;

	[Tooltip("True if weapon can be fired underwater.")]
	public bool fireableUnderwater = true;

	private bool waterFireState;

	[Tooltip("Should this weapon be null/unarmed? (true for first weapon in Weapon Order array of PlayerWeapons.cs).")]
	public bool unarmed;

	[Tooltip("False if weapon only needs ammo to fire, for grenades and other disposable, one-shot weapons.")]
	public bool doReload = true;

	[Tooltip("This weapon does not reload (doReload = false) but a weapon is still needed to fire ammo (like bow & arrow).")]
	public bool nonReloadWeapon;

	[HideInInspector]
	public bool swingSide;

	[HideInInspector]
	public float shootStartTime = -16f;

	[Tooltip("False if aiming reticule should not be displayed when not zoomed, used for weapons like sniper rifles.")]
	public bool showAimingCrosshair = true;

	[Tooltip("True if crosshair should be shown when zoomed.")]
	public bool showZoomedCrosshair;

	[Header("Melee Attacks", order = 4)]
	[Tooltip("Delay after firing to check for hit, to simulate time taken for melee weapon to reach target (this weapon will be treated as a melee weapon when this value is > 0).")]
	[Space(10f, order = 5)]
	public float meleeSwingDelay;

	[Tooltip("Delay after firing to check for hit, to simulate time taken for melee weapon to reach target (this weapon will be treated as a melee weapon when this value is > 0).")]
	public float tpMeleeSwingDelay = 1f;

	[Tooltip("True if this weapon has an offhand melee attack like pistol whip.")]
	public bool offhandMeleeAttack;

	[Tooltip("Total time of offhand melee attack untill another can be done.")]
	public float meleeAttackTime = 0.3f;

	[Tooltip("Delay between offhand melee start time and hit time.")]
	public float offhandMeleeDelay = 0.15f;

	[Tooltip("Range of offhand melee attack.")]
	public float offhandMeleeRange = 1.5f;

	[Tooltip("Damage of offhand melee attack.")]
	public float offhandMeleeDamage = 100f;

	[Tooltip("False if melee attack shouldn't be allowed when looking straight down, to prevent clipping of large weapons into player model.")]
	public bool allowDownwardMelee = true;

	[Tooltip("True if a melee attack should be performed with fire button if this is a ranged weapon without ammo.")]
	public bool meleeIfNoAmmo;

	[HideInInspector]
	public float lastMeleeTime;

	private bool meleeBlendState;

	[HideInInspector]
	public bool meleeActive;

	[Header("Camera and Zooming", order = 8)]
	[Space(10f, order = 9)]
	[Tooltip("Upper vertical limit for deadzone aiming.")]
	public float verticalDZUpper = 31f;

	[Tooltip("Lower vertical limit for deadzone aiming.")]
	public float verticalDZLower = 12f;

	[Tooltip("Horizontal limit for deadzone aiming.")]
	public float horizontalDZ = 48f;

	[Tooltip("True if zoom can be used with this weapon.")]
	public bool canZoom = true;

	[Tooltip("True if zooming action should block instead of activate ironsights (mostly for melee weapons).")]
	public bool zoomIsBlock;

	[Tooltip("True if jumping while zoomed is allowed.")]
	public bool canJumpZoom;

	[Tooltip("True if blocking an attack cancels block (true for parrying with swords, false for shield weapons that don't cancel guard after block).")]
	public bool hitCancelsBlock = true;

	[Tooltip("True if a backstab attack should trigger slow motion/bullet time for a few seconds .")]
	public bool slomoBackstab = true;

	[Tooltip("Percentage of blocked damage that should be ignored.")]
	[Range(0f, 1f)]
	public float blockDefenseAmt = 0.5f;

	[Range(-1f, 1f)]
	[Tooltip("Angle in front of player where attacks will be blocked.")]
	public float blockCoverage = 0.45f;

	[Tooltip("Blocking with this weapon will only block melee attacks, ranged attacks still cause full damage (shield block vs sword block/parry).")]
	public bool onlyBlockMelee;

	[Tooltip("If true, weapon will shoot from blocked position, instead of returning to unzoomed position to fire (for firing guns or swords over shields).")]
	public bool shootFromBlock;

	[Tooltip("FOV value to use when zoomed, lower values with scoped weapons for more zoom.")]
	public float zoomFOV = 55f;

	[Tooltip("FOV value to use when in third person mode, if zero, no change from Zoom FOV above.")]
	public float zoomFOVTp;

	[Tooltip("FOV value for zooming when also deadzone aiming (goldeneye/perfect dark style).")]
	public float zoomFOVDz = 53f;

	[Tooltip("Sensitivity of view movement when zooming.")]
	public float zoomSensitivity = 0.3f;

	[Tooltip("True if this weapon should switch to first person when zooming in third person (for scoped weapons).")]
	public bool fpZoomForTp;

	[HideInInspector]
	public bool dropWillDupe;

	[Range(-0.3f, 0.3f)]
	[Tooltip("Horizontal modifier of gun position when not zoomed.")]
	[Header("View Model Positioning", order = 6)]
	[Space(10f, order = 7)]
	public float unzoomXPosition = -0.02f;

	[Tooltip("Vertical modifier of gun position when not zoomed.")]
	[Range(-0.3f, 0.3f)]
	public float unzoomYPosition = 0.0127f;

	[Tooltip("Forward modifier of gun position when not zoomed.")]
	[Range(-0.3f, 0.3f)]
	public float unzoomZPosition;

	[Tooltip("Horizontal modifier of gun position when zoomed.")]
	[Range(-0.3f, 0.3f)]
	public float zoomXPosition = -0.07f;

	[Tooltip("Vertical modifier of gun position when zoomed.")]
	[Range(-0.3f, 0.3f)]
	public float zoomYPosition = 0.032f;

	[Tooltip("Forward modifier of gun position when zoomed.")]
	[Range(-0.3f, 0.3f)]
	public float zoomZPosition;

	[Tooltip("Weapon roll angle when blocking.")]
	[Range(-180f, 180f)]
	public float blockRoll = 30f;

	[Tooltip("Horizontal modifier of gun position when backstab is ready.")]
	[Range(-0.3f, 0.3f)]
	public float backstabXPosition = -0.07f;

	[Tooltip("Vertical modifier of gun position when backstab is ready.")]
	[Range(-0.3f, 0.3f)]
	public float backstabYPosition = 0.032f;

	[Tooltip("Forward modifier of gun position when backstab is ready.")]
	[Range(-0.3f, 0.3f)]
	public float backstabZPosition;

	[Tooltip("Weapon roll angle when backstab is ready.")]
	[Range(-180f, 180f)]
	public float backstabRoll = 30f;

	private float weapRollAmt = 1f;

	private float rollAngleVel;

	private float weapRollAmtSmoothed;

	private float strafeSmoothed;

	[Range(-0.3f, 0.3f)]
	[Tooltip("Amount to move weapon down when walking and crouched.")]
	public float crouchWalkYPosition = -0.01f;

	[Tooltip("Amount to move weapon down when crouched and not walking.")]
	[Range(-0.3f, 0.3f)]
	public float crouchYPosition = -0.005f;

	[Tooltip("Amount to move weapon horizontally when crouched.")]
	[Range(-0.3f, 0.3f)]
	public float crouchXPosition = -0.005f;

	[Tooltip("Amount to move weapon down when walking.")]
	[Range(-0.3f, 0.3f)]
	public float walkYPosition = -0.008f;

	[Tooltip("Horizontal modifier of gun position when sprinting.")]
	[Range(-0.3f, 0.3f)]
	public float sprintXPosition = 0.075f;

	[Range(-0.3f, 0.3f)]
	[Tooltip("Vertical modifier of gun position when sprinting.")]
	public float sprintYPosition = 0.0075f;

	[Range(-0.3f, 0.3f)]
	[Tooltip("Forward modifier of gun position when sprinting.")]
	public float sprintZPosition;

	[Tooltip("Angle sway amount for this weapon when walking (yaw, pitch, roll).")]
	[Space(10f, order = 11)]
	[Header("Bobbing and Sway Amounts", order = 10)]
	public Vector3 walkBobAngles = Vector3.one;

	[Tooltip("Position sway amount for this weapon when walking (side, up, forward).")]
	public Vector3 walkBobPosition = Vector3.one;

	[Tooltip("Angle sway amount for this weapon when sprinting (yaw, pitch, roll).")]
	public Vector3 sprintBobAngles = Vector3.one;

	[Tooltip("Position sway amount for this weapon when walking (side, up, forward).")]
	public Vector3 sprintBobPosition = Vector3.one;

	[Tooltip("Angle sway amount for this weapon when zoomed (yaw, pitch, roll).")]
	public Vector3 zoomBobAngles = Vector3.one;

	[Tooltip("Position sway amount for this weapon when zoomed (side, up, forward).")]
	public Vector3 zoomBobPosition = Vector3.one;

	[Tooltip("Angle sway amount for this weapon when crouched (yaw, pitch, roll).")]
	public Vector3 crouchBobAngles = Vector3.one;

	[Tooltip("Position sway amount for this weapon when crouched (side, up, forward).")]
	public Vector3 crouchBobPosition = Vector3.one;

	[Tooltip("Angle sway amount for this weapon when prone (yaw, pitch, roll).")]
	public Vector3 proneBobAngles = Vector3.one;

	[Tooltip("Position sway amount for this weapon when prone (side, up, forward).")]
	public Vector3 proneBobPosition = Vector3.one;

	[Tooltip("View sway amount for this weapon when not zoomed.")]
	public float swayAmountUnzoomed = 1f;

	[Tooltip("View sway amount for this weapon when zoomed.")]
	public float swayAmountZoomed = 1f;

	[Tooltip("Amount the weapon moves randomly when idle.")]
	public float idleSwayAmt = 1f;

	[Tooltip("Amount the weapon moves randomly when swimming.")]
	public float swimIdleSwayAmt = 1f;

	[Tooltip("Amount the weapon moves randomly when zoomed.")]
	public float zoomIdleSwayAmt = 1f;

	[Tooltip("Set to true to use alternate sprinting animation with pistols or one handed weapons.")]
	public bool PistolSprintAnim;

	[Tooltip("True if this weapon extends further up the screen and will be moved further down when switching so it goes completely offscreen (for swords and bows).")]
	public bool verticalWeapon;

	[Tooltip("To reset weapon anim after unhiding weapon (mostly for fire on release weapons that were pulled before hiding).")]
	private bool hideAnimState;

	private float rollRot;

	private float rollNeutralAngle;

	private float initialRollAngle;

	[Tooltip("Amount to roll weapon left or right when moving view.")]
	public float rollSwayAmt = 30f;

	[Tooltip("Amount to roll weapon when pressing left or right sprint button.")]
	public float strafeRoll = 0.5f;

	[Tooltip("Amount to move weapon left or right when pressing left or right strafe button while unzoomed.")]
	public float strafeSideUnzoom = 0.5f;

	[Tooltip("Amount to move weapon left or right when pressing left or right strafe button while zoomed.")]
	public float strafeSideZoom = 0.2f;

	[Tooltip("Amount to move weapon left or right when pressing left or right strafe button while sprinting.")]
	public float strafeSideSprint = 0.2f;

	[Header("Animation Timings and Effects", order = 12)]
	[Space(10f, order = 13)]
	[Tooltip("Number than indicates the type of third person weapon animations to use. 0 = unarmed, 1 = one handed melee, 2 = two handed melee, 3 = pistol, 4 = shotgun (unused), 5 = automatic rifle, 6 = bow, 7 = grenade")]
	public int tpWeaponAnimType;

	[Tooltip("True if weapon model in third person should animate shotgun pump")]
	public bool tpUseShotgunAnims;

	[Tooltip("True if player model in third person should play sheild bash animation for offhand melee attack.")]
	public bool tpOffhandMeleeIsBash;

	[Tooltip("Time to play the left hand melee weapon animation in third person mode.")]
	public float tpSwingTimeL = 0.14f;

	[Tooltip("Time to play the right hand melee weapon animation in third person mode.")]
	public float tpSwingTimeR = 0.25f;

	[Tooltip("Only layers to include in bullet hit detection (for efficiency).")]
	public LayerMask bulletMask;

	private LayerMask liquidMask;

	private Vector3 gunAngleVel = Vector3.zero;

	[HideInInspector]
	public Vector3 gunAnglesTarget = Vector3.zero;

	[HideInInspector]
	public Vector3 gunAngles = Vector3.zero;

	private bool canShoot = true;

	[HideInInspector]
	public bool shooting;

	[HideInInspector]
	public bool sprintAnimState;

	[HideInInspector]
	public bool sprintState;

	[HideInInspector]
	public float recoveryTime;

	private float horizontal;

	private float vertical;

	private int bulletsNeeded;

	[HideInInspector]
	public int bulletsReloaded;

	[Tooltip("Time per reload cycle, should be shorter if reloading one bullet at a time and longer if reloading magazine.")]
	public float reloadTime = 1.75f;

	private float reloadStartTime;

	[HideInInspector]
	public bool reloadState;

	private bool sprintReloadState;

	private float reloadEndTime;

	[Tooltip("Amount of time needed to finish the ready anim after weapon has just been switched to/selected.")]
	public float readyTime = 0.6f;

	[Tooltip("Percentage of total ready time for offhand throw (usually shorter than ready time).")]
	public float offhandThrowReadyMod = 0.4f;

	[HideInInspector]
	public float readyTimeAmt;

	[HideInInspector]
	public bool isOffhandThrow;

	[HideInInspector]
	public float recoveryTimeAmt;

	[HideInInspector]
	public float startTime;

	[HideInInspector]
	public float reloadLastTime = 1.2f;

	[HideInInspector]
	public float reloadLastStartTime;

	[HideInInspector]
	public bool lastReload;

	private bool cantFireState;

	public Transform shotOrigin;

	[HideInInspector]
	public Vector3 origin;

	[HideInInspector]
	public Vector3 direction;

	[Tooltip("Distance from muzzle flash to start shot (closer if using two cameras and weapon models are larger than player capsule).")]
	public float shotOriginDist = 0.5f;

	[HideInInspector]
	public Ray weaponRay;

	[HideInInspector]
	public Vector3 weaponLookDirection;

	private Vector3 lookDirection;

	[Tooltip("The game object that will be used as a muzzle flash.")]
	public Transform muzzleFlash;

	private float muzzleFlashReduction = 6.5f;

	[HideInInspector]
	public Color muzzleFlashColor = new Color(1f, 1f, 1f, 0f);

	[Tooltip("The game object with a light component that will be used for muzzle light.")]
	public GameObject muzzleLightObj;

	[Tooltip("Time to wait until the muzzle light starts fading out.")]
	public float muzzleLightDelay = 0.1f;

	[Tooltip("Speed of muzzle light fading.")]
	public float muzzleLightReduction = 100f;

	private Renderer FPmuzzleRenderer;

	[HideInInspector]
	public Renderer TPmuzzleRenderer;

	private Renderer muzzleRendererComponent;

	private Light muzzleLightComponent;

	private Renderer muzzleSmokeComponent;

	[HideInInspector]
	public Transform thirdPersonSmokePos;

	[HideInInspector]
	public bool fireActive;

	[HideInInspector]
	public bool muzzActive = true;

	[HideInInspector]
	public bool disableFiring;

	[Tooltip("True if barrel smoke should be emitted (long trail from end of barrel).")]
	public bool useBarrelSmoke = true;

	[Tooltip("Number of consecutive shots required for barrel smoke to emit.")]
	public int barrelSmokeShots;

	[Tooltip("Distance forward to emit smoke effects.")]
	public float smokeForward = 0.15f;

	[Tooltip("Particle effect for smoke rising from barrel after firing more bullets than barrelSmokeShots amount.")]
	public ParticleSystem barrelSmokeParticles;

	[Tooltip("Horizontal and vertical offset for emitting barrel smoke.")]
	public Vector3 barrelSmokeOffset;

	private int bulletsJustFired;

	private bool emitBarrelSmoke;

	private float barrelSmokeTime;

	[Tooltip("Duration of barrel smoke particle emission.")]
	public float barrelSmokeDuration = 0.35f;

	private bool barrelSmokeActive;

	[Tooltip("True if muzzle smoke should be emitted (short cloud from end of barrel).")]
	public bool useMuzzleSmoke = true;

	[Tooltip("Particle effect for puff of smoke when firing.")]
	public ParticleSystem muzzleSmokeParticles;

	[Tooltip("Horizontal and vertical offset for emitting muzzle smoke.")]
	public Vector3 muzzleSmokeOffset;

	private Color muzzleSmokeColor = Color.white;

	[Tooltip("Alpha transparency of muzzle smoke.")]
	public float muzzleSmokeAlpha = 0.25f;

	[Tooltip("True if tracers should be emitted for bullet shots.")]
	public bool useTracers = true;

	[Tooltip("Offset from shot origin to emit tracers.")]
	public Vector3 tracerOffset;

	[Tooltip("Distance from shot origin to emit tracers.")]
	public float tracerDist;

	[Tooltip("Distance from shot origin to emit tracers when swimming.")]
	public float tracerSwimDist;

	[Tooltip("Distance from shot origin to emit tracers when in third person mode.")]
	public float tracerDistTp;

	private CapsuleCollider capsule;

	[Tooltip("True if the albedo color of the weapon model materials should should be dimmed to simulate shadows from geometry obstructing the sun (used by two camera setup).")]
	public bool shadeWeapon;

	[Tooltip("True if shading (shade boolean value) should be handled manually by triggers, not by linecast to sun object, to allow for bright interiors.")]
	public bool manualShadingMode;

	[HideInInspector]
	public bool shaded;

	private Transform sunLightObj;

	private Vector3 litPos;

	[Tooltip("Do not change the albedo color for shading on this mesh renderer (for scope lenses or other effects).")]
	public Renderer ignoreMeshShading;

	private Renderer[] meshRenderers;

	private Material[] tempWeapMaterials;

	private List<Material> materialList = new List<Material>();

	private float updateInterval = 0.25f;

	private float lastUpdate;

	private Color shadeColor;

	private Color smoothedColor = Color.white;

	private float initSpec;

	private float smoothedSpec;

	[Space(10f, order = 15)]
	[Tooltip("Amount that shot accuracy will decrease over time.")]
	[Header("Recoil", order = 14)]
	public float shotSpread;

	private float shotSpreadAmt;

	[HideInInspector]
	public Quaternion kickRotation;

	[Tooltip("Amount to kick view up when firing.")]
	public float kickUp = 7f;

	[Tooltip("Amount to kick view sideways when firing.")]
	public float kickSide = 2f;

	[Tooltip("Amount to kick view up when firing.")]
	public float kickRoll = 15f;

	private bool kickRollState;

	private float kickUpAmt;

	private float kickSideAmt;

	private float kickRollAmt = 20f;

	[Tooltip("Distance that gun pushes back when firing and not zoomed.")]
	public float kickBackAmtUnzoom = -0.025f;

	[Tooltip("Distance that gun pushes back when firing and zoomed.")]
	public float kickBackAmtZoom = -0.0175f;

	[Tooltip("True if view should climb with weapon fire (player has to compensate by moving view the opposite direction).")]
	public bool useViewClimb;

	[Tooltip("Amount that view climbs upwards with non-recovering recoil.")]
	public float viewClimbUp = 1f;

	[Tooltip("Amount that view moves side to side with non-recovering recoil.")]
	public float viewClimbSide = 1f;

	[Tooltip("Amount that view moves right with non-recovering recoil.")]
	public float viewClimbRight = 0.75f;

	[Tooltip("True if weapon accuracy should decrease with sustained fire.")]
	public bool useRecoilIncrease;

	[Tooltip("Number of shots before weapon recoil increases with sustained fire.")]
	public int shotsBeforeRecoil = 4;

	[Tooltip("Growth rate of sustained fire recoil for view angles/input.")]
	public float viewKickIncrease = 1.75f;

	[Tooltip("Growth Rate of sustained fire recoil for weapon accuracy.")]
	public float aimDirRecoilIncrease = 2f;

	[HideInInspector]
	public float randZkick;

	[Space(10f, order = 17)]
	[Header("Shell Ejection", order = 16)]
	[Tooltip("True if weapon should eject shell casing when firing.")]
	public bool spawnShell;

	[Tooltip("Object pool index for the shell object with rigidbody (physics for ejected shell).")]
	public int shellRBPoolIndex;

	private GameObject shell;

	private GameObject shell2;

	private Vector3 shellEjectDirection;

	[Tooltip("Position shell is ejected from when not zoomed.")]
	public Transform shellEjectPosition;

	[Tooltip("Position shell is ejected from when zoomed.")]
	public Transform shellEjectPositionZoom;

	[HideInInspector]
	public Transform shellEjectPositionTP;

	private Transform shellEjectPos;

	[Tooltip("Scale of shell, can be used to make different shaped shells from one model.")]
	public Vector3 shellScale = new Vector3(1f, 1f, 1f);

	[Tooltip("Delay before ejecting shell (used for bolt action rifles and pump shotguns).")]
	public float shellEjectDelay;

	[Tooltip("Overall movement force of ejected shell.")]
	public float shellForce = 0.2f;

	[Tooltip("Vertical amount to apply to shellForce.")]
	public float shellUp = 0.75f;

	[Tooltip("Horizontal amount to apply to shellForce.")]
	public float shellSide = 1f;

	[Tooltip("Forward amount to apply to shellForce.")]
	public float shellForward = 0.1f;

	[Tooltip("Amount of vertical shell rotation.")]
	public float shellRotateUp = 0.25f;

	[Tooltip("Amount of horizontal shell rotation.")]
	public float shellRotateSide = 0.25f;

	[Tooltip("Time in seconds that shells persist in the world before being removed.")]
	public int shellDuration = 5;

	private AudioSource[] aSources;

	private AudioSource firefx;

	[HideInInspector]
	public AudioSource otherfx;

	[Header("Sound Effects", order = 18)]
	[Space(10f, order = 19)]
	[Tooltip("Weapon firing sound effect.")]
	public AudioClip fireSnd;

	[Tooltip("Offhand melee attack sound effect.")]
	public AudioClip meleeSnd;

	[Tooltip("Volume of firing sound effect.")]
	public float fireVol = 0.9f;

	[Tooltip("Weapon reloading sound effect.")]
	public AudioClip reloadSnd;

	[Tooltip("Usually shell reload sound + shotgun pump or rifle chambering sound.")]
	public AudioClip reloadLastSnd;

	[Tooltip("Sound effect for when weapon has no ammo.")]
	public AudioClip noammoSnd;

	[Tooltip("Sound effect for wepaon readying animation.")]
	public AudioClip readySnd;

	[Tooltip("Sound effect for pulling weapon back if firing on release of fire button.")]
	public AudioClip pullSnd;

	[Tooltip("Sound effect for blocking of attack.")]
	public AudioClip blockSound;

	private double nextFireTime;

	private double schedulingTime = 0.1;

	private double lastFireTime;

	private double firingPauseMinimumTime = 0.05;

	private AudioSource autoFireAsource1;

	private AudioSource autoFireAsource2;

	private bool curAutofireAsource;

	private float firePitch;

	private double time;

	private double oldTime;

	private float oldTimeChecked;

	private bool audioDisabled;

	[Header("Flashlight", order = 20)]
	[Space(10f, order = 21)]
	[Tooltip("True if this weapon has a flashlight attachment.")]
	public bool useLight;

	[Tooltip("Object that flashlight should inherit rotation from.")]
	public Transform lightBaseObj;

	[Tooltip("Should the facing of the light be reversed if this model was exported with yaw rotated 180 degrees? (Some weapon models need this).")]
	public bool flipLightFacing;

	private bool lightOnState;

	[Tooltip("True if the zoom key should toggle the flashlight in addition to the toggle light key.")]
	public bool useZoomSwitch;

	[Tooltip("True if the flashlight is on.")]
	public bool lightOn;

	[Tooltip("Mesh Renderer/Object of the flashlight cone effect that will be toggled on/off.")]
	public MeshRenderer lightConeMesh;

	[Tooltip("Spotlight object for flashlight.")]
	public Light spot;

	[Tooltip("Point light object for flashlight (shines on player's hand and weapon when flashlight is on).")]
	public Light point;

	[Tooltip("Range of point light shining on weapon model when flashlight/attachment light is on for 2 camera mode.")]
	public float pointRange2Cam = 8f;

	[Tooltip("Range of point light shining on weapon model when flashlight/attachment light is on for 1 camera mode.")]
	public float pointRange1Cam = 0.2f;

	private void Start()
	{
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		playerObj = mainCamTransform.GetComponent<CameraControl>().playerObj;
		weaponObj = mainCamTransform.GetComponent<CameraControl>().weaponObj;
		AnimatorComponent = GetComponent<Animator>();
		CameraAnimatorComponent = Camera.main.GetComponent<Animator>();
		WeaponAnimatorComponent = weaponMesh.GetComponent<Animator>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		FPSWalkerComponent = FPSPlayerComponent.FPSWalkerComponent;
		IronsightsComponent = FPSPlayerComponent.IronsightsComponent;
		PlayerWeaponsComponent = FPSPlayerComponent.PlayerWeaponsComponent;
		InputComponent = FPSPlayerComponent.InputComponent;
		MouseLookComponent = FPSPlayerComponent.MouseLookComponent;
		WeaponEffectsComponent = FPSPlayerComponent.WeaponEffectsComponent;
		CameraControlComponent = Camera.main.GetComponent<CameraControl>();
		WeaponPivotComponent = FPSPlayerComponent.WeaponPivotComponent;
		CamAndWeapAnimsComponent = IronsightsComponent.CamAndWeapAnimsComponent;
		PlayerCharacterComponent = CameraControlComponent.PlayerCharacterComponent;
		weaponMeshInitalPos = weaponMesh.transform.localPosition;
		initialWeaponMeshScale = weaponMesh.transform.localScale;
		AmmoText1 = FPSPlayerComponent.ammoUiObj.GetComponent<AmmoText>();
		AmmoText2 = FPSPlayerComponent.ammoUiObjShadow.GetComponent<AmmoText>();
		aSources = weaponObj.GetComponents<AudioSource>();
		otherfx = aSources[1];
		firefx = aSources[0];
		firefx.clip = fireSnd;
		autoFireAsource1 = aSources[0];
		autoFireAsource2 = aSources[2];
		shadeColor = PlayerWeaponsComponent.shadeColor;
		sunLightObj = PlayerWeaponsComponent.sunLightObj;
		meshRenderers = weaponMesh.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < meshRenderers.Length; i++)
		{
			if (meshRenderers[i] != ignoreMeshShading)
			{
				tempWeapMaterials = meshRenderers[i].materials;
				for (int j = 0; j < tempWeapMaterials.Length; j++)
				{
					materialList.Add(tempWeapMaterials[j]);
				}
			}
		}
		capsule = playerObj.GetComponent<CapsuleCollider>();
		if ((bool)shotOrigin)
		{
			origin = shotOrigin.position;
		}
		else if ((bool)muzzleFlash)
		{
			origin = muzzleFlash.position;
			origin += muzzleFlash.forward * shotOriginDist;
			muzzleFlash.localEulerAngles = Vector3.zero;
		}
		if ((bool)muzzleFlash)
		{
			FPmuzzleRenderer = muzzleFlash.GetComponent<Renderer>();
			if ((bool)muzzleLightObj)
			{
				muzzleLightComponent = muzzleLightObj.GetComponent<Light>();
			}
		}
		if ((bool)muzzleSmokeParticles)
		{
			muzzleSmokeComponent = muzzleSmokeParticles.GetComponent<Renderer>();
		}
		if (unarmed)
		{
			return;
		}
		if (meleeSwingDelay == 0f)
		{
			if ((bool)muzzleFlash)
			{
				FPmuzzleRenderer.enabled = false;
				muzzleFlashColor = FPmuzzleRenderer.material.GetColor("_TintColor");
				if ((bool)TPmuzzleRenderer)
				{
					TPmuzzleRenderer.enabled = false;
				}
				if ((bool)muzzleLightObj)
				{
					muzzleLightComponent.enabled = false;
				}
			}
			bulletsLeft = Mathf.Clamp(bulletsLeft, 0, bulletsPerClip);
		}
		else
		{
			bulletsLeft = bulletsPerClip;
		}
		if (semiAuto)
		{
			if (projectileCount < 2)
			{
				muzzleFlashReduction = 3.5f;
			}
			else
			{
				muzzleFlashReduction = 2f;
			}
		}
		else if (projectileCount < 2)
		{
			muzzleFlashReduction = 6.5f;
		}
		else
		{
			muzzleFlashReduction = 2f;
		}
		liquidMask = (int)bulletMask & -262145;
		shootStartTime = -1f;
		shotSpreadAmt = shotSpread;
		gunAngles = gunAnglesTarget;
		initialRollAngle = weaponMesh.transform.localEulerAngles.z;
		myTransform.localEulerAngles = gunAngles;
		initialSilentShots = silentShots;
		ammo = Mathf.Clamp(ammo, 0, maxAmmo);
		bulletsToReload = Mathf.Clamp(bulletsToReload, 0, bulletsPerClip);
		if (!PistolSprintAnim)
		{
			recoveryTimeAmt = 0.4f;
		}
		else
		{
			recoveryTimeAmt = 0.2f;
		}
	}

	public void InitializeWeapon()
	{
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		playerObj = mainCamTransform.GetComponent<CameraControl>().playerObj;
		weaponObj = mainCamTransform.GetComponent<CameraControl>().weaponObj;
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		IronsightsComponent = FPSPlayerComponent.IronsightsComponent;
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		WeaponAnimatorComponent = weaponMesh.GetComponent<Animator>();
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		aSources = weaponObj.GetComponents<AudioSource>();
		otherfx = aSources[1];
		firefx = aSources[0];
		firefx.clip = fireSnd;
		firefx.spatialBlend = 0f;
		firefx.panStereo = 0f;
		firefx.velocityUpdateMode = AudioVelocityUpdateMode.Auto;
		autoFireAsource1 = aSources[0];
		autoFireAsource2 = aSources[2];
		autoFireAsource2.clip = fireSnd;
		firingPauseMinimumTime = fireRate;
		CancelWeaponPull();
		capsule = playerObj.GetComponent<CapsuleCollider>();
		muzzActive = true;
		AmmoText1 = FPSPlayerComponent.ammoUiObj.GetComponent<AmmoText>();
		AmmoText2 = FPSPlayerComponent.ammoUiObjShadow.GetComponent<AmmoText>();
		FPSPlayerComponent.canvasObj.SetActive(true);
		if (!FPSPlayerComponent.showAmmo || unarmed || meleeSwingDelay != 0f)
		{
			AmmoText1.uiTextComponent.enabled = false;
			AmmoText2.uiTextComponent.enabled = false;
		}
		else
		{
			AmmoText1.uiTextComponent.enabled = true;
			AmmoText2.uiTextComponent.enabled = true;
		}
		if (!unarmed && !doReload && !nonReloadWeapon && ammo <= 0)
		{
			FPSWalkerComponent.hideWeapon = true;
		}
		if (useLight)
		{
			if (!lightOn)
			{
				if ((bool)lightConeMesh)
				{
					lightConeMesh.enabled = false;
				}
				if ((bool)spot)
				{
					spot.enabled = false;
				}
				if ((bool)point)
				{
					point.enabled = false;
				}
			}
			else
			{
				if ((bool)lightConeMesh)
				{
					lightConeMesh.enabled = true;
				}
				if ((bool)spot)
				{
					spot.enabled = true;
				}
				if ((bool)point)
				{
					point.enabled = true;
				}
			}
		}
		if (unarmed || (ammo <= 0 && !doReload && !(meleeSwingDelay > 0f)) || !(Time.timeSinceLevelLoad > 2f) || !PlayerWeaponsComponent.switching)
		{
			return;
		}
		StopCoroutine("Reload");
		IronsightsComponent.reloading = false;
		startTime = Time.time;
		myTransform = base.transform;
		gunAngles = gunAnglesTarget;
		myTransform.localEulerAngles = gunAngles;
		if (!isOffhandThrow)
		{
			readyTimeAmt = readyTime;
			otherfx.volume = 1f;
			otherfx.clip = readySnd;
			otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
			if (!PlayerWeaponsComponent.offhandThrowActive)
			{
				WeaponAnimatorComponent.SetTrigger("Ready");
			}
		}
		else
		{
			readyTimeAmt = readyTime * offhandThrowReadyMod;
			isOffhandThrow = false;
		}
	}

	private void LateUpdate()
	{
		if (!(Time.timeScale > 0f) || !(Time.deltaTime > 0f) || !(Time.smoothDeltaTime > 0f))
		{
			return;
		}
		if (shootStartTime + fireRate < Time.time)
		{
			if (zoomIsBlock && FPSPlayerComponent.zoomed)
			{
				if (FPSPlayerComponent.canBackstab)
				{
					weapRollAmt = backstabRoll;
				}
				else
				{
					weapRollAmt = blockRoll;
				}
			}
			else if (FPSPlayerComponent.canBackstab)
			{
				weapRollAmt = backstabRoll;
			}
			else
			{
				weapRollAmt = 0f;
			}
		}
		else
		{
			weapRollAmt = 0f;
		}
		weapRollAmtSmoothed = Mathf.SmoothDampAngle(weapRollAmtSmoothed, weapRollAmt, ref rollAngleVel, 0.1f, float.PositiveInfinity, Time.smoothDeltaTime);
		strafeSmoothed = Mathf.Lerp(strafeSmoothed, FPSWalkerComponent.inputX * 1f, Time.smoothDeltaTime * 7f);
		rollRot = initialRollAngle + weapRollAmtSmoothed + IronsightsComponent.side * rollSwayAmt + WeaponPivotComponent.animOffsetTarg.z - strafeSmoothed * strafeRoll - CamAndWeapAnimsComponent.weapAngleAnim.z * IronsightsComponent.weapAngleBobAmt.z;
		rollNeutralAngle = Mathf.DeltaAngle(weaponMesh.transform.localEulerAngles.z, rollRot);
		if (FPSPlayerComponent.zoomed && !zoomIsBlock)
		{
			weaponMesh.transform.localPosition = Vector3.MoveTowards(weaponMesh.transform.localPosition, weaponMeshInitalPos, 0.05f * Time.smoothDeltaTime);
		}
		if ((bool)muzzleFlash)
		{
			weaponMesh.transform.RotateAround(muzzleFlash.position, weaponMesh.transform.forward, rollNeutralAngle);
		}
		else if ((bool)shotOrigin)
		{
			weaponMesh.transform.RotateAround(shotOrigin.position, weaponMesh.transform.forward, rollNeutralAngle);
		}
	}

	private void Update()
	{
		if (Time.timeScale > 0f && Time.deltaTime > 0f)
		{
			horizontal = FPSWalkerComponent.inputX;
			vertical = FPSWalkerComponent.inputY;
			weaponRay = new Ray(mainCamTransform.position, myTransform.parent.transform.forward);
			weaponLookDirection = (weaponRay.GetPoint(20f) - mainCamTransform.position).normalized;
			if (CameraControlComponent.thirdPersonActive)
			{
				if (!thirdpersonState)
				{
					weaponMesh.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
					thirdpersonState = true;
				}
				readyTimeAmt = readyTime * 0.2f;
			}
			else if (thirdpersonState)
			{
				weaponMesh.transform.localScale = initialWeaponMeshScale;
				thirdpersonState = false;
			}
			if (!doReload && !nonReloadWeapon && ammo <= 0)
			{
				FPSWalkerComponent.hideWeapon = true;
			}
			if (shadeWeapon)
			{
				if ((bool)sunLightObj && Time.time > lastUpdate + updateInterval)
				{
					litPos = mainCamTransform.position + mainCamTransform.up * -0.25f;
					if (!manualShadingMode)
					{
						if (Physics.Linecast(litPos, sunLightObj.position, bulletMask))
						{
							shaded = true;
						}
						else
						{
							shaded = false;
						}
					}
					lastUpdate = Time.time;
				}
				if (shaded)
				{
					if (smoothedColor.r > shadeColor.r)
					{
						smoothedColor = Color.Lerp(smoothedColor, shadeColor, Time.smoothDeltaTime * 5f);
						for (int i = 0; i < materialList.Count; i++)
						{
							materialList[i].SetColor("_Color", smoothedColor);
						}
					}
				}
				else if (smoothedColor.r < 255f)
				{
					smoothedColor = Color.Lerp(smoothedColor, Color.white, Time.smoothDeltaTime * 5f);
					for (int j = 0; j < materialList.Count; j++)
					{
						materialList[j].SetColor("_Color", smoothedColor);
					}
				}
			}
			else if (smoothedColor.r < 255f)
			{
				smoothedColor = Color.Lerp(smoothedColor, Color.white, Time.smoothDeltaTime * 5f);
				for (int k = 0; k < materialList.Count; k++)
				{
					materialList[k].SetColor("_Color", smoothedColor);
				}
				shaded = false;
			}
			if ((bool)shotOrigin)
			{
				origin = shotOrigin.position;
			}
			else if ((bool)muzzleFlash)
			{
				origin = muzzleFlash.position;
				origin += muzzleFlash.forward * (0f - shotOriginDist);
			}
			if (meleeSwingDelay == 0f && !unarmed)
			{
				AmmoText1.ammoGui = bulletsLeft;
				AmmoText1.ammoGui2 = ammo;
				AmmoText2.ammoGui = bulletsLeft;
				AmmoText2.ammoGui2 = ammo;
				if (!doReload)
				{
					AmmoText1.showMags = false;
					AmmoText2.showMags = false;
				}
				else
				{
					AmmoText1.showMags = true;
					AmmoText2.showMags = true;
				}
			}
			else
			{
				AmmoText1.ammoGui = bulletsLeft;
				AmmoText1.ammoGui2 = ammo;
				AmmoText2.ammoGui = bulletsLeft;
				AmmoText2.ammoGui2 = ammo;
			}
			if (!unarmed)
			{
				if (FPSPlayerComponent.hitPoints >= 1f)
				{
					if (reloadLastStartTime + reloadLastTime > Time.time)
					{
						lastReload = true;
					}
					else
					{
						lastReload = false;
					}
					if (doReload && !meleeActive)
					{
						if ((FPSWalkerComponent.sprintActive && !FPSWalkerComponent.sprintReload && !InputComponent.fireHold && !FPSWalkerComponent.cancelSprint && FPSWalkerComponent.moving) || (InputComponent.fireHold && bulletsToReload != bulletsPerClip && (float)bulletsReloaded >= (float)bulletsToReload * 2f && reloadEndTime + reloadTime < Time.time) || FPSWalkerComponent.hideWeapon)
						{
							if (IronsightsComponent.reloading)
							{
								IronsightsComponent.reloading = false;
								StopCoroutine("Reload");
								if (bulletsToReload != bulletsPerClip)
								{
									bulletsReloaded = 0;
									reloadStartTime = -16f;
									reloadEndTime = -16f;
									reloadLastStartTime = -16f;
								}
								CameraAnimatorComponent.speed = 1f;
								CameraAnimatorComponent.SetTrigger("CamIdle");
								if (bulletsToReload == bulletsPerClip && reloadStartTime + reloadTime / 2f < Time.time && !sprintReloadState)
								{
									bulletsNeeded = bulletsPerClip - bulletsLeft;
									if (ammo >= bulletsNeeded)
									{
										ammo -= bulletsNeeded;
										bulletsLeft = bulletsPerClip;
									}
									else
									{
										bulletsLeft += ammo;
										ammo = 0;
									}
									sprintReloadState = true;
								}
								else
								{
									otherfx.clip = null;
									if (bulletsToReload == bulletsPerClip)
									{
										WeaponAnimatorComponent.SetTrigger("Reload");
									}
								}
							}
						}
						else if (bulletsLeft <= 0 && Time.time - shootStartTime > fireRate && (canShoot || FPSWalkerComponent.sprintReload) && doReload && ammo > 0 && !IronsightsComponent.reloading && !PlayerWeaponsComponent.switching && !FPSWalkerComponent.hideWeapon && startTime + readyTimeAmt < Time.time)
						{
							StartCoroutine("Reload");
						}
					}
					if (!FPSWalkerComponent.hideWeapon)
					{
						if (hideAnimState)
						{
							hideAnimState = false;
						}
						IronsightsComponent.climbMove = 0f;
						if ((!FPSWalkerComponent.sprintActive && !FPSWalkerComponent.prone) || (IronsightsComponent.reloading && !FPSWalkerComponent.sprintReload) || FPSWalkerComponent.crouched || (FPSPlayerComponent.zoomed && meleeSwingDelay == 0f) || (Mathf.Abs(horizontal) > 0.75f && Mathf.Abs(vertical) < 0f && !FPSWalkerComponent.prone && FPSWalkerComponent.forwardSprintOnly) || FPSWalkerComponent.cancelSprint || (!FPSWalkerComponent.grounded && FPSWalkerComponent.jumping) || FPSWalkerComponent.fallingDistance > 0.75f || (InputComponent.fireHold && !FPSWalkerComponent.prone))
						{
							if (!sprintState)
							{
								recoveryTime = Time.time;
								sprintState = true;
							}
							canShoot = true;
							sprintReloadState = false;
						}
						else if ((Mathf.Abs(vertical) > 0f && FPSWalkerComponent.forwardSprintOnly) || (!FPSWalkerComponent.forwardSprintOnly && FPSWalkerComponent.moving) || (Mathf.Abs(horizontal) > 0f && FPSWalkerComponent.prone))
						{
							sprintState = false;
							if (IronsightsComponent.reloading && !FPSWalkerComponent.sprintReload)
							{
								canShoot = false;
							}
							else if (FPSPlayerComponent.zoomed && meleeSwingDelay == 0f)
							{
								canShoot = true;
							}
							else
							{
								canShoot = false;
							}
						}
						else
						{
							if (!sprintState)
							{
								recoveryTime = Time.time;
								sprintState = true;
							}
							canShoot = true;
						}
					}
					else
					{
						CancelWeaponPull();
						hideAnimState = true;
						FPSPlayerComponent.PlayerWeaponsComponent.pullGrenadeState = false;
						FPSPlayerComponent.PlayerWeaponsComponent.offhandThrowActive = false;
						FPSPlayerComponent.PlayerWeaponsComponent.grenadeThrownState = false;
						if (!FPSWalkerComponent.lowerGunForClimb)
						{
							if (!sprintState)
							{
								recoveryTime = Time.time;
								sprintState = true;
							}
							canShoot = true;
						}
						else
						{
							if (meleeSwingDelay == 0f)
							{
								IronsightsComponent.climbMove = -0.4f;
							}
							else
							{
								IronsightsComponent.climbMove = -1.4f;
							}
							canShoot = false;
							sprintState = false;
						}
					}
					if (InputComponent.fireHold)
					{
						if (cantFireState && canShoot && bulletsLeft <= 0 && doReload && ammo <= 0 && !meleeIfNoAmmo && AnimatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f)
						{
							otherfx.volume = 1f;
							otherfx.clip = noammoSnd;
							otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
							shooting = false;
							cantFireState = false;
						}
					}
					else
					{
						cantFireState = true;
						waterFireState = false;
						if (Time.time - oldTimeChecked > 2f)
						{
							oldTime = AudioSettings.dspTime;
							oldTimeChecked = Time.time;
						}
					}
					if (InputComponent.fireModePress || InputComponent.xboxDpadUpPress)
					{
						if (fireModeState && canShoot && !IronsightsComponent.reloading && AnimatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f)
						{
							burstState = false;
							burstShotsFired = 0;
							doReleaseFire = false;
							if (fireModeSelectable && semiAuto)
							{
								semiAuto = false;
								fireModeState = false;
								if (projectileCount < 2)
								{
									muzzleFlashReduction = 6.5f;
								}
								else
								{
									muzzleFlashReduction = 2f;
								}
								otherfx.volume = 1f;
								otherfx.clip = noammoSnd;
								otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
								if (burstAndAuto && !burstFire)
								{
									burstFire = true;
								}
							}
							else if (fireModeSelectable && !semiAuto)
							{
								if (!burstAndAuto)
								{
									semiAuto = true;
								}
								else if (burstFire)
								{
									burstFire = false;
								}
								else
								{
									semiAuto = true;
								}
								fireModeState = false;
								if (projectileCount < 2)
								{
									muzzleFlashReduction = 3.5f;
								}
								else
								{
									muzzleFlashReduction = 2f;
								}
								otherfx.volume = 1f;
								otherfx.clip = noammoSnd;
								otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
							}
						}
					}
					else
					{
						fireModeState = true;
					}
					if (Time.time - shootStartTime > fireRate + 0.2f)
					{
						bulletsJustFired = 0;
					}
				}
				if (canShoot || FPSWalkerComponent.hideWeapon || FPSWalkerComponent.crouched || (FPSWalkerComponent.midPos < FPSWalkerComponent.standingCamHeight && FPSWalkerComponent.proneRisen) || IronsightsComponent.reloading || FPSWalkerComponent.cancelSprint || !FPSWalkerComponent.moving)
				{
					if (sprintAnimState)
					{
						PlayerWeaponsComponent.sprintSwitchTime = Time.time;
						AnimatorComponent.SetTrigger("SprintingReverse");
						sprintAnimState = false;
					}
				}
				else if (!sprintAnimState)
				{
					PlayerWeaponsComponent.sprintSwitchTime = Time.time;
					AnimatorComponent.SetTrigger("Sprinting");
					sprintAnimState = true;
				}
			}
		}
		otherfx.pitch = Time.timeScale;
		firefx.pitch = firePitch * Time.timeScale;
		autoFireAsource1.pitch = firefx.pitch;
		autoFireAsource2.pitch = firefx.pitch;
		if (!(Time.timeScale > 0f))
		{
			return;
		}
		if (!CameraControlComponent.thirdPersonActive)
		{
			muzzleRendererComponent = FPmuzzleRenderer;
		}
		else
		{
			muzzleRendererComponent = TPmuzzleRenderer;
			if ((bool)FPmuzzleRenderer)
			{
				FPmuzzleRenderer.enabled = false;
			}
		}
		if (!unarmed && FPSPlayerComponent.hitPoints > 0f)
		{
			if ((bool)muzzleFlash)
			{
				if ((bool)muzzleRendererComponent && muzzleRendererComponent.enabled)
				{
					if (muzzleFlashColor.a > 0f)
					{
						muzzleFlashColor.a -= muzzleFlashReduction * Time.deltaTime;
						if (muzzleFlashColor.a < 0f)
						{
							muzzleFlashColor.a = 0f;
						}
						muzzleRendererComponent.material.SetColor("_TintColor", muzzleFlashColor);
					}
					else
					{
						muzzleRendererComponent.enabled = false;
					}
				}
			}
			else if ((bool)muzzleFlash)
			{
				muzzleRendererComponent.enabled = false;
			}
			if ((bool)muzzleLightObj)
			{
				if (!CameraControlComponent.thirdPersonActive)
				{
					if (muzzleLightComponent.enabled)
					{
						if (muzzleLightComponent.intensity > 0f)
						{
							if (Time.time - shootStartTime > muzzleLightDelay)
							{
								muzzleLightComponent.intensity -= muzzleLightReduction * Time.deltaTime;
							}
						}
						else
						{
							muzzleLightComponent.enabled = false;
						}
					}
				}
				else
				{
					muzzleLightComponent.enabled = false;
					muzzleLightComponent.intensity = 0f;
				}
			}
			if (InputComponent.reloadPress && !IronsightsComponent.reloading && ammo > 0 && doReload && bulletsLeft < bulletsPerClip && Time.time - shootStartTime > fireRate && !InputComponent.fireHold)
			{
				sprintReloadState = true;
				StartCoroutine("Reload");
			}
			if (fireOnRelease && ammo > 0 && !meleeActive && !CameraControlComponent.rotating && !PlayerWeaponsComponent.displayingGrenade && !FPSWalkerComponent.holdingObject && !FPSWalkerComponent.hideWeapon && !hideAnimState)
			{
				if (((InputComponent.fireHold && !PlayerWeaponsComponent.offhandThrowActive) || (weaponNumber == PlayerWeaponsComponent.grenadeWeapon && PlayerWeaponsComponent.pullGrenadeState)) && Time.time - shootStartTime > fireRate && releaseTimer + releaseTime < Time.time)
				{
					if (fireHoldTimer < maxHoldTime)
					{
						fireHoldTimer += Time.deltaTime;
						fireHoldMult = fireHoldTimer / maxHoldTime;
					}
					if (!pullAnimState && !releaseAnimState)
					{
						WeaponAnimatorComponent.SetTrigger("Pull");
						if (tpWeaponAnimType == 6 && (bool)PlayerModelAnim)
						{
							PlayerModelAnim.SetTrigger("BowPull");
							if ((bool)tpBowAnim && !FPSWalkerComponent.prone && tpBowAnim.isInitialized)
							{
								tpBowAnim.SetTrigger("BowPull");
							}
						}
						if (doCamPullAnim1)
						{
							CameraAnimatorComponent.speed = CamPullAnim1speed;
							CameraAnimatorComponent.SetTrigger("CamMeleeSwingRight");
						}
						if (doCamPullAnim2)
						{
							CameraAnimatorComponent.speed = CamPullAnim2speed;
							CameraAnimatorComponent.SetTrigger("CamSwitch");
						}
						if ((bool)pullSnd)
						{
							otherfx.volume = 1f;
							otherfx.clip = pullSnd;
							otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
						}
						pullTimer = Time.time;
						pullAnimState = true;
						fireOnReleaseState = true;
						doReleaseFire = false;
						releaseAnimState = true;
					}
					if (PlayerWeaponsComponent.pullGrenadeState && weaponNumber == PlayerWeaponsComponent.grenadeWeapon && !InputComponent.grenadeHold)
					{
						PlayerWeaponsComponent.pullGrenadeState = false;
					}
				}
				else if (fireOnReleaseState && pullTimer + pullTime < Time.time && startTime + readyTimeAmt < Time.time)
				{
					pullAnimState = false;
					releaseTimer = Time.time;
					if (!doReload || !IronsightsComponent.reloading)
					{
						WeaponAnimatorComponent.SetTrigger("Fire");
						if (tpWeaponAnimType == 7 && (bool)PlayerModelAnim)
						{
							PlayerModelAnim.SetTrigger("Throw");
						}
						if ((bool)tpBowAnim && tpBowAnim.isInitialized)
						{
							tpBowAnim.SetTrigger("Release");
						}
						if (doCamReleaseAnim1)
						{
							CameraAnimatorComponent.speed = CamReleaseAnim1speed;
							CameraAnimatorComponent.SetTrigger("CamMeleeSwingRight");
						}
						if (doCamReleaseAnim2)
						{
							CameraAnimatorComponent.speed = CamReleaseAnim2speed;
							CameraAnimatorComponent.SetTrigger("CamSwitch");
						}
					}
					fireOnReleaseState = false;
				}
				if (!fireOnReleaseState && releaseTimer + releaseTime < Time.time)
				{
					if (releaseAnimState)
					{
						if (!doReload || !IronsightsComponent.reloading)
						{
							doReleaseFire = true;
							otherfx.Stop();
						}
						releaseAnimState = false;
					}
					if (PlayerWeaponsComponent.offhandThrowActive && weaponNumber == PlayerWeaponsComponent.grenadeWeapon)
					{
						PlayerWeaponsComponent.grenadeThrownState = true;
					}
				}
			}
			if (offhandMeleeAttack && !FPSWalkerComponent.holdingObject && !FPSWalkerComponent.hideWeapon && !CameraControlComponent.rotating && ((MouseLookComponent.inputY > -70f && !allowDownwardMelee) || allowDownwardMelee))
			{
				if ((InputComponent.meleePress || (meleeIfNoAmmo && meleeSwingDelay == 0f && ammo == 0 && bulletsLeft == 0 && InputComponent.firePress)) && !meleeActive && Time.time - shootStartTime > fireRate && lastMeleeTime + meleeAttackTime < Time.time && !PlayerWeaponsComponent.offhandThrowActive && fireHoldMult == 0f && startTime + readyTimeAmt < Time.time && (!FPSWalkerComponent.prone || !FPSWalkerComponent.moving) && !burstState)
				{
					meleeActive = true;
					lastMeleeTime = Time.time;
					meleeBlendState = true;
					lastFireTime = 0.0;
					nextFireTime = 0.0;
					StopCoroutine("Reload");
					IronsightsComponent.reloading = false;
					otherfx.clip = null;
					Fire();
				}
				if (lastMeleeTime + meleeAttackTime < Time.time)
				{
					meleeActive = false;
					if (meleeBlendState && bulletsToReload != bulletsPerClip)
					{
						meleeBlendState = false;
					}
				}
			}
			if ((InputComponent.fireHold && !fireOnRelease && !meleeActive) || (fireOnRelease && doReleaseFire && !waterFireState))
			{
				if (semiAuto)
				{
					if (!semiState)
					{
						Fire();
						semiState = true;
					}
				}
				else if (!burstFire)
				{
					Fire();
				}
				else if (!burstState && !burstHold)
				{
					burstState = true;
					if (burstShotsFired < burstShots)
					{
						burstHold = true;
					}
				}
			}
			else
			{
				semiState = false;
				burstHold = false;
			}
			if (burstState && burstFire)
			{
				if (!canShoot || IronsightsComponent.reloading || PlayerWeaponsComponent.switching || startTime + readyTimeAmt > Time.time)
				{
					burstState = false;
					burstShotsFired = 0;
				}
				if (burstShotsFired < burstShots)
				{
					Fire();
				}
				else if (burstShotsFired >= burstShots)
				{
					burstState = false;
					burstShotsFired = 0;
				}
			}
			if (Time.time - shootStartTime > 0.2f)
			{
				shooting = false;
			}
		}
		else if (!unarmed && FPSPlayerComponent.hitPoints < 1f && doReleaseFire)
		{
			PlayerWeaponsComponent.offhandThrowActive = false;
		}
		if (Time.smoothDeltaTime > 0f && !unarmed)
		{
			gunAngles.x = Mathf.SmoothDampAngle(gunAngles.x, gunAnglesTarget.x, ref gunAngleVel.x, 0.14f, float.PositiveInfinity, Time.smoothDeltaTime);
			gunAngles.y = Mathf.SmoothDampAngle(gunAngles.y, gunAnglesTarget.y + WeaponPivotComponent.animOffsetTarg.y, ref gunAngleVel.y, 0.14f, float.PositiveInfinity, Time.smoothDeltaTime);
			gunAngles.z = Mathf.SmoothDampAngle(gunAngles.z, gunAnglesTarget.z, ref gunAngleVel.z, 0.14f, float.PositiveInfinity, Time.smoothDeltaTime);
			myTransform.localEulerAngles = gunAngles;
		}
		if (!useLight)
		{
			return;
		}
		if (!CameraControlComponent.thirdPersonActive || FPSPlayerComponent.zoomed)
		{
			spot.transform.position = mainCamTransform.position;
			spot.transform.rotation = lightBaseObj.rotation;
			if (flipLightFacing)
			{
				spot.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
			}
		}
		else if ((bool)PlayerCharacterComponent)
		{
			spot.transform.position = PlayerCharacterComponent.transform.position - PlayerCharacterComponent.transform.forward * 1.2f + PlayerCharacterComponent.transform.up * 0.5f + PlayerCharacterComponent.transform.right * 0.5f;
			spot.transform.rotation = PlayerCharacterComponent.transform.rotation;
		}
		if (!InputComponent.flashlightPress && (!useZoomSwitch || !InputComponent.zoomPress))
		{
			return;
		}
		otherfx.volume = 1f;
		otherfx.clip = noammoSnd;
		otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
		if (!lightOn)
		{
			if ((bool)lightConeMesh)
			{
				lightConeMesh.enabled = true;
			}
			if ((bool)spot)
			{
				spot.enabled = true;
			}
			if ((bool)point)
			{
				point.enabled = true;
			}
			lightOn = true;
		}
		else
		{
			if ((bool)lightConeMesh)
			{
				lightConeMesh.enabled = false;
			}
			if ((bool)spot)
			{
				spot.enabled = false;
			}
			if ((bool)point)
			{
				point.enabled = false;
			}
			lightOn = false;
		}
	}

	private void Fire()
	{
		if ((!meleeActive && ((bulletsLeft <= 0 && doReload) || (!doReload && ammo <= 0) || (semiAuto && semiState))) || CameraControlComponent.rotating || (disableFiring && CameraControlComponent.thirdPersonActive))
		{
			return;
		}
		if (FPSWalkerComponent.holdingBreath && !meleeActive && (!fireableUnderwater || FPSWalkerComponent.lowerGunForSwim))
		{
			if (cantFireState)
			{
				otherfx.volume = 1f;
				otherfx.clip = noammoSnd;
				otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
				cantFireState = false;
				waterFireState = true;
			}
		}
		else
		{
			if (((bulletsToReload != bulletsPerClip || IronsightsComponent.reloading) && !meleeActive && (doReload || ammo <= 0) && (IronsightsComponent.reloading || bulletsToReload == bulletsPerClip || bulletsLeft <= 0) && (!IronsightsComponent.reloading || bulletsToReload == bulletsPerClip || !((float)bulletsLeft >= (float)bulletsToReload * 2f) || !(reloadEndTime + reloadTime < Time.time))) || ((!canShoot || PlayerWeaponsComponent.switching) && !doReleaseFire && !meleeActive) || (!(recoveryTime + recoveryTimeAmt < Time.time) && !CameraControlComponent.thirdPersonActive && !doReleaseFire && !meleeActive) || !(startTime + readyTimeAmt < Time.time))
			{
				return;
			}
			if (AudioSettings.dspTime != oldTime && fireRate < 0.3f)
			{
				time = AudioSettings.dspTime;
				audioDisabled = false;
			}
			else
			{
				time = Time.time;
				if (!audioDisabled)
				{
					lastFireTime = 0.0;
					nextFireTime = 0.0;
					audioDisabled = true;
				}
			}
			if (lastFireTime + (double)(fireRate * 0.8f) > time)
			{
				return;
			}
			if (time > lastFireTime + firingPauseMinimumTime)
			{
				nextFireTime = time;
			}
			if ((Time.timeScale < 1f && Time.time < shootStartTime + fireRate) || !(time > nextFireTime - schedulingTime))
			{
				return;
			}
			firePitch = Random.Range(0.96f, 1f);
			if (!meleeActive)
			{
				if (Time.timeScale == 1f)
				{
					PlayerAltAutoFireSources(nextFireTime);
				}
				else
				{
					firefx.clip = fireSnd;
					firefx.PlayOneShot(firefx.clip, fireVol);
				}
				silentShots = initialSilentShots;
			}
			else
			{
				firefx.clip = meleeSnd;
				firefx.PlayOneShot(firefx.clip, fireVol);
				silentShots = true;
			}
			lastFireTime = nextFireTime;
			nextFireTime += fireRate;
			if (bulletsToReload != bulletsPerClip)
			{
				bulletsReloaded = 0;
				reloadStartTime = -16f;
				reloadEndTime = -16f;
				reloadLastStartTime = -16f;
			}
			StopCoroutine(FireOneShot());
			StartCoroutine(FireOneShot());
			StopCoroutine(Reload());
			IronsightsComponent.reloading = false;
			otherfx.clip = null;
			if (meleeSwingDelay == 0f && !meleeActive)
			{
				bulletsJustFired++;
				StartCoroutine("MuzzFlash");
				if (spawnShell)
				{
					StartCoroutine("SpawnShell");
				}
			}
			if (!FPSWalkerComponent.holdingObject)
			{
				shootStartTime = Time.time;
				shooting = true;
				doReleaseFire = false;
			}
		}
	}

	public void PlayerAltAutoFireSources(double playTime)
	{
		firefx.clip = fireSnd;
		autoFireAsource2.clip = fireSnd;
		if (curAutofireAsource)
		{
			autoFireAsource1.volume = fireVol;
			autoFireAsource1.PlayScheduled(playTime);
			curAutofireAsource = false;
		}
		else
		{
			autoFireAsource2.volume = fireVol;
			autoFireAsource2.PlayScheduled(playTime);
			curAutofireAsource = true;
		}
	}

	private IEnumerator FireOneShot()
	{
		if (meleeActive && FPSWalkerComponent.sprintActive)
		{
			yield return new WaitForSeconds(0.15f);
		}
		hitCount = 0;
		if (meleeSwingDelay == 0f && !meleeActive)
		{
			if (useWeaponKick)
			{
				WeaponKick();
				WeaponAnimatorComponent.SetTrigger("Fire");
			}
			if (doReload)
			{
				bulletsLeft--;
			}
			else
			{
				ammo--;
			}
			if (burstFire)
			{
				burstShotsFired++;
			}
			if ((bool)PlayerModelAnim)
			{
				PlayerModelAnim.SetTrigger("Fire");
			}
			if (tpUseShotgunAnims && (bool)tpShotgunAnim && tpShotgunAnim.isInitialized)
			{
				tpShotgunAnim.SetTrigger("Pump");
			}
			if ((bool)tpPistolAnim && (bool)tpPistolAnim && tpPistolAnim.isInitialized)
			{
				tpPistolAnim.SetTrigger("Fire");
			}
		}
		else
		{
			if ((swingSide || FPSPlayerComponent.canBackstab) && !meleeActive)
			{
				CameraAnimatorComponent.speed = 1.7f;
				CameraAnimatorComponent.SetTrigger("CamMeleeSwingRight");
				WeaponAnimatorComponent.SetTrigger("MeleeSwingRight");
				if ((bool)PlayerModelAnim)
				{
					PlayerModelAnim.SetTrigger("SwingRight");
				}
				swingSide = false;
			}
			else
			{
				CameraAnimatorComponent.speed = 1.6f;
				CameraAnimatorComponent.SetTrigger("CamMeleeSwingLeft");
				if (!meleeActive)
				{
					WeaponAnimatorComponent.SetTrigger("MeleeSwingLeft");
					if ((bool)PlayerModelAnim)
					{
						PlayerModelAnim.SetTrigger("SwingLeft");
					}
					swingSide = true;
				}
				else
				{
					WeaponAnimatorComponent.SetTrigger("Melee");
					if ((bool)PlayerModelAnim)
					{
						PlayerModelAnim.SetTrigger("OffhandMelee");
					}
				}
			}
			if (!meleeActive)
			{
				if (!CameraControlComponent.thirdPersonActive)
				{
					yield return new WaitForSeconds(meleeSwingDelay);
				}
				else
				{
					yield return new WaitForSeconds(tpMeleeSwingDelay);
				}
			}
			else
			{
				yield return new WaitForSeconds(offhandMeleeDelay);
			}
		}
		for (float num = 0f; num < (float)projectileCount; num += 1f)
		{
			direction = SprayDirection();
			Vector3 vector = playerObj.transform.position + playerObj.transform.up * 0.8f;
			float num2 = Vector3.Distance(mainCamTransform.position, vector);
			float num3 = Vector3.Distance(playerObj.transform.position, mainCamTransform.position) * 0.9f;
			RaycastHit hitInfo;
			Vector3 vector2 = ((!Physics.Raycast(mainCamTransform.position + mainCamTransform.forward * num2, mainCamTransform.forward + direction, out hitInfo, range, bulletMask)) ? (mainCamTransform.position + (mainCamTransform.forward + direction) * 20f) : hitInfo.point);
			lookDirection = (vector2 - vector).normalized;
			if (projectilePoolIndex != 0 && !meleeActive)
			{
				float num4 = ((!MouseLookComponent.dzAiming) ? projSpawnForward : 0.5f);
				if (!CameraControlComponent.thirdPersonActive)
				{
					projectile = AzuObjectPool.instance.SpawnPooledObj(projectilePoolIndex, mainCamTransform.position + mainCamTransform.forward * num4, mainCamTransform.rotation);
				}
				else
				{
					projectile = AzuObjectPool.instance.SpawnPooledObj(projectilePoolIndex, mainCamTransform.position + mainCamTransform.forward * num3, mainCamTransform.rotation);
				}
				Physics.IgnoreCollision(projectile.GetComponent<Collider>(), FPSWalkerComponent.capsule, true);
				if ((bool)projectile.transform.GetComponent<ArrowObject>())
				{
					ArrowObject component = projectile.transform.GetComponent<ArrowObject>();
					component.InitializeProjectile();
					component.damageAddAmt = component.damageAdd * fireHoldMult;
					component.velFactor = projectileForce * fireHoldMult / projectileForce;
					component.objectPoolIndex = projectilePoolIndex;
					if ((Camera.main.nearClipPlane < 0.1f || CameraControlComponent.thirdPersonActive) && !MouseLookComponent.dzAiming)
					{
						component.visibleDelay = 0.2f;
					}
					else
					{
						component.visibleDelay = 0f;
					}
				}
				projBody = projectile.GetComponent<Rigidbody>();
				projBody.velocity = Vector3.zero;
				projBody.angularVelocity = Vector3.zero;
				if (pullProjectileForce)
				{
					projectileForceAmt = projectileForce * fireHoldMult + minimumProjForce;
				}
				else
				{
					projectileForceAmt = projectileForce;
				}
				if (!CameraControlComponent.thirdPersonActive)
				{
					projBody.AddForce(direction * projectileForceAmt, ForceMode.Impulse);
				}
				else
				{
					projBody.AddForce(mainCamTransform.forward * projectileForceAmt, ForceMode.Impulse);
				}
				if (projRotUp > 0f || projRotSide > 0f)
				{
					projBody.maxAngularVelocity = 10f;
					projBody.AddRelativeTorque(Vector3.up * Random.Range(projRotSide, projRotSide * 1.5f));
					projBody.AddRelativeTorque(Vector3.right * Random.Range(projRotUp, projRotUp * 1.5f));
				}
				if ((bool)projectile.GetComponent<GrenadeObject>())
				{
					projectile.GetComponent<GrenadeObject>().fuseTimeAmt = fuseTime * (1f - fireHoldMult) + 0.2f;
				}
				fireHoldTimer = 0f;
				fireHoldMult = 0f;
				continue;
			}
			RaycastHit hitInfo2;
			if (meleeSwingDelay == 0f && !meleeActive)
			{
				if (!CameraControlComponent.thirdPersonActive)
				{
					if ((MouseLookComponent.dzAiming && Physics.Raycast(mainCamTransform.position, weaponLookDirection + direction, out hitInfo2, range, bulletMask)) || (!MouseLookComponent.dzAiming && Physics.Raycast(mainCamTransform.position, direction, out hitInfo2, range, bulletMask)))
					{
						HitObject(hitInfo2, weaponLookDirection + direction);
						if (hitInfo2.transform.tag == "Water" && Physics.Raycast(hitInfo2.point, direction, out hitInfo2, range, liquidMask))
						{
							HitObject(hitInfo2, weaponLookDirection + direction, true);
						}
					}
				}
				else if (Physics.Raycast(vector, lookDirection, out hitInfo2, range, bulletMask))
				{
					if (hitInfo2.distance < 1.5f)
					{
						HitObject(hitInfo2, lookDirection);
						if (hitInfo2.transform.tag == "Water" && Physics.Raycast(hitInfo2.point, direction, out hitInfo2, range, liquidMask))
						{
							HitObject(hitInfo2, lookDirection, true);
						}
					}
					else if (Physics.Raycast(mainCamTransform.position + mainCamTransform.forward * num3, mainCamTransform.forward + direction, out hitInfo2, range, bulletMask))
					{
						HitObject(hitInfo2, mainCamTransform.forward + direction);
						if (hitInfo2.transform.tag == "Water" && Physics.Raycast(hitInfo2.point, direction, out hitInfo2, range, liquidMask))
						{
							HitObject(hitInfo2, mainCamTransform.forward + direction, true);
						}
					}
				}
				if (useTracers)
				{
					StartCoroutine(EmitTracers(direction));
				}
				continue;
			}
			if (meleeActive)
			{
				rangeAmt = offhandMeleeRange;
			}
			else
			{
				rangeAmt = range;
			}
			if (!CameraControlComponent.thirdPersonActive)
			{
				if (Physics.SphereCast(mainCamTransform.position, capsule.radius * 0.3f, weaponLookDirection + direction, out hitInfo2, rangeAmt + rangeAmt * FPSWalkerComponent.playerHeightMod * 0.5f, bulletMask))
				{
					HitObject(hitInfo2, direction);
				}
				else if (Physics.Raycast(mainCamTransform.position, weaponLookDirection + direction, out hitInfo2, rangeAmt, bulletMask))
				{
					HitObject(hitInfo2, direction);
				}
			}
			else if (Physics.SphereCast(mainCamTransform.position, capsule.radius * 0.3f, mainCamTransform.forward, out hitInfo2, rangeAmt + CameraControlComponent.zoomDistance + rangeAmt * FPSWalkerComponent.playerHeightMod * 0.5f, bulletMask))
			{
				HitObject(hitInfo2, mainCamTransform.forward);
			}
			else if (Physics.Raycast(mainCamTransform.position, mainCamTransform.forward, out hitInfo2, rangeAmt + CameraControlComponent.zoomDistance, bulletMask))
			{
				HitObject(hitInfo2, mainCamTransform.forward);
			}
			if (meleeActive)
			{
				break;
			}
		}
	}

	private void HitObject(RaycastHit hit, Vector3 directionArg, bool isSecondCast = false)
	{
		if ((bool)hit.rigidbody && hit.rigidbody.useGravity)
		{
			hit.rigidbody.AddForceAtPosition(force * directionArg / (Time.fixedDeltaTime * 100f), hit.point);
		}
		if (meleeActive)
		{
			damageAmt = offhandMeleeDamage;
		}
		else
		{
			damageAmt = damage;
		}
		switch (hit.collider.gameObject.layer)
		{
		case 0:
			if ((bool)hit.collider.gameObject.GetComponent<AppleFall>())
			{
				hit.collider.gameObject.GetComponent<AppleFall>().ApplyDamage(damageAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			if ((bool)hit.collider.gameObject.GetComponent<BreakableObject>())
			{
				hit.collider.gameObject.GetComponent<BreakableObject>().ApplyDamage(damageAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			else if ((bool)hit.collider.gameObject.GetComponent<ExplosiveObject>())
			{
				hit.collider.gameObject.GetComponent<ExplosiveObject>().ApplyDamage(damageAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			else if ((bool)hit.collider.gameObject.GetComponent<MineExplosion>())
			{
				hit.collider.gameObject.GetComponent<MineExplosion>().ApplyDamage(damageAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			break;
		case 1:
			if ((bool)hit.collider.gameObject.GetComponent<BreakableObject>())
			{
				hit.collider.gameObject.GetComponent<BreakableObject>().ApplyDamage(damageAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			break;
		case 13:
			if ((bool)hit.collider.gameObject.GetComponent<CharacterDamage>() && hit.collider.gameObject.GetComponent<AI>().enabled)
			{
				hit.collider.gameObject.GetComponent<CharacterDamage>().ApplyDamage(damageAmt, directionArg, mainCamTransform.position, myTransform, true, false);
				FPSPlayerComponent.UpdateHitTime();
			}
			if ((bool)hit.collider.gameObject.GetComponent<LocationDamage>() && hit.collider.gameObject.GetComponent<LocationDamage>().AIComponent.enabled)
			{
				hit.collider.gameObject.GetComponent<LocationDamage>().ApplyDamage(damageAmt, directionArg, mainCamTransform.position, myTransform, true, false);
				FPSPlayerComponent.UpdateHitTime();
			}
			break;
		}
		if (isSecondCast)
		{
			return;
		}
		if (hit.collider.gameObject.tag == "Flesh")
		{
			hitCount++;
			if (hitCount < 4)
			{
				if (meleeSwingDelay == 0f && !meleeActive)
				{
					WeaponEffectsComponent.ImpactEffects(hit.collider, hit.point - mainCamTransform.forward * 0.2f, false, false, hit.normal);
				}
				else
				{
					WeaponEffectsComponent.ImpactEffects(hit.collider, hit.point - mainCamTransform.forward * 0.2f, false, true, hit.normal);
				}
			}
		}
		else if (meleeSwingDelay == 0f && !meleeActive)
		{
			WeaponEffectsComponent.ImpactEffects(hit.collider, hit.point, false, false, hit.normal);
			if (!hit.collider.isTrigger)
			{
				WeaponEffectsComponent.BulletMarks(hit, false);
			}
		}
		else
		{
			WeaponEffectsComponent.ImpactEffects(hit.collider, hit.point, false, true, hit.normal);
			if (!hit.collider.isTrigger)
			{
				WeaponEffectsComponent.BulletMarks(hit, true);
			}
		}
	}

	private IEnumerator EmitTracers(Vector3 tracerDirection)
	{
		while (!muzzActive && CameraControlComponent.thirdPersonActive && !IronsightsComponent.reloading)
		{
			yield return null;
		}
		if (!CameraControlComponent.thirdPersonActive)
		{
			Vector3 position = mainCamTransform.position + mainCamTransform.right * tracerOffset.x + mainCamTransform.up * tracerOffset.y + mainCamTransform.forward * smokeForward + weaponLookDirection * tracerOffset.z;
			WeaponEffectsComponent.BulletTracers(weaponLookDirection + tracerDirection, position, tracerDist, tracerSwimDist);
		}
		else
		{
			WeaponEffectsComponent.BulletTracers(lookDirection + tracerDirection, thirdPersonSmokePos.position, tracerDistTp);
		}
	}

	private IEnumerator MuzzFlash()
	{
		if (!muzzleFlash)
		{
			yield break;
		}
		while (!muzzActive && CameraControlComponent.thirdPersonActive && !IronsightsComponent.reloading)
		{
			yield return null;
		}
		if (!FPSWalkerComponent.holdingBreath)
		{
			if ((bool)muzzleLightObj && !CameraControlComponent.thirdPersonActive)
			{
				muzzleLightComponent.enabled = true;
				muzzleLightComponent.intensity = 8f;
			}
			if (useMuzzleSmoke && (bool)muzzleSmokeParticles)
			{
				muzzleSmokeColor.a = muzzleSmokeAlpha;
				muzzleSmokeComponent.material.SetColor("_TintColor", muzzleSmokeColor);
				if (!CameraControlComponent.thirdPersonActive)
				{
					if (!FPSPlayerComponent.zoomed)
					{
						muzzleSmokeParticles.transform.position = mainCamTransform.position + mainCamTransform.right * muzzleSmokeOffset.x + mainCamTransform.up * muzzleSmokeOffset.y + mainCamTransform.forward * smokeForward + weaponLookDirection * muzzleSmokeOffset.z;
					}
					else
					{
						muzzleSmokeParticles.transform.position = mainCamTransform.position + mainCamTransform.forward * smokeForward + weaponLookDirection * muzzleSmokeOffset.z;
					}
				}
				else
				{
					muzzleSmokeParticles.transform.position = thirdPersonSmokePos.position;
				}
				muzzleSmokeParticles.Emit(Mathf.RoundToInt(muzzleSmokeParticles.emissionRate));
			}
		}
		if ((bool)barrelSmokeParticles && bulletsJustFired >= barrelSmokeShots && !barrelSmokeActive)
		{
			StartCoroutine(BarrelSmoke());
			barrelSmokeActive = true;
		}
		if (!FPSWalkerComponent.holdingBreath)
		{
			muzzleFlashColor.r = 1f;
			muzzleFlashColor.g = 1f;
			muzzleFlashColor.b = 1f;
		}
		else
		{
			muzzleFlashColor.r = PlayerWeaponsComponent.waterMuzzleFlashColor.r;
			muzzleFlashColor.g = PlayerWeaponsComponent.waterMuzzleFlashColor.g;
			muzzleFlashColor.b = PlayerWeaponsComponent.waterMuzzleFlashColor.b;
		}
		muzzleFlashColor.a = Random.Range(0.4f, 0.5f);
		muzzleRendererComponent.material.SetColor("_TintColor", muzzleFlashColor);
		muzzleRendererComponent.enabled = true;
		if (!CameraControlComponent.thirdPersonActive)
		{
			muzzleFlash.localRotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.forward);
		}
		else
		{
			muzzleRendererComponent.transform.localRotation *= Quaternion.Euler(0f, 0f, Random.value * 360f);
		}
	}

	private IEnumerator BarrelSmoke()
	{
		while (!muzzActive && CameraControlComponent.thirdPersonActive)
		{
			yield return null;
		}
		float barrelSmokeTime = Time.time;
		yield return new WaitForSeconds(0.45f);
		while (barrelSmokeTime + barrelSmokeDuration > Time.time)
		{
			if (lastMeleeTime + meleeAttackTime * 1f < Time.time && !FPSWalkerComponent.sprintActive && !PlayerWeaponsComponent.offhandThrowActive)
			{
				if (!InputComponent.fireHold || IronsightsComponent.reloading || (shootStartTime + 0.1f < Time.time && InputComponent.fireHold))
				{
					if (!CameraControlComponent.thirdPersonActive)
					{
						if (!FPSPlayerComponent.zoomed || WeaponPivotComponent.deadzoneZooming)
						{
							barrelSmokeParticles.transform.position = mainCamTransform.position + mainCamTransform.right * barrelSmokeOffset.x + mainCamTransform.up * barrelSmokeOffset.y + mainCamTransform.forward * smokeForward + weaponLookDirection * barrelSmokeOffset.z;
						}
						else
						{
							barrelSmokeParticles.transform.position = mainCamTransform.position + mainCamTransform.forward * smokeForward + weaponLookDirection * barrelSmokeOffset.z;
						}
					}
					else
					{
						barrelSmokeParticles.transform.position = thirdPersonSmokePos.position;
					}
					if (Time.timeScale > 0f)
					{
						barrelSmokeParticles.Emit(Mathf.RoundToInt(barrelSmokeParticles.emissionRate));
					}
				}
				yield return null;
				continue;
			}
			barrelSmokeActive = false;
			break;
		}
		barrelSmokeActive = false;
	}

	private IEnumerator SpawnShell()
	{
		while (!muzzActive && CameraControlComponent.thirdPersonActive && !IronsightsComponent.reloading)
		{
			yield return null;
		}
		if (shellEjectDelay > 0f)
		{
			yield return new WaitForSeconds(shellEjectDelay);
		}
		if (!CameraControlComponent.thirdPersonActive)
		{
			if ((bool)shellEjectPositionZoom && FPSPlayerComponent.zoomed && !WeaponPivotComponent.deadzoneZooming)
			{
				shellEjectPos = shellEjectPositionZoom;
			}
			else
			{
				shellEjectPos = shellEjectPosition;
			}
		}
		else
		{
			shellEjectPos = shellEjectPositionTP;
		}
		shell = AzuObjectPool.instance.SpawnPooledObj(shellRBPoolIndex, shellEjectPos.position, shellEjectPos.transform.rotation);
		ShellEjection ShellEjectionComponent = shell.GetComponent<ShellEjection>();
		ShellEjectionComponent.playerObj = playerObj;
		if (MouseLookComponent.dzAiming)
		{
			ShellEjectionComponent.dzAiming = true;
		}
		else
		{
			ShellEjectionComponent.dzAiming = false;
		}
		ShellEjectionComponent.PlayerRigidbodyComponent = playerObj.GetComponent<Rigidbody>();
		ShellEjectionComponent.WeaponBehaviorComponent = this;
		ShellEjectionComponent.RigidbodyComponent = ShellEjectionComponent.gameObject.GetComponent<Rigidbody>();
		ShellEjectionComponent.FPSPlayerComponent = FPSPlayerComponent;
		ShellEjectionComponent.gunObj = base.transform.gameObject;
		shell.transform.parent = shellEjectPosition.transform.parent;
		ShellEjectionComponent.RBPoolIndex = shellRBPoolIndex;
		if (!CameraControlComponent.thirdPersonActive)
		{
			shell.transform.localScale = shellScale;
		}
		else if (shellScale.x > 3f)
		{
			shell.transform.localScale = shellScale * 0.65f;
		}
		else
		{
			shell.transform.localScale = shellScale * 1.5f;
		}
		shellEjectDirection = new Vector3(shellSide * 0.7f + shellSide * 0.4f * Random.value, shellUp * 0.6f + shellUp * 0.5f * Random.value, shellForward * 0.4f + shellForward * 0.2f * Random.value);
		if (!CameraControlComponent.thirdPersonActive)
		{
			ShellEjectionComponent.RigidbodyComponent.AddForce(base.transform.TransformDirection(shellEjectDirection) * shellForce, ForceMode.Impulse);
		}
		else
		{
			Vector3 vector = shellEjectPos.TransformDirection(shellEjectDirection);
			if (shellScale.x < 3f)
			{
				ShellEjectionComponent.RigidbodyComponent.AddForce(vector * shellForce * 1.5f, ForceMode.Impulse);
			}
			else
			{
				ShellEjectionComponent.RigidbodyComponent.AddForce(vector * shellForce, ForceMode.Impulse);
			}
		}
		ShellEjectionComponent.InitializeShell();
	}

	public bool CheckForReload()
	{
		if (!IronsightsComponent.reloading && ammo > 0 && doReload && bulletsLeft < bulletsPerClip && Time.time - shootStartTime > fireRate && !InputComponent.fireHold)
		{
			sprintReloadState = true;
			StartCoroutine("Reload");
			return true;
		}
		return false;
	}

	private IEnumerator Reload()
	{
		if (meleeActive || FPSWalkerComponent.hideWeapon || !(Time.timeSinceLevelLoad > 2f) || (FPSWalkerComponent.sprintActive && (Mathf.Abs(horizontal) > 0.75f || Mathf.Abs(vertical) > 0.75f) && !FPSWalkerComponent.cancelSprint && !FPSWalkerComponent.sprintReload) || ammo <= 0)
		{
			yield break;
		}
		FPSPlayerComponent.zoomed = false;
		if (bulletsToReload == bulletsPerClip)
		{
			otherfx.volume = 1f;
			otherfx.clip = reloadSnd;
			otherfx.Play();
			CameraAnimatorComponent.speed = 1f;
			if (!PistolSprintAnim)
			{
				if (weaponNumber == 5 || weaponNumber == 6 || weaponNumber == 7)
				{
					CameraAnimatorComponent.SetTrigger("CamReloadAK47");
				}
				else
				{
					CameraAnimatorComponent.SetTrigger("CamReloadMP5");
				}
			}
			else
			{
				CameraAnimatorComponent.SetTrigger("CamReloadPistol");
			}
			WeaponAnimatorComponent.SetTrigger("Reload");
		}
		IronsightsComponent.reloading = true;
		reloadStartTime = Time.time;
		burstState = false;
		burstShotsFired = 0;
		if ((bulletsToReload != bulletsPerClip && bulletsReloaded > 0) || bulletsToReload == bulletsPerClip)
		{
			yield return new WaitForSeconds(reloadTime);
		}
		bulletsNeeded = bulletsPerClip - bulletsLeft;
		if (bulletsToReload == bulletsPerClip)
		{
			IronsightsComponent.reloading = false;
			if (ammo >= bulletsNeeded)
			{
				ammo -= bulletsNeeded;
				bulletsLeft = bulletsPerClip;
			}
			else
			{
				bulletsLeft += ammo;
				ammo = 0;
			}
			yield break;
		}
		if (bulletsNeeded >= bulletsToReload)
		{
			if (ammo >= bulletsToReload)
			{
				bulletsLeft += bulletsToReload;
				ammo -= bulletsToReload;
				bulletsReloaded += bulletsToReload;
			}
			else
			{
				bulletsLeft += ammo;
				ammo = 0;
			}
		}
		else if (ammo >= bulletsNeeded)
		{
			bulletsLeft += bulletsNeeded;
			ammo -= bulletsNeeded;
			bulletsReloaded += bulletsToReload;
		}
		else
		{
			bulletsLeft += ammo;
			ammo = 0;
		}
		if (bulletsNeeded > 0)
		{
			StartCoroutine("Reload");
			WeaponAnimatorComponent.SetTrigger("Reload");
			CameraAnimatorComponent.SetTrigger("CamReloadSingle");
			if (bulletsNeeded <= bulletsToReload || ammo <= 0)
			{
				otherfx.clip = reloadLastSnd;
				WeaponAnimatorComponent.SetTrigger("Neutral");
				if (!FPSWalkerComponent.prone && (bool)PlayerModelAnim)
				{
					PlayerModelAnim.SetTrigger("ReloadLast");
					if (tpUseShotgunAnims)
					{
						tpShotgunAnim.SetTrigger("Pump");
					}
				}
				reloadLastStartTime = Time.time;
				IronsightsComponent.reloading = false;
			}
			else
			{
				otherfx.clip = reloadSnd;
				if (!FPSWalkerComponent.prone && (bool)PlayerModelAnim)
				{
					PlayerModelAnim.SetTrigger("ReloadSingle");
				}
			}
			otherfx.volume = 1f;
			otherfx.pitch = Random.Range(0.95f * Time.timeScale, 1f * Time.timeScale);
			otherfx.PlayOneShot(otherfx.clip, 1f / otherfx.volume);
			reloadEndTime = Time.time;
		}
		else
		{
			IronsightsComponent.reloading = false;
			bulletsReloaded = 0;
		}
	}

	private Vector3 SprayDirection()
	{
		float num = 1f;
		float num2 = 1.2f;
		num = ((!FPSWalkerComponent.crouched) ? 1f : 0.75f);
		if (FPSPlayerComponent.zoomed && meleeSwingDelay == 0f)
		{
			if (fireModeSelectable && semiAuto)
			{
				shotSpreadAmt = shotSpread / 5f * num;
			}
			else
			{
				shotSpreadAmt = shotSpread / 3f * num;
			}
		}
		else if (fireModeSelectable && semiAuto)
		{
			shotSpreadAmt = shotSpread / 2f * num;
		}
		else
		{
			shotSpreadAmt = shotSpread * num;
		}
		num2 = ((!useRecoilIncrease) ? 1f : ((bulletsJustFired <= shotsBeforeRecoil) ? 1.2f : Mathf.Pow(bulletsJustFired - (shotsBeforeRecoil - 1), num2 / aimDirRecoilIncrease)));
		float x = (1f - 2f * Random.value) * shotSpreadAmt * num2;
		float y = (1f - 2f * Random.value) * shotSpreadAmt * num2;
		float z = 1f;
		if (!CameraControlComponent.thirdPersonActive)
		{
			return myTransform.TransformDirection(new Vector3(x, y, z));
		}
		return Camera.main.transform.TransformDirection(new Vector3(x, y, z));
	}

	private void WeaponKick()
	{
		if (CameraControlComponent.thirdPersonActive)
		{
			return;
		}
		float num = 1.2f;
		if ((FPSPlayerComponent.zoomed || FPSWalkerComponent.prone) && meleeSwingDelay == 0f)
		{
			kickUpAmt = kickUp;
			kickSideAmt = kickSide;
			kickRollAmt = kickRoll;
		}
		else
		{
			kickRollAmt = kickRoll * 0.5f;
			if (!FPSWalkerComponent.crouched || (FPSWalkerComponent.crouched && Mathf.Abs(horizontal) == 0f && Mathf.Abs(vertical) == 0f))
			{
				kickUpAmt = kickUp * 1.75f;
				kickSideAmt = kickSide * 1.75f;
			}
			else
			{
				kickUpAmt = kickUp * 2.75f;
				kickSideAmt = kickSide * 2.75f;
			}
		}
		float num2 = Random.Range((0f - kickSideAmt) * 2f, kickSideAmt * 2f);
		float num3 = Random.Range(kickUpAmt * 1.5f, kickUpAmt * 2f);
		if (!MouseLookComponent.dzAiming)
		{
			if (Random.value <= 0.5f)
			{
				randZkick = Random.Range(0f - kickRollAmt, (0f - kickRollAmt) * 0.5f);
			}
			else
			{
				randZkick = Random.Range(kickRollAmt * 0.5f, kickRollAmt);
			}
			randZkick += randZkick * 0.5f;
			Mathf.Clamp(randZkick, 0f - kickRollAmt, kickRollAmt);
		}
		kickRotation = Quaternion.Euler(mainCamTransform.localRotation.eulerAngles - new Vector3(num3, num2, 0f));
		mainCamTransform.localRotation = Quaternion.Slerp(mainCamTransform.localRotation, kickRotation, 0.1f);
		if (useRecoilIncrease)
		{
			num = ((bulletsJustFired > shotsBeforeRecoil && !FPSWalkerComponent.prone) ? Mathf.Pow(bulletsJustFired - (shotsBeforeRecoil - 1), num / viewKickIncrease) : (FPSWalkerComponent.prone ? 0.5f : 1.2f));
			if (useViewClimb && !MouseLookComponent.dzAiming)
			{
				if (viewClimbUp > 0f)
				{
					MouseLookComponent.recoilY += num3 / 8f * viewClimbUp * (num / 6f);
				}
				if (viewClimbSide > 0f || viewClimbRight > 0f)
				{
					MouseLookComponent.recoilX += (num2 / 4f * viewClimbSide + viewClimbRight) * (num / 2f);
				}
			}
		}
		else if (useViewClimb && !MouseLookComponent.dzAiming)
		{
			if (viewClimbUp > 0f)
			{
				MouseLookComponent.recoilY += num3 / 8f * viewClimbUp;
			}
			if (viewClimbSide > 0f || viewClimbRight > 0f)
			{
				MouseLookComponent.recoilX += num2 / 4f * viewClimbSide + viewClimbRight;
			}
		}
	}

	public void CancelWeaponPull(bool neutralAnim = false)
	{
		releaseAnimState = false;
		fireOnReleaseState = false;
		pullAnimState = false;
		fireHoldTimer = 0f;
		fireHoldMult = 0f;
		otherfx.Stop();
		if (neutralAnim && weaponNumber != 0)
		{
			WeaponAnimatorComponent.SetTrigger("Neutral");
		}
	}
}
