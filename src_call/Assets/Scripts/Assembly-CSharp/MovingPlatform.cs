using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	[Tooltip("Destination point for elevator to cycle to.")]
	public Transform pointB;

	[Tooltip("Destination point for elevator to cycle to.")]
	public Transform pointA;

	[Tooltip("Speed of elevator movement.")]
	public float speed = 1f;

	private float direction = 1f;

	private Transform myTransform;

	private IEnumerator Start()
	{
		myTransform = base.transform;
		while (true)
		{
			if (myTransform.position.z < pointA.position.z)
			{
				direction = 1f;
			}
			else if (myTransform.position.z > pointB.position.z)
			{
				direction = -1f;
			}
			float delta = Time.deltaTime * 60f;
			myTransform.Translate(direction * (0f - speed) * delta, 0f, direction * speed * delta, Space.World);
			yield return null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 12 && (bool)collision.gameObject.GetComponent<ShellEjection>())
		{
			AzuObjectPool.instance.RecyclePooledObj(collision.gameObject.GetComponent<ShellEjection>().RBPoolIndex, collision.gameObject);
		}
	}
}
