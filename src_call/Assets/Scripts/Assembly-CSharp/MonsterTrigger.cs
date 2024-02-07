using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
	[Tooltip("NPC objects to deactivate on level load and activate when player walks into trigger.")]
	public GameObject[] npcsToTrigger;

	private void Start()
	{
		for (int i = 0; i < npcsToTrigger.Length; i++)
		{
			if ((bool)npcsToTrigger[i])
			{
				npcsToTrigger[i].SetActive(false);
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!(col.gameObject.tag == "Player"))
		{
			return;
		}
		for (int i = 0; i < npcsToTrigger.Length; i++)
		{
			if ((bool)npcsToTrigger[i])
			{
				npcsToTrigger[i].SetActive(true);
			}
		}
		Object.Destroy(base.transform.gameObject);
	}
}
