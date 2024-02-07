using System;
using UnityEngine;

[Serializable]
public class MultiDimensionalInt
{
	[Tooltip("Total number of NPCs to spawn for this wave.")]
	public int[] NpcCounts;

	[Tooltip("Maximum number of NPCs from the spawner that can be active in the scene at once.")]
	public int[] NpcLoads;

	[Tooltip("Delay between spawning of NPCs for this wave.")]
	public float[] NpcDelay;

	[Tooltip("The NPC Prefabs that will be spawned for this wave.")]
	public GameObject[] NpcTypes;
}
