using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Extensions/Return Key Trigger")]
	public class ReturnKeyTriggersButton : MonoBehaviour, IEventSystemHandler, ISubmitHandler
	{
		private EventSystem _system;

		public Button button;

		private bool highlight = true;

		public float highlightDuration = 0.2f;

		private void Start()
		{
			_system = EventSystem.current;
		}

		private void RemoveHighlight()
		{
			button.OnPointerExit(new PointerEventData(_system));
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (highlight)
			{
				button.OnPointerEnter(new PointerEventData(_system));
			}
			button.OnPointerClick(new PointerEventData(_system));
			if (highlight)
			{
				Invoke("RemoveHighlight", highlightDuration);
			}
		}
	}
}