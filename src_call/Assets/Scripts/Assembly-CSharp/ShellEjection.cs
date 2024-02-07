using System.Collections;
using UnityEngine;

public class ShellEjection : MonoBehaviour
{
	[HideInInspector]
	public FPSPlayer FPSPlayerComponent;

	private FPSRigidBodyWalker FPSWalkerComponent;

	private PlayerWeapons PlayerWeaponsComponent;

	[HideInInspector]
	public WeaponBehavior WeaponBehaviorComponent;

	[HideInInspector]
	public Rigidbody RigidbodyComponent;

	[HideInInspector]
	public Rigidbody PlayerRigidbodyComponent;

	[HideInInspector]
	public GameObject playerObj;

	[HideInInspector]
	public GameObject gunObj;

	private Transform gunObjTransform;

	[HideInInspector]
	public Transform lerpShell;

	private Vector3 tempPos;

	private Vector3 tempRot;

	private bool rotated;

	private Transform myTransform;

	private Transform FPSMainTransform;

	[Tooltip("Sound effects to play when shell lands on a surface")]
	public AudioClip[] shellSounds;

	private bool parentState = true;

	private bool soundState = true;

	private float rotateAmt;

	[HideInInspector]
	public float shellRotateUp;

	[HideInInspector]
	public float shellRotateSide;

	private float shellRemovalTime;

	[HideInInspector]
	public int shellDuration;

	private float startTime;

	[HideInInspector]
	public int shellPoolIndex;

	[HideInInspector]
	public int RBPoolIndex;

	[HideInInspector]
	public bool dzAiming;

	private void Start()
	{
	}

	public void InitializeShell()
	{
		FPSPlayerComponent = Camera.main.transform.GetComponent<CameraControl>().FPSPlayerComponent;
		PlayerWeaponsComponent = FPSPlayerComponent.PlayerWeaponsComponent;
		FPSWalkerComponent = FPSPlayerComponent.FPSWalkerComponent;
		myTransform = base.transform;
		FPSWalkerComponent = FPSPlayerComponent.FPSWalkerComponent;
		parentState = true;
		soundState = true;
		shellRotateUp = WeaponBehaviorComponent.shellRotateUp;
		shellRotateSide = WeaponBehaviorComponent.shellRotateSide;
		shellDuration = WeaponBehaviorComponent.shellDuration;
		startTime = Time.time;
		shellRemovalTime = Time.time + (float)shellDuration;
		RigidbodyComponent.maxAngularVelocity = 100f;
		if (Random.value < 0.5f)
		{
			shellRotateUp *= -1f;
		}
		RigidbodyComponent.velocity = Vector3.zero;
		RigidbodyComponent.angularVelocity = Vector3.zero;
		rotateAmt = 0.1f;
		RigidbodyComponent.AddRelativeTorque(Vector3.up * (Random.Range(0.175f, rotateAmt) * shellRotateSide), ForceMode.Impulse);
		RigidbodyComponent.AddRelativeTorque(Vector3.right * (Random.Range(0.4f, rotateAmt * 6f) * shellRotateUp), ForceMode.Impulse);
		StartCoroutine(CalcShellPos());
	}

	private void Update()
	{
		if (Time.time > shellRemovalTime)
		{
			AzuObjectPool.instance.RecyclePooledObj(WeaponBehaviorComponent.shellRBPoolIndex, base.gameObject);
		}
	}

	private IEnumerator CalcShellPos()
	{
		while (true)
		{
			if (!FPSWalkerComponent.playerParented)
			{
				if (parentState && ((startTime + PlayerWeaponsComponent.shellParentTime < Time.time && !FPSPlayerComponent.bulletTimeActive) || (startTime + PlayerWeaponsComponent.shellBtParentTime < Time.time && FPSPlayerComponent.bulletTimeActive) || (startTime + PlayerWeaponsComponent.shellDzParentTime < Time.time && dzAiming) || PlayerWeaponsComponent.switching || (FPSWalkerComponent.sprintActive && !FPSWalkerComponent.cancelSprint) || (FPSWalkerComponent.prone && FPSWalkerComponent.moving)))
				{
					Vector3 velocity = PlayerRigidbodyComponent.velocity;
					velocity.y = 0f;
					myTransform.parent = null;
					if (!FPSWalkerComponent.sprintActive && !FPSWalkerComponent.canRun && FPSWalkerComponent.moving)
					{
						RigidbodyComponent.AddForce(velocity, ForceMode.VelocityChange);
					}
					parentState = false;
					yield break;
				}
			}
			else if ((startTime + PlayerWeaponsComponent.shellParentTime < Time.time && !FPSPlayerComponent.bulletTimeActive) || (startTime + PlayerWeaponsComponent.shellBtParentTime < Time.time && FPSPlayerComponent.bulletTimeActive))
			{
				break;
			}
			yield return null;
		}
		myTransform.parent = null;
		RigidbodyComponent.AddForce(PlayerRigidbodyComponent.velocity, ForceMode.VelocityChange);
		parentState = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (soundState)
		{
			if (shellSounds.Length > 0)
			{
				PlayAudioAtPos.PlayClipAt(shellSounds[Random.Range(0, shellSounds.Length)], myTransform.position, 0.75f);
			}
			soundState = false;
		}
	}
}
