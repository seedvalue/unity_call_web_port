using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
	[Tooltip("When hitpoints are depleted, object is destroyed.")]
	public float hitPoints = 150f;

	private ParticleSystem breakParticles;

	private bool broken;

	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
		breakParticles = myTransform.GetComponent<ParticleSystem>();
	}

	private IEnumerator DetectBroken()
	{
		while (true)
		{
			if (broken)
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
				if ((bool)breakParticles && (float)breakParticles.particleCount == 0f)
				{
					break;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(myTransform.gameObject);
	}

	public void ApplyDamage(float damage)
	{
		hitPoints -= damage;
		if (hitPoints <= 0f && !broken)
		{
			if ((bool)breakParticles)
			{
				breakParticles.Emit(Mathf.RoundToInt(breakParticles.emissionRate));
			}
			if ((bool)GetComponent<AudioSource>())
			{
				GetComponent<AudioSource>().pitch = Random.Range(0.95f * Time.timeScale, 1f * Time.timeScale);
				GetComponent<AudioSource>().Play();
			}
			myTransform.GetComponent<MeshRenderer>().enabled = false;
			myTransform.GetComponent<BoxCollider>().enabled = false;
			broken = true;
			StartCoroutine(DetectBroken());
		}
	}
}
