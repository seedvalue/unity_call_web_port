using HedgehogTeam.EasyTouch;
using UnityEngine;

public class TwoDoubleTapMe : MonoBehaviour
{
	private void OnEnable()
	{
		EasyTouch.On_DoubleTap2Fingers += On_DoubleTap2Fingers;
	}

	private void OnDisable()
	{
		UnsubscribeEvent();
	}

	private void OnDestroy()
	{
		UnsubscribeEvent();
	}

	private void UnsubscribeEvent()
	{
		EasyTouch.On_DoubleTap2Fingers -= On_DoubleTap2Fingers;
	}

	private void On_DoubleTap2Fingers(Gesture gesture)
	{
		if (gesture.pickedObject == base.gameObject)
		{
			base.gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		}
	}
}
