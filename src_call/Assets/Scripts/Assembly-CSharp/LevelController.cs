using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public Vector3 startRotationAngles;

	[HideInInspector]
	public EnemyBase[] enemies;

	public bool allEnemiesActive;

	public string missionDetails;

	[HideInInspector]
	public int enemiesKilled;

	[HideInInspector]
	public GameController gc;

	[HideInInspector]
	public GameObject activationTrigger;

	public string hintText;

	[HideInInspector]
	public GameObject plantedBomb;

	[HideInInspector]
	public GameObject bombParticles;

	[HideInInspector]
	public bool isSaperatelyTriggered;

	public int totalEnemiesCount;

	public List<AI> levelEnemies;

	public static LevelController instance;

	[HideInInspector]
	public Transform[] playerPoints;

	[HideInInspector]
	public int[] playerIndexWise;

	private int currentPointIndex = -1;

	private int currentPointPlayers;

	private int currentWaveEnemiesKilled;

	public InputControl inputController;

	public Transform playerStartingPosition;

	public bool NoMap;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void playerEnterInRange(GameObject player)
	{
		allEnemiesActive = true;
		activationTrigger.SetActive(false);
	}

	public void activateCertainEnemies(GameObject player, GameObject[] enemiesToActivate)
	{
	}

	public void EnemyKilled(Transform enemy)
	{
		Debug.Log("LevelController : EnemyKilled ");
		enemiesKilled++;
		currentWaveEnemiesKilled++;
		if (currentWaveEnemiesKilled >= currentPointPlayers)
		{
			currentWaveEnemiesKilled = 0;
			ChangePlayerPosition();
		}
		
		if (enemiesKilled >= totalEnemiesCount)
		{
			if (gc)
			{
				gc.SetMissionCompleted();
			}
			else
			{
				Debug.LogError("EnemyKilled : gc == NULL");
			}
		}
		
		Vector3 position = gc.fpsPlayer.position;
		position.z += Vector3.Distance(gc.fpsPlayer.position, enemy.position) / 10f;
		AudioSource.PlayClipAtPoint(gc.deathClips[Random.Range(0, gc.deathClips.Length)], position);
	}

	public void enablePlantBomb()
	{
		gc.enablePlantBomb();
	}

	public void pickupBomb()
	{
		gc.showBombBtn();
	}

	public void ChangePlayerPosition()
	{
	}

	private IEnumerator WaitToCallNext()
	{
		yield return new WaitForSeconds(2f);
		if (playerPoints.Length > 0 && currentPointIndex < playerPoints.Length - 1)
		{
			currentPointIndex++;
			inputController.MoveToNextPos(playerPoints[currentPointIndex]);
			if (playerIndexWise.Length > 0)
			{
				currentPointPlayers = playerIndexWise[currentPointIndex];
			}
		}
	}

	public void AlertEnemies()
	{
		StartCoroutine(waitToAlert());
	}

	private IEnumerator waitToAlert()
	{
		yield return new WaitForSeconds(1f);
		if (levelEnemies.Count > 0)
		{
			int j;
			for (j = 0; j < playerIndexWise[currentPointIndex]; j++)
			{
				levelEnemies[j].transform.gameObject.SetActive(true);
				yield return new WaitForSeconds(1f);
				levelEnemies.RemoveAt(j);
				playerIndexWise[currentPointIndex]--;
				j--;
				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
