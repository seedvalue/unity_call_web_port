using UnityEngine;

public static class PlayAudioAtPos
{
	public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float vol, float blend = 1f, float pitch = 1f, float minDist = 1f, float maxDist = 500f)
	{
		GameObject gameObject = AzuObjectPool.instance.SpawnPooledObj(0, pos, Quaternion.identity);
		TempAudioTimer component = gameObject.GetComponent<TempAudioTimer>();
		AudioSource aSource = component.aSource;
		aSource.clip = clip;
		aSource.spatialBlend = blend;
		aSource.minDistance = minDist;
		aSource.maxDistance = maxDist;
		aSource.volume = vol;
		aSource.pitch = pitch;
		aSource.Play();
		component.StartCoroutine(component.DeactivateTimer());
		return aSource;
	}
}
