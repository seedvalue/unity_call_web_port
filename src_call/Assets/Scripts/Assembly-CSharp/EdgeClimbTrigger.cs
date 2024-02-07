using UnityEngine;

public class EdgeClimbTrigger : MonoBehaviour
{
	[Tooltip("Force that pulls the player upwards when they enter the vault trigger when jumping.")]
	public float upwardPullForce = 0.3f;

	private GameObject playerObj;

	private void Start()
	{
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			FPSRigidBodyWalker component = playerObj.GetComponent<FPSRigidBodyWalker>();
			playerObj.GetComponent<Rigidbody>().AddForce(new Vector3(0f, upwardPullForce, 0f), ForceMode.VelocityChange);
			component.climbing = true;
			component.noClimbingSfx = true;
			component.inputY = 1f;
			component.grounded = true;
			component.jumpBtn = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			FPSRigidBodyWalker component = playerObj.GetComponent<FPSRigidBodyWalker>();
			component.climbing = false;
			component.noClimbingSfx = false;
			base.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
		}
	}
}
