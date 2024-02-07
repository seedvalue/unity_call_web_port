using System.Collections;
using UnityEngine;

public class EnemyTakePositionController : MonoBehaviour
{
	public Vector3 positionToTake;

	public bool moveRight;

	public bool moveLeft;

	public float movementSpeed;

	public float startWaitTime;

	private Vector3 initialPos;

	public IEnumerator moveToTargetPosition()
	{
		yield return new WaitForSeconds(startWaitTime);
		if (moveRight)
		{
			GetComponent<Animator>().SetBool("walkRightWhileAiming", true);
		}
		else
		{
			GetComponent<Animator>().SetBool("walkLeftWhileAiming", true);
		}
		while (Vector3.Distance(base.transform.position, positionToTake) > 0.2f)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, positionToTake, movementSpeed * Time.deltaTime);
			yield return null;
		}
		GetComponent<Animator>().SetBool("walkRightWhileAiming", false);
		GetComponent<Animator>().SetBool("walkLeftWhileAiming", false);
		GetComponent<EnemyController>().startFiringAfterLocking();
	}

	public IEnumerator startMovingEnemy()
	{
		yield return new WaitForSeconds(startWaitTime);
		initialPos = base.transform.position;
		if (moveRight)
		{
			GetComponent<Animator>().SetBool("walkRightWhileAiming", true);
		}
		else
		{
			GetComponent<Animator>().SetBool("walkLeftWhileAiming", true);
		}
	}
}
