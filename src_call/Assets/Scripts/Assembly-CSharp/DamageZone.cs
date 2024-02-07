using UnityEngine;

public class DamageZone : MonoBehaviour
{
	[Tooltip("Amount of damage to apply to player while in damage trigger.")]
	public float damage = 1f;

	[Tooltip("Delay before player is damaged again by this damage zone.")]
	public float delay = 1.75f;

	private float damageTime;

	private FPSPlayer FPSPlayerComponent;

	private void Start()
	{
		FPSPlayerComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
	}

	private void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Player" && damageTime < Time.time)
		{
			FPSPlayerComponent.ApplyDamage(damage);
			damageTime = Time.time + delay;
		}
		if (col.gameObject.layer == 13 && (bool)col.GetComponent<CharacterDamage>())
		{
			CharacterDamage component = col.GetComponent<CharacterDamage>();
			if (damageTime < Time.time)
			{
				component.ApplyDamage(damage, Vector3.zero, base.transform.position, null, false, false);
				damageTime = Time.time + delay;
			}
		}
	}
}
