using System;
using UnityEngine;

public class CamMovementBehavior : MonoBehaviour
{
	private SelectionBehavior selectionBehavior;

	private bool follow_toggle;

	private bool KeyMovementOverride;

	private bool MouseMovement;

	private bool BetweenAngleCalculated;

	private bool TargetPositionCalculated;

	private bool ActiveScrolling;

	private RaycastHit TargetCastInfo;

	private Vector3 TargetPosition = Vector3.zero;

	private Quaternion look_rotation = Quaternion.identity;

	private float zoom_distance = 320f;

	private float angle_between = 4.712389f;

	private float cam_range_time;

	private float followcam_delay_counter;

	private float TargetDistance;

	public KeyCode FollowToggleKey = KeyCode.Space;

	public LayerMask IgnoreMask;

	public bool SteeperAngle = true;

	public bool EnableMouseScroll;

	public ControllerType ControllerToggleType = ControllerType.RealTimeStratToggle;

	public float ScreenMovementBuffer = 5f;

	public float MinZoomDistance = 20f;

	public float MaxZoomDistance = 400f;

	public float MoveToSpeed = 3f;

	public float ZoomSpeed = 3f;

	public float RotateSpeed = 3f;

	private void Start()
	{
		selectionBehavior = base.transform.GetComponent<SelectionBehavior>();
		ActiveScrolling = ControllerToggleType.Equals(ControllerType.RealTimeStratToggle) && EnableMouseScroll;
	}

