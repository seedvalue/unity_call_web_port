using HedgehogTeam.EasyTouch;
using UnityEngine;

public class TapMe : MonoBehaviour
{
	private void OnEnable()
	{
		EasyTouch.On_SimpleTap += On_SimpleTap;
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
		EasyTouch.On_SimpleTap -= On_SimpleTap;
	}

	private void On_SimpleTap(Gesture gesture)
	{
		if (gesture.pickedObject == base.gameObject)
		{
			base.gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		}
	}
}
