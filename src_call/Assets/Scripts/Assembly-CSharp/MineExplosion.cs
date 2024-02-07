using System.Collections;
using UnityEngine;

public class MineExplosion : MonoBehaviour
{
	private WeaponEffects WeaponEffectsComponent;

	[Tooltip("Damage dealt by explosion.")]
	public float explosionDamage = 200f;

	[Tooltip("Delay before this object applies explosion force and damage to other objects;.")]
	public float damageDelay = 0.2f;

	[Tooltip("Explosive physics force applied to objects in blast radius.")]
	public float blastForce = 15f;

	[Tooltip("Explosion sound effect.")]
	public AudioClip explosionFX;

	[Tooltip("mine trigger sound effect.")]
	public AudioClip beepFx;

	[Tooltip("Radius of explosion.")]
	private float radius;

	[Tooltip("True if object is the mine detection radius object (used only for triggering mine, not explosion effects).")]
	public bool isRadiusCollider;

	[Tooltip("Layers that will be hit by explosion.")]
	public LayerMask blastMask;

	[Tooltip("Layers that mine will auto-align angles to surface on scene start.")]
	public LayerMask initPosMask;

	private Transform myTransform;

	private bool audioPlayed;

	private bool triggered;

	private bool detonated;

	private bool inPosition;

	private RaycastHit hit;

	private RaycastHit hitInit;

	private Vector3 explodePos;

	private void Start()
	{
		WeaponEffectsComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>().WeaponEffectsComponent;
		myTransform = base.transform;
		if (!isRadiusCollider)
		{
			radius = myTransform.GetComponentInChildren<SphereCollider>().radius;
			AlignToGround();
		}
	}

	private void AlignToGround()
	{
		if (Physics.Raycast(myTransform.position, -base.transform.up, out hitInit, 2f, initPosMask.value))
		{
			myTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInit.normal);
			myTransform.position = hitInit.point;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (isRadiusCollider && !col.isTrigger && (col.gameObject.layer == 11 || col.gameObject.layer == 13 || col.attachedRigidbody != null) && !detonated)
		{
			detonated = true;
			myTransform.parent.transform.GetComponent<MineExplosion>().triggered = true;
			myTransform.parent.transform.GetComponent<MineExplosion>().detonated = true;
			myTransform.GetComponent<SphereCollider>().enabled = false;
			myTransform.parent.transform.GetComponent<MineExplosion>().StartCoroutine("Detonate");
		}
	}

	private IEnumerator DetectDestroyed()
	{
		while (isRadiusCollider || !audioPlayed || GetComponent<AudioSource>().isPlaying)
		{
			yield return new WaitForSeconds(0.5f);
		}
		FadeOutDecals[] componentsInChildren = base.gameObject.GetComponentsInChildren<FadeOutDecals>(true);
		FadeOutDecals[] array = componentsInChildren;
		foreach (FadeOutDecals fadeOutDecals in array)
		{
			fadeOutDecals.parentObjTransform.parent = AzuObjectPool.instance.transform;
			fadeOutDecals.parentObj.SetActive(false);
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

	private IEnumerator Detonate()
	{
		if (triggered)
		{
			if ((bool)beepFx)
			{
				GetComponent<AudioSource>().clip = beepFx;
				GetComponent<AudioSource>().Play();
			}
			yield return new WaitForSeconds(damageDelay);
		}
		WeaponEffectsComponent.ExplosionEffect(myTransform.position);
		myTransform.GetComponent<MeshRenderer>().enabled = false;
		GetComponent<AudioSource>().clip = explosionFX;
		GetComponent<AudioSource>().pitch = Random.Range(0.75f * Time.timeScale, 1f * Time.timeScale);
		GetComponent<AudioSource>().Play();
		audioPlayed = true;
		if (!triggered)
		{
			yield return new WaitForSeconds(damageDelay);
		}
		Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius * 1.5f, blastMask);
		for (int i = 0; i < hitColliders.Length; i++)
		{
			Transform transform = hitColliders[i].transform;
			if (transform != myTransform && Physics.Linecast(transform.position, myTransform.position, out hit, blastMask) && hit.collider == myTransform.GetComponent<Collider>())
			{
				switch (hitColliders[i].GetComponent<Collider>().gameObject.layer)
				{
				case 0:
					if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<BreakableObject>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<BreakableObject>().ApplyDamage(explosionDamage);
					}
					else if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<ExplosiveObject>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<ExplosiveObject>().ApplyDamage(explosionDamage);
					}
					else if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<MineExplosion>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<MineExplosion>().ApplyDamage(explosionDamage);
					}
					else if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<AppleFall>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<AppleFall>().ApplyDamage(explosionDamage);
					}
					break;
				case 11:
					if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<FPSPlayer>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<FPSPlayer>().ApplyDamage(explosionDamage);
					}
					break;
				case 13:
					if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<CharacterDamage>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<CharacterDamage>().ApplyDamage(explosionDamage, Vector3.zero, myTransform.position, null, false, true);
					}
					if ((bool)hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<LocationDamage>())
					{
						hitColliders[i].GetComponent<Collider>().gameObject.GetComponent<LocationDamage>().ApplyDamage(explosionDamage, Vector3.zero, myTransform.position, null, false, true);
					}
					break;
				}
				if ((bool)transform.GetComponent<Rigidbody>())
				{
					transform.GetComponent<Rigidbody>().AddExplosionForce(blastForce * transform.GetComponent<Rigidbody>().mass, myTransform.position, radius, 3f, ForceMode.Impulse);
				}
			}
			if (i >= hitColliders.Length - 1)
			{
				myTransform.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	public void ApplyDamage(float damage)
	{
		if (!isRadiusCollider && !detonated)
		{
			detonated = true;
			myTransform.GetComponentInChildren<SphereCollider>().enabled = false;
			StartCoroutine(Detonate());
			StartCoroutine(DetectDestroyed());
		}
	}
}
