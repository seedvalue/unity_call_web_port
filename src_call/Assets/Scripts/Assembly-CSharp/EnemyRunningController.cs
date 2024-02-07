using System.Collections;
using UnityEngine;

public class EnemyRunningController : MonoBehaviour
{
	public Vector3[] moveToPoints;

	public int currentPointIndex;

	public float firstRunDelay;

	public void moveToNextPoint()
	{
		currentPointIndex++;
		if (currentPointIndex >= moveToPoints.Length)
		{
			currentPointIndex = 0;
		}
		StartCoroutine(runTowardsPoint());
	}

	private IEnumerator runTowardsPoint()
	{
		if (currentPointIndex == 0)
		{
			yield return new WaitForSeconds(firstRunDelay);
		}
		base.transform.LookAt(moveToPoints[currentPointIndex]);
		GetComponent<Animator>().SetBool("Run", true);
		while (Vector3.Distance(base.transform.position, moveToPoints[currentPointIndex]) > 0.1f)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPoints[currentPointIndex], 3f * Time.deltaTime);
			yield return null;
		}
		GetComponent<EnemyController>().stopRunningAndAim();
	}
}
