using UnityEngine;

public class RFPS_DamageManager : MonoBehaviour
{
	[Tooltip("Amount to increase or decrease base damage of weapon hit on this collider (increase for head shots, decrease for limb hits).")]
	public float damageMultiplier = 1f;

	[Tooltip("Amount of physics force to apply with weapon hit on this collider.")]
	public float damageForce = 2.75f;

	[Tooltip("Sound effect to use for a hit on this collider (doesn't have to be a head shot).")]
	public AudioClip headShot;

	private bool headShotState;

	private Transform myTransform;

	private Rigidbody thisRigidBody;

	public TerroristController damageManager;

	public void ApplyDamage(float damage, Vector3 attackDir, Vector3 attackerPos, Transform attacker, bool isPlayer, bool isExplosion, bool isHead)
	{
		damageManager.damageHit(damageMultiplier * damage, attackDir, attackerPos, attacker, false, isExplosion, isHead);
	}

	private void Start()
	{
		damageManager = GetComponentInParent<TerroristController>();
	}

	private void OnEnable()
	{
		myTransform = base.transform;
		headShotState = false;
		thisRigidBody = myTransform.GetComponent<Rigidbody>();
	}
}
