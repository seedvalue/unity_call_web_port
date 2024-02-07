using UnityEngine;

[ExecuteInEditMode]
public class LaunchEffect : MonoBehaviour
{
	public float thrust;

	public Vector3 Dir;

	private Rigidbody rb;

	private ParticleSystem Ps;

	public GameObject FxPrefabLOW;

	public GameObject FxPrefabHIGH;

	public GameObject Anim;

	private MeshRenderer mh;

	private void Start()
	{
		Invoke("HideSplash", 2.5f);
	}

	public void LaunchSlow(int _Quality)
	{
		Time.timeScale = 0.5f;
		Launch(_Quality);
	}

	public void LaunchNormal(int _Quality)
	{
		Time.timeScale = 1f;
		Launch(_Quality);
	}

	private void Launch(int _Quality)
	{
		if (_Quality == 0)
		{
			GameObject gameObject = Object.Instantiate(FxPrefabLOW, new Vector3(0f, 0f, 0f), Quaternion.identity);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(320f, -90f, 0f);
			rb = gameObject.GetComponent<Rigidbody>();
			mh = gameObject.GetComponent<MeshRenderer>();
			LaunchFX();
		}
		else
		{
			GameObject gameObject2 = Object.Instantiate(FxPrefabHIGH, new Vector3(0f, 0f, 0f), Quaternion.identity);
			gameObject2.transform.parent = base.gameObject.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.Euler(320f, -90f, 0f);
			rb = gameObject2.GetComponent<Rigidbody>();
			mh = gameObject2.GetComponent<MeshRenderer>();
			LaunchFX();
		}
	}

	private void LaunchFX()
	{
		mh.enabled = true;
		rb.AddForce(Dir * thrust);
	}

	private void HideSplash()
	{
		Anim.SetActive(false);
	}
}
