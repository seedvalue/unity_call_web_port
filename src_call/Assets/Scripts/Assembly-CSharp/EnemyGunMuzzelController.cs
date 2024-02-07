using UnityEngine;

public class EnemyGunMuzzelController : MonoBehaviour
{
	private void OnEnable()
	{
		CancelInvoke();
		Invoke("disableObj", 0.1f);
	}

	private void disableObj()
	{
		base.gameObject.SetActive(false);
	}
}
