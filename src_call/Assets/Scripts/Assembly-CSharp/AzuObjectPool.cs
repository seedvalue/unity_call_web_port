using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AzuObjectPool : MonoBehaviour
{
	private Collider FPSWalkerCapsule;

	public static AzuObjectPool instance;

	private Transform myTransform;

	[Tooltip("List of object pools that are active in the scene. Index numbers of pools in this list are used by other scripts to identify which pooled objects to spawn.")]
	public List<MultiDimensionalPool> objRegistry = new List<MultiDimensionalPool>();

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		FPSWalkerCapsule = Camera.main.transform.GetComponent<CameraControl>().FPSWalkerComponent.GetComponent<Collider>();
		myTransform = base.transform;
		for (int i = 0; i < objRegistry.Count; i++)
		{
			for (int j = 0; j < objRegistry[i].poolSize; j++)
			{
				if ((bool)objRegistry[i].prefab)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(objRegistry[i].prefab, myTransform.position, myTransform.rotation);
					if (objRegistry[i].ignorePlayerCollision)
					{
						Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), FPSWalkerCapsule, true);
					}
					gameObject.SetActive(false);
					objRegistry[i].pooledObjs.Add(gameObject);
					gameObject.transform.parent = myTransform;
				}
			}
		}
	}

	public GameObject SpawnPooledObj(int objRegIndex, Vector3 spawnPosition, Quaternion spawnRotation)
	{
		GameObject gameObject = objRegistry[objRegIndex].pooledObjs[objRegistry[objRegIndex].nextActive];
		gameObject.SetActive(true);
		gameObject.transform.position = spawnPosition;
		gameObject.transform.rotation = spawnRotation;
		if (objRegistry[objRegIndex].nextActive == objRegistry[objRegIndex].pooledObjs.Count - 1)
		{
			objRegistry[objRegIndex].nextActive = 0;
		}
		else
		{
			objRegistry[objRegIndex].nextActive++;
		}
		return gameObject;
	}

	public GameObject RecyclePooledObj(int objRegIndex, GameObject obj)
	{
		GameObject gameObject = objRegistry[objRegIndex].pooledObjs[objRegistry[objRegIndex].pooledObjs.IndexOf(obj)];
		gameObject.transform.parent = myTransform;
		gameObject.SetActive(false);
		return gameObject;
	}
}
