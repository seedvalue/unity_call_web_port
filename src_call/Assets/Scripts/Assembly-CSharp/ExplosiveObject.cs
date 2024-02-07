using System.Collections;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour
{
	private WeaponEffects WeaponEffectsComponent;

	[Tooltip("When hit points of object are depleted, object will explode.")]
	public float hitPoints = 100f;

	private float initialHitPoints = 100f;

	[Tooltip("Maximum damage dealt at center of explosion (damage decreases from center).")]
	public float explosionDamage = 200f;

	[Tooltip("Delay before this object applies explosion force and damage to other objects;.")]
	public float damageDelay = 0.2f;

	private float explosionDamageAmt;

	[Tooltip("Explosive physics force applied to objects in blast radius.")]
	public float blastForce = 15f;

	[Tooltip("Radius of explosion.")]
	public float radius = 7f;

	[Tooltip("Layers that will be hit by explosion.")]
	public LayerMask blastMask;

	[Tooltip("Layers that will block explosion blast.")]
	public LayerMask obstructionMask;

	private Transform myTransform;

	private bool detonated;

	private bool audioPlayed;

	public int objectPoolIndex;

	private AudioSource aSource;

	private Collider hitCollider;

	private Rigidbody hitRigidbody;

	private void Start()
	{
		WeaponEffectsComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>().WeaponEffectsComponent;
		myTransform = base.transform;
		initialHitPoints = hitPoints;
		aSource = GetComponent<AudioSource>();
	}

	private IEnumerator DetectDestroyed()
	{
		while (!audioPlayed || aSource.isPlaying)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (objectPoolIndex == 0)
		{
			FadeOutDecals[] componentsInChildren = base.gameObject.GetComponentsInChildren<FadeOutDecals>(true);
			FadeOutDecals[] array = componentsInChildren;
			foreach (FadeOutDecals fadeOutDecals in array)
			{
				fadeOutDecals.parentObjTransform.parent = AzuObjectPool.instance.transform;
				fadeOutDecals.parentObj.SetActive(false);
				fadeOutDecals.gameObject.SetActive(false);
			}
			ArrowObject[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ArrowObject>(true);
			ArrowObject[] array2 = componentsInChildren2;
			foreach (ArrowObject arrowObject in array2)
			{
				arrowObject.transform.parent = null;
				arrowObject.myRigidbody.isKinematic = false;
				arrowObject.myBoxCol.isTrigger = false;
				arrowObject.gameObject.tag = "Usable";
				arrowObject.falling = true;
			}
			Object.Destroy(myTransform.gameObject);
		}
		else
		{
			ResetExplosiveObject();
			AzuObjectPool.instance.RecyclePooledObj(objectPoolIndex, myTransform.gameObject);
		}
	}

	private void ResetExplosiveObject()
	{
		detonated = false;
		audioPlayed = false;
		hitPoints = initialHitPoints;
		myTransform.GetComponent<MeshRenderer>().enabled = true;
	}

	private IEnumerator Detonate()
	{
		yield return new WaitForSeconds(damageDelay);
		WeaponEffectsComponent.ExplosionEffect(myTransform.position);
		myTransform.GetComponent<MeshRenderer>().enabled = false;
		aSource.pitch = Random.Range(0.75f * Time.timeScale, 1f * Time.timeScale);
		aSource.Play();
		audioPlayed = true;
		Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, blastMask, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < hitColliders.Length; i++)
		{
			RaycastHit hitInfo;
			if (hitColliders[i].transform != myTransform && Physics.Raycast(myTransform.position, (hitColliders[i].transform.position - myTransform.position).normalized, out hitInfo, radius * 2f))
			{
				hitCollider = hitColliders[i].GetComponent<Collider>();
				explosionDamageAmt = explosionDamage * Mathf.Clamp01(1f - (myTransform.position - hitColliders[i].transform.position).magnitude / radius);
				if (explosionDamageAmt >= 1f && hitInfo.collider == hitCollider)
				{
					switch (hitCollider.gameObject.layer)
					{
					case 13:
						if ((bool)hitCollider.gameObject.GetComponent<CharacterDamage>())
						{
							hitCollider.gameObject.GetComponent<CharacterDamage>().ApplyDamage(explosionDamageAmt, Vector3.zero, myTransform.position, myTransform, false, true);
						}
						if ((bool)hitCollider.gameObject.GetComponent<LocationDamage>())
						{
							hitCollider.gameObject.GetComponent<LocationDamage>().ApplyDamage(explosionDamageAmt, Vector3.zero, myTransform.position, myTransform, false, true);
						}
						break;
					case 0:
						if ((bool)hitCollider.gameObject.GetComponent<BreakableObject>())
						{
							hitCollider.gameObject.GetComponent<BreakableObject>().ApplyDamage(explosionDamageAmt);
						}
						else if ((bool)hitCollider.gameObject.GetComponent<AppleFall>())
						{
							hitCollider.gameObject.GetComponent<AppleFall>().ApplyDamage(explosionDamageAmt);
						}
						else if ((bool)hitCollider.gameObject.GetComponent<ExplosiveObject>())
						{
							hitCollider.gameObject.GetComponent<ExplosiveObject>().ApplyDamage(explosionDamageAmt);
						}
						else if ((bool)hitCollider.gameObject.GetComponent<MineExplosion>())
						{
							hitCollider.gameObject.GetComponent<MineExplosion>().ApplyDamage(explosionDamageAmt);
						}
						break;
					case 11:
						if ((bool)hitCollider.gameObject.GetComponent<FPSPlayer>())
						{
							hitCollider.gameObject.GetComponent<FPSPlayer>().ApplyDamage(explosionDamageAmt);
						}
						break;
					}
					if ((bool)hitColliders[i].transform.GetComponent<Rigidbody>())
					{
						hitRigidbody = hitColliders[i].transform.GetComponent<Rigidbody>();
						hitRigidbody.AddExplosionForce(blastForce * hitRigidbody.mass, myTransform.position, radius, 3f, ForceMode.Impulse);
					}
				}
			}
			if (i >= hitColliders.Length - 1 && objectPoolIndex == 0)
			{
				myTransform.GetComponent<MeshCollider>().enabled = false;
			}
		}
		StartCoroutine(DetectDestroyed());
	}

	public void ApplyDamage(float damage)
	{
		hitPoints -= damage;
		if (!detonated && hitPoints <= 0f)
		{
			detonated = true;
			StartCoroutine(Detonate());
		}
	}
}
