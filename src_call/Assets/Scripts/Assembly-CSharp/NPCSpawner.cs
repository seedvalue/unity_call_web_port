using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
	[Tooltip("Set to the wave manager object if this spawner should be controled by the wave manager")]
	public WaveManager WaveManager;

	[Tooltip("If not linked to a wave manager to spawn NPC waves, this is the NPC prefab that will be spawned")]
	public GameObject NPCPrefab;

	[Tooltip("Delay until spawning next NPC from this spawner.")]
	public float spawnDelay = 30f;

	private float spawnTime;

	private GameObject NPCInstance;

	private List<AI> Npcs = new List<AI>();

	private float timeLeft;

	[Tooltip("The waypoint group that this NPC should patrol after spawning.")]
	public WaypointGroup waypointGroup;

	[Tooltip("The number of the waypoint in the waypoint group that should be traveled to first.")]
	public int firstWaypoint = 1;

	private AI AIcomponent;

	[Tooltip("True if spawner should continuously spawn NPCs.")]
	public bool unlimitedSpawning = true;

	[Tooltip("True if this NPC should hunt the player across the map")]
	public bool huntPlayer;

	[Tooltip("Maximuim number of NPCs from this spawner that can be active in the scene at a time.")]
	public int maxActiveNpcs = 3;

	[Tooltip("The number of NPCs to spawn if not spawning unlimited NPCs.")]
	public int NpcsToSpawn = 5;

	[HideInInspector]
	public int spawnedNpcAmt;

	[HideInInspector]
	public bool pauseSpawning;

	private void Start()
	{
		spawnTime = -1024f;
	}

	private void Update()
	{
		if (!pauseSpawning && Npcs.Count < maxActiveNpcs && (unlimitedSpawning || (!unlimitedSpawning && spawnedNpcAmt < NpcsToSpawn)) && spawnTime + spawnDelay < Time.time)
		{
			Spawn(NPCPrefab);
		}
	}

	private void Spawn(GameObject NpcPrefab)
	{
		if ((bool)NPCPrefab)
		{
			NPCInstance = Object.Instantiate(NpcPrefab, base.transform.position, base.transform.rotation);
			AI component = NPCInstance.GetComponent<AI>();
			Npcs.Add(component);
			component.NPCSpawnerComponent = base.transform.GetComponent<NPCSpawner>();
			if (huntPlayer)
			{
				component.huntPlayer = true;
			}
			component.spawned = true;
			component.waypointGroup = waypointGroup;
			component.firstWaypoint = firstWaypoint;
			component.walkOnPatrol = false;
			component.standWatch = false;
			component.SpawnNPC();
			spawnTime = Time.time;
			if (huntPlayer)
			{
				component.GoToPosition(component.playerObj.transform.position, true);
			}
			else
			{
				component.GoToPosition(component.waypointGroup.wayPoints[firstWaypoint].transform.position, true);
			}
			spawnedNpcAmt++;
		}
	}

	public void UnregisterSpawnedNPC(AI NpcAI)
	{
		for (int i = 0; i < Npcs.Count; i++)
		{
			if (!(Npcs[i] == NpcAI))
			{
				continue;
			}
			Npcs.RemoveAt(i);
			if ((bool)WaveManager && WaveManager.enabled)
			{
				WaveManager.killedNpcs++;
				if (WaveManager.killedNpcs >= WaveManager.NpcsToSpawn)
				{
					StartCoroutine(WaveManager.StartWave());
				}
			}
			break;
		}
	}
}
