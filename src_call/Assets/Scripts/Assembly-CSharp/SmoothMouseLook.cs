using _0_WebPort;
using _00_YaTutor;
using UnityEngine;

public class SmoothMouseLook : MonoBehaviour
{
	private InputControl InputComponent;

	private GameObject playerObj;

	[Tooltip("Mouse look sensitivity/camera move speed.")]
	public float sensitivity = 4f;

	[HideInInspector]
	public float sensitivityAmt = 4f;

	private float minimumX = -360f;

	private float maximumX = 360f;

	[Tooltip("Minumum pitch of camera for mouselook.")]
	public float minimumY = -85f;

	[Tooltip("Maximum pitch of camera for mouselook.")]
	public float maximumY = 85f;

	[HideInInspector]
	public float rotationX;

	[HideInInspector]
	public float rotationY;

	[HideInInspector]
	public Quaternion xQuaternion;

	[HideInInspector]
	public Quaternion yQuaternion;

	[HideInInspector]
	public float inputY;

	[HideInInspector]
	public float horizontalDelta;

	[Tooltip("Smooth speed of camera angles for mouse look.")]
	public float smoothSpeed = 0.35f;

	[HideInInspector]
	public float playerMovedTime;

	[HideInInspector]
	public Quaternion originalRotation;

	[HideInInspector]
	public Transform myTransform;

	[HideInInspector]
	public float recoilX;

	[HideInInspector]
	public float recoilY;

	[HideInInspector]
	public bool dzAiming;

	[Tooltip("Reverse vertical input for mouselook.")]
	public bool invertVerticalLook;

	[HideInInspector]
	public bool thirdPerson;

	[HideInInspector]
	public bool tpIdleCamRotate;

	private bool canMouseLook;

	public bool OnUI;

	public float mouseX;

	public float pointerX;

	public float mouseY;

	public float pointerY;

	private void Start()
	{
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		InputComponent = playerObj.GetComponent<InputControl>();
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
		myTransform = base.transform;
		originalRotation = Quaternion.Euler(myTransform.parent.transform.eulerAngles.x, myTransform.parent.transform.eulerAngles.y, 0f);
		sensitivityAmt = sensitivity;
	}

	public void updateSensitivity()
	{
		float @float = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
		sensitivity = 0.5f + @float * 2.5f;
	}

	private void Update()
	{
		// Без этого на PC не работает, а мобила ок 
		if (InputComponent.changePoint) { }
		
		
		if (!canMouseLook)
		{
			return;
		}

		if (CtrlYa.Instance)
		{
			var device = CtrlYa.Instance.GetDevice();
			if (device == CtrlYa.YaDevice.Mobile)
			{
				if (OnUI)
				{
					InputComponent.lookX = pointerX;
					InputComponent.lookY = pointerY;
				}
			}
			else
			{
				// PC
				//Debug.Log( Input.GetAxis("Mouse X"));
				// Работает, мышь передается. шде клики?
				InputComponent.lookX = Input.GetAxis("Mouse X");
				InputComponent.lookY = Input.GetAxis("Mouse Y");
				//Debug.Log("PC");
			}
		}
		
		
		if (!(Time.timeScale > 0f) || !(Time.smoothDeltaTime > 0f))
		{
			return;
		}
		horizontalDelta = rotationX;
		if (!dzAiming)
		{
			rotationX += InputComponent.lookX * sensitivityAmt * Time.timeScale;
			if (!invertVerticalLook)
			{
				rotationY += InputComponent.lookY * sensitivityAmt * Time.timeScale;
			}
			else
			{
				rotationY -= InputComponent.lookY * sensitivityAmt * Time.timeScale;
			}
		}
		if (maximumY - InputComponent.lookY * sensitivityAmt * Time.timeScale < recoilY)
		{
			rotationY += recoilY;
			recoilY = 0f;
		}
		if (maximumX - InputComponent.lookX * sensitivityAmt * Time.timeScale < recoilX)
		{
			rotationX += recoilX;
			recoilX = 0f;
		}
		rotationX = ClampAngle(rotationX, minimumX, maximumX);
		rotationY = ClampAngle(rotationY, minimumY - recoilY, maximumY - recoilY);
		inputY = rotationY + recoilY;
		xQuaternion = Quaternion.AngleAxis(rotationX + recoilX, Vector3.up);
		yQuaternion = Quaternion.AngleAxis(rotationY + recoilY, -Vector3.right);
		horizontalDelta = Mathf.DeltaAngle(horizontalDelta, rotationX);
		if (!thirdPerson)
		{
			if (playerMovedTime + 0.1f < Time.time)
			{
				myTransform.rotation = Quaternion.Slerp(myTransform.rotation, originalRotation * xQuaternion * yQuaternion, smoothSpeed * Time.smoothDeltaTime * 60f / Time.timeScale);
			}
			else
			{
				myTransform.rotation = originalRotation * xQuaternion * yQuaternion;
			}
			myTransform.rotation = Quaternion.Euler(myTransform.rotation.eulerAngles.x, myTransform.rotation.eulerAngles.y, 0f);
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360f;
		if (angle >= -360f && angle <= 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void setMouseData(Vector2 data)
	{
		pointerX = data.x / 10f;
		pointerY = data.y / 10f;
	}

	public void allowAim(bool allow)
	{
		canMouseLook = allow;
	}
}
