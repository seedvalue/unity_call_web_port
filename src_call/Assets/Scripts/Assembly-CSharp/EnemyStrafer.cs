using UnityEngine;

public class EnemyStrafer : MonoBehaviour
{
	private bool canLookAtPlayer;

	[HideInInspector]
	public EnemyBase enemyBase;

	public float strafeSpeed = 1f;

	public float fireIntervalMin;

	public float fireIntervalMax;

	public float strafIntervalMin;

	public float strafIntervalMax;

	private Vector3 initialPos;

	public Vector3 maxMoveToPos;

	public bool isMoveRight;

	public float startWait;

	private Vector3 targetPos;

	private bool canMove;

	private void OnEnable()
	{
		initialPos = base.transform.position;
		targetPos = maxMoveToPos;
		enemyBase = GetComponent<EnemyBase>();
	}

	public void activateEnemy()
	{
		canLookAtPlayer = true;
		Invoke("startStrafing", startWait);
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
		if (!canMove)
		{
			return;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, strafeSpeed * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, targetPos) <= 0.2f)
		{
			if (targetPos == maxMoveToPos)
			{
				targetPos = initialPos;
				isMoveRight = !isMoveRight;
			}
			else
			{
				targetPos = maxMoveToPos;
				isMoveRight = !isMoveRight;
			}
			if (isMoveRight)
			{
				GetComponent<Animator>().SetBool("walkLeftWhileAiming", false);
				GetComponent<Animator>().SetBool("walkRightWhileAiming", true);
			}
			else
			{
				GetComponent<Animator>().SetBool("walkRightWhileAiming", false);
				GetComponent<Animator>().SetBool("walkLeftWhileAiming", true);
			}
		}
	}

	private void startStrafing()
	{
		CancelInvoke("Fire");
		canMove = true;
		if (isMoveRight)
		{
			GetComponent<Animator>().SetBool("walkRightWhileAiming", true);
		}
		else
		{
			GetComponent<Animator>().SetBool("walkLeftWhileAiming", true);
		}
		Invoke("stopStraffing", Random.Range(strafIntervalMin, strafIntervalMax));
	}

	private void stopStraffing()
	{
		canMove = false;
		GetComponent<Animator>().SetBool("walkRightWhileAiming", false);
		GetComponent<Animator>().SetBool("walkLeftWhileAiming", false);
		Invoke("Fire", enemyBase.fireSpeed);
		Invoke("startStrafing", Random.Range(fireIntervalMin, fireIntervalMax));
	}

	private void Fire()
	{
		enemyBase.fireAtPlayer();
		Invoke("Fire", enemyBase.fireSpeed);
	}

	public void deadAction()
	{
		canLookAtPlayer = false;
		canMove = false;
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
		Debug.Log("REcovered");
		canLookAtPlayer = true;
		Invoke("Fire", enemyBase.fireSpeed);
	}
}
