using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SWS
{
	public class EventCollisionTrigger : MonoBehaviour
	{
		public bool onTrigger = true;

		public bool onCollision = true;

		public UnityEvent myEvent;

		private void OnTriggerEnter(Collider other)
		{
			if (onTrigger)
			{
				myEvent.Invoke();
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if (onCollision)
			{
				myEvent.Invoke();
			}
		}

		public void ApplyForce(int power)
		{
			Vector3 position = base.transform.position;
			float num = 5f;
			Collider[] array = Physics.OverlapSphere(position, num);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				navMove component = collider.GetComponent<navMove>();
				if (component != null)
				{
					component.Stop();
					collider.GetComponent<NavMeshAgent>().enabled = false;
					collider.isTrigger = false;
				}
				Rigidbody component2 = collider.GetComponent<Rigidbody>();
				if (component2 != null)
				{
					component2.AddExplosionForce(power, position, num, 100f);
				}
			}
		}
	}
}
