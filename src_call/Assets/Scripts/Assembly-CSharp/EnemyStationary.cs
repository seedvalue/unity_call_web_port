using UnityEngine;

public class EnemyStationary : MonoBehaviour
{
	private bool canLookAtPlayer;

	[HideInInspector]
	public EnemyBase enemyBase;

	private void OnEnable()
	{
		enemyBase = GetComponent<EnemyBase>();
	}

	public void activateEnemy()
	{
		canLookAtPlayer = true;
		Invoke("Fire", enemyBase.fireSpeed);
	}

	private void Update()
	{
		if (canLookAtPlayer)
		{
			Vector3 forward = enemyBase.target.position - base.transform.position;
			if (!enemyBase.isAboveGround)
			{
				forward.y = 0f;
			}
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, b, Time.deltaTime * 10f);
		}
	}

	private void Fire()
	{
		enemyBase.fireAtPlayer();
		Invoke("Fire", enemyBase.fireSpeed);
	}

	public void deadAction()
	{
		canLookAtPlayer = false;
		CancelInvoke();
	}

	public void flashedEnemy()
	{
		CancelInvoke();
		StopAllCoroutines();
		canLookAtPlayer = false;
	}

	public void recoverFromFlash()
	{
		canLookAtPlayer = true;
		Invoke("Fire", enemyBase.fireSpeed);
	}
}
