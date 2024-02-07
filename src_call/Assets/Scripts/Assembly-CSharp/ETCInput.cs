using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ETCInput : MonoBehaviour
{
	public static ETCInput _instance;

	private Dictionary<string, ETCAxis> axes = new Dictionary<string, ETCAxis>();

	private Dictionary<string, ETCBase> controls = new Dictionary<string, ETCBase>();

	private static ETCBase control;

	private static ETCAxis axis;

	public static ETCInput instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(ETCInput)) as ETCInput;
				if (!_instance)
				{
					GameObject gameObject = new GameObject("InputManager");
					_instance = gameObject.AddComponent<ETCInput>();
				}
			}
			return _instance;
		}
	}

	public void RegisterControl(ETCBase ctrl)
	{
		if (controls.ContainsKey(ctrl.name))
		{
			Debug.LogWarning("ETCInput control : " + ctrl.name + " already exists");
			return;
		}
		controls.Add(ctrl.name, ctrl);
		if (ctrl.GetType() == typeof(ETCJoystick))
		{
			RegisterAxis((ctrl as ETCJoystick).axisX);
			RegisterAxis((ctrl as ETCJoystick).axisY);
		}
		else if (ctrl.GetType() == typeof(ETCTouchPad))
		{
			RegisterAxis((ctrl as ETCTouchPad).axisX);
			RegisterAxis((ctrl as ETCTouchPad).axisY);
		}
		else if (ctrl.GetType() == typeof(ETCDPad))
		{
			RegisterAxis((ctrl as ETCDPad).axisX);
			RegisterAxis((ctrl as ETCDPad).axisY);
		}
		else if (ctrl.GetType() == typeof(ETCButton))
		{
			RegisterAxis((ctrl as ETCButton).axis);
		}
	}

	public void UnRegisterControl(ETCBase ctrl)
	{
		if (controls.ContainsKey(ctrl.name) && ctrl.enabled)
		{
			controls.Remove(ctrl.name);
			if (ctrl.GetType() == typeof(ETCJoystick))
			{
				UnRegisterAxis((ctrl as ETCJoystick).axisX);
				UnRegisterAxis((ctrl as ETCJoystick).axisY);
			}
			else if (ctrl.GetType() == typeof(ETCTouchPad))
			{
				UnRegisterAxis((ctrl as ETCTouchPad).axisX);
				UnRegisterAxis((ctrl as ETCTouchPad).axisY);
			}
			else if (ctrl.GetType() == typeof(ETCDPad))
			{
				UnRegisterAxis((ctrl as ETCDPad).axisX);
				UnRegisterAxis((ctrl as ETCDPad).axisY);
			}
			else if (ctrl.GetType() == typeof(ETCButton))
			{
				UnRegisterAxis((ctrl as ETCButton).axis);
			}
		}
	}

	public void Create()
	{
	}

	public static void Register(ETCBase ctrl)
	{
		instance.RegisterControl(ctrl);
	}

	public static void UnRegister(ETCBase ctrl)
	{
		instance.UnRegisterControl(ctrl);
	}

	public static void SetControlVisible(string ctrlName, bool value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			control.visible = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlVisible(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			return control.visible;
		}
		Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		return false;
	}

	public static void SetControlActivated(string ctrlName, bool value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			control.activated = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlActivated(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			return control.activated;
		}
		Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		return false;
	}

	public static void SetControlSwipeIn(string ctrlName, bool value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			control.isSwipeIn = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlSwipeIn(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			return control.isSwipeIn;
		}
		Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		return false;
	}

	public static void SetControlSwipeOut(string ctrlName, bool value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			control.isSwipeOut = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static bool GetControlSwipeOut(string ctrlName, bool value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			return control.isSwipeOut;
		}
		Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		return false;
	}

	public static void SetDPadAxesCount(string ctrlName, ETCBase.DPadAxis value)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			control.dPadAxisCount = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		}
	}

	public static ETCBase.DPadAxis GetDPadAxesCount(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			return control.dPadAxisCount;
		}
		Debug.LogWarning("ETCInput : " + ctrlName + " doesn't exist");
		return ETCBase.DPadAxis.Two_Axis;
	}

	public static ETCJoystick GetControlJoystick(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control) && control.GetType() == typeof(ETCJoystick))
		{
			return (ETCJoystick)control;
		}
		return null;
	}

	public static ETCDPad GetControlDPad(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control) && control.GetType() == typeof(ETCDPad))
		{
			return (ETCDPad)control;
		}
		return null;
	}

	public static ETCTouchPad GetControlTouchPad(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control) && control.GetType() == typeof(ETCTouchPad))
		{
			return (ETCTouchPad)control;
		}
		return null;
	}

	public static ETCButton GetControlButton(string ctrlName)
	{
		if (instance.controls.TryGetValue(ctrlName, out control) && control.GetType() == typeof(ETCJoystick))
		{
			return (ETCButton)control;
		}
		return null;
	}

	public static void SetControlSprite(string ctrlName, Sprite spr, Color color = default(Color))
	{
		if (instance.controls.TryGetValue(ctrlName, out control))
		{
			Image component = control.GetComponent<Image>();
			if ((bool)component)
			{
				component.sprite = spr;
				component.color = color;
			}
		}
	}

	public static void SetJoystickThumbSprite(string ctrlName, Sprite spr, Color color = default(Color))
	{
		if (!instance.controls.TryGetValue(ctrlName, out control) || control.GetType() != typeof(ETCJoystick))
		{
			return;
		}
		ETCJoystick eTCJoystick = (ETCJoystick)control;
		if ((bool)eTCJoystick)
		{
			Image component = eTCJoystick.thumb.GetComponent<Image>();
			if ((bool)component)
			{
				component.sprite = spr;
				component.color = color;
			}
		}
	}

	public static void SetButtonPressedSprite(string ctrlName, Sprite spr, Color color = default(Color))
	{
	}

	public static void SetAxisSpeed(string axisName, float speed)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.speed = speed;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisGravity(string axisName, float gravity)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.gravity = gravity;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetTurnMoveSpeed(string ctrlName, float speed)
	{
		ETCJoystick controlJoystick = GetControlJoystick(ctrlName);
		if ((bool)controlJoystick)
		{
			controlJoystick.tmSpeed = speed;
		}
	}

	public static void ResetAxis(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.axisValue = 0f;
			axis.axisSpeedValue = 0f;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisEnabled(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.enable = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisEnabled(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.enable;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisInverted(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.invertedAxis = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisInverted(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.invertedAxis;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisDeadValue(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.deadValue = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisDeadValue(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.deadValue;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisSensitivity(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.speed = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisSensitivity(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.speed;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisThreshold(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.axisThreshold = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisThreshold(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.axisThreshold;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisInertia(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.isEnertia = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisInertia(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.isEnertia;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisInertiaSpeed(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.inertia = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisInertiaSpeed(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.inertia;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisInertiaThreshold(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.inertiaThreshold = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisInertiaThreshold(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.inertiaThreshold;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisAutoStabilization(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.isAutoStab = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisAutoStabilization(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.isAutoStab;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisAutoStabilizationSpeed(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.autoStabSpeed = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisAutoStabilizationSpeed(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.autoStabSpeed;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisAutoStabilizationThreshold(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.autoStabThreshold = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisAutoStabilizationThreshold(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.autoStabThreshold;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisClampRotation(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.isClampRotation = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisClampRotation(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.isClampRotation;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisClampRotationValue(string axisName, float min, float max)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.minAngle = min;
			axis.maxAngle = max;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisClampRotationMinValue(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.minAngle = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static void SetAxisClampRotationMaxValue(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.maxAngle = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisClampRotationMinValue(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.minAngle;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static float GetAxisClampRotationMaxValue(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.maxAngle;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisDirecTransform(string axisName, Transform value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.directTransform = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static Transform GetAxisDirectTransform(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.directTransform;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return null;
	}

	public static void SetAxisDirectAction(string axisName, ETCAxis.DirectAction value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.directAction = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static ETCAxis.DirectAction GetAxisDirectAction(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.directAction;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return ETCAxis.DirectAction.Rotate;
	}

	public static void SetAxisAffectedAxis(string axisName, ETCAxis.AxisInfluenced value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.axisInfluenced = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static ETCAxis.AxisInfluenced GetAxisAffectedAxis(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.axisInfluenced;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return ETCAxis.AxisInfluenced.X;
	}

	public static void SetAxisOverTime(string axisName, bool value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.isValueOverTime = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static bool GetAxisOverTime(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.isValueOverTime;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return false;
	}

	public static void SetAxisOverTimeStep(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.overTimeStep = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisOverTimeStep(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.overTimeStep;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static void SetAxisOverTimeMaxValue(string axisName, float value)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			axis.maxOverTimeValue = value;
		}
		else
		{
			Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		}
	}

	public static float GetAxisOverTimeMaxValue(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.maxOverTimeValue;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return -1f;
	}

	public static float GetAxis(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.axisValue;
		}
		Debug.LogWarning("ETCInput : " + axisName + " doesn't exist");
		return 0f;
	}

	public static float GetAxisSpeed(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			return axis.axisSpeedValue;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return 0f;
	}

	public static bool GetAxisDownUp(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.DownUp)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisDownDown(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.DownDown)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisDownRight(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.DownRight)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisDownLeft(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.DownLeft)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisPressedUp(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.PressUp)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisPressedDown(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.PressDown)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisPressedRight(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.PressRight)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetAxisPressedLeft(string axisName)
	{
		if (instance.axes.TryGetValue(axisName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.PressLeft)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(axisName + " doesn't exist");
		return false;
	}

	public static bool GetButtonDown(string buttonName)
	{
		if (instance.axes.TryGetValue(buttonName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.Down)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(buttonName + " doesn't exist");
		return false;
	}

	public static bool GetButton(string buttonName)
	{
		if (instance.axes.TryGetValue(buttonName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.Down || axis.axisState == ETCAxis.AxisState.Press)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(buttonName + " doesn't exist");
		return false;
	}

	public static bool GetButtonUp(string buttonName)
	{
		if (instance.axes.TryGetValue(buttonName, out axis))
		{
			if (axis.axisState == ETCAxis.AxisState.Up)
			{
				return true;
			}
			return false;
		}
		Debug.LogWarning(buttonName + " doesn't exist");
		return false;
	}

	public static float GetButtonValue(string buttonName)
	{
		if (instance.axes.TryGetValue(buttonName, out axis))
		{
			return axis.axisValue;
		}
		Debug.LogWarning(buttonName + " doesn't exist");
		return -1f;
	}

	private void RegisterAxis(ETCAxis axis)
	{
		if (instance.axes.ContainsKey(axis.name))
		{
			Debug.LogWarning("ETCInput axis : " + axis.name + " already exists");
		}
		else
		{
			axes.Add(axis.name, axis);
		}
	}

	private void UnRegisterAxis(ETCAxis axis)
	{
		if (instance.axes.ContainsKey(axis.name))
		{
			axes.Remove(axis.name);
		}
	}
}
