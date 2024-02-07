using UnityEngine;

public class StairDismount : MonoBehaviour
{
	private float impactEndTime;

	private Rigidbody impactTarget;

	private Vector3 impact;

	public int score;

	public GameObject scoreTextTemplate;

	private void Start()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			RagdollPartScript ragdollPartScript = rigidbody.gameObject.AddComponent<RagdollPartScript>();
			ragdollPartScript.mainScript = this;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo) && hitInfo.rigidbody != null)
			{
				RagdollHelper component = GetComponent<RagdollHelper>();
				component.ragdolled = true;
				impactTarget = hitInfo.rigidbody;
				impact = ray.direction * 2f;
				impactEndTime = Time.time + 0.25f;
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RagdollHelper component2 = GetComponent<RagdollHelper>();
			component2.ragdolled = false;
		}
		if (Time.time < impactEndTime)
		{
			impactTarget.AddForce(impact, ForceMode.VelocityChange);
		}
	}
}
