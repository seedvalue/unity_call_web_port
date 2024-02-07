using UnityEngine;

public class WaterZone : MonoBehaviour
{
	private FPSRigidBodyWalker FPSWalkerComponent;

	private Footsteps FootstepsComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	private WeaponEffects WeaponEffectsComponent;

	private GameObject playerObj;

	private GameObject weaponObj;

	private bool swimTimeState;

	private AudioSource audioSource;

	[Tooltip("Sound effect to play underwater.")]
	public AudioClip underwaterSound;

	[Tooltip("Above-water ambient audio sources to pause when submerged.")]
	public AudioSource[] aboveWaterAudioSources;

	[Tooltip("The mesh to use for the water surface plane (if flip water plane is false, this will just be used for underwater surface plane).")]
	public Transform waterPlane;

	private float underwaterYpos;

	[Tooltip("The top water surface plane (is deactivated when camera is underwater).")]
	public Transform waterPlaneTop;

	private Vector3 waterPlaneRot;

	[Tooltip("Particles emitted around player treading water.")]
	private ParticleSystem rippleEffect;

	private float rippleEmitTime;

	public float rippleEmitDelay = 0.5f;

	[Tooltip("Particles emitted underwater for ambient bubbles/particles.")]
	private ParticleSystem bubblesEffect;

	private float particlesYPos;

	private float particleForwardPos;

	[Tooltip("Splash particles effect to play when object enter water.")]
	private GameObject waterSplash;

	private Vector3 splashPos;

	private float lastSplashTime;

	[Tooltip("Particles to emit when player is swimming on water surface.")]
	private ParticleSystem splashTrail;

	private Vector3 trailPos;

	private float splashTrailTime;

	[Tooltip("True if sunlight color should be changed when underwater.")]
	public bool changeSunlightColor;

	[Tooltip("Sun/directional light that  should be changed to underwaterSunightColor when player is submerged .")]
	public Light SunlightObj;

	[Tooltip("Color of sunlight when underwater.")]
	public Color underwaterSunightColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	private Color origSunightColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	[Tooltip("True if scene ambient lighting is changed when underwater.")]
	public bool underwaterLightEnabled;

	[Tooltip("Color of underwater scene ambient lighting.")]
	public Color underwaterLightColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	[Tooltip("True if spotlight with underwter caustics (shimmery lighting) should be active when underwater.")]
	public bool useCausticsLight;

	[Tooltip("Spotlight with caustics cookie to activate when underwater.")]
	public Light causticsLight;

	private int index;

	[Tooltip("Frames per second of underwater caustics animation.")]
	public int fps = 30;

	[Tooltip("Frames of underwater caustic animation.")]
	public Texture2D[] frames;

	private bool effectsState;

	[Tooltip("True if underwater fog settings will be applied when submerged.")]
	public bool underwaterFogEnabled;

	[Tooltip("Fog mode when underwater.")]
	public FogMode underwaterFogMode = FogMode.Linear;

	[Tooltip("Fog color when underwater.")]
	public Color underwaterFogColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	[Tooltip("Underwater exponential fog density (if exponential mode is selected).")]
	public float underwaterFogDensity = 0.15f;

	[Tooltip("Underwater linear fog start distance.")]
	public float underwaterLinearFogStart;

	[Tooltip("Underwater linerar fog end distance.")]
	public float underwaterLinearFogEnd = 15f;

	private bool fogEnabled;

	private FogMode origFogMode = FogMode.Linear;

	private Color origFogColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	private Color origLightColor = new Color(0.15f, 0.32f, 0.4f, 1f);

	private float origFogDensity = 0.15f;

	private float origLinearFogStart = 15f;

	private float origLinearFogEnd = 30f;

	private Transform myTransform;

	private Transform mainCamTransform;

