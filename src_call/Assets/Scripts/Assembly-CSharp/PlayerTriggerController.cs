using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
	public bool isHostageLevel;

	public bool isPlantBombLevel;

	public bool isSpecialTrigger;

	public GameObject[] enemiesToActivate;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (isHostageLevel)
			{
				base.transform.parent.GetComponent<HostageLevelController>().fireMolotove();
			}
			else if (isPlantBombLevel)
			{
				base.transform.parent.GetComponent<LevelController>().enablePlantBomb();
			}
			else if (isSpecialTrigger)
			{
				base.transform.parent.GetComponent<LevelController>().activateCertainEnemies(other.gameObject, enemiesToActivate);
			}
			else
			{
				base.transform.parent.GetComponent<LevelController>().playerEnterInRange(other.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (isPlantBombLevel)
		{
			base.transform.parent.GetComponent<LevelController>().gc.showBombBtn();
		}
	}
}
