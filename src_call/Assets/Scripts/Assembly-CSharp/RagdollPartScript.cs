using UnityEngine;

public class RagdollPartScript : MonoBehaviour
{
	public StairDismount mainScript;

	private void Start()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (base.transform.root != collision.transform.root && collision.relativeVelocity.magnitude > 4f)
		{
			int num = 100 * Mathf.RoundToInt(collision.relativeVelocity.magnitude);
			MonoBehaviour.print(base.gameObject.name + " collided with " + collision.gameObject.name + ", giving score " + num);
			mainScript.score += num;
		}
	}

	private void Update()
	{
	}
}
