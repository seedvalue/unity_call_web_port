using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSPlayer : MonoBehaviour
{
	[HideInInspector]
	public Ironsights IronsightsComponent;

	[HideInInspector]
	public InputControl InputComponent;

	[HideInInspector]
	public FPSRigidBodyWalker FPSWalkerComponent;

	[HideInInspector]
	public PlayerWeapons PlayerWeaponsComponent;

	[HideInInspector]
	public WorldRecenter WorldRecenterComponent;

	[HideInInspector]
	public WeaponBehavior WeaponBehaviorComponent;

	[HideInInspector]
	public SmoothMouseLook MouseLookComponent;

	[HideInInspector]
	public WeaponEffects WeaponEffectsComponent;

	[HideInInspector]
	public WeaponPivot WeaponPivotComponent;

	[HideInInspector]
	public CameraControl CameraControlComponent;

	[HideInInspector]
	public NPCRegistry NPCRegistryComponent;

	[HideInInspector]
	public GameObject NPCMgrObj;

	private AI AIComponent;

	[HideInInspector]
	public GameObject[] children;

	[HideInInspector]
	public GameObject weaponCameraObj;

	[HideInInspector]
	public GameObject weaponObj;

	[Tooltip("Reference to the UI Canvas object.")]
	public GameObject canvasObj;

	[Tooltip("Object reference to the GUITexture object in the project library that renders pain effects on screen.")]
	public GameObject painFadeObj;

	private PainFade painFadeComponent;

	private Image painFadeImage;

	public GameObject levelLoadFadeObj;

	private LevelLoadFade levelLoadFadeRef;

	[Tooltip("Object reference to the Text object in the project library that renders health amounts on screen.")]
	public GameObject healthUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders health amounts on screen.")]
	public GameObject healthUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders ammo amounts on screen.")]
	public GameObject ammoUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders ammo amounts on screen.")]
	public GameObject ammoUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders hunger amounts on screen.")]
	public GameObject hungerUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders hunger amounts on screen.")]
	public GameObject hungerUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders thirst amounts on screen.")]
	public GameObject thirstUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders thirst amounts on screen.")]
	public GameObject thirstUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders help text on screen.")]
	public GameObject helpGuiObj;

	[Tooltip("Object reference to the Text object in the project library that renders help text on screen.")]
	public GameObject helpGuiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders hhelp amounts on screen.")]
	public GameObject waveUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders wave text on screen.")]
	public GameObject waveUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders wave amounts on screen.")]
	public GameObject warmupUiObj;

	[Tooltip("Object reference to the Text object in the project library that renders warmup text on screen.")]
	public GameObject warmupUiObjShadow;

	[Tooltip("Object reference to the Text object in the project library that renders warmup text on screen.")]
	public GameObject crosshairUiObj;

	[HideInInspector]
	public Image crosshairUiImage;

	[HideInInspector]
	public RectTransform crosshairUiRect;

	[Tooltip("Object reference to the GUITexture object in the project library that renders crosshair on screen.")]
	public GameObject hitmarkerUiObj;

	[HideInInspector]
	public Image hitmarkerUiImage;

	[HideInInspector]
	public RectTransform hitmarkerUiRect;

	private HealthText HealthText;

	private HealthText HealthText2;

	private Text healthUiText;

	private HungerText HungerText;

	private HungerText HungerText2;

	private Text HungerUIText;

	private ThirstText ThirstText;

	private ThirstText ThirstText2;

	private Text ThirstUIText;

	[HideInInspector]
	public float crosshairWidth;

	[Tooltip("Size of crosshair relative to screen size.")]
	public float crosshairSize;

	private float oldWidth;

	[HideInInspector]
	public float hitTime = -10f;

	[HideInInspector]
	public bool hitMarkerState;

	private Transform mainCamTransform;

	[Tooltip("True if the prefab parent object will be removed on scene load.")]
	public bool removePrefabRoot = true;

	public float hitPoints = 100f;

	public float maximumHitPoints = 200f;

	[Tooltip("True if player's health should be displayed on the screen.")]
	public bool showHealth = true;

	[Tooltip("True if player's ammo should be displayed on the screen.")]
	public bool showAmmo = true;

	[Tooltip("True if negative hitpoint values should be shown.")]
	public bool showHpUnderZero = true;

	[Tooltip("True if player cannot take damage.")]
	public bool invulnerable;

	[Tooltip("True if the player regenerates their health after health regen delay elapses without player taking damage.")]
	public bool regenerateHealth;

	[Tooltip("The maximum amount of hitpoints that should be regenerated.")]
	public float maxRegenHealth = 100f;

	[Tooltip("Delay after being damaged that the player should start to regenerate health.")]
	public float healthRegenDelay = 7f;

	[Tooltip("Rate at which the player should regenerate health.")]
	public float healthRegenRate = 25f;

	private float timeLastDamaged;

	[Tooltip("True if player should have a hunger attribute that increases over time.")]
	public bool usePlayerHunger;

	[HideInInspector]
	public float maxHungerPoints = 100f;

	[Tooltip("Seconds it takes for player to accumulate 1 hunger point.")]
	public float hungerInterval = 7f;

	[HideInInspector]
	public float hungerPoints;

	private float lastHungerTime;

	private float lastStarveTime;

	[Tooltip("Seconds to wait before starve damaging again (should be less than healthRegenDelay to prevent healing of starvation damage).")]
	public float starveInterval = 3f;

	[Tooltip("Anount of damage to apply per starve interval.")]
	public float starveDmgAmt = -5f;

	[Tooltip("True if player should have a thirst attribute that increases over time.")]
	public bool usePlayerThirst;

	[HideInInspector]
	public float maxThirstPoints = 100f;

	[Tooltip("Seconds it takes for player to accumulate 1 thirst point.")]
	public float thirstInterval = 7f;

	[HideInInspector]
	public float thirstPoints;

	private float lastThirstTime;

	private float lastThirstDmgTime;

	[Tooltip("Seconds to wait before thirst damaging again (should be less than healthRegenDelay to prevent healing of thirst damage).")]
	public float thirstDmgInterval = 3f;

	[Tooltip("Amount to damage player per thirst damage interval.")]
	public float thirstDmgAmt = -5f;

	[Tooltip("True if player can activate bullet time by pressing button (default T).")]
	public bool allowBulletTime = true;

	[Tooltip("True if help text should be displayed.")]
	public bool showHelpText = true;

	[Tooltip("True if pause (default: Tab) should hide cursor.")]
	public bool pauseHidesCursor = true;

	private float gotHitTimer = -1f;

	private Color PainColor = Color.white;

	private Color painFadeColor;

	[Tooltip("Amount to kick the player's camera view when damaged.")]
	public float painScreenKickAmt = 0.016f;

	[Tooltip("Percentage of normal time to use when in bullet time.")]
	[Range(0f, 1f)]
	public float bulletTimeSpeed = 0.35f;

	[Tooltip("Movement multiplier during bullet time.")]
	public float sloMoMoveSpeed = 2f;

	private float pausedTime;

	[HideInInspector]
	public bool bulletTimeActive;

	[HideInInspector]
	public float backstabBtTime;

	[HideInInspector]
	public bool backstabBtState;

	private float initialFixedTime;

	[HideInInspector]
	public float usePressTime;

	[HideInInspector]
	public float useReleaseTime;

	private bool useState;

	[HideInInspector]
	public bool pressButtonUpState;

	[HideInInspector]
	public Collider objToPickup;

	private bool zoomBtnState = true;

	private float zoomStopTime;

	[HideInInspector]
	public bool zoomed;

	[HideInInspector]
	public float zoomStart = -2f;

	[HideInInspector]
	public bool zoomStartState;

	[HideInInspector]
	public float zoomEnd;

	[HideInInspector]
	public bool zoomEndState;

	private float zoomDelay = 0.4f;

	[HideInInspector]
	public bool dzAiming;

	[Tooltip("Enable or disable the aiming reticle.")]
	public bool crosshairEnabled = true;

	private bool crosshairVisibleState = true;

	private bool crosshairTextureState;

	[Tooltip("Set to true to display swap reticle when item under reticle will replace current weapon.")]
	public bool useSwapReticle = true;

	[Tooltip("The texture used for the aiming crosshair.")]
	public Sprite aimingReticle;

	[Tooltip("The texture used for the hitmarker.")]
	public Sprite hitmarkerReticle;

	[Tooltip("The texture used for the pick up crosshair.")]
	public Sprite pickupReticle;

	[Tooltip("The texture used for when the weapon under reticle will replace current weapon.")]
	public Sprite swapReticle;

	[Tooltip("The texture used for showing that weapon under reticle cannot be picked up.")]
	public Sprite noPickupReticle;

	[Tooltip("The texture used for the pick up crosshair.")]
	private Sprite pickupTex;

	private Color pickupReticleColor = Color.white;

	[HideInInspector]
	public Color reticleColor = Color.white;

	private Color initialReticleColor;

	private Color hitmarkerColor = Color.white;

	[Tooltip("Layers to include for crosshair raycast in hit detection.")]
	public LayerMask rayMask;

	[Tooltip("Distance that player can pickup and activate items.")]
	public float reachDistance = 2.1f;

	private RaycastHit hit;

	private RaycastHit hit2;

	private Vector3 camCrosshairPos;

	[HideInInspector]
	public bool raycastCrosshair;

	private bool pickUpBtnState = true;

	[HideInInspector]
	public bool restarting;

	public AudioClip painLittle;

	public AudioClip painBig;

	public AudioClip painDrown;

	public AudioClip gasp;

	public AudioClip catchBreath;

	public AudioClip die;

	public AudioClip dieDrown;

	public AudioClip jumpfx;

	public AudioClip enterBulletTimeFx;

	public AudioClip exitBulletTimeFx;

	public AudioClip hitMarker;

	[Tooltip("Particle effect to play when player blocks attack.")]
	public GameObject blockParticles;

	[Tooltip("Distance from camera to emit blocking particle effect.")]
	public float blockParticlesPos;

	private ParticleSystem blockParticleSys;

	private AudioSource[] aSources;

	[HideInInspector]
	public AudioSource otherfx;

	[HideInInspector]
	public AudioSource hitmarkfx;

	private bool bullettimefxstate;

	[HideInInspector]
	public bool blockState;

	[HideInInspector]
	public float blockAngle;

	[HideInInspector]
	public bool canBackstab;

	private float moveCommandedTime;

	[HideInInspector]
	public bool menuDisplayed;

	[HideInInspector]
	public float menuTime;

	[HideInInspector]
	public float pauseTime;

	[HideInInspector]
	public bool paused;

	private MainMenu MainMenuComponent;

	private Transform myTransform;

	private RaycastHit hit3;

	public LayerMask bulletMask;

	private void Start()
	{
		if (removePrefabRoot)
		{
			GameObject obj = base.transform.parent.transform.gameObject;
			base.transform.parent.transform.DetachChildren();
			Object.Destroy(obj);
		}
		mainCamTransform = Camera.main.transform;
		myTransform = base.transform;
		IronsightsComponent = GetComponent<Ironsights>();
		InputComponent = GetComponent<InputControl>();
		FPSWalkerComponent = GetComponent<FPSRigidBodyWalker>();
		WorldRecenterComponent = GetComponent<WorldRecenter>();
		MouseLookComponent = mainCamTransform.parent.transform.GetComponent<SmoothMouseLook>();
		CameraControlComponent = mainCamTransform.GetComponent<CameraControl>();
		weaponObj = CameraControlComponent.weaponObj;
		WeaponEffectsComponent = weaponObj.GetComponent<WeaponEffects>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		painFadeObj.SetActive(true);
		painFadeComponent = painFadeObj.GetComponent<PainFade>();
		painFadeComponent.painImageComponent = painFadeComponent.GetComponent<Image>();
		PainColor = painFadeComponent.painImageComponent.color;
		painFadeObj.SetActive(false);
		MainMenuComponent = mainCamTransform.parent.transform.GetComponent<MainMenu>();
		menuDisplayed = false;
		canvasObj.SetActive(true);
		NPCMgrObj = GameObject.Find("NPC Manager");
		NPCRegistryComponent = NPCMgrObj.GetComponent<NPCRegistry>();
		aSources = GetComponents<AudioSource>();
		otherfx = aSources[0];
		hitmarkfx = aSources[1];
		otherfx.spatialBlend = 0f;
		hitmarkfx.spatialBlend = 0f;
		Time.timeScale = 1f;
		initialFixedTime = Time.fixedDeltaTime;
		usePressTime = 0f;
		useReleaseTime = -8f;
		Physics.IgnoreLayerCollision(11, 12);
		Physics.IgnoreLayerCollision(12, 12);
		Physics.IgnoreLayerCollision(8, 2);
		Physics.IgnoreLayerCollision(8, 13);
		Physics.IgnoreLayerCollision(8, 12);
		Physics.IgnoreLayerCollision(8, 11);
		Physics.IgnoreLayerCollision(8, 10);
		levelLoadFadeRef = levelLoadFadeObj.GetComponent<LevelLoadFade>();
		levelLoadFadeRef.LevelLoadFadeobj = levelLoadFadeObj;
		levelLoadFadeRef.fadeImage = levelLoadFadeObj.GetComponent<Image>();
		levelLoadFadeRef.FadeAndLoadLevel(Color.black, 1.5f, true);
		if (showHelpText)
		{
			helpGuiObj.SetActive(true);
			helpGuiObjShadow.SetActive(true);
		}
		else
		{
			helpGuiObj.SetActive(false);
			helpGuiObjShadow.SetActive(false);
		}
		crosshairUiImage = crosshairUiObj.GetComponent<Image>();
		crosshairUiRect = crosshairUiObj.GetComponent<RectTransform>();
		crosshairUiImage.sprite = aimingReticle;
		hitmarkerUiImage = hitmarkerUiObj.GetComponent<Image>();
		hitmarkerUiRect = hitmarkerUiObj.GetComponent<RectTransform>();
		hitmarkerUiImage.sprite = hitmarkerReticle;
		hitmarkerUiImage.enabled = false;
		pickupReticleColor.a = 0.95f;
		initialReticleColor = crosshairUiImage.color;
		if (crosshairEnabled)
		{
			reticleColor.a = initialReticleColor.a;
			hitmarkerUiImage.color = hitmarkerColor;
		}
		else
		{
			reticleColor.a = 0f;
			crosshairUiImage.color = reticleColor;
			hitmarkerUiImage.color = reticleColor;
		}
		HealthText = healthUiObj.GetComponent<HealthText>();
		HealthText2 = healthUiObjShadow.GetComponent<HealthText>();
		HealthText.healthGui = hitPoints;
		HealthText2.healthGui = hitPoints;
		healthUiText = HealthText.GetComponent<Text>();
		healthUiText.material.color = Color.white;
		if (!showHealth)
		{
			healthUiObj.gameObject.SetActive(false);
		}
		HungerText = hungerUiObj.GetComponent<HungerText>();
		HungerText2 = hungerUiObjShadow.GetComponent<HungerText>();
		HungerText.hungerGui = hungerPoints;
		HungerText2.hungerGui = hungerPoints;
		HungerUIText = HungerText.GetComponent<Text>();
		if (!usePlayerHunger)
		{
			hungerUiObj.SetActive(false);
			hungerUiObjShadow.SetActive(false);
		}
		ThirstText = thirstUiObj.GetComponent<ThirstText>();
		ThirstText2 = thirstUiObjShadow.GetComponent<ThirstText>();
		ThirstText.thirstGui = thirstPoints;
		ThirstText2.thirstGui = thirstPoints;
		ThirstUIText = ThirstText.GetComponent<Text>();
		if (!usePlayerThirst)
		{
			thirstUiObj.SetActive(false);
			thirstUiObjShadow.SetActive(false);
		}
		if (PlayerPrefs.GetInt("Game Type") != 2)
		{
			waveUiObj.SetActive(false);
			waveUiObjShadow.SetActive(false);
			warmupUiObj.SetActive(false);
			warmupUiObjShadow.SetActive(false);
		}
		else
		{
			waveUiObj.SetActive(true);
			waveUiObjShadow.SetActive(true);
			warmupUiObj.SetActive(true);
			warmupUiObjShadow.SetActive(true);
		}
	}

	private void LateUpdate()
	{
		if (MouseLookComponent.dzAiming || raycastCrosshair)
		{
			if (!WeaponBehaviorComponent.unarmed && Physics.Raycast(mainCamTransform.position, WeaponBehaviorComponent.weaponLookDirection, out hit, 100f, rayMask))
			{
				camCrosshairPos = Camera.main.WorldToViewportPoint(hit.point);
			}
			else if (WeaponBehaviorComponent.unarmed)
			{
				camCrosshairPos = new Vector3(0.5f, 0.5f, 0f);
			}
			else
			{
				camCrosshairPos = Camera.main.WorldToViewportPoint(WeaponBehaviorComponent.origin + WeaponPivotComponent.childTransform.forward * 200f);
			}
		}
		else
		{
			camCrosshairPos = new Vector3(0.5f, 0.5f, 0f);
		}
		crosshairUiRect.anchorMax = camCrosshairPos;
		crosshairUiRect.anchorMin = camCrosshairPos;
		hitmarkerUiRect.anchorMax = camCrosshairPos;
		hitmarkerUiRect.anchorMin = camCrosshairPos;
		hitmarkerColor.a = 0.7f;
		hitmarkerUiImage.color = hitmarkerColor;
	}

	private void Update()
	{
		if (!(PlayerPrefs.GetString("isManualShoot", "YES") == "YES"))
		{
			WeaponBehavior currentWeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
			if (!CameraControlComponent.thirdPersonActive && Physics.Raycast(mainCamTransform.position, currentWeaponBehaviorComponent.weaponLookDirection, out hit3, 1000f, bulletMask))
			{
				Debug.DrawLine(mainCamTransform.position, hit3.point, Color.blue);
				if (hit3.transform.gameObject.layer == 13)
				{
					base.gameObject.GetComponent<InputControl>().fireHold = true;
				}
				else
				{
					base.gameObject.GetComponent<InputControl>().fireHold = false;
				}
			}
			else
			{
				base.gameObject.GetComponent<InputControl>().fireHold = false;
			}
		}
		if (InputComponent.menuPress && MainMenuComponent.useMainMenu)
		{
			if (!menuDisplayed)
			{
				MainMenuComponent.enabled = true;
				menuDisplayed = true;
			}
			else
			{
				MainMenuComponent.enabled = false;
				paused = false;
				menuDisplayed = false;
			}
			if (Time.timeScale > 0f || paused)
			{
				if (!paused)
				{
					menuTime = Time.timeScale;
				}
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = menuTime;
			}
		}
		if (InputComponent.pausePress && pauseHidesCursor)
		{
			if (Time.timeScale > 0f)
			{
				paused = true;
				pauseTime = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
			{
				paused = false;
				Time.timeScale = pauseTime;
			}
		}
		if (allowBulletTime)
		{
			if (InputComponent.bulletTimePress)
			{
				if (!bulletTimeActive)
				{
					FPSWalkerComponent.moveSpeedMult = Mathf.Clamp(sloMoMoveSpeed, 1f, sloMoMoveSpeed);
					bulletTimeActive = true;
				}
				else
				{
					FPSWalkerComponent.moveSpeedMult = 1f;
					bulletTimeActive = false;
				}
			}
			otherfx.pitch = Time.timeScale;
			hitmarkfx.pitch = Time.timeScale;
			if (Time.timeScale > 0f && !restarting)
			{
				Time.fixedDeltaTime = initialFixedTime * Time.timeScale;
				if (bulletTimeActive)
				{
					if (!bullettimefxstate)
					{
						otherfx.clip = enterBulletTimeFx;
						otherfx.PlayOneShot(otherfx.clip, 1f);
						bullettimefxstate = true;
					}
					Time.timeScale = Mathf.MoveTowards(Time.timeScale, bulletTimeSpeed, Time.deltaTime * 3f);
				}
				else
				{
					if (bullettimefxstate)
					{
						otherfx.clip = exitBulletTimeFx;
						otherfx.PlayOneShot(otherfx.clip, 1f);
						FPSWalkerComponent.moveSpeedMult = 1f;
						bullettimefxstate = false;
					}
					if (1f - Mathf.Abs(Time.timeScale) > 0.05f)
					{
						Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.deltaTime * 3f);
					}
					else
					{
						Time.timeScale = 1f;
					}
				}
			}
		}
		switch (IronsightsComponent.zoomMode)
		{
		case Ironsights.zoomType.both:
			zoomDelay = 0.4f;
			break;
		case Ironsights.zoomType.hold:
			zoomDelay = 0f;
			break;
		case Ironsights.zoomType.toggle:
			zoomDelay = 999f;
			break;
		}
		if (regenerateHealth && hitPoints < maxRegenHealth && timeLastDamaged + healthRegenDelay < Time.time)
		{
			HealPlayer(healthRegenRate * Time.deltaTime);
		}
		if (usePlayerHunger)
		{
			hungerUiObj.SetActive(true);
			hungerUiObjShadow.SetActive(true);
			if (lastHungerTime + hungerInterval < Time.time)
			{
				UpdateHunger(1f);
			}
			if (hungerPoints == maxHungerPoints && lastStarveTime + starveInterval < Time.time && hitPoints > 0f)
			{
				HealPlayer(starveDmgAmt, true);
				painFadeObj.SetActive(true);
				painFadeComponent.StartCoroutine(painFadeComponent.FadeIn(PainColor, 0.75f));
				if (hitPoints < 1f)
				{
					SendMessage("Die");
				}
				timeLastDamaged = Time.time;
				lastStarveTime = Time.time;
			}
		}
		else if ((bool)hungerUiObj)
		{
			hungerUiObj.SetActive(false);
			hungerUiObjShadow.SetActive(false);
		}
		if (usePlayerThirst)
		{
			thirstUiObj.SetActive(true);
			thirstUiObjShadow.SetActive(true);
			if (lastThirstTime + thirstInterval < Time.time)
			{
				UpdateThirst(1f);
			}
			if (thirstPoints == maxThirstPoints && lastThirstDmgTime + thirstDmgInterval < Time.time && hitPoints > 0f)
			{
				HealPlayer(thirstDmgAmt, true);
				painFadeObj.SetActive(true);
				painFadeComponent.StartCoroutine(painFadeComponent.FadeIn(PainColor, 0.75f));
				if (hitPoints < 1f)
				{
					Die();
				}
				timeLastDamaged = Time.time;
				lastThirstDmgTime = Time.time;
			}
		}
		else if ((bool)thirstUiObj)
		{
			thirstUiObj.SetActive(false);
			thirstUiObjShadow.SetActive(false);
		}
		WeaponBehavior currentWeaponBehaviorComponent2 = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
		if (InputComponent.zoomHold && currentWeaponBehaviorComponent2.canZoom && !blockState && !IronsightsComponent.reloading && !FPSWalkerComponent.proneMove && !FPSWalkerComponent.hideWeapon)
		{
			if (!zoomStartState)
			{
				zoomStart = Time.time;
				zoomStartState = true;
				zoomEndState = false;
				if (zoomEnd - zoomStart < zoomDelay * Time.timeScale)
				{
					if (!zoomed)
					{
						zoomed = true;
					}
					else
					{
						zoomed = false;
					}
				}
			}
		}
		else
		{
			if (!InputComponent.zoomHold)
			{
				blockState = false;
			}
			if (!zoomEndState)
			{
				zoomEnd = Time.time;
				zoomEndState = true;
				zoomStartState = false;
				if (zoomEnd - zoomStart > zoomDelay * Time.timeScale)
				{
					zoomed = false;
				}
			}
		}
		if (FPSWalkerComponent.proneMove)
		{
			zoomEndState = true;
			zoomStartState = false;
			zoomed = false;
		}
		if (zoomed)
		{
			zoomBtnState = false;
		}
		else if (!zoomBtnState)
		{
			zoomStopTime = Time.time;
			zoomBtnState = true;
		}
		UpdateHitmarker();
		if ((IronsightsComponent.reloading || (zoomed && (!dzAiming || currentWeaponBehaviorComponent2.zoomIsBlock) && !currentWeaponBehaviorComponent2.showZoomedCrosshair)) && !CameraControlComponent.thirdPersonActive)
		{
			if ((currentWeaponBehaviorComponent2.meleeSwingDelay == 0f || currentWeaponBehaviorComponent2.zoomIsBlock) && !currentWeaponBehaviorComponent2.unarmed && crosshairVisibleState)
			{
				crosshairUiImage.enabled = false;
				crosshairVisibleState = false;
			}
		}
		else if (((currentWeaponBehaviorComponent2.bulletsPerClip != currentWeaponBehaviorComponent2.bulletsToReload && currentWeaponBehaviorComponent2.reloadLastStartTime + currentWeaponBehaviorComponent2.reloadLastTime < Time.time) || currentWeaponBehaviorComponent2.bulletsPerClip == currentWeaponBehaviorComponent2.bulletsToReload) && zoomStopTime + 0.2f < Time.time && !crosshairVisibleState)
		{
			crosshairUiImage.enabled = true;
			crosshairVisibleState = true;
		}
		if (crosshairEnabled)
		{
			if (currentWeaponBehaviorComponent2.showAimingCrosshair)
			{
				if (!WeaponPivotComponent.deadzoneZooming)
				{
					if (!WeaponPivotComponent.deadzoneLooking)
					{
						reticleColor.a = initialReticleColor.a;
					}
					else
					{
						reticleColor.a = 1f;
					}
				}
				else if (!CameraControlComponent.thirdPersonActive)
				{
					if (zoomed)
					{
						reticleColor.a = 1f;
					}
					else if (WeaponPivotComponent.swayLeadingMode)
					{
						reticleColor.a = initialReticleColor.a;
					}
					else
					{
						reticleColor.a = 0f;
					}
				}
				else
				{
					reticleColor.a = initialReticleColor.a;
				}
				crosshairUiImage.color = reticleColor;
			}
			else
			{
				reticleColor.a = 0f;
				crosshairUiImage.color = reticleColor;
			}
		}
		else
		{
			reticleColor.a = 0f;
			crosshairUiImage.color = reticleColor;
		}
		if (InputComponent.useHold)
		{
			if (!useState)
			{
				usePressTime = Time.time;
				objToPickup = hit.collider;
				useState = true;
			}
		}
		else
		{
			if (useState)
			{
				useReleaseTime = Time.time;
				useState = false;
			}
			pressButtonUpState = false;
		}
		if (!IronsightsComponent.reloading && !currentWeaponBehaviorComponent2.lastReload && !PlayerWeaponsComponent.switching && !FPSWalkerComponent.holdingObject && (!FPSWalkerComponent.canRun || FPSWalkerComponent.inputY == 0f) && FPSWalkerComponent.sprintStopTime + 0.4f < Time.time)
		{
			if ((!CameraControlComponent.thirdPersonActive && Physics.Raycast(mainCamTransform.position, currentWeaponBehaviorComponent2.weaponLookDirection, out hit, reachDistance + FPSWalkerComponent.playerHeightMod, rayMask)) || (CameraControlComponent.thirdPersonActive && Physics.Raycast(mainCamTransform.position + mainCamTransform.forward * (CameraControlComponent.zoomDistance + CameraControlComponent.currentDistance + 0.5f), mainCamTransform.forward, out hit, reachDistance + FPSWalkerComponent.playerHeightMod, rayMask)))
			{
				if (currentWeaponBehaviorComponent2.meleeSwingDelay > 0f && hit.collider.gameObject.layer == 13)
				{
					if ((bool)hit.collider.gameObject.GetComponent<AI>() || (bool)hit.collider.gameObject.GetComponent<LocationDamage>())
					{
						if ((bool)hit.collider.gameObject.GetComponent<AI>())
						{
							AIComponent = hit.collider.gameObject.GetComponent<AI>();
						}
						else
						{
							AIComponent = hit.collider.gameObject.GetComponent<LocationDamage>().AIComponent;
						}
						if (AIComponent.playerIsBehind && AIComponent.CharacterDamageComponent.hitPoints > 0f)
						{
							canBackstab = true;
						}
						else
						{
							canBackstab = false;
						}
					}
					else
					{
						canBackstab = false;
					}
				}
				else
				{
					canBackstab = false;
				}
				if (hit.collider.gameObject.tag == "Usable")
				{
					if (pickUpBtnState && usePressTime - useReleaseTime < 0.4f && usePressTime + 0.4f > Time.time && objToPickup == hit.collider)
					{
						hit.collider.SendMessageUpwards("PickUpItem", myTransform.gameObject, SendMessageOptions.DontRequireReceiver);
						hit.collider.SendMessageUpwards("ActivateObject", SendMessageOptions.DontRequireReceiver);
						pickUpBtnState = false;
						FPSWalkerComponent.cancelSprint = true;
						usePressTime = -8f;
						objToPickup = null;
					}
					if (pickUpBtnState)
					{
						if ((bool)hit.collider.gameObject.GetComponent<WeaponPickup>())
						{
							WeaponBehavior component = PlayerWeaponsComponent.weaponOrder[hit.collider.gameObject.GetComponent<WeaponPickup>().weaponNumber].GetComponent<WeaponBehavior>();
							WeaponPickup component2 = hit.collider.gameObject.GetComponent<WeaponPickup>();
							if (PlayerWeaponsComponent.totalWeapons == PlayerWeaponsComponent.maxWeapons && component.addsToTotalWeaps)
							{
								if (!component.haveWeapon && !component.dropWillDupe)
								{
									if (!useSwapReticle)
									{
										if ((bool)component2.weaponPickupReticle)
										{
											pickupTex = component2.weaponPickupReticle;
										}
										else
										{
											pickupTex = pickupReticle;
										}
									}
									else
									{
										pickupTex = swapReticle;
									}
								}
								else if (!component2.removeOnUse)
								{
									pickupTex = noPickupReticle;
								}
								else if ((bool)component2.weaponPickupReticle)
								{
									pickupTex = component2.weaponPickupReticle;
								}
								else
								{
									pickupTex = pickupReticle;
								}
							}
							else if ((!component.haveWeapon && !component.dropWillDupe) || component2.removeOnUse)
							{
								if ((bool)component2.weaponPickupReticle)
								{
									pickupTex = component2.weaponPickupReticle;
								}
								else
								{
									pickupTex = pickupReticle;
								}
							}
							else
							{
								pickupTex = noPickupReticle;
							}
						}
						else if ((bool)hit.collider.gameObject.GetComponent<HealthPickup>())
						{
							HealthPickup component3 = hit.collider.gameObject.GetComponent<HealthPickup>();
							if ((bool)component3)
							{
								hit.collider.gameObject.SendMessage("PickUpItem", base.gameObject);
							}
							if ((bool)component3.healthPickupReticle)
							{
								pickupTex = component3.healthPickupReticle;
							}
							else
							{
								pickupTex = pickupReticle;
							}
						}
						else if ((bool)hit.collider.gameObject.GetComponent<AmmoPickup>())
						{
							AmmoPickup component4 = hit.collider.gameObject.GetComponent<AmmoPickup>();
							if ((bool)component4.ammoPickupReticle)
							{
								pickupTex = component4.ammoPickupReticle;
							}
							else
							{
								pickupTex = pickupReticle;
							}
						}
						else
						{
							pickupTex = pickupReticle;
						}
					}
					UpdateReticle(false);
				}
				else
				{
					objToPickup = null;
					if (hit.collider.gameObject.layer == 13)
					{
						if ((bool)hit.collider.gameObject.GetComponent<AI>() || (bool)hit.collider.gameObject.GetComponent<LocationDamage>())
						{
							if ((bool)hit.collider.gameObject.GetComponent<AI>())
							{
								AIComponent = hit.collider.gameObject.GetComponent<AI>();
							}
							else
							{
								AIComponent = hit.collider.gameObject.GetComponent<LocationDamage>().AIComponent;
							}
							if (AIComponent.factionNum == 1 && AIComponent.followOnUse && AIComponent.enabled)
							{
								pickupTex = pickupReticle;
								UpdateReticle(false);
								if (pickUpBtnState && InputComponent.useHold)
								{
									AIComponent.CommandNPC();
									pickUpBtnState = false;
									FPSWalkerComponent.cancelSprint = true;
								}
							}
							else
							{
								UpdateReticle(true);
							}
						}
						else if (crosshairTextureState)
						{
							UpdateReticle(true);
						}
					}
					else if (crosshairTextureState)
					{
						UpdateReticle(true);
					}
				}
			}
			else
			{
				canBackstab = false;
				if (crosshairTextureState)
				{
					UpdateReticle(true);
				}
				if (moveCommandedTime + 0.5f < Time.time && ((!CameraControlComponent.thirdPersonActive && Physics.Raycast(mainCamTransform.position, currentWeaponBehaviorComponent2.weaponLookDirection, out hit2, 500f, rayMask)) || (CameraControlComponent.thirdPersonActive && Physics.Raycast(mainCamTransform.position, mainCamTransform.forward, out hit2, 500f, rayMask))) && hit2.collider.gameObject.layer == 0 && pickUpBtnState && InputComponent.useHold)
				{
					NPCRegistryComponent.MoveFolowingNpcs(hit2.point);
					moveCommandedTime = Time.time;
					pickUpBtnState = false;
				}
			}
		}
		else
		{
			canBackstab = false;
			if (crosshairTextureState)
			{
				UpdateReticle(true);
			}
		}
		if (InputComponent.useHold)
		{
			pickUpBtnState = false;
		}
		else
		{
			pickUpBtnState = true;
		}
	}

	public void UpdateReticle(bool reticleType)
	{
		if (!reticleType)
		{
			crosshairUiImage.sprite = pickupTex;
			crosshairUiImage.color = pickupReticleColor;
			crosshairTextureState = true;
		}
		else
		{
			crosshairUiImage.sprite = aimingReticle;
			crosshairUiImage.color = reticleColor;
			crosshairTextureState = false;
		}
	}

	private void UpdateHitmarker()
	{
		if (hitTime + 0.3f > Time.time)
		{
			if (!hitMarkerState && WeaponBehaviorComponent.meleeSwingDelay == 0f && !WeaponBehaviorComponent.meleeActive)
			{
				hitmarkerUiImage.enabled = true;
				hitmarkfx.clip = hitMarker;
				hitmarkfx.PlayOneShot(hitmarkfx.clip, 1f);
				hitMarkerState = true;
			}
		}
		else
		{
			if (hitMarkerState)
			{
				hitMarkerState = false;
			}
			hitmarkerUiImage.enabled = false;
		}
	}

	public void UpdateHitTime()
	{
		hitTime = Time.time;
		hitMarkerState = false;
	}

	public IEnumerator ActivateBulletTime(float duration)
	{
		if (!bulletTimeActive)
		{
			bulletTimeActive = true;
			float startTime = Time.time;
			while (!(startTime + duration < Time.time))
			{
				yield return new WaitForSeconds(0.1f);
			}
			bulletTimeActive = false;
		}
	}

	public void HealPlayer(float healAmt, bool isHungryThirsty = false)
	{
		if (hitPoints < 1f)
		{
			return;
		}
		if (hitPoints + healAmt > maximumHitPoints)
		{
			hitPoints = maximumHitPoints;
		}
		else if (healAmt < 0f)
		{
			if (!isHungryThirsty)
			{
				ApplyDamage(healAmt);
			}
			else
			{
				hitPoints += healAmt;
			}
		}
		else
		{
			hitPoints += healAmt;
		}
		HealthText.healthGui = Mathf.Round(hitPoints);
		HealthText2.healthGui = Mathf.Round(hitPoints);
		if (hitPoints <= 25f)
		{
			healthUiText.color = Color.red;
		}
		else if (hitPoints <= 40f)
		{
			healthUiText.color = Color.yellow;
		}
		else
		{
			healthUiText.color = HealthText.textColor;
		}
	}

	public void UpdateHunger(float hungerAmt)
	{
		if (!(hitPoints < 1f))
		{
			if (hungerPoints + hungerAmt > maxHungerPoints)
			{
				hungerPoints = maxHungerPoints;
			}
			else
			{
				hungerPoints += hungerAmt;
			}
			hungerPoints = Mathf.Clamp(hungerPoints, 0f, hungerPoints);
			HungerText.hungerGui = Mathf.Round(hungerPoints);
			HungerText2.hungerGui = Mathf.Round(hungerPoints);
			if (hungerPoints <= 65f)
			{
				HungerUIText.color = HungerText.textColor;
			}
			else if (hungerPoints <= 85f)
			{
				HungerUIText.color = Color.yellow;
			}
			else
			{
				HungerUIText.color = Color.red;
			}
			lastHungerTime = Time.time;
		}
	}

	public void UpdateThirst(float thirstAmt)
	{
		if (!(hitPoints < 1f))
		{
			if (thirstPoints + thirstAmt > maxThirstPoints)
			{
				thirstPoints = maxThirstPoints;
			}
			else
			{
				thirstPoints += thirstAmt;
			}
			thirstPoints = Mathf.Clamp(thirstPoints, 0f, thirstPoints);
			ThirstText.thirstGui = Mathf.Round(thirstPoints);
			ThirstText2.thirstGui = Mathf.Round(thirstPoints);
			if (thirstPoints <= 65f)
			{
				ThirstUIText.color = ThirstText.textColor;
			}
			else if (thirstPoints <= 85f)
			{
				ThirstUIText.color = Color.yellow;
			}
			else
			{
				ThirstUIText.color = Color.red;
			}
			lastThirstTime = Time.time;
		}
	}

	public void ApplyDamage(float damage, Transform attacker = null, bool isMeleeAttack = false)
	{
		if (hitPoints < 1f)
		{
			if (!showHpUnderZero)
			{
				hitPoints = 0f;
			}
			return;
		}
		if (attacker != null && WeaponBehaviorComponent.zoomIsBlock && WeaponBehaviorComponent.blockDefenseAmt > 0f && zoomed && ((WeaponBehaviorComponent.onlyBlockMelee && isMeleeAttack) || !WeaponBehaviorComponent.onlyBlockMelee) && WeaponBehaviorComponent.shootStartTime + WeaponBehaviorComponent.fireRate < Time.time)
		{
			Vector3 normalized = (attacker.position - myTransform.position).normalized;
			blockAngle = Vector3.Dot(normalized, myTransform.forward);
			if (Vector3.Dot(normalized, myTransform.forward) > WeaponBehaviorComponent.blockCoverage)
			{
				damage *= 1f - WeaponBehaviorComponent.blockDefenseAmt;
				otherfx.clip = WeaponBehaviorComponent.blockSound;
				otherfx.PlayOneShot(otherfx.clip, 1f);
				if ((bool)blockParticles)
				{
					if (!CameraControlComponent.thirdPersonActive)
					{
						blockParticles.transform.position = mainCamTransform.position + mainCamTransform.forward * (blockParticlesPos + CameraControlComponent.zoomDistance + CameraControlComponent.currentDistance);
					}
					else
					{
						blockParticles.transform.position = myTransform.position + mainCamTransform.forward * blockParticlesPos + base.transform.up * 0.5f;
					}
					foreach (Transform item in blockParticles.transform)
					{
						blockParticleSys = item.GetComponent<ParticleSystem>();
						blockParticleSys.Emit(Mathf.RoundToInt(blockParticleSys.emissionRate));
					}
				}
				blockState = true;
			}
		}
		timeLastDamaged = Time.time;
		int num = 0;
		int num2 = 0;
		if (!invulnerable)
		{
			hitPoints -= damage;
		}
		HealthText.healthGui = Mathf.Round(hitPoints);
		HealthText2.healthGui = Mathf.Round(hitPoints);
		if (hitPoints <= 25f)
		{
			healthUiText.color = Color.red;
		}
		else if (hitPoints <= 40f)
		{
			healthUiText.color = Color.yellow;
		}
		else
		{
			healthUiText.color = HealthText.textColor;
		}
		if (!blockState)
		{
			painFadeColor = PainColor;
			painFadeColor.a = PainColor.a + Random.value * 0.1f;
			painFadeObj.SetActive(true);
			painFadeComponent.StartCoroutine(painFadeComponent.FadeIn(painFadeColor, 0.75f));
		}
		if (!FPSWalkerComponent.holdingBreath)
		{
			if (!blockState && Time.time > gotHitTimer && (bool)painBig && (bool)painLittle)
			{
				if (hitPoints < 40f || damage > 30f)
				{
					otherfx.clip = painBig;
					otherfx.PlayOneShot(otherfx.clip, 1f);
					gotHitTimer = Time.time + Random.Range(0.5f, 0.75f);
				}
				else
				{
					otherfx.clip = painLittle;
					otherfx.PlayOneShot(otherfx.clip, 1f);
					gotHitTimer = Time.time + Random.Range(0.5f, 0.75f);
				}
			}
		}
		else if (Time.time > gotHitTimer && (bool)painDrown)
		{
			otherfx.clip = painDrown;
			otherfx.PlayOneShot(otherfx.clip, 1f);
			gotHitTimer = Time.time + Random.Range(0.5f, 0.75f);
		}
		if (!CameraControlComponent.thirdPersonActive)
		{
			num = Random.Range(100, -100);
			if (num < 50 && num > 0)
			{
				num = 50;
			}
			if (num < 0 && num > -50)
			{
				num = -50;
			}
			num2 = Random.Range(100, -100);
			if (num2 < 50 && num2 > 0)
			{
				num2 = 50;
			}
			if (num2 < 0 && num2 > -50)
			{
				num2 = -50;
			}
			Quaternion b = Quaternion.Euler(mainCamTransform.localRotation.eulerAngles - new Vector3(num, num2, 0f));
			float value = ((!zoomed || WeaponBehaviorComponent.zoomIsBlock) ? (damage / (painScreenKickAmt * 10f)) : (damage / (painScreenKickAmt * 10f) / 3f));
			if (blockState)
			{
				value = 0.025f;
			}
			value = Mathf.Clamp(value, 0f, 0.15f);
			mainCamTransform.localRotation = Quaternion.Slerp(mainCamTransform.localRotation, b, value);
		}
		if (WeaponBehaviorComponent.zoomIsBlock)
		{
			if (!WeaponBehaviorComponent.hitCancelsBlock)
			{
				blockState = false;
			}
			else
			{
				zoomed = false;
			}
		}
		if (hitPoints < 1f)
		{
			SendMessage("Die");
		}
	}

	public void ApplyDamageEmerald(object[] DamageArray)
	{
		ApplyDamage((float)DamageArray[0], (Transform)DamageArray[1], (bool)DamageArray[2]);
	}

	private void Die()
	{
		bulletTimeActive = false;
		if (!FPSWalkerComponent.drowning)
		{
			otherfx.clip = die;
			otherfx.PlayOneShot(otherfx.clip, 1f);
		}
		else
		{
			otherfx.clip = dieDrown;
			otherfx.PlayOneShot(otherfx.clip, 1f);
		}
		FPSWalkerComponent.inputX = 0f;
		FPSWalkerComponent.inputY = 0f;
		FPSWalkerComponent.cancelSprint = true;
		levelLoadFadeRef.StopAllCoroutines();
		GameController.instance.setGameOver();
	}

	public void RestartMap()
	{
		Time.timeScale = 1f;
		levelLoadFadeRef.StopAllCoroutines();
		levelLoadFadeRef.FadeAndLoadLevel(Color.black, 1.2f, false);
		restarting = true;
		FPSWalkerComponent.inputX = 0f;
		FPSWalkerComponent.inputY = 0f;
		FPSWalkerComponent.cancelSprint = true;
		WeaponBehaviorComponent.shooting = false;
	}
}