	private void Start()
	{
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		playerObj = mainCamTransform.GetComponent<CameraControl>().playerObj;
		weaponObj = mainCamTransform.GetComponent<CameraControl>().weaponObj;
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		FootstepsComponent = mainCamTransform.parent.transform.GetComponent<Footsteps>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		WeaponEffectsComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>().WeaponEffectsComponent;
		waterSplash = WeaponEffectsComponent.waterSplash;
		rippleEffect = WeaponEffectsComponent.rippleEffect;
		bubblesEffect = WeaponEffectsComponent.bubblesEffect;
		splashTrail = WeaponEffectsComponent.splashTrail;
		Physics.IgnoreCollision(myTransform.GetComponent<Collider>(), FPSWalkerComponent.leanCol, true);
		audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = underwaterSound;
		audioSource.loop = true;
		audioSource.volume = 0.8f;
		fogEnabled = RenderSettings.fog;
		origFogColor = RenderSettings.fogColor;
		origFogDensity = RenderSettings.fogDensity;
		origFogMode = RenderSettings.fogMode;
		origLinearFogStart = RenderSettings.fogStartDistance;
		origLinearFogEnd = RenderSettings.fogEndDistance;
		origLightColor = RenderSettings.ambientLight;
		if (!SunlightObj)
		{
			Light[] array = Object.FindObjectsOfType(typeof(Light)) as Light[];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].type == LightType.Directional && array[i].gameObject.activeInHierarchy)
				{
					SunlightObj = array[i];
					break;
				}
			}
		}
		if ((bool)SunlightObj)
		{
			origSunightColor = SunlightObj.color;
		}
		if ((bool)waterPlane)
		{
			waterPlane.gameObject.SetActive(false);
			underwaterYpos = waterPlane.transform.position.y;
		}
		if (useCausticsLight && (bool)causticsLight)
		{
			causticsLight.gameObject.SetActive(false);
		}
		if ((bool)waterPlaneTop)
		{
			waterPlaneTop.gameObject.SetActive(true);
		}
		swimTimeState = false;
		FPSWalkerComponent.inWater = false;
		FPSWalkerComponent.swimming = false;
		FPSWalkerComponent.belowWater = false;
		FPSWalkerComponent.canWaterJump = true;
		FPSWalkerComponent.holdingBreath = false;
		if (frames.Length > 0)
		{
			InvokeRepeating("NextFrame", 0f, 1f / (float)fps);
		}
	}

	private void Update()
	{
		if (!playerObj.activeInHierarchy)
		{
			swimTimeState = false;
			FPSWalkerComponent.inWater = false;
			FPSWalkerComponent.swimming = false;
			FPSWalkerComponent.belowWater = false;
			FPSWalkerComponent.canWaterJump = true;
			FPSWalkerComponent.holdingBreath = false;
			StopUnderwaterEffects();
		}
		if (!myTransform.GetComponent<Collider>().bounds.Contains(mainCamTransform.position - mainCamTransform.up * 0.2f))
		{
			StopUnderwaterEffects();
		}
		else
		{
			StartUnderwaterEffects();
		}
	}

	private void NextFrame()
	{
		if (useCausticsLight && (bool)causticsLight && causticsLight.isActiveAndEnabled)
		{
			index = (index + 1) % frames.Length;
			causticsLight.cookie = frames[index];
		}
	}

	private void OnTriggerStay(Collider col)
	{
		EnterWater(col);
	}

	private void OnTriggerEnter(Collider col)
	{
		string text = col.gameObject.tag;
		Transform transform = col.gameObject.transform;
		if (!(text == "Player") || (!FPSWalkerComponent.jumping && FPSWalkerComponent.grounded))
		{
			switch (text)
			{
			default:
				if (!(col.gameObject.name == "Chest"))
				{
					return;
				}
				break;
			case "Usable":
			case "Metal":
			case "Wood":
			case "Glass":
			case "Flesh":
				break;
			}
			if (!(transform.position.y > myTransform.GetComponent<Collider>().bounds.max.y - 0.4f) || !(lastSplashTime + 0.6f < Time.time))
			{
				return;
			}
		}
		if (text == "Player")
		{
			EnterWater(col);
			if ((bool)FootstepsComponent.waterLand)
			{
				PlayAudioAtPos.PlayClipAt(FootstepsComponent.waterLand, col.gameObject.transform.position, 1f, 0f);
			}
		}
		else if ((bool)FootstepsComponent.waterLand)
		{
			PlayAudioAtPos.PlayClipAt(FootstepsComponent.waterLand, transform.position, 1f);
		}
		splashPos = new Vector3(transform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.01f, transform.position.z);
		waterSplash.transform.position = splashPos;
		foreach (Transform item in waterSplash.transform)
		{
			ParticleSystem component = item.GetComponent<ParticleSystem>();
			component.Emit(Mathf.RoundToInt(component.emissionRate));
		}
		lastSplashTime = Time.time;
	}

	private void EnterWater(Collider col)
	{
		if (!(col.gameObject.tag == "Player"))
		{
			return;
		}
		FPSWalkerComponent.inWater = true;
		if (col.gameObject.GetComponent<Collider>().bounds.max.y - 0.95f <= myTransform.GetComponent<Collider>().bounds.max.y)
		{
			FPSWalkerComponent.swimming = true;
			if (!swimTimeState)
			{
				FPSWalkerComponent.swimStartTime = Time.time;
				swimTimeState = true;
			}
			if (col.gameObject.GetComponent<Collider>().bounds.max.y - 0.9f <= myTransform.GetComponent<Collider>().bounds.max.y)
			{
				FPSWalkerComponent.belowWater = true;
			}
			else
			{
				FPSWalkerComponent.belowWater = false;
			}
		}
		else
		{
			FPSWalkerComponent.swimming = false;
		}
		if (FPSWalkerComponent.eyePos.y <= myTransform.GetComponent<Collider>().bounds.max.y)
		{
			if (!FPSWalkerComponent.holdingBreath)
			{
				FPSWalkerComponent.diveStartTime = Time.time;
				FPSWalkerComponent.holdingBreath = true;
			}
			return;
		}
		FPSWalkerComponent.holdingBreath = false;
		if (FPSWalkerComponent.inputY == 0f && FPSWalkerComponent.inputX == 0f)
		{
			Vector3 position = new Vector3(playerObj.transform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.0005f, playerObj.transform.position.z);
			rippleEffect.transform.position = position;
			if (rippleEmitTime + rippleEmitDelay < Time.time)
			{
				rippleEffect.Emit(Mathf.RoundToInt(rippleEffect.emissionRate));
				rippleEmitTime = Time.time;
			}
		}
		else if (splashTrailTime + 0.075f < Time.time)
		{
			trailPos = new Vector3(playerObj.transform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.0005f, playerObj.transform.position.z);
			splashTrail.transform.position = trailPos;
			splashTrail.Emit(Mathf.RoundToInt(splashTrail.emissionRate));
			splashTrailTime = Time.time;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Player")
		{
			swimTimeState = false;
			FPSWalkerComponent.inWater = false;
			FPSWalkerComponent.swimming = false;
			FPSWalkerComponent.belowWater = false;
			FPSWalkerComponent.canWaterJump = true;
			FPSWalkerComponent.holdingBreath = false;
		}
	}

	private void StartUnderwaterEffects()
	{
		if (!effectsState)
		{
			this.audioSource.Play();
			AudioSource[] array = aboveWaterAudioSources;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource != null)
				{
					audioSource.Pause();
				}
			}
			bubblesEffect.gameObject.SetActive(true);
			PlayerWeaponsComponent.waterMuzzleFlashColor.r = underwaterFogColor.r;
			PlayerWeaponsComponent.waterMuzzleFlashColor.g = underwaterFogColor.g;
			PlayerWeaponsComponent.waterMuzzleFlashColor.b = underwaterFogColor.b;
			if (underwaterFogEnabled)
			{
				RenderSettings.fog = underwaterFogEnabled;
				RenderSettings.fogColor = underwaterFogColor;
				RenderSettings.fogDensity = underwaterFogDensity;
				RenderSettings.fogMode = underwaterFogMode;
				RenderSettings.fogStartDistance = underwaterLinearFogStart;
				RenderSettings.fogEndDistance = underwaterLinearFogEnd;
			}
			if (underwaterLightEnabled)
			{
				RenderSettings.ambientLight = underwaterLightColor;
			}
			if ((bool)SunlightObj && changeSunlightColor)
			{
				SunlightObj.color = underwaterSunightColor;
			}
			if ((bool)waterPlane)
			{
				waterPlane.gameObject.SetActive(true);
			}
			if (useCausticsLight && (bool)causticsLight)
			{
				causticsLight.gameObject.SetActive(true);
			}
			if ((bool)waterPlaneTop)
			{
				waterPlaneTop.gameObject.SetActive(false);
			}
			FPSWalkerComponent.FPSPlayerComponent.CameraControlComponent.viewUnderwater = true;
			effectsState = true;
		}
		if ((bool)waterPlane && FPSWalkerComponent.holdingBreath)
		{
			waterPlane.position = new Vector3(FPSWalkerComponent.myTransform.position.x, underwaterYpos, FPSWalkerComponent.myTransform.position.z);
		}
		if (myTransform.GetComponent<Collider>().bounds.max.y - 1.04f > mainCamTransform.position.y)
		{
			particlesYPos = mainCamTransform.position.y;
			particleForwardPos = 3.25f;
		}
		else
		{
			particlesYPos = myTransform.GetComponent<Collider>().bounds.max.y - 1.04f;
			particleForwardPos = 0f;
		}
		bubblesEffect.Emit(Mathf.RoundToInt(bubblesEffect.emissionRate));
		bubblesEffect.transform.position = new Vector3(mainCamTransform.position.x, particlesYPos, mainCamTransform.position.z) + mainCamTransform.forward * particleForwardPos;
	}

	private void StopUnderwaterEffects()
	{
		if (!effectsState)
		{
			return;
		}
		this.audioSource.Pause();
		if (aboveWaterAudioSources.Length > 0)
		{
			AudioSource[] array = aboveWaterAudioSources;
			foreach (AudioSource audioSource in array)
			{
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
			}
		}
		bubblesEffect.gameObject.SetActive(false);
		RenderSettings.fog = fogEnabled;
		RenderSettings.fogColor = origFogColor;
		RenderSettings.fogDensity = origFogDensity;
		RenderSettings.fogMode = origFogMode;
		RenderSettings.fogStartDistance = origLinearFogStart;
		RenderSettings.fogEndDistance = origLinearFogEnd;
		RenderSettings.ambientLight = origLightColor;
		if ((bool)SunlightObj && changeSunlightColor)
		{
			SunlightObj.color = origSunightColor;
		}
		if ((bool)waterPlane)
		{
			waterPlane.gameObject.SetActive(false);
		}
		if (useCausticsLight && (bool)causticsLight)
		{
			causticsLight.gameObject.SetActive(false);
		}
		if ((bool)waterPlaneTop)
		{
			waterPlaneTop.gameObject.SetActive(true);
		}
		FPSWalkerComponent.FPSPlayerComponent.CameraControlComponent.viewUnderwater = false;
		effectsState = false;
	}
}
