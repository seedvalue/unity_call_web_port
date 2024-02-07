using UnityEngine;

public class LocationDamage : MonoBehaviour
{
	[Tooltip("Set to Ai.cs component in main NPC object (drag main object from hierachry window into this field).")]
	public AI AIComponent;

	[Tooltip("Amount to increase or decrease base damage of weapon hit on this collider (increase for head shots, decrease for limb hits).")]
	public float damageMultiplier = 1f;

	[Tooltip("Amount of physics force to apply with weapon hit on this collider.")]
	public float damageForce = 2.75f;

	[Tooltip("Sound effect to use for a hit on this collider (doesn't have to be a head shot).")]
	public AudioClip headShot;

	private bool headShotState;

	[Tooltip("Chance between 0 and 1 that killing an NPC with a shot to this collider will trigger slow motion for a few seconds.")]
	[Range(0f, 1f)]
	public float sloMoKillChance;

	[Tooltip("Duration of slow motion time in seconds if slo mo kill chance check is successful.")]
	public float sloMoTime = 0.9f;

	private Transform myTransform;

	private Rigidbody thisRigidBody;

	public bool isHeadShot;

	private void OnEnable()
	{
		myTransform = base.transform;
		headShotState = false;
		thisRigidBody = myTransform.GetComponent<Rigidbody>();
		Mathf.Clamp01(sloMoKillChance);
	}

	public void ApplyDamage(float damage, Vector3 attackDir, Vector3 attackerPos, Transform attacker, bool isPlayer, bool isExplosion)
	{
		if ((bool)AIComponent && (bool)AIComponent.CharacterDamageComponent)
		{
			if (isPlayer)
			{
				if (isHeadShot)
				{
					GameController.instance.AddHeadShot();
				}
				AIComponent.CharacterDamageComponent.ApplyDamage(damage * damageMultiplier, attackDir, attackerPos, attacker, isPlayer, isExplosion, thisRigidBody, damageForce);
				if ((bool)headShot && !headShotState && AIComponent.CharacterDamageComponent.hitPoints <= 0f && Vector3.Distance(myTransform.position, attackerPos) < 15f && !isExplosion)
				{
					PlayAudioAtPos.PlayClipAt(headShot, myTransform.position, 0.6f, 0f);
					if (sloMoKillChance >= Random.value && isPlayer)
					{
						AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.StartCoroutine(AIComponent.PlayerWeaponsComponent.FPSPlayerComponent.ActivateBulletTime(sloMoTime));
					}
					headShotState = true;
				}
			}
			else
			{
				AIComponent.CharacterDamageComponent.ApplyDamage(damage, attackDir, attackerPos, attacker, isPlayer, isExplosion, thisRigidBody, damageForce);
			}
		}
		else
		{
			Debug.Log("<color=red>LocationDamage.cs:</color> NPC body part hit without reference to its main AI.cs script component, please set reference in inspector.");
		}
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (AIComponent.enabled)
		{
			LocationDamage component = hit.collider.GetComponent<LocationDamage>();
			if ((bool)component && !component.AIComponent.enabled)
			{
				Physics.IgnoreCollision(hit.collider, myTransform.GetComponent<Collider>(), true);
			}
		}
	}
}
