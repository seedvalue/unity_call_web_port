using UnityEngine;

public class InstantDeathCollider : MonoBehaviour
{
	[Tooltip("True if this instant death collider should kill an invulnerable player.")]
	public bool killInvulnerable = true;

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player")
		{
			FPSPlayer component = col.GetComponent<FPSPlayer>();
			if (killInvulnerable)
			{
				component.invulnerable = false;
			}
			component.ApplyDamage(component.maximumHitPoints + 1f);
		}
		else if ((bool)col.GetComponent<Rigidbody>())
		{
			col.gameObject.SetActive(false);
		}
	}

	private void Reset()
	{
		if (GetComponent<Collider>() == null)
		{
			base.gameObject.AddComponent<BoxCollider>();
			GetComponent<Collider>().isTrigger = true;
		}
	}
}
