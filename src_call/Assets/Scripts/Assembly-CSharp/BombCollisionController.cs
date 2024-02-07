using UnityEngine;

public class BombCollisionController : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			base.transform.parent.GetComponent<LevelController>().pickupBomb();
			base.gameObject.SetActive(false);
		}
	}
}
