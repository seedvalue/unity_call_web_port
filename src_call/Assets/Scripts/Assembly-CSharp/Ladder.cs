using UnityEngine;

public class Ladder : MonoBehaviour
{
	private GameObject playerObj;

	[Tooltip("If false, climbing footstep sounds won't be played.")]
	public bool playClimbingAudio = true;

	private InputControl InputComponent;

	private void Start()
	{
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		InputComponent = playerObj.GetComponent<InputControl>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			FPSRigidBodyWalker component = playerObj.GetComponent<FPSRigidBodyWalker>();
			component.climbing = true;
			if (!playClimbingAudio)
			{
				component.noClimbingSfx = true;
			}
			if (Mathf.Abs(component.inputY) < 0.1f && InputComponent.crouchHold)
			{
				component.crouchState = false;
			}
		}
	}

	private void OnTriggerExit(Collider other2)
	{
		if (other2.gameObject.tag == "Player")
		{
			FPSRigidBodyWalker component = playerObj.GetComponent<FPSRigidBodyWalker>();
			component.climbing = false;
			component.landStartTime = Time.time + 0.25f;
			component.jumpBtn = false;
			component.noClimbingSfx = false;
		}
	}
}
