using UnityEngine;

public class GrenadeObject : MonoBehaviour
{
	[HideInInspector]
	public float fuseTimeAmt;

	private float startTime;

	private ExplosiveObject ExplosiveObjectComponent;

	private WeaponBehavior WeaponBehaviorComponent;

	private void Start()
	{
		ExplosiveObjectComponent = GetComponent<ExplosiveObject>();
	}

	private void OnEnable()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		if (startTime + fuseTimeAmt < Time.time)
		{
			ExplosiveObjectComponent.ApplyDamage(ExplosiveObjectComponent.hitPoints + 1f);
		}
	}
}
