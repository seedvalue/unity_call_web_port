using UnityEngine;

public class NPCAttack : MonoBehaviour
{
	private AI AIComponent;

	private WeaponEffects WeaponEffectsComponent;

	private FPSRigidBodyWalker FPSWalker;

	private GameObject playerObj;

	private Transform myTransform;

	private Vector3 targetPos;

	[Tooltip("Maximum range of NPC attack.")]
	public float range = 100f;

	[Tooltip("Random range in units around target that NPC attack will hit (so NPCs won't have perfect aim).")]
	public float inaccuracy = 0.5f;

	[Tooltip("Fire rate of NPC attack.")]
	public float fireRate = 0.097f;

	[Tooltip("Number of attacks to fire in quick succession during NPC attack (for automatic weapons).")]
	public int burstShots;

	[Tooltip("Maximum number of random shots to add to end of attack (so automatic weapons will fire different number of bullets per attack).")]
	public int randomShots;

	[Tooltip("Physics force to apply to collider hit by NPC attack.")]
	public float force = 20f;

	[Tooltip("Damage to inflict per NPC attack.")]
	public float damage = 10f;

	[Tooltip("True if this is a melee attack (so actions like blocking can identify attack type).")]
	public bool isMeleeAttack;

	private float damageAmt;

	private bool doneShooting = true;

	private int shotsFired;

	private bool randBurstState;

	private int randShotsAmt;

	private bool shooting;

	private bool reloading;

	private bool mFlashState;

	private Vector3 rayOrigin;

	private Vector3 targetDir;

	private RaycastHit[] hits;

	private bool hitObject;

	[Tooltip("Muzzle flash object to display during NPC attacks.")]
	public Renderer muzzleFlash;

	private Transform muzzleFlashTransform;

	[Tooltip("Sound effect for NPC attack.")]
	public AudioClip firesnd;

	[Tooltip("Random pitch chosen between this value and 1.0 for NPC attack sound variety.")]
	public float fireFxRandPitch = 0.86f;

	private AudioSource aSource;

	private float shootStartTime;

	private void OnEnable()
	{
		myTransform = base.transform;
		if ((bool)muzzleFlash)
		{
			muzzleFlashTransform = muzzleFlash.transform;
		}
		AIComponent = myTransform.GetComponent<AI>();
		WeaponEffectsComponent = AIComponent.playerObj.GetComponent<FPSPlayer>().weaponObj.GetComponent<WeaponEffects>();
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		aSource = GetComponent<AudioSource>();
		shootStartTime = (0f - fireRate) * 2f;
	}

	private void Update()
	{
		if (shootStartTime + fireRate < Time.time)
		{
			shooting = false;
		}
		if (!doneShooting && shotsFired < burstShots + randShotsAmt && (AIComponent.target == AIComponent.playerTransform || AIComponent.target == AIComponent.FPSWalker.leanObj.transform || ((bool)AIComponent.TargetAIComponent && AIComponent.TargetAIComponent.enabled)))
		{
			Fire();
			if (!randBurstState)
			{
				randShotsAmt = Random.Range(0, randomShots);
				randBurstState = true;
			}
		}
		else
		{
			doneShooting = true;
			shotsFired = 0;
			randBurstState = false;
		}
	}

	private void LateUpdate()
	{
		if (!muzzleFlash)
		{
			return;
		}
		if ((double)(Time.time - shootStartTime) < (double)fireRate * 0.33)
		{
			if (mFlashState)
			{
				muzzleFlash.enabled = true;
				mFlashState = false;
			}
		}
		else if (!mFlashState)
		{
			muzzleFlash.enabled = false;
		}
	}

	public void Fire()
	{
		if (!reloading)
		{
			if (!shooting)
			{
				FireOneShot();
				shootStartTime = Time.time;
				shooting = true;
				doneShooting = false;
			}
			else if (shootStartTime + fireRate < Time.time)
			{
				shooting = false;
			}
		}
		else
		{
			shooting = false;
		}
	}

