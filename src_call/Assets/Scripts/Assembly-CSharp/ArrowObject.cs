using UnityEngine;

public class ArrowObject : MonoBehaviour
{
	[HideInInspector]
	public Rigidbody myRigidbody;

	private MeshRenderer myMeshRenderer;

	[HideInInspector]
	public bool hit;

	[HideInInspector]
	public bool falling;

	[Tooltip("Base damage of arrow without damage add amount.")]
	public float damage = 50f;

	[Tooltip("Maximum additional damage of arrow to inflict with full strength pull.")]
	public float damageAdd = 20f;

	[Tooltip("Force to apply to rigidbody that is hit with arrow.")]
	public float force = 3f;

	[HideInInspector]
	public float damageAddAmt;

	private FPSPlayer FPSPlayerComponent;

	private AmmoPickup AmmoPickupComponent;

	private float hitTime;

	private float startTime;

	private bool startState;

	[Tooltip("Time that arrow object stays in scene after hitting object.")]
	public float waitDuration = 30f;

	[HideInInspector]
	public int objectPoolIndex;

	private Collider hitCol;

	[HideInInspector]
	public BoxCollider myBoxCol;

	private GameObject emptyObject;

	[Tooltip("Scale of the arrow object.")]
	public Vector3 scale;

	[Tooltip("Initial size of the arrow collider (increased after hit to make pick up easier).")]
	public Vector3 initialColSize;

	[Tooltip("True if helper gizmos for arrow object should be shown to assist setting script values.")]
	public bool drawHelperGizmos;

	public RaycastHit arrowRayHit;

	[Tooltip("Distance in front of arrow to check for hits (scaled up at higher velocities).")]
	public float hitCheckDist = 0.4f;

	[Tooltip("Layers that the arrow will collide with.")]
	public LayerMask rayMask;

	[HideInInspector]
	public float velFactor;

	[HideInInspector]
	public float visibleDelay;

	private void Start()
	{
		FPSPlayerComponent = Camera.main.transform.GetComponent<CameraControl>().playerObj.transform.GetComponent<FPSPlayer>();
	}

	public void InitializeProjectile()
	{
		hit = false;
		falling = false;
		AmmoPickupComponent = GetComponent<AmmoPickup>();
		AmmoPickupComponent.enabled = false;
		myRigidbody = GetComponent<Rigidbody>();
		myBoxCol = GetComponent<BoxCollider>();
		myMeshRenderer = GetComponent<MeshRenderer>();
		myMeshRenderer.enabled = false;
		myBoxCol.size = initialColSize;
		startTime = Time.time;
		myRigidbody.isKinematic = false;
		myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		myBoxCol.isTrigger = true;
		base.transform.gameObject.tag = "Untagged";
		base.transform.parent = AzuObjectPool.instance.transform;
		base.transform.localScale = scale;
		DeleteEmptyObj();
	}

	public void DeleteEmptyObj()
	{
		if (emptyObject != null)
		{
			base.transform.parent = AzuObjectPool.instance.transform;
			Object.DestroyImmediate(emptyObject);
		}
	}

