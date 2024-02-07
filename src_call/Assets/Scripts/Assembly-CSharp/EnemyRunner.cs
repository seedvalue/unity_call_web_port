using System.Collections;
using UnityEngine;

public class EnemyRunner : MonoBehaviour
{
	private bool canLookAtPlayer;

	[HideInInspector]
	public EnemyBase enemyBase;

	public Vector3[] moveToPoints;

	private int currentPointIndex;

	public float startDelay;

	public float stayTimeMin;

	public float stayTimeMax;

	public bool stopAtLast;

	private void OnEnable()
	{
		moveToPoints[0] = base.transform.position;
		enemyBase = GetComponent<EnemyBase>();
	}

	public void activateEnemy()
	{
		canLookAtPlayer = true;
		StartCoroutine(runAtPoint(startDelay));
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

	private IEnumerator runAtPoint(float startWait)
	{
		yield return new WaitForSeconds(startWait);
		CancelInvoke();
		canLookAtPlayer = false;
		currentPointIndex++;
		if (currentPointIndex >= moveToPoints.Length)
		{
			currentPointIndex = 0;
			if (stopAtLast)
			{
				Invoke("Fire", enemyBase.fireSpeed);
				canLookAtPlayer = true;
				StopAllCoroutines();
				yield return null;
			}
		}
		base.transform.LookAt(moveToPoints[currentPointIndex]);
		GetComponent<Animator>().SetBool("Run", true);
		while (Vector3.Distance(base.transform.position, moveToPoints[currentPointIndex]) > 0.1f)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPoints[currentPointIndex], 4f * Time.deltaTime);
			yield return null;
		}
		canLookAtPlayer = true;
		GetComponent<Animator>().SetBool("Run", false);
		Invoke("Fire", enemyBase.fireSpeed);
		StartCoroutine(runAtPoint(Random.Range(stayTimeMin, stayTimeMax)));
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
		StopAllCoroutines();
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