	private void FireOneShot()
	{
		if ((bool)AIComponent.TargetAIComponent)
		{
			if (Vector3.Distance(myTransform.position, AIComponent.lastVisibleTargetPosition) > 2.5f)
			{
				targetPos = new Vector3(AIComponent.lastVisibleTargetPosition.x + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.y + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.z + Random.Range(0f - inaccuracy, inaccuracy));
			}
			else
			{
				targetPos = new Vector3(AIComponent.lastVisibleTargetPosition.x, AIComponent.lastVisibleTargetPosition.y, AIComponent.lastVisibleTargetPosition.z);
			}
		}
		else if (Vector3.Distance(myTransform.position, AIComponent.lastVisibleTargetPosition) > 2.5f)
		{
			if (FPSWalker.crouched || FPSWalker.prone)
			{
				targetPos = new Vector3(AIComponent.lastVisibleTargetPosition.x + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.y + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.z + Random.Range(0f - inaccuracy, inaccuracy));
			}
			else
			{
				targetPos = new Vector3(AIComponent.lastVisibleTargetPosition.x + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.y + Random.Range(0f - inaccuracy, inaccuracy), AIComponent.lastVisibleTargetPosition.z + Random.Range(0f - inaccuracy, inaccuracy)) + AIComponent.playerObj.transform.up * AIComponent.targetEyeHeight;
			}
		}
		else
		{
			targetPos = new Vector3(AIComponent.lastVisibleTargetPosition.x, AIComponent.lastVisibleTargetPosition.y, AIComponent.lastVisibleTargetPosition.z);
		}
		rayOrigin = new Vector3(myTransform.position.x, myTransform.position.y + AIComponent.eyeHeight, myTransform.position.z);
		targetDir = (targetPos - rayOrigin).normalized;
		if ((bool)muzzleFlashTransform)
		{
			WeaponEffectsComponent.BulletTracers(targetDir, muzzleFlashTransform.position, -3f, 0f, false);
		}
		hitObject = false;
		RaycastHit raycastHit = default(RaycastHit);
		hits = Physics.RaycastAll(rayOrigin, targetDir, Vector3.Distance(rayOrigin, targetPos), AIComponent.searchMask);
		for (int i = 0; i < hits.Length; i++)
		{
			if (!hits[i].transform.IsChildOf(myTransform))
			{
				hitObject = true;
				raycastHit = hits[i];
				break;
			}
		}
		if (hitObject)
		{
			if ((bool)raycastHit.rigidbody)
			{
				raycastHit.rigidbody.AddForceAtPosition(force * targetDir / (Time.fixedDeltaTime * 100f), raycastHit.point);
			}
			if ((bool)raycastHit.collider)
			{
				if (raycastHit.collider.gameObject.layer != 20 && raycastHit.collider.gameObject.layer != 11)
				{
					if (!isMeleeAttack)
					{
						WeaponEffectsComponent.ImpactEffects(raycastHit.collider, raycastHit.point, true, false, raycastHit.normal);
					}
					else
					{
						WeaponEffectsComponent.ImpactEffects(raycastHit.collider, raycastHit.point, true, true, raycastHit.normal);
					}
				}
				damageAmt = Random.Range(damage, damage + damage);
				switch (raycastHit.collider.gameObject.layer)
				{
				case 0:
					if ((bool)raycastHit.collider.gameObject.GetComponent<AppleFall>())
					{
						raycastHit.collider.gameObject.GetComponent<AppleFall>().ApplyDamage(damageAmt);
					}
					if ((bool)raycastHit.collider.gameObject.GetComponent<BreakableObject>())
					{
						raycastHit.collider.gameObject.GetComponent<BreakableObject>().ApplyDamage(damageAmt);
					}
					else if ((bool)raycastHit.collider.gameObject.GetComponent<ExplosiveObject>())
					{
						raycastHit.collider.gameObject.GetComponent<ExplosiveObject>().ApplyDamage(damageAmt);
					}
					else if ((bool)raycastHit.collider.gameObject.GetComponent<MineExplosion>())
					{
						raycastHit.collider.gameObject.GetComponent<MineExplosion>().ApplyDamage(damageAmt);
					}
					break;
				case 13:
					if ((bool)raycastHit.collider.gameObject.GetComponent<CharacterDamage>())
					{
						raycastHit.collider.gameObject.GetComponent<CharacterDamage>().ApplyDamage(damageAmt, Vector3.zero, myTransform.position, myTransform, false, false);
					}
					if ((bool)raycastHit.collider.gameObject.GetComponent<LocationDamage>())
					{
						raycastHit.collider.gameObject.GetComponent<LocationDamage>().ApplyDamage(damageAmt, Vector3.zero, myTransform.position, myTransform, false, false);
					}
					break;
				case 11:
					if ((bool)raycastHit.collider.gameObject.GetComponent<FPSPlayer>())
					{
						raycastHit.collider.gameObject.GetComponent<FPSPlayer>().ApplyDamage(damageAmt, myTransform, isMeleeAttack);
					}
					if ((bool)raycastHit.collider.gameObject.GetComponent<LeanColliderDamage>())
					{
						raycastHit.collider.gameObject.GetComponent<LeanColliderDamage>().ApplyDamage(damageAmt);
					}
					break;
				case 20:
					if ((bool)raycastHit.collider.gameObject.GetComponent<FPSPlayer>())
					{
						raycastHit.collider.gameObject.GetComponent<FPSPlayer>().ApplyDamage(damageAmt, myTransform, isMeleeAttack);
					}
					if ((bool)raycastHit.collider.gameObject.GetComponent<LeanColliderDamage>())
					{
						raycastHit.collider.gameObject.GetComponent<LeanColliderDamage>().ApplyDamage(damageAmt);
					}
					break;
				}
			}
		}
		aSource.clip = firesnd;
		aSource.pitch = Random.Range(fireFxRandPitch, 1f);
		if (aSource.volume > 0f)
		{
			aSource.PlayOneShot(aSource.clip, 0.8f / aSource.volume);
		}
		shotsFired++;
		mFlashState = true;
		base.enabled = true;
	}
}
