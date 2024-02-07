using UnityEngine;

public class FoliageRustle : MonoBehaviour
{
	private AudioSource rustleFx;

	private Footsteps FootstepsComponent;

	private void Start()
	{
		FootstepsComponent = Camera.main.transform.parent.transform.GetComponent<Footsteps>();
		rustleFx = base.transform.gameObject.AddComponent<AudioSource>();
		rustleFx.spatialBlend = 1f;
		rustleFx.volume = FootstepsComponent.foliageRustleVol;
		rustleFx.pitch = 1f;
		rustleFx.dopplerLevel = 0f;
		rustleFx.maxDistance = 10f;
		rustleFx.rolloffMode = AudioRolloffMode.Linear;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (FootstepsComponent.foliageRustles.Length > 0 && (col.gameObject.layer == 11 || col.gameObject.layer == 13))
		{
			rustleFx.clip = FootstepsComponent.foliageRustles[Random.Range(0, FootstepsComponent.foliageRustles.Length)];
			rustleFx.PlayOneShot(rustleFx.clip);
		}
	}
}
