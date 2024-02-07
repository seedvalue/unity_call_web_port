using System.Collections;
using UnityEngine;

public class HostageLevelController : MonoBehaviour
{
	public GameObject[] fireObjs;

	public GameObject molotovObj;

	public GameObject[] breakablePipObjs;

	public GameObject[] waterStreams;

	public GameObject[] smokeObjs;

	public Animator[] hostages;

	public bool molotovFired;

	private void Start()
	{
		molotovObj.SetActive(false);
		for (int i = 0; i < hostages.Length; i++)
		{
			hostages[i].Play("TiedUp", 0, Random.Range(0f, 1f));
		}
	}

	public void fireMolotove()
	{
		GetComponent<LevelController>().gc.takePlayerInput();
		molotovObj.SetActive(true);
		molotovObj.GetComponent<Rigidbody>().isKinematic = false;
		molotovObj.GetComponent<Rigidbody>().AddForce(new Vector3(15f, 50f, -120f), ForceMode.Impulse);
		molotovObj.GetComponent<Rigidbody>().AddTorque(5f, 5f, 5f, ForceMode.Impulse);
		molotovFired = true;
		StartCoroutine(fireHostages());
	}

	private IEnumerator fireHostages()
	{
		yield return new WaitForSeconds(1.5f);
		for (int i = 0; i < fireObjs.Length; i++)
		{
			fireObjs[i].SetActive(true);
			if (i < hostages.Length)
			{
				hostages[i].speed = 1.5f;
			}
			yield return new WaitForSeconds(0.2f);
		}
		GetComponent<LevelController>().gc.showHintMenu();
	}

	public IEnumerator breakThePipe()
	{
		for (int i = 0; i < breakablePipObjs.Length; i++)
		{
			breakablePipObjs[i].SetActive(false);
			waterStreams[i].SetActive(true);
			smokeObjs[i].SetActive(true);
			yield return new WaitForSeconds(0.1f);
		}
		waterStreams[3].SetActive(true);
		yield return new WaitForSeconds(2.5f);
		for (int j = fireObjs.Length - 1; j >= 0; j--)
		{
			fireObjs[j].SetActive(false);
			yield return new WaitForSeconds(0.6f);
		}
		molotovObj.SetActive(false);
		yield return new WaitForSeconds(1.5f);
		GetComponent<LevelController>().gc.setHostageLevelCompleted();
	}
}
