using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDamage : MonoBehaviour
{
	public UnityEvent onDie;

	private AI AIComponent;

	private RemoveBody RemoveBodyComponent;

	[Tooltip("Number of hitpoints for this character or body part.")]
	public float hitPoints = 100f;

	private float initialHitPoints;

	[Tooltip("Force to apply to this collider when NPC is killed.")]
	public float attackForce = 2.75f;

	[Tooltip("Item to spawn when NPC is killed.")]
	public GameObject gunItem;

	[Tooltip("Weapon mesh to hide when NPC dies (replaced with usable gun item).")]
	public Transform gunObj;

	private GameObject gunInst;

	[Tooltip("Rotation of spawed gun object after NPC death.")]
	public Vector3 gunItemRotation = new Vector3(0f, 180f, 180f);

	private Rigidbody[] bodies;

	[Tooltip("True if ragdoll mode is active for this NPC.")]
	public bool ragdollActive;

	private bool ragdollState;

	[Tooltip("If NPC only has one capsule collider for hit detection, replace the NPC's character mesh with a ragdoll, instead of transitioning instantly to ragdoll.")]
	public Transform deadReplacement;

	private Transform dead;

	[Tooltip("Sound effect to play when NPC dies.")]
	public AudioClip dieSound;

	[Tooltip("Determine if this object or parent should be removed on death. This is to allow for different hit detection collider types as children of NPC parent.")]
	public bool notParent;

	[Tooltip("Should this NPC's body be removed after Body Stay Time?")]
	public bool removeBody;

	[Tooltip("Time for body to stay in the scene before it is removed.")]
	public float bodyStayTime = 15f;

	[Tooltip("Time for dropped weapon item to stay in scene before it is removed.")]
	public float gunStayTime = -1f;

	[Tooltip("Chance between 0 and 1 that death of this NPC will trigger slow motion for a few seconds (regardless of the body part hit).")]
	[Range(0f, 1f)]
	public float sloMoDeathChance;

	[Tooltip("True if backstabbing this NPC should trigger slow motion for the duration of slo mo backstab time.")]
	public bool sloMoBackstab = true;

	[Tooltip("Duration of slow motion time in seconds if slo mo death chance check is successful.")]
	public float sloMoDeathTime = 0.9f;

	[Tooltip("Duration of slow motion time in seconds if this NPC is backstabbed.")]
	public float sloMoBackstabTime = 0.9f;

	private Vector3 attackerPos2;

	private Vector3 attackDir2;

	private Transform myTransform;

	private bool explosionCheck;

	private LayerMask raymask = 8192;

	public bool isFriendlyWolf;

	private void OnEnable()
	{
		myTransform = base.transform;
		RemoveBodyComponent = GetComponent<RemoveBody>();
		Mathf.Clamp01(sloMoDeathChance);
		if (removeBody && (bool)RemoveBodyComponent)
		{
			RemoveBodyComponent.enabled = false;
		}
		if (!AIComponent)
		{
			AIComponent = myTransform.GetComponent<AI>();
		}
		initialHitPoints = hitPoints;
		bodies = GetComponentsInChildren<Rigidbody>();
	}

	private void Update()
	{
		if (bodies.Length <= 1)
		{
			return;
		}
		if (!ragdollActive)
		{
			if (ragdollState)
			{
				return;
			}
			Rigidbody[] array = bodies;
			foreach (Rigidbody rigidbody in array)
			{
				rigidbody.isKinematic = true;
			}
			AIComponent.AnimatorComponent.enabled = true;
			if ((bool)gunObj)
			{
				gunObj.gameObject.SetActive(true);
				if ((bool)gunInst)
				{
					Object.Destroy(gunInst);
				}
			}
			ragdollState = true;
		}
		else
		{
			if (!ragdollState)
			{
				return;
			}
			AIComponent.AnimatorComponent.enabled = false;
			Collider[] colliders = AIComponent.colliders;
			foreach (Collider collider in colliders)
			{
				if (AIComponent.FPSWalker.gameObject.activeInHierarchy)
				{
					Physics.IgnoreCollision(collider, AIComponent.FPSWalker.capsule, true);
				}
			}
			Rigidbody[] array2 = bodies;
			foreach (Rigidbody rigidbody2 in array2)
			{
				rigidbody2.isKinematic = false;
			}
			if ((bool)gunObj)
			{
				gunObj.gameObject.SetActive(false);
				gunInst = Object.Instantiate(gunItem, gunObj.position, gunObj.rotation);
				Collider[] colliders2 = AIComponent.colliders;
				foreach (Collider collider2 in colliders2)
				{
					Physics.IgnoreCollision(collider2, gunInst.GetComponent<Collider>(), true);
				}
				gunInst.transform.Rotate(gunItemRotation);
				Vector3 position = gunInst.transform.position + base.transform.forward * 0.45f;
				gunInst.transform.position = position;
				if (gunStayTime > 0f && (bool)gunInst.GetComponent<WeaponPickup>())
				{
					gunInst.GetComponent<WeaponPickup>().StartCoroutine(gunInst.GetComponent<WeaponPickup>().DestroyWeapon(gunStayTime));
				}
			}
			ragdollState = false;
		}
	}

	public void ApplyDamage(float damage, Vector3 attackDir, Vector3 attackerPos, Transform attacker, bool isPlayer, bool isExplosion, Rigidbody hitBody = null, float bodyForce = 0f)
	{
		if (hitPoints <= 0f)
		{
			return;
		}
		if (!AIComponent.damaged && !AIComponent.huntPlayer && hitPoints / initialHitPoints < 0.65f && (bool)attacker && !isExplosion)
		{
			if (!isPlayer)
			{
				if (attacker.GetComponent<AI>().factionNum != AIComponent.factionNum)
				{
					AIComponent.target = attacker;
					AIComponent.TargetAIComponent = attacker.GetComponent<AI>();
					AIComponent.targetEyeHeight = AIComponent.TargetAIComponent.eyeHeight;
					AIComponent.damaged = true;
				}
			}
			else if (!AIComponent.ignoreFriendlyFire)
			{
				AIComponent.target = AIComponent.playerObj.transform;
				AIComponent.targetEyeHeight = AIComponent.FPSWalker.capsule.height * 0.25f;
				AIComponent.playerAttacked = true;
				AIComponent.TargetAIComponent = null;
				AIComponent.damaged = true;
			}
		}
		if (hitPoints - damage > 0f)
		{
			if (AIComponent.playerIsBehind && (AIComponent.PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.meleeSwingDelay > 0f || AIComponent.PlayerWeaponsComponent.CurrentWeaponBehaviorComponent.meleeActive))
			{
				hitPoints -= damage * 32f;
				if (sloMoBackstab)
				{
					AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.StartCoroutine(AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.ActivateBulletTime(sloMoBackstabTime));
				}
			}
			else
			{
				hitPoints -= damage;
			}
		}
		else
		{
			hitPoints = 0f;
		}
		attackDir2 = attackDir;
		attackerPos2 = attackerPos;
		explosionCheck = isExplosion;
		AIComponent.attackedTime = Time.time;
		if (hitPoints <= 0f)
		{
			onDie.Invoke();
			AIComponent.vocalFx.Stop();
			if (!isFriendlyWolf)
			{
				GameController.instance.level.EnemyKilled(base.transform);
			}
			if (sloMoDeathChance >= Random.value && isPlayer)
			{
				AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.StartCoroutine(AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.ActivateBulletTime(sloMoDeathTime));
			}
			if (bodies.Length < 2)
			{
				Die();
			}
			else if (!ragdollActive)
			{
				RagDollDie(hitBody, bodyForce);
			}
		}
	}

	private void RagDollDie(Rigidbody hitBody, float bodyForce)
	{
		if ((bool)dieSound)
		{
			PlayAudioAtPos.PlayClipAt(dieSound, base.transform.position, 1f);
		}
		AIComponent.NPCRegistryComponent.UnregisterNPC(AIComponent);
		if (AIComponent.spawned && (bool)AIComponent.NPCSpawnerComponent)
		{
			AIComponent.NPCSpawnerComponent.UnregisterSpawnedNPC(AIComponent);
		}
		AIComponent.AnimatorComponent.enabled = false;
		ragdollActive = true;
		if ((bool)AIComponent.NPCAttackComponent.muzzleFlash)
		{
			AIComponent.NPCAttackComponent.muzzleFlash.enabled = false;
		}
		AIComponent.NPCAttackComponent.enabled = false;
		AIComponent.StopAllCoroutines();
		AIComponent.agent.enabled = false;
		AIComponent.enabled = false;
		StartCoroutine(ApplyForce(hitBody, bodyForce));
		if ((bool)RemoveBodyComponent)
		{
			if (removeBody)
			{
				RemoveBodyComponent.enabled = true;
				RemoveBodyComponent.bodyStayTime = bodyStayTime;
			}
			else
			{
				RemoveBodyComponent.enabled = false;
			}
		}
	}

	public IEnumerator ApplyForce(Rigidbody body, float force)
	{
		yield return new WaitForSeconds(0.02f);
		if (!explosionCheck)
		{
			body.AddForce(attackDir2 * attackForce, ForceMode.Impulse);
			yield break;
		}
		Rigidbody[] array = bodies;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.AddForce((myTransform.position - (attackerPos2 + Vector3.up * -2.5f)).normalized * Random.Range(2.5f, 4.5f), ForceMode.Impulse);
		}
	}

	private void Die()
	{
		if ((bool)dieSound)
		{
			PlayAudioAtPos.PlayClipAt(dieSound, base.transform.position, 1f);
		}
		AIComponent.NPCRegistryComponent.UnregisterNPC(AIComponent);
		if (AIComponent.spawned && (bool)AIComponent.NPCSpawnerComponent)
		{
			AIComponent.NPCSpawnerComponent.UnregisterSpawnedNPC(AIComponent);
		}
		if (!isFriendlyWolf)
		{
			GameController.instance.level.EnemyKilled(base.transform);
		}
		AIComponent.agent.Stop();
		AIComponent.StopAllCoroutines();
		if (!deadReplacement)
		{
			return;
		}
		ArrowObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<ArrowObject>(true);
		ArrowObject[] array = componentsInChildren;
		foreach (ArrowObject arrowObject in array)
		{
			arrowObject.transform.parent = null;
			arrowObject.myRigidbody.isKinematic = false;
			arrowObject.myBoxCol.isTrigger = false;
			arrowObject.gameObject.tag = "Usable";
			arrowObject.falling = true;
		}
		dead = Object.Instantiate(deadReplacement, base.transform.position, base.transform.rotation);
		RemoveBodyComponent = dead.GetComponent<RemoveBody>();
		CopyTransformsRecurse(base.transform, dead);
		Collider[] componentsInChildren2 = dead.GetComponentsInChildren<Collider>();
		Collider[] array2 = componentsInChildren2;
		foreach (Collider collider in array2)
		{
			Physics.IgnoreCollision(collider, AIComponent.FPSWalker.capsule, true);
		}
		RaycastHit hitInfo;
		if (Physics.SphereCast(attackerPos2, 0.2f, attackDir2, out hitInfo, 750f, raymask) && (bool)hitInfo.rigidbody && attackDir2.x != 0f)
		{
			hitInfo.rigidbody.AddForce(attackDir2 * 10f, ForceMode.Impulse);
		}
		else
		{
			Component[] componentsInChildren3 = dead.GetComponentsInChildren<Rigidbody>();
			Component[] array3 = componentsInChildren3;
			for (int k = 0; k < array3.Length; k++)
			{
				Rigidbody rigidbody = (Rigidbody)array3[k];
				if (explosionCheck)
				{
					rigidbody.AddForce((myTransform.position - (attackerPos2 + Vector3.up * -2.5f)).normalized * Random.Range(4.5f, 7.5f), ForceMode.Impulse);
				}
				else if (rigidbody.transform.name == "Chest")
				{
					rigidbody.AddForce((myTransform.position - attackerPos2).normalized * 10f, ForceMode.Impulse);
				}
			}
		}
		if ((bool)RemoveBodyComponent)
		{
			if (removeBody)
			{
				RemoveBodyComponent.enabled = true;
				RemoveBodyComponent.bodyStayTime = bodyStayTime;
			}
			else
			{
				RemoveBodyComponent.enabled = false;
			}
		}
		if (notParent)
		{
			Object.Destroy(base.transform.parent.gameObject);
		}
		else
		{
			Object.Destroy(base.transform.gameObject);
		}
	}

	private static void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		foreach (Transform item in dst)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyTransformsRecurse(transform2, item);
			}
		}
	}
}