	private void OnDrawGizmos()
	{
		if (drawHelperGizmos)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, 0.04f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.position + base.transform.forward * (hitCheckDist + hitCheckDist * 3f * velFactor), 0.04f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(arrowRayHit.point, 0.06f);
		}
	}

	private void Update()
	{
		if (startTime + visibleDelay < Time.time)
		{
			myMeshRenderer.enabled = true;
		}
		if (!hit)
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward, out arrowRayHit, hitCheckDist + hitCheckDist * 3f * velFactor, rayMask, QueryTriggerInteraction.Ignore))
			{
				HitTarget();
			}
			if (!falling && myRigidbody.velocity.magnitude > 0.01f)
			{
				base.transform.rotation = Quaternion.LookRotation(myRigidbody.velocity);
			}
			return;
		}
		if (hitTime + waitDuration < Time.time)
		{
			base.transform.parent = AzuObjectPool.instance.transform;
			Object.DestroyImmediate(emptyObject);
			AzuObjectPool.instance.RecyclePooledObj(objectPoolIndex, base.transform.gameObject);
		}
		if ((bool)hitCol && !hitCol.enabled)
		{
			myRigidbody.isKinematic = false;
			myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			myBoxCol.isTrigger = false;
			base.transform.gameObject.tag = "Usable";
			falling = true;
		}
	}

	private void HitTarget()
	{
		if (hit || !(arrowRayHit.collider != FPSPlayerComponent.FPSWalkerComponent.capsule) || (bool)arrowRayHit.collider.gameObject.GetComponent<ArrowObject>())
		{
			return;
		}
		hitCol = arrowRayHit.collider;
		myRigidbody.isKinematic = true;
		myRigidbody.interpolation = RigidbodyInterpolation.None;
		base.transform.gameObject.tag = "Usable";
		myBoxCol.size = new Vector3(initialColSize.x * 2f, initialColSize.y * 2f, initialColSize.z);
		AmmoPickupComponent.enabled = true;
		base.transform.position = arrowRayHit.point;
		if ((bool)hitCol.GetComponent<Rigidbody>() || (hitCol.transform.parent != null && (bool)hitCol.transform.parent.GetComponent<Rigidbody>()))
		{
			if ((bool)hitCol.GetComponent<Rigidbody>())
			{
				hitCol.GetComponent<Rigidbody>().AddForce(base.transform.forward * force, ForceMode.Impulse);
			}
			else if (hitCol.transform.parent != null && (bool)hitCol.transform.parent.GetComponent<Rigidbody>())
			{
				hitCol.transform.parent.GetComponent<Rigidbody>().AddForce(base.transform.forward * force, ForceMode.Impulse);
			}
			emptyObject = new GameObject();
			emptyObject.transform.position = arrowRayHit.point;
			emptyObject.transform.rotation = hitCol.transform.rotation;
			emptyObject.transform.parent = hitCol.transform;
			base.transform.parent = emptyObject.transform;
			Object.Destroy(emptyObject.gameObject, waitDuration + 1f);
		}
		switch (hitCol.gameObject.layer)
		{
		case 0:
			if ((bool)hitCol.gameObject.GetComponent<AppleFall>())
			{
				hitCol.gameObject.GetComponent<AppleFall>().ApplyDamage(damage + damageAddAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			else if ((bool)hitCol.gameObject.GetComponent<BreakableObject>())
			{
				hitCol.gameObject.GetComponent<BreakableObject>().ApplyDamage(damage + damageAddAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			else if ((bool)hitCol.gameObject.GetComponent<ExplosiveObject>())
			{
				hitCol.gameObject.GetComponent<ExplosiveObject>().ApplyDamage(damage + damageAddAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			else if ((bool)hitCol.gameObject.GetComponent<MineExplosion>())
			{
				hitCol.gameObject.GetComponent<MineExplosion>().ApplyDamage(damage + damageAddAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			break;
		case 1:
			if ((bool)hitCol.gameObject.GetComponent<BreakableObject>())
			{
				hitCol.gameObject.GetComponent<BreakableObject>().ApplyDamage(damage + damageAddAmt);
				FPSPlayerComponent.UpdateHitTime();
			}
			break;
		case 13:
			if ((bool)hitCol.gameObject.GetComponent<CharacterDamage>() && hitCol.gameObject.GetComponent<AI>().enabled)
			{
				hitCol.gameObject.GetComponent<CharacterDamage>().ApplyDamage(damage + damageAddAmt, base.transform.forward, Camera.main.transform.position, base.transform, true, false);
				FPSPlayerComponent.UpdateHitTime();
			}
			if ((bool)hitCol.gameObject.GetComponent<LocationDamage>() && hitCol.gameObject.GetComponent<LocationDamage>().AIComponent.enabled)
			{
				hitCol.gameObject.GetComponent<LocationDamage>().ApplyDamage(damage + damageAddAmt, base.transform.forward, Camera.main.transform.position, base.transform, true, false);
				FPSPlayerComponent.UpdateHitTime();
			}
			base.transform.position = hitCol.transform.position - (hitCol.transform.position - arrowRayHit.point).normalized * 0.15f;
			break;
		}
		FPSPlayerComponent.WeaponEffectsComponent.ImpactEffects(hitCol, arrowRayHit.point, false, true, arrowRayHit.normal);
		hitTime = Time.time;
		hit = true;
	}
}
