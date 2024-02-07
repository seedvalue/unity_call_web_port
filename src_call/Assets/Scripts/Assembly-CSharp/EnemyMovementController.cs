using System.Collections;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
	public Vector3 maxMoveTo;

	private Vector3 initialPos;

	public float movementSpeed;

	public float startWait;

	public float waitTimeOnAPosition;

	public bool canMove;

	public bool isMoveRight;

	private Vector3 targetPos;

	private void Start()
	{
		initialPos = base.transform.position;
		targetPos = maxMoveTo;
	}

	private void Update()
	{
		if (!canMove)
		{
			return;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, movementSpeed * Time.deltaTime);
		if (Vector3.Distance(base.transform.position, targetPos) <= 0.2f)
		{
			if (targetPos == maxMoveTo)
			{
				targetPos = initialPos;
				isMoveRight = !isMoveRight;
			}
			else
			{
				targetPos = maxMoveTo;
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

	public void startMoving()
	{
		canMove = true;
		if (isMoveRight)
		{
			GetComponent<Animator>().SetBool("walkRightWhileAiming", true);
		}
		else
		{
			GetComponent<Animator>().SetBool("walkLeftWhileAiming", true);
		}
	}

	public IEnumerator startChangingPos()
	{
		yield return new WaitForSeconds(startWait);
		startMoving();
		yield return null;
	}

	public void stopMoving()
	{
		canMove = false;
		GetComponent<Animator>().SetBool("walkRightWhileAiming", false);
		GetComponent<Animator>().SetBool("walkLeftWhileAiming", false);
	}
}
