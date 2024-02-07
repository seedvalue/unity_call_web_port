using System;
using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
	[AddComponentMenu("EasyTouch/Quick Tap")]
	public class QuickTap : QuickBase
	{
		[Serializable]
		public class OnTap : UnityEvent<Gesture>
		{
		}

		public enum ActionTriggering
		{
			Simple_Tap = 0,
			Double_Tap = 1
		}

		[SerializeField]
		public OnTap onTap;

		public ActionTriggering actionTriggering;

		private Gesture currentGesture;

		public QuickTap()
		{
			quickActionName = "QuickTap" + GetInstanceID();
		}

		private void Update()
		{
			currentGesture = EasyTouch.current;
			if (!is2Finger)
			{
				if (currentGesture.type == EasyTouch.EvtType.On_DoubleTap && actionTriggering == ActionTriggering.Double_Tap)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_SimpleTap && actionTriggering == ActionTriggering.Simple_Tap)
				{
					DoAction(currentGesture);
				}
			}
			else
			{
				if (currentGesture.type == EasyTouch.EvtType.On_DoubleTap2Fingers && actionTriggering == ActionTriggering.Double_Tap)
				{
					DoAction(currentGesture);
				}
				if (currentGesture.type == EasyTouch.EvtType.On_SimpleTap2Fingers && actionTriggering == ActionTriggering.Simple_Tap)
				{
					DoAction(currentGesture);
				}
			}
		}

		private void DoAction(Gesture gesture)
		{
			if (realType == GameObjectType.UI)
			{
				if (gesture.isOverGui && (gesture.pickedUIElement == base.gameObject || gesture.pickedUIElement.transform.IsChildOf(base.transform)))
				{
					onTap.Invoke(gesture);
				}
			}
			else if (((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI) && EasyTouch.GetGameObjectAt(gesture.position, is2Finger) == base.gameObject)
			{
				onTap.Invoke(gesture);
			}
		}
	}
}
