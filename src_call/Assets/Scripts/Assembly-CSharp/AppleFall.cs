using UnityEngine;

public class AppleFall : MonoBehaviour
{
	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
	}

	public void ApplyDamage(float damage)
	{
		if (!myTransform.GetComponent<Rigidbody>().useGravity)
		{
			GetComponent<AudioSource>().pitch = Random.Range(0.75f * Time.timeScale, 1f * Time.timeScale);
			GetComponent<AudioSource>().Play();
			myTransform.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	public void OnCollisionEnter()
	{
		if (!myTransform.GetComponent<Rigidbody>().useGravity)
		{
			GetComponent<AudioSource>().pitch = Random.Range(0.75f * Time.timeScale, 1f * Time.timeScale);
			GetComponent<AudioSource>().Play();
			myTransform.GetComponent<Rigidbody>().useGravity = true;
		}
	}
}