	private void Update()
	{
		Transform selectedTarget = selectionBehavior.getSelectedTarget();
		Transform activeTarget = selectionBehavior.getActiveTarget();
		DetermineKeyPressEvent(selectedTarget, activeTarget);
		if (selectedTarget == null)
		{
			DoDefaultCameraBehavior();
		}
		else if (follow_toggle)
		{
			if (!selectionBehavior.SimilarSelected)
			{
				base.transform.position = CalculateNewMoveToPosition(base.transform.position, selectedTarget.position, MaxZoomDistance - zoom_distance);
				look_rotation = Quaternion.LookRotation(selectedTarget.position - base.transform.position);
				if (!SteeperAngle)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(35.264f, look_rotation.eulerAngles.y, 0f), RotateSpeed);
				}
				else
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(45f, look_rotation.eulerAngles.y, 0f), RotateSpeed);
				}
			}
			else
			{
				followcam_delay_counter += Time.deltaTime;
			}
			if (followcam_delay_counter > Time.deltaTime * 10f)
			{
				selectionBehavior.SimilarSelected = false;
				followcam_delay_counter = 0f;
			}
		}
		else
		{
			DoDefaultCameraBehavior();
		}
	}

	private void DetermineKeyPressEvent(Transform selected, Transform active)
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			if (follow_toggle && selected != null)
			{
				if ((selected.position - GetComponent<Camera>().transform.position).magnitude > MinZoomDistance)
				{
					zoom_distance += ZoomSpeed * Time.deltaTime * 60f;
					TargetPositionCalculated = false;
				}
			}
			else if (!follow_toggle && (TargetPosition - GetComponent<Camera>().transform.position).magnitude > MinZoomDistance)
			{
				GetComponent<Camera>().transform.position += GetComponent<Camera>().transform.forward * (ZoomSpeed * Time.deltaTime * 60f);
				TargetPositionCalculated = false;
			}
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			if (follow_toggle && selected != null)
			{
				if ((selected.position - GetComponent<Camera>().transform.position).magnitude < MaxZoomDistance)
				{
					zoom_distance -= ZoomSpeed * Time.deltaTime * 60f;
					TargetPositionCalculated = false;
				}
			}
			else if (!follow_toggle && (TargetPosition - GetComponent<Camera>().transform.position).magnitude < MaxZoomDistance)
			{
				GetComponent<Camera>().transform.position += GetComponent<Camera>().transform.forward * (0f - ZoomSpeed * Time.deltaTime * 60f);
				TargetPositionCalculated = false;
			}
		}
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			KeyMovementOverride = true;
		}
		else if (KeyMovementOverride)
		{
			KeyMovementOverride = false;
		}
		if (ActiveScrolling && !KeyMovementOverride && (Input.mousePosition.x < ScreenMovementBuffer || Input.mousePosition.x > (float)Screen.width - ScreenMovementBuffer || Input.mousePosition.y > (float)Screen.height - ScreenMovementBuffer || Input.mousePosition.y < ScreenMovementBuffer))
		{
			MouseMovement = true;
		}
		else if (MouseMovement)
		{
			MouseMovement = false;
		}
		if (Input.GetKey(KeyCode.A) || (ActiveScrolling && !KeyMovementOverride && Input.mousePosition.x < ScreenMovementBuffer))
		{
			base.transform.position += base.transform.right * (0f - MoveToSpeed * Time.deltaTime * 60f);
			follow_toggle = false;
			TargetPositionCalculated = false;
			if (!KeyMovementOverride && selectionBehavior.MouseDirections != null && Input.mousePosition.x < ScreenMovementBuffer)
			{
				selectionBehavior.setActiveCursorScrollValues(3, -8f, -16f);
			}
		}
		else if (Input.GetKey(KeyCode.D) || (ActiveScrolling && !KeyMovementOverride && Input.mousePosition.x > (float)Screen.width - ScreenMovementBuffer))
		{
			base.transform.position += base.transform.right * (MoveToSpeed * Time.deltaTime * 60f);
			follow_toggle = false;
			TargetPositionCalculated = false;
			if (!KeyMovementOverride && selectionBehavior.MouseDirections != null && Input.mousePosition.x > (float)Screen.width - ScreenMovementBuffer)
			{
				selectionBehavior.setActiveCursorScrollValues(1, -24f, -16f);
			}
		}
		if (Input.GetKey(KeyCode.W) || (ActiveScrolling && !KeyMovementOverride && Input.mousePosition.y > (float)Screen.height - ScreenMovementBuffer))
		{
			base.transform.position += Vector3.Cross(Vector3.up, -base.transform.right) * (MoveToSpeed * Time.deltaTime * 60f);
			follow_toggle = false;
			TargetPositionCalculated = false;
			if (!KeyMovementOverride && selectionBehavior.MouseDirections != null && Input.mousePosition.y > (float)Screen.height - ScreenMovementBuffer)
			{
				if (Input.mousePosition.x < ScreenMovementBuffer)
				{
					selectionBehavior.setActiveCursorScrollValues(6, -8f, -8f);
				}
				else if (Input.mousePosition.x > (float)Screen.width - ScreenMovementBuffer)
				{
					selectionBehavior.setActiveCursorScrollValues(7, -24f, -8f);
				}
				else
				{
					selectionBehavior.setActiveCursorScrollValues(0, -16f, -8f);
				}
			}
		}
		else if (Input.GetKey(KeyCode.S) || (ActiveScrolling && !KeyMovementOverride && Input.mousePosition.y < ScreenMovementBuffer))
		{
			base.transform.position += Vector3.Cross(Vector3.up, -base.transform.right) * (0f - MoveToSpeed * Time.deltaTime * 60f);
			follow_toggle = false;
			TargetPositionCalculated = false;
			if (!KeyMovementOverride && selectionBehavior.MouseDirections != null && Input.mousePosition.y < ScreenMovementBuffer)
			{
				if (Input.mousePosition.x < ScreenMovementBuffer)
				{
					selectionBehavior.setActiveCursorScrollValues(4, -8f, -24f);
				}
				else if (Input.mousePosition.x > (float)Screen.width - ScreenMovementBuffer)
				{
					selectionBehavior.setActiveCursorScrollValues(5, -24f, -24f);
				}
				else
				{
					selectionBehavior.setActiveCursorScrollValues(2, -16f, -24f);
				}
			}
		}
		if (Input.GetKey(KeyCode.Q))
		{
			angle_between -= RotateSpeed * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.E))
		{
			angle_between += RotateSpeed * Time.deltaTime;
		}
		if (selected != null && Input.GetKeyDown(FollowToggleKey) && !follow_toggle)
		{
			follow_toggle = true;
			BetweenAngleCalculated = false;
			cam_range_time = 0f;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			selectionBehavior.SendMessage("ExecuteEscapeSequence");
		}
	}

	private void DoDefaultCameraBehavior()
	{
		if (!TargetPositionCalculated)
		{
			Ray ray = new Ray(GetComponent<Camera>().transform.position, GetComponent<Camera>().transform.forward);
			if (Physics.Raycast(ray, out TargetCastInfo, 5f * MaxZoomDistance, ~IgnoreMask.value))
			{
				TargetPosition = TargetCastInfo.point;
				TargetDistance = TargetCastInfo.distance;
			}
			TargetPositionCalculated = true;
		}
		base.transform.position = TargetPosition + TargetDistance * (CalculateNewMoveToPosition(base.transform.position, TargetPosition, TargetDistance) - TargetPosition).normalized;
		look_rotation = Quaternion.LookRotation(TargetPosition - base.transform.position);
		if (!SteeperAngle)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(35.264f, look_rotation.eulerAngles.y, 0f), RotateSpeed);
		}
		else
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(45f, look_rotation.eulerAngles.y, 0f), RotateSpeed);
		}
	}

	private Vector3 CalculateNewMoveToPosition(Vector3 Cam, Vector3 Selected, float MaxDistance)
	{
		Vector3 zero = Vector3.zero;
		if ((double)cam_range_time < 0.06 && !BetweenAngleCalculated)
		{
			CalculateRadiansAngle(Cam, Selected);
		}
		zero.x = Selected.x + MaxDistance * Mathf.Cos(angle_between);
		zero.z = Selected.z + MaxDistance * Mathf.Sin(angle_between);
		if (!SteeperAngle)
		{
			zero.y = Selected.y + MaxDistance * Mathf.Sin((float)Math.PI / 4f);
		}
		else
		{
			zero.y = Selected.y + MaxDistance;
		}
		if ((double)cam_range_time < 0.06 && Vector3.Distance(Cam, Selected) < zero.magnitude)
		{
			cam_range_time += Time.deltaTime;
		}
		return zero;
	}

	private void CalculateRadiansAngle(Vector3 Cam, Vector3 Selected)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		num2 = ((!(Cam.x >= Selected.x)) ? (Selected.x - Cam.x) : (Cam.x - Selected.x));
		num3 = ((!(Cam.z >= Selected.z)) ? (Selected.z - Cam.z) : (Cam.z - Selected.z));
		if (Cam.x < Selected.x)
		{
			num = (float)Math.PI;
		}
		if (Cam.z < Selected.z)
		{
			num = -(float)Math.PI;
		}
		if (Cam.x > Selected.x && Cam.z < Selected.z)
		{
			num = (float)Math.PI * 2f;
		}
		angle_between = Mathf.Abs(num - Mathf.Atan(num3 / num2));
		BetweenAngleCalculated = true;
	}

	private void ExecuteEscapeSequence()
	{
		follow_toggle = false;
		TargetPositionCalculated = false;
	}

	public bool isMouseMovement()
	{
		return MouseMovement;
	}

	public bool isFollowToggle()
	{
		return follow_toggle;
	}
}
