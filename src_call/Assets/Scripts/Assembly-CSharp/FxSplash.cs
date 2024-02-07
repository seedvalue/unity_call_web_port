using UnityEngine;

public class FxSplash : MonoBehaviour
{
	private Rigidbody rb;

	public ParticleSystem Ps_Splash;

	public ParticleSystem Ps_Trail;

	private MeshRenderer mh;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		mh = GetComponent<MeshRenderer>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		mh.enabled = false;
		Ps_Trail.Stop();
		Ps_Splash.Play();
	}

	private void DestroyFx()
	{
		Object.Destroy(base.gameObject);
	}
}
