using System.Collections;
using UnityEngine;

public class DragRigidbody : MonoBehaviour
{
	private FPSRigidBodyWalker FPSWalkerComponent;

	private Ironsights IronsightsComponent;

	private FPSPlayer FPSPlayerComponent;

	private InputControl InputComponent;

	private float spring = 75f;

	private float damper = 1f;

	private float drag = 10f;

	private float angularDrag = 5f;

	private float distance;

	[Tooltip("Max distance to drag objects.")]
	public float reachDistance = 2.5f;

	private float reachDistanceAmt;

	[Tooltip("Physics force to apply to thrown objects.")]
	public float throwForce = 7f;

	private bool attachToCenterOfMass;

	private SpringJoint springJoint;

	private float oldDrag;

	private float oldAngularDrag;

	private bool dragState;

	private Vector3 dragDirRayCast;

	private Vector3 dragDirRay;

	[Tooltip("If true, dragged object will be dropped if it contacts player object to prevent pushing or lifting player.")]
	public bool dropOnPlayerCollision;

	[Tooltip("Only check these layers for draggable objects.")]
	public LayerMask layersToDrag = 0;

	private Transform mainCamTransform;

	private void Start()
	{
		FPSWalkerComponent = GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = GetComponent<FPSPlayer>();
		InputComponent = GetComponent<InputControl>();
		mainCamTransform = Camera.main.transform;
		reachDistanceAmt = reachDistance / (1f - FPSWalkerComponent.playerHeightMod / FPSWalkerComponent.capsule.height);
	}

	private void Update()
	{
		if (!InputComponent.useHold || FPSPlayerComponent.usePressTime + 0.3f > Time.time || FPSPlayerComponent.useReleaseTime + 0.3f > Time.time || dragState || FPSPlayerComponent.pressButtonUpState || FPSPlayerComponent.zoomed || FPSPlayerComponent.hitPoints < 1f)
		{
			return;
		}
		if (!FPSPlayerComponent.CameraControlComponent.thirdPersonActive)
		{
			dragDirRayCast = FPSPlayerComponent.WeaponBehaviorComponent.weaponLookDirection;
		}
		else
		{
			dragDirRayCast = (mainCamTransform.position + mainCamTransform.forward * reachDistanceAmt - mainCamTransform.position).normalized;
		}
		RaycastHit hitInfo;
		if (!Physics.Raycast(mainCamTransform.position, dragDirRayCast, out hitInfo, reachDistanceAmt + FPSPlayerComponent.CameraControlComponent.zoomDistance, layersToDrag))
		{
			FPSPlayerComponent.pressButtonUpState = true;
			FPSPlayerComponent.useReleaseTime = -8f;
		}
		else if ((bool)hitInfo.rigidbody && !hitInfo.rigidbody.isKinematic && !FPSPlayerComponent.pressButtonUpState)
		{
			if (!springJoint)
			{
				GameObject gameObject = new GameObject("Rigidbody dragger");
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				springJoint = gameObject.AddComponent<SpringJoint>();
				rigidbody.isKinematic = true;
			}
			springJoint.connectedBody = hitInfo.rigidbody;
			springJoint.transform.position = hitInfo.point;
			if (attachToCenterOfMass)
			{
				Vector3 position = base.transform.TransformDirection(hitInfo.rigidbody.centerOfMass) + hitInfo.rigidbody.transform.position;
				position = springJoint.transform.InverseTransformPoint(position);
				springJoint.anchor = position;
			}
			else
			{
				springJoint.anchor = Vector3.zero;
			}
			springJoint.spring = spring;
			springJoint.damper = damper;
			springJoint.maxDistance = distance;
			StartCoroutine(DragObject(hitInfo.distance));
		}
	}

	private IEnumerator DragObject(float distance)
	{
		if (!dragState)
		{
			oldDrag = springJoint.connectedBody.drag;
			oldAngularDrag = springJoint.connectedBody.angularDrag;
			dragState = true;
		}
		if (!springJoint.connectedBody.useGravity)
		{
			if ((bool)springJoint.connectedBody.GetComponent<AudioSource>() && springJoint.connectedBody.GetComponent<AudioSource>().enabled)
			{
				springJoint.connectedBody.GetComponent<AudioSource>().pitch = Random.Range(0.75f * Time.timeScale, 1f * Time.timeScale);
				springJoint.connectedBody.GetComponent<AudioSource>().Play();
			}
			springJoint.connectedBody.useGravity = true;
		}
		springJoint.connectedBody.drag = drag;
		springJoint.connectedBody.angularDrag = angularDrag;
		while (InputComponent.useHold && FPSPlayerComponent.usePressTime + 0.3f < Time.time && (bool)springJoint.connectedBody && !FPSPlayerComponent.zoomed && FPSPlayerComponent.hitPoints > 0f)
		{
			if (!FPSPlayerComponent.CameraControlComponent.thirdPersonActive)
			{
				dragDirRay = FPSPlayerComponent.WeaponBehaviorComponent.weaponLookDirection;
			}
			else
			{
				dragDirRay = (mainCamTransform.position + mainCamTransform.forward * (reachDistanceAmt + FPSPlayerComponent.CameraControlComponent.zoomDistance) - mainCamTransform.position).normalized;
			}
			Ray ray = new Ray(mainCamTransform.position, dragDirRay);
			springJoint.transform.position = ray.GetPoint(distance);
			if (!InputComponent.firePress)
			{
				if (Vector3.Distance(springJoint.connectedBody.transform.position, mainCamTransform.position) < (reachDistanceAmt + FPSPlayerComponent.CameraControlComponent.zoomDistance) * 1.4f)
				{
					FPSWalkerComponent.holdingObject = true;
					yield return null;
					continue;
				}
				break;
			}
			float num = ((!(springJoint.connectedBody.mass < 1f)) ? (throwForce * springJoint.connectedBody.mass) : (throwForce / 2f));
			if (!FPSPlayerComponent.CameraControlComponent.thirdPersonActive)
			{
				springJoint.connectedBody.AddForceAtPosition(num * FPSPlayerComponent.WeaponBehaviorComponent.weaponLookDirection, springJoint.transform.position, ForceMode.Impulse);
			}
			else
			{
				springJoint.connectedBody.AddForceAtPosition(num * mainCamTransform.forward, springJoint.transform.position, ForceMode.Impulse);
			}
			FPSPlayerComponent.WeaponBehaviorComponent.shootStartTime = Time.time;
			break;
		}
		if ((bool)springJoint.connectedBody)
		{
			DropObject();
		}
		else
		{
			FPSWalkerComponent.holdingObject = false;
			FPSWalkerComponent.dropTime = Time.time;
		}
		dragState = false;
		FPSPlayerComponent.pressButtonUpState = true;
	}

	private void OnCollisionStay(Collision col)
	{
		if (dropOnPlayerCollision && (bool)springJoint && (bool)springJoint.connectedBody && col.gameObject.GetComponent<Rigidbody>() == springJoint.connectedBody)
		{
			DropObject();
		}
	}

	private void DropObject()
	{
		FPSWalkerComponent.holdingObject = false;
		FPSWalkerComponent.dropTime = Time.time;
		springJoint.connectedBody.drag = oldDrag;
		springJoint.connectedBody.angularDrag = oldAngularDrag;
		springJoint.connectedBody = null;
		dragState = false;
	}
}
