using UnityEngine;

public class RagdollScript : MonoBehaviour
{
	private void SetKinematic(bool newValue)
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.isKinematic = newValue;
		}
	}

	private void Start()
	{
		SetKinematic(true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetKinematic(false);
			GetComponent<Animator>().enabled = false;
		}
	}
}
