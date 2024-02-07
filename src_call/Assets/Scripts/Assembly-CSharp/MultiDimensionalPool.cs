using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultiDimensionalPool
{
	[Tooltip("The gameobject used to create an object pool.")]
	public GameObject prefab;

	[Tooltip("Number of this type of object to store in object pool.")]
	public int poolSize;

	[HideInInspector]
	public int nextActive;

	[Tooltip("True if spawned pooled object should ignore collision with player.")]
	public bool ignorePlayerCollision;

	[Tooltip("List of pooled game objects (should never have any missing list entries at runtime).")]
	public List<GameObject> pooledObjs = new List<GameObject>();
}
