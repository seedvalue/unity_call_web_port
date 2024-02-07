using UnityEngine;

public class EdgeClimbTriggerActivate : MonoBehaviour
{
	[Tooltip("The box collider to reactivate (set in inspector by dragging trigger object over this field).")]
	public BoxCollider triggerToActivate;

	private void OnTriggerEnter(Collider other)
	{
		triggerToActivate.enabled = true;
	}
}
