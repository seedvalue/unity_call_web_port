using UnityEngine;

public class WorldRecenter : MonoBehaviour
{
	private Object[] objects;

	[Tooltip("Re-center objects if player moves further than this distance from scene origin (prevents floating point imprecision and model jitter in large scenes, but currently incompatible with navmesh and static objects).")]
	public float threshold = 700f;

	[Tooltip("Refresh terrain to update tree colliders (can cause momentary hiccup on large terrains).")]
	public bool refreshTerrain = true;

	[HideInInspector]
	public float worldRecenterTime;

	private ParticleSystem.Particle[] emitterParticles = new ParticleSystem.Particle[1];

	private int numParticlesAlive;

	private void LateUpdate()
	{
		Vector3 position = base.gameObject.transform.position;
		position.y = 0f;
		if (!(position.magnitude > threshold))
		{
			return;
		}
		worldRecenterTime = Time.time;
		if (!(worldRecenterTime + 0.2f * Time.timeScale > Time.time))
		{
			return;
		}
		objects = Object.FindObjectsOfType(typeof(Transform));
		Object[] array = objects;
		foreach (Object @object in array)
		{
			Transform transform = (Transform)@object;
			if (transform.parent == null && transform.gameObject.layer != 14)
			{
				transform.position -= position;
			}
		}
		objects = Object.FindObjectsOfType(typeof(ParticleSystem));
		Object[] array2 = objects;
		foreach (Object object2 in array2)
		{
			ParticleSystem particleSystem = (ParticleSystem)object2;
			numParticlesAlive = particleSystem.GetParticles(emitterParticles);
			for (int k = 0; k < numParticlesAlive; k++)
			{
				emitterParticles[k].position -= position;
			}
			particleSystem.SetParticles(emitterParticles, numParticlesAlive);
		}
		if (refreshTerrain && (bool)Terrain.activeTerrain)
		{
			TerrainData terrainData = Terrain.activeTerrain.terrainData;
			float[,] heights = terrainData.GetHeights(0, 0, 0, 0);
			terrainData.SetHeights(0, 0, heights);
		}
	}
}
