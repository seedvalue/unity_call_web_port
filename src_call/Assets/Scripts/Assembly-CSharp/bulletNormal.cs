using UnityEngine;

public class bulletNormal : MonoBehaviour
{
	public float LifeTime = 2f;

	public float Damage = 5f;

	public float speed = 100f;

	public Transform target;

	public float homingTrackingSpeed = 3f;

	private Quaternion targetRotation;

	private FPSPlayer fpsPlayer;

	private void Start()
	{
		Move();
		Object.Destroy(base.gameObject, LifeTime);
	}

	public void SetTarget(Transform Target)
	{
		target = Target;
	}

	private void Update()
	{
		Move();
	}

	private void Move()
	{
		base.transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Player")
		{
			fpsPlayer = other.gameObject.GetComponent<FPSPlayer>();
			if ((bool)fpsPlayer)
			{
				fpsPlayer.ApplyDamage(Damage);
			}
		}
		base.gameObject.SetActive(false);
	}
}
