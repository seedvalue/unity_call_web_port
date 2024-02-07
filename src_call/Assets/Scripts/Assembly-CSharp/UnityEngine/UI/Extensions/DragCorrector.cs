using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/DragCorrector")]
	[RequireComponent(typeof(EventSystem))]
	public class DragCorrector : MonoBehaviour
	{
		public int baseTH = 6;

		public int basePPI = 210;

		public int dragTH;

		private void Start()
		{
			dragTH = baseTH * (int)Screen.dpi / basePPI;
			EventSystem component = GetComponent<EventSystem>();
			if ((bool)component)
			{
				component.pixelDragThreshold = dragTH;
			}
		}
	}
}
