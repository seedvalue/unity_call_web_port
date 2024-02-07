using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
	private PlayerWeapons PlayerWeaponsComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	private FPSRigidBodyWalker FPSWalkerComponent;

	[HideInInspector]
	public GameObject weaponObj;

	[Tooltip("Particle effect to use for dirt impacts.")]
	public GameObject dirtImpact;

	[Tooltip("Particle effect to use for metal impacts.")]
	public GameObject metalImpact;

	[Tooltip("Particle effect to use for wood impacts.")]
	public GameObject woodImpact;

	[Tooltip("Particle effect to use for water impacts.")]
	public GameObject waterImpact;

	[Tooltip("Particle effect to use for glass impacts.")]
	public GameObject glassImpact;

	[Tooltip("Particle effect to use for flesh impacts.")]
	public GameObject fleshImpact;

	[Tooltip("Particle effect to use for stone impacts.")]
	public GameObject stoneImpact;

	[Tooltip("Particle effect to use for explosions.")]
	public GameObject explosion;

	private GameObject explosionObj;

	[Tooltip("Particle effect to use for water splashes.")]
	public GameObject waterSplash;

	[Tooltip("Particles emitted around player treading water.")]
	public ParticleSystem rippleEffect;

	[Tooltip("Particles emitted underwater for ambient bubbles/particles.")]
	public ParticleSystem bubblesEffect;

	[Tooltip("Particles to emit when player is swimming on water surface.")]
	public ParticleSystem splashTrail;

	private int impactObjID;

	private GameObject impactObj;

	[Tooltip("Particle effect to use for NPC tracers.")]
	public ParticleSystem npcTracerParticles;

	[Tooltip("Particle effect to use for player tracers.")]
	public ParticleSystem playerTracerParticles;

	[Tooltip("Particle effect to use for underwater tracers.")]
	public ParticleSystem bubbleParticles;

	[Tooltip("Index in the object pool of dirt mark objects.")]
	public int[] dirtMarksID;

	[Tooltip("Index in the object pool of metal mark objects.")]
	public int[] metalMarksID;

	[Tooltip("Index in the object pool of wood mark objects.")]
	public int[] woodMarksID;

	[Tooltip("Index in the object pool of glass mark objects.")]
	public int[] glassMarksID;

	[Tooltip("Index in the object pool of stone mark objects.")]
	public int[] stoneMarksID;

	[Tooltip("Index in the object pool of dirt mark objects for melee weapons.")]
	public int[] dirtMarksMeleeID;

	[Tooltip("Index in the object pool of metal mark objects for melee weapons.")]
	public int[] metalMarksMeleeID;

	[Tooltip("Index in the object pool of wood mark objects for melee weapons.")]
	public int[] woodMarksMeleeID;

	private int markObjID;

	[Tooltip("Sounds to use for default impacts.")]
	public AudioClip[] defaultImpactSounds;

	[Tooltip("Sounds to use for metal impacts.")]
	public AudioClip[] metalImpactSounds;

	[Tooltip("Sounds to use for wood impacts.")]
	public AudioClip[] woodImpactSounds;

	[Tooltip("Sounds to use for water impacts.")]
	public AudioClip[] waterImpactSounds;

	[Tooltip("Sounds to use for glass impacts.")]
	public AudioClip[] glassImpactSounds;

	[Tooltip("Sounds to use for flesh impacts.")]
	public AudioClip[] fleshImpactSounds;

	[Tooltip("Sounds to use for stone impacts.")]
	public AudioClip[] stoneImpactSounds;

	[Tooltip("Sounds to use for default melee impacts.")]
	public AudioClip[] defaultImpactSoundsMelee;

	[Tooltip("Sounds to use for metal melee impacts.")]
	public AudioClip[] metalImpactSoundsMelee;

	[Tooltip("Sounds to use for wood melee impacts.")]
	public AudioClip[] woodImpactSoundsMelee;

	[Tooltip("Sounds to use for flesh melee impacts.")]
	public AudioClip[] fleshImpactSoundsMelee;

	[Tooltip("Sounds to use for stone melee impacts.")]
	public AudioClip[] stoneImpactSoundsMelee;

	private AudioClip hitSound;

	private float hitVolumeAmt = 1f;

	private ParticleSystem partSys;

	private ParticleSystem tracerParticles;

	private ParticleSystem.Particle[] activeParticles;

	private int numparticles;

	private int numParticlesAlive;

	private bool rotateParticle;

	private float randvel;

	public void Start()
	{
		weaponObj = base.transform.gameObject;
		PlayerWeaponsComponent = weaponObj.GetComponentInChildren<PlayerWeapons>();
		WeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
		FPSWalkerComponent = Camera.main.transform.GetComponent<CameraControl>().playerObj.GetComponent<FPSRigidBodyWalker>();
		activeParticles = new ParticleSystem.Particle[256];
	}

	public bool ImpactEffects(Collider hitcol, Vector3 hitPoint, bool NpcAttack, bool meleeAttack, Vector3 rayNormal = default(Vector3))
	{
		WeaponBehaviorComponent = PlayerWeaponsComponent.CurrentWeaponBehaviorComponent;
		switch (hitcol.gameObject.tag)
		{
		case "Dirt":
			impactObj = dirtImpact;
			rotateParticle = true;
			if (!meleeAttack)
			{
				if ((bool)defaultImpactSounds[0])
				{
					hitSound = defaultImpactSounds[Random.Range(0, defaultImpactSounds.Length)];
				}
			}
			else if ((bool)defaultImpactSoundsMelee[0])
			{
				hitSound = defaultImpactSoundsMelee[Random.Range(0, defaultImpactSoundsMelee.Length)];
			}
			break;
		case "Metal":
			impactObj = metalImpact;
			rotateParticle = true;
			if (!meleeAttack)
			{
				if ((bool)metalImpactSounds[0])
				{
					hitSound = metalImpactSounds[Random.Range(0, metalImpactSounds.Length)];
				}
			}
			else if ((bool)metalImpactSoundsMelee[0])
			{
				hitSound = metalImpactSoundsMelee[Random.Range(0, metalImpactSoundsMelee.Length)];
			}
			break;
		case "Wood":
			impactObj = woodImpact;
			rotateParticle = true;
			if (!meleeAttack)
			{
				if ((bool)woodImpactSounds[0])
				{
					hitSound = woodImpactSounds[Random.Range(0, woodImpactSounds.Length)];
				}
			}
			else if ((bool)woodImpactSoundsMelee[0])
			{
				hitSound = woodImpactSoundsMelee[Random.Range(0, woodImpactSoundsMelee.Length)];
			}
			break;
		case "Water":
			impactObj = waterImpact;
			rotateParticle = false;
			if ((bool)waterImpactSounds[0])
			{
				hitSound = waterImpactSounds[Random.Range(0, waterImpactSounds.Length)];
			}
			break;
		case "Glass":
			impactObj = glassImpact;
			rotateParticle = false;
			if ((bool)glassImpactSounds[0])
			{
				hitSound = glassImpactSounds[Random.Range(0, glassImpactSounds.Length)];
			}
			break;
		case "Flesh":
			impactObj = fleshImpact;
			rotateParticle = false;
			if (!meleeAttack)
			{
				if ((bool)fleshImpactSounds[0])
				{
					hitSound = fleshImpactSounds[Random.Range(0, fleshImpactSounds.Length)];
				}
			}
			else if ((bool)fleshImpactSoundsMelee[0])
			{
				hitSound = fleshImpactSoundsMelee[Random.Range(0, fleshImpactSoundsMelee.Length)];
			}
			break;
		case "Stone":
			impactObj = stoneImpact;
			rotateParticle = true;
			if (!meleeAttack)
			{
				if ((bool)stoneImpactSounds[0])
				{
					hitSound = stoneImpactSounds[Random.Range(0, stoneImpactSounds.Length)];
				}
			}
			else if ((bool)stoneImpactSoundsMelee[0])
			{
				hitSound = stoneImpactSoundsMelee[Random.Range(0, stoneImpactSoundsMelee.Length)];
			}
			break;
		default:
			impactObj = metalImpact;
			rotateParticle = false;
			if (!meleeAttack)
			{
				if ((bool)defaultImpactSounds[0])
				{
					hitSound = defaultImpactSounds[Random.Range(0, defaultImpactSounds.Length)];
				}
			}
			else if ((bool)defaultImpactSoundsMelee[0])
			{
				hitSound = defaultImpactSoundsMelee[Random.Range(0, defaultImpactSoundsMelee.Length)];
			}
			break;
		}
		impactObj.SetActive(true);
		impactObj.transform.position = hitPoint;
		foreach (Transform item in impactObj.transform)
		{
			partSys = item.GetComponent<ParticleSystem>();
			EmitRotatedParticle(partSys, rayNormal);
		}
		if (!NpcAttack && !WeaponBehaviorComponent.meleeActive)
		{
			if (WeaponBehaviorComponent.projectileCount > 1)
			{
				hitVolumeAmt = 0.2f;
			}
			else if (!WeaponBehaviorComponent.semiAuto)
			{
				hitVolumeAmt = 0.8f;
			}
			else
			{
				hitVolumeAmt = 1f;
			}
		}
		else
		{
			hitVolumeAmt = 1f;
		}
		PlayAudioAtPos.PlayClipAt(hitSound, hitPoint, hitVolumeAmt, 1f, 1f, 3f);
		return true;
	}

	public void EmitRotatedParticle(ParticleSystem partSysToRotate, Vector3 direction)
	{
		partSysToRotate.Emit(Mathf.RoundToInt(partSysToRotate.emissionRate));
		if (!rotateParticle)
		{
			return;
		}
		numParticlesAlive = partSysToRotate.GetParticles(activeParticles);
		int num = numParticlesAlive - 1;
		while ((float)num > (float)(numParticlesAlive - 1) - partSysToRotate.emissionRate)
		{
			if (Random.value > 0.5f)
			{
				randvel = partSysToRotate.startSpeed * Mathf.Clamp(Random.value, 0.1f, 1f);
			}
			else
			{
				randvel = partSysToRotate.startSpeed + partSysToRotate.startSpeed * Mathf.Clamp(Random.value, 0.25f, 0.75f);
			}
			activeParticles[num].velocity = direction * randvel;
			num--;
		}
		partSysToRotate.SetParticles(activeParticles, numParticlesAlive);
	}

	public void BulletMarks(RaycastHit hit, bool meleeAttack)
	{
		if (!meleeAttack)
		{
			switch (hit.collider.gameObject.tag)
			{
			case "Dirt":
				markObjID = dirtMarksID[Random.Range(0, dirtMarksID.Length)];
				break;
			case "Metal":
				markObjID = metalMarksID[Random.Range(0, metalMarksID.Length)];
				break;
			case "Wood":
				markObjID = woodMarksID[Random.Range(0, woodMarksID.Length)];
				break;
			case "Glass":
				markObjID = glassMarksID[Random.Range(0, glassMarksID.Length)];
				break;
			case "Stone":
				markObjID = stoneMarksID[Random.Range(0, stoneMarksID.Length)];
				break;
			default:
				markObjID = dirtMarksID[Random.Range(0, dirtMarksID.Length)];
				break;
			}
		}
		else
		{
			switch (hit.collider.gameObject.tag)
			{
			case "Dirt":
				markObjID = dirtMarksMeleeID[Random.Range(0, dirtMarksMeleeID.Length)];
				break;
			case "Metal":
				markObjID = metalMarksMeleeID[Random.Range(0, metalMarksMeleeID.Length)];
				break;
			case "Wood":
				markObjID = woodMarksMeleeID[Random.Range(0, woodMarksMeleeID.Length)];
				break;
			case "Glass":
				markObjID = glassMarksID[Random.Range(0, glassMarksID.Length)];
				break;
			default:
				markObjID = dirtMarksMeleeID[Random.Range(0, dirtMarksMeleeID.Length)];
				break;
			}
		}
		if ((bool)hit.collider && hit.collider.gameObject.layer != 9 && hit.collider.gameObject.layer != 13 && hit.collider.gameObject.tag != "NoHitMark" && hit.collider.gameObject.tag != "PickUp" && hit.collider.gameObject.tag != "Flesh" && hit.collider.gameObject.tag != "Usable" && hit.collider.gameObject.tag != "Water")
		{
			GameObject gameObject = AzuObjectPool.instance.SpawnPooledObj(markObjID, hit.point + hit.normal * 0.025f, Quaternion.identity);
			GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
			FadeOutDecals component = gameObject2.transform.GetComponent<FadeOutDecals>();
			Transform parentObjTransform = component.parentObjTransform;
			Transform myTransform = component.myTransform;
			component.InitializeDecal();
			myTransform.parent = null;
			gameObject2.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			Vector3 localScale = myTransform.localScale;
			parentObjTransform.parent = hit.transform;
			parentObjTransform.localScale = Vector3.one;
			Quaternion rotation = hit.transform.rotation;
			parentObjTransform.rotation = rotation;
			Vector3 localScale2 = Vector3.one;
			if (parentObjTransform.parent.transform.localScale != Vector3.one || parentObjTransform.parent.transform == hit.transform.root)
			{
				localScale2 = new Vector3(1f / parentObjTransform.parent.transform.localScale.x, 1f / parentObjTransform.parent.transform.localScale.y, 1f / parentObjTransform.parent.transform.localScale.z);
			}
			else if (parentObjTransform.parent.transform.parent.transform.localScale != Vector3.one)
			{
				localScale2 = new Vector3(1f / parentObjTransform.parent.transform.parent.transform.localScale.x, 1f / parentObjTransform.parent.transform.parent.transform.localScale.y, 1f / parentObjTransform.parent.transform.parent.transform.localScale.z);
			}
			parentObjTransform.localScale = localScale2;
			myTransform.parent = parentObjTransform;
			myTransform.localScale = localScale;
			if (!meleeAttack)
			{
				float num = Random.Range(-0.25f, 0.25f);
				myTransform.localScale = localScale + new Vector3(num, 0f, num);
				myTransform.RotateAround(hit.point, hit.normal, Random.Range(-50, 50));
			}
			else
			{
				myTransform.RotateAround(hit.point, hit.normal, Random.Range(-70, 70));
			}
		}
	}

	public void BulletTracers(Vector3 direction, Vector3 position, float tracerDist = 0f, float tracerDistSwim = 0f, bool isPlayer = true)
	{
		if (isPlayer && !WeaponBehaviorComponent.MouseLookComponent.thirdPerson)
		{
			tracerParticles = playerTracerParticles;
		}
		else
		{
			tracerParticles = npcTracerParticles;
		}
		if (!FPSWalkerComponent.holdingBreath)
		{
			if ((bool)tracerParticles)
			{
				tracerParticles.transform.position = position + direction * tracerDist;
				tracerParticles.Emit(Mathf.RoundToInt(tracerParticles.emissionRate));
				numParticlesAlive = tracerParticles.GetParticles(activeParticles);
				activeParticles[numParticlesAlive - 1].velocity = direction * tracerParticles.startSpeed;
				tracerParticles.SetParticles(activeParticles, numParticlesAlive);
			}
		}
		else if ((bool)bubbleParticles)
		{
			bubbleParticles.transform.position = position - direction * tracerDistSwim;
			bubbleParticles.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
			bubbleParticles.Emit(Mathf.RoundToInt(bubbleParticles.emissionRate));
		}
	}

	public void ExplosionEffect(Vector3 position)
	{
		explosionObj = explosion;
		explosionObj.transform.position = position;
		foreach (Transform item in explosionObj.transform)
		{
			if ((bool)item.GetComponent<ParticleSystem>())
			{
				partSys = item.GetComponent<ParticleSystem>();
				partSys.Emit(Mathf.RoundToInt(partSys.emissionRate));
			}
		}
	}
}
