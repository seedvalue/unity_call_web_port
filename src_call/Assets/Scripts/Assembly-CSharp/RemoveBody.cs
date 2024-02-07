using UnityEngine;

public class RemoveBody : MonoBehaviour
{
	private float startTime;

	[HideInInspector]
	public float bodyStayTime = 15f;

	[Tooltip("Weapon item pickup that should be spawned after NPC dies (used for single capsule collider NPCs which instantiate ragdoll on death).")]
	public GameObject GunPickup;

	private void Start()
	{
		startTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (startTime + bodyStayTime < Time.time)
		{
			if ((bool)GunPickup)
			{
				GunPickup.transform.parent = null;
			}
			ArrowObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<ArrowObject>(true);
			ArrowObject[] array = componentsInChildren;
			foreach (ArrowObject arrowObject in array)
			{
				arrowObject.transform.parent = null;
				arrowObject.myRigidbody.isKinematic = false;
				arrowObject.myBoxCol.isTrigger = false;
				arrowObject.gameObject.tag = "Usable";
				arrowObject.falling = true;
			}
			Object.Destroy(base.gameObject);
		}
	}
}
