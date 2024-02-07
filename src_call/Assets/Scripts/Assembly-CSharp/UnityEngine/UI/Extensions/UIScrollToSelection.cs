using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UIScrollToSelection")]
	[RequireComponent(typeof(ScrollRect))]
	public class UIScrollToSelection : MonoBehaviour
	{
		public enum ScrollType
		{
			VERTICAL = 0,
			HORIZONTAL = 1,
			BOTH = 2
		}

		[SerializeField]
		[Header("[ Settings ]")]
		private ScrollType scrollDirection;

		[SerializeField]
		private float scrollSpeed = 10f;

		[SerializeField]
		[Header("[ Input ]")]
		private bool cancelScrollOnInput;

		[SerializeField]
		private List<KeyCode> cancelScrollKeycodes = new List<KeyCode>();

		protected RectTransform LayoutListGroup
		{
			get
			{
				return (!(TargetScrollRect != null)) ? null : TargetScrollRect.content;
			}
		}

		protected ScrollType ScrollDirection
		{
			get
			{
				return scrollDirection;
			}
		}

		protected float ScrollSpeed
		{
			get
			{
				return scrollSpeed;
			}
		}

		protected bool CancelScrollOnInput
		{
			get
			{
				return cancelScrollOnInput;
			}
		}

		protected List<KeyCode> CancelScrollKeycodes
		{
			get
			{
				return cancelScrollKeycodes;
			}
		}

		protected RectTransform ScrollWindow { get; set; }

		protected ScrollRect TargetScrollRect { get; set; }

		protected EventSystem CurrentEventSystem
		{
			get
			{
				return EventSystem.current;
			}
		}

		protected GameObject LastCheckedGameObject { get; set; }

		protected GameObject CurrentSelectedGameObject
		{
			get
			{
				return EventSystem.current.currentSelectedGameObject;
			}
		}

		protected RectTransform CurrentTargetRectTransform { get; set; }

		protected bool IsManualScrollingAvailable { get; set; }

		protected virtual void Awake()
		{
			TargetScrollRect = GetComponent<ScrollRect>();
			ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			UpdateReferences();
			CheckIfScrollingShouldBeLocked();
			ScrollRectToLevelSelection();
		}

		private void UpdateReferences()
		{
			if (CurrentSelectedGameObject != LastCheckedGameObject)
			{
				CurrentTargetRectTransform = ((!(CurrentSelectedGameObject != null)) ? null : CurrentSelectedGameObject.GetComponent<RectTransform>());
				if (CurrentSelectedGameObject != null && CurrentSelectedGameObject.transform.parent == LayoutListGroup.transform)
				{
					IsManualScrollingAvailable = false;
				}
			}
			LastCheckedGameObject = CurrentSelectedGameObject;
		}

		private void CheckIfScrollingShouldBeLocked()
		{
			if (!CancelScrollOnInput || IsManualScrollingAvailable)
			{
				return;
			}
			for (int i = 0; i < CancelScrollKeycodes.Count; i++)
			{
				if (Input.GetKeyDown(CancelScrollKeycodes[i]))
				{
					IsManualScrollingAvailable = true;
					break;
				}
			}
		}

		private void ScrollRectToLevelSelection()
		{
			if (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null || IsManualScrollingAvailable)
			{
				return;
			}
			RectTransform currentTargetRectTransform = CurrentTargetRectTransform;
			if (!(currentTargetRectTransform == null) && !(currentTargetRectTransform.transform.parent != LayoutListGroup.transform))
			{
				switch (ScrollDirection)
				{
				case ScrollType.VERTICAL:
					UpdateVerticalScrollPosition(currentTargetRectTransform);
					break;
				case ScrollType.HORIZONTAL:
					UpdateHorizontalScrollPosition(currentTargetRectTransform);
					break;
				case ScrollType.BOTH:
					UpdateVerticalScrollPosition(currentTargetRectTransform);
					UpdateHorizontalScrollPosition(currentTargetRectTransform);
					break;
				}
			}
		}

		private void UpdateVerticalScrollPosition(RectTransform selection)
		{
			float position = 0f - selection.anchoredPosition.y;
			float height = selection.rect.height;
			float height2 = ScrollWindow.rect.height;
			float y = LayoutListGroup.anchoredPosition.y;
			float scrollOffset = GetScrollOffset(position, y, height, height2);
			TargetScrollRect.verticalNormalizedPosition += scrollOffset / LayoutListGroup.rect.height * Time.deltaTime * scrollSpeed;
		}

		private void UpdateHorizontalScrollPosition(RectTransform selection)
		{
			float x = selection.anchoredPosition.x;
			float width = selection.rect.width;
			float width2 = ScrollWindow.rect.width;
			float listAnchorPosition = 0f - LayoutListGroup.anchoredPosition.x;
			float num = 0f - GetScrollOffset(x, listAnchorPosition, width, width2);
			TargetScrollRect.horizontalNormalizedPosition += num / LayoutListGroup.rect.width * Time.deltaTime * scrollSpeed;
		}

		private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
		{
			if (position < listAnchorPosition)
			{
				return listAnchorPosition - position;
			}
			if (position + targetLength > listAnchorPosition + maskLength)
			{
				return listAnchorPosition + maskLength - (position + targetLength);
			}
			return 0f;
		}
	}
}
