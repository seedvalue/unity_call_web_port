using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
	[Tooltip("Pickup item to spawn repeatedly with this script.")]
	public GameObject gunPrefab;

	[Tooltip("Delay between item spawns from spawner.")]
	public float spawnTime = 30f;

	[HideInInspector]
	public GameObject gunInstance;

	private float timeLeft;

	private void Start()
	{
		timeLeft = 0f;
	}

	private void Update()
	{
		if (timeLeft > spawnTime || (bool)gunInstance)
		{
			timeLeft = spawnTime;
		}
		else if (timeLeft <= 0f)
		{
			timeLeft = 0f;
			Spawn();
		}
		if (!gunInstance)
		{
			timeLeft -= Time.deltaTime;
		}
	}

	private void Spawn()
	{
		gunInstance = Object.Instantiate(gunPrefab, base.transform.position, base.transform.rotation);
		timeLeft = spawnTime;
	}
}
