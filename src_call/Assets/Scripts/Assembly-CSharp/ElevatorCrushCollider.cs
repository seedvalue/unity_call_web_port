using UnityEngine;

public class ElevatorCrushCollider : MonoBehaviour
{
	[Tooltip("Sound effect to play when player is crushed under elevator.")]
	public AudioClip squishSnd;

	private bool fxPlayed;

	private float crushTime;

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player")
		{
			FPSPlayer component = col.GetComponent<FPSPlayer>();
			if ((bool)component && !fxPlayed)
			{
				component.ApplyDamage(component.maximumHitPoints + 1f);
				PlayAudioAtPos.PlayClipAt(squishSnd, component.transform.position, 0.75f);
				crushTime = Time.time;
				fxPlayed = true;
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Player" && crushTime + 1f < Time.time)
		{
			fxPlayed = false;
		}
	}
}
