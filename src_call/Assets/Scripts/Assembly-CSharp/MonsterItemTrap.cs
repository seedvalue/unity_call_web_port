using UnityEngine;

public class MonsterItemTrap : MonoBehaviour
{
	[Tooltip("NPC objects to deactivate on level load and activate when player picks up this item.")]
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

	private void ActivateObject()
	{
		for (int i = 0; i < npcsToTrigger.Length; i++)
		{
			if ((bool)npcsToTrigger[i])
			{
				npcsToTrigger[i].SetActive(true);
			}
		}
	}
}
