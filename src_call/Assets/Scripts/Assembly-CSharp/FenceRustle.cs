using UnityEngine;

public class FenceRustle : MonoBehaviour
{
	[Tooltip("Sound effects to play when player runs up against this object.")]
	public AudioClip[] fenceRustles;

	[Tooltip("Volume of sound effects when player runs up against this object.")]
	public float rustleVol = 1f;

	private float lastRustleTime;

	private void OnTriggerEnter(Collider col)
	{
		if (fenceRustles.Length > 0 && (col.gameObject.layer == 11 || col.gameObject.layer == 13) && Time.time - lastRustleTime > 1f)
		{
			PlayAudioAtPos.PlayClipAt(fenceRustles[Random.Range(0, fenceRustles.Length)], base.transform.position, rustleVol);
			lastRustleTime = Time.time;
		}
	}
}
