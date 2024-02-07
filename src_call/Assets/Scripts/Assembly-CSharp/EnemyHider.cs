using UnityEngine;

public class EnemyHider : MonoBehaviour
{
	private bool canLookAtPlayer;

	[HideInInspector]
	public EnemyBase enemyBase;

	public float hideTimeMin;

	public float hideTimeMax;

	public float fireTimeMin;

	public float fireTimeMax;

	public void activateEnemy()
	{
		canLookAtPlayer = true;
		enemyBase = GetComponent<EnemyBase>();
		hideBehind();
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

	private void hideBehind()
	{
		CancelInvoke();
		GetComponent<Animator>().SetBool("crouch", true);
		Invoke("comeOutAndFire", Random.Range(hideTimeMin, hideTimeMax));
	}

	private void comeOutAndFire()
	{
		GetComponent<Animator>().SetBool("crouch", false);
		Invoke("Fire", 1f);
		Invoke("hideBehind", Random.Range(fireTimeMin, fireTimeMax));
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
