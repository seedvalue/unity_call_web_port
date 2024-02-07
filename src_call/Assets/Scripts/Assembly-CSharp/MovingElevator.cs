using System.Collections;
using UnityEngine;

public class MovingElevator : MonoBehaviour
{
	[Tooltip("Destination point for elevator to cycle to.")]
	public Transform pointB;

	private Vector3 pointA;

	[Tooltip("Speed of elevator movement.")]
	public float speed = 1f;

	private float direction = 1f;

	private Transform myTransform;

	private IEnumerator Start()
	{
		myTransform = base.transform;
		pointA = base.transform.position;
		while (true)
		{
			if (myTransform.position.y > pointA.y)
			{
				direction = -1f;
			}
			else if (myTransform.position.y < pointB.position.y)
			{
				direction = 1f;
			}
			float delta = Time.deltaTime * 60f;
			myTransform.Translate(0f, direction * speed * delta, 0f, Space.World);
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
