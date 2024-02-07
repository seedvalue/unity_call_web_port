using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobleInputController : MonoBehaviour
{
	public InputControl input;

	public ETCJoystick walker;

	public float moveValue = 0.75f;

	private float sensitivity = 1f;

	private void Start()
	{
		updateSensitivity();
	}

	public void updateSensitivity()
	{
		sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
		sensitivity = Mathf.Clamp(sensitivity, 0.2f, 1f);
	}

	private void Update()
	{
		input.moveX = walker.axisX.axisValue * 2f;
		input.moveY = walker.axisY.axisValue * 2f;
		if (walker.axisY.axisValue > 0.9f || walker.axisY.axisValue < -0.9f)
		{
			input.sprintHold = true;
		}
		else
		{
			input.sprintHold = false;
		}
		if (Input.touchCount <= 0)
		{
			return;
		}
		Touch aT = Input.GetTouch(0);
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			int fingerId = touch.fingerId;
			if (!EventSystem.current.IsPointerOverGameObject(fingerId))
			{
				aT = touch;
			}
		}
		if (aT.phase == TouchPhase.Moved)
		{
			if (EventSystem.current.IsPointerOverGameObject(aT.fingerId))
			{
				return;
			}
			if (fixedTouchDelta(aT).x < -0.1f)
			{
				input.lookX = 0.08f * fixedTouchDelta(aT).x * sensitivity;
			}
			else if (fixedTouchDelta(aT).x > 0.1f)
			{
				input.lookX = 0.08f * fixedTouchDelta(aT).x * sensitivity;
			}
			else
			{
				input.lookX = 0f;
			}
			if (fixedTouchDelta(aT).y < -0.1f)
			{
				input.lookY = 0.08f * fixedTouchDelta(aT).y * sensitivity;
			}
			else if (fixedTouchDelta(aT).y > 0.1f)
			{
				input.lookY = 0.08f * fixedTouchDelta(aT).y * sensitivity;
			}
			else
			{
				input.lookY = 0f;
			}
		}
		if (aT.phase == TouchPhase.Ended)
		{
			input.lookX = 0f;
			input.lookY = 0f;
		}
	}

	public void zoomBtnOnClick()
	{
		input.zoomHold = !input.zoomHold;
	}

	private void OnDisable()
	{
		input.lookX = 0f;
		input.lookY = 0f;
	}

	public void walkBtnPreseed()
	{
		input.moveY = 1f;
	}

	public void walkBtnUnPressed()
	{
		input.moveY = 0f;
	}

	public void runBtnPressed()
	{
		input.sprintHold = true;
		input.moveY = 1f;
	}

	public void runBtnUnPressed()
	{
		input.sprintHold = false;
		StartCoroutine(decreaseRunInput());
	}

	private IEnumerator decreaseRunInput()
	{
		while (input.moveY != 0f)
		{
			input.moveY = Mathf.MoveTowards(input.moveY, 0f, 5f * Time.deltaTime);
			yield return null;
		}
	}

	public void selectGrenade()
	{
		input.grenadeHold = true;
	}

	public void deselectGrenade()
	{
		input.grenadeHold = false;
	}

	public void selectNextGrenade()
	{
		input.selectGrenPress = true;
		Invoke("disableSelectGrenade", 0.1f);
	}

	public void disableSelectGrenade()
	{
		input.selectGrenPress = false;
	}

	private Vector2 fixedTouchDelta(Touch aT)
	{
		float num = Time.deltaTime / aT.deltaTime;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		return aT.deltaPosition * num;
	}
}
