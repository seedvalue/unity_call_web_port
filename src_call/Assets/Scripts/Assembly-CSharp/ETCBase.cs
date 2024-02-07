using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public abstract class ETCBase : MonoBehaviour
{
	public enum ControlType
	{
		Joystick = 0,
		TouchPad = 1,
		DPad = 2,
		Button = 3
	}

	public enum RectAnchor
	{
		UserDefined = 0,
		BottomLeft = 1,
		BottomCenter = 2,
		BottonRight = 3,
		CenterLeft = 4,
		Center = 5,
		CenterRight = 6,
		TopLeft = 7,
		TopCenter = 8,
		TopRight = 9
	}

	public enum DPadAxis
	{
		Two_Axis = 0,
		Four_Axis = 1
	}

	public enum CameraMode
	{
		Follow = 0,
		SmoothFollow = 1
	}

	public enum CameraTargetMode
	{
		UserDefined = 0,
		LinkOnTag = 1,
		FromDirectActionAxisX = 2,
		FromDirectActionAxisY = 3
	}

	protected RectTransform cachedRectTransform;

	protected Canvas cachedRootCanvas;

	public bool isUnregisterAtDisable;

	private bool visibleAtStart = true;

	private bool activatedAtStart = true;

	[SerializeField]
	protected RectAnchor _anchor;

	[SerializeField]
	protected Vector2 _anchorOffet;

	[SerializeField]
	protected bool _visible;

	[SerializeField]
	protected bool _activated;

	public bool enableCamera;

	public CameraMode cameraMode;

	public string camTargetTag = "Player";

	public bool autoLinkTagCam = true;

	public string autoCamTag = "MainCamera";

	public Transform cameraTransform;

	public CameraTargetMode cameraTargetMode;

	public bool enableWallDetection;

	public LayerMask wallLayer = 0;

	public Transform cameraLookAt;

	protected CharacterController cameraLookAtCC;

	public Vector3 followOffset = new Vector3(0f, 6f, -6f);

	public float followDistance = 10f;

	public float followHeight = 5f;

	public float followRotationDamping = 5f;

	public float followHeightDamping = 5f;

	public int pointId = -1;

	public bool enableKeySimulation;

	public bool allowSimulationStandalone;

	public bool visibleOnStandalone = true;

	public DPadAxis dPadAxisCount;

	public bool useFixedUpdate;

	private List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();

	private PointerEventData uiPointerEventData;

	private EventSystem uiEventSystem;

	public bool isOnDrag;

	public bool isSwipeIn;

	public bool isSwipeOut;

	public bool showPSInspector;

	public bool showSpriteInspector;

	public bool showEventInspector;

	public bool showBehaviourInspector;

	public bool showAxesInspector;

	public bool showTouchEventInspector;

	public bool showDownEventInspector;

	public bool showPressEventInspector;

	public bool showCameraInspector;

	public RectAnchor anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			if (value != _anchor)
			{
				_anchor = value;
				SetAnchorPosition();
			}
		}
	}

	public Vector2 anchorOffet
	{
		get
		{
			return _anchorOffet;
		}
		set
		{
			if (value != _anchorOffet)
			{
				_anchorOffet = value;
				SetAnchorPosition();
			}
		}
	}

	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (value != _visible)
			{
				_visible = value;
				SetVisible();
			}
		}
	}

	public bool activated
	{
		get
		{
			return _activated;
		}
		set
		{
			if (value != _activated)
			{
				_activated = value;
				SetActivated();
			}
		}
	}

	protected virtual void Awake()
	{
		cachedRectTransform = base.transform as RectTransform;
		cachedRootCanvas = base.transform.parent.GetComponent<Canvas>();
		if (!allowSimulationStandalone)
		{
			enableKeySimulation = false;
		}
		visibleAtStart = _visible;
		activatedAtStart = _activated;
		if (!isUnregisterAtDisable)
		{
			ETCInput.instance.RegisterControl(this);
		}
	}

	public virtual void Start()
	{
		if (enableCamera && autoLinkTagCam)
		{
			cameraTransform = null;
			GameObject gameObject = GameObject.FindGameObjectWithTag(autoCamTag);
			if ((bool)gameObject)
			{
				cameraTransform = gameObject.transform;
			}
		}
	}

	public virtual void OnEnable()
	{
		if (isUnregisterAtDisable)
		{
			ETCInput.instance.RegisterControl(this);
		}
		visible = visibleAtStart;
		activated = activatedAtStart;
	}

	private void OnDisable()
	{
		if ((bool)ETCInput._instance && isUnregisterAtDisable)
		{
			ETCInput.instance.UnRegisterControl(this);
		}
		visibleAtStart = _visible;
		activated = _activated;
		visible = false;
		activated = false;
	}

	private void OnDestroy()
	{
		if ((bool)ETCInput._instance)
		{
			ETCInput.instance.UnRegisterControl(this);
		}
	}

	public virtual void Update()
	{
		if (!useFixedUpdate)
		{
			StartCoroutine("UpdateVirtualControl");
		}
	}

	public virtual void FixedUpdate()
	{
		if (useFixedUpdate)
		{
			StartCoroutine("FixedUpdateVirtualControl");
		}
	}

	public virtual void LateUpdate()
	{
		if (!enableCamera)
		{
			return;
		}
		if (autoLinkTagCam && cameraTransform == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag(autoCamTag);
			if ((bool)gameObject)
			{
				cameraTransform = gameObject.transform;
			}
		}
		switch (cameraMode)
		{
		case CameraMode.Follow:
			CameraFollow();
			break;
		case CameraMode.SmoothFollow:
			CameraSmoothFollow();
			break;
		}
	}

	protected virtual void UpdateControlState()
	{
	}

	protected virtual void SetVisible(bool forceUnvisible = true)
	{
	}

	protected virtual void SetActivated()
	{
	}

	public void SetAnchorPosition()
	{
		switch (_anchor)
		{
		case RectAnchor.TopLeft:
			this.rectTransform().anchorMin = new Vector2(0f, 1f);
			this.rectTransform().anchorMax = new Vector2(0f, 1f);
			this.rectTransform().anchoredPosition = new Vector2(this.rectTransform().sizeDelta.x / 2f + _anchorOffet.x, (0f - this.rectTransform().sizeDelta.y) / 2f - _anchorOffet.y);
			break;
		case RectAnchor.TopCenter:
			this.rectTransform().anchorMin = new Vector2(0.5f, 1f);
			this.rectTransform().anchorMax = new Vector2(0.5f, 1f);
			this.rectTransform().anchoredPosition = new Vector2(_anchorOffet.x, (0f - this.rectTransform().sizeDelta.y) / 2f - _anchorOffet.y);
			break;
		case RectAnchor.TopRight:
			this.rectTransform().anchorMin = new Vector2(1f, 1f);
			this.rectTransform().anchorMax = new Vector2(1f, 1f);
			this.rectTransform().anchoredPosition = new Vector2((0f - this.rectTransform().sizeDelta.x) / 2f - _anchorOffet.x, (0f - this.rectTransform().sizeDelta.y) / 2f - _anchorOffet.y);
			break;
		case RectAnchor.CenterLeft:
			this.rectTransform().anchorMin = new Vector2(0f, 0.5f);
			this.rectTransform().anchorMax = new Vector2(0f, 0.5f);
			this.rectTransform().anchoredPosition = new Vector2(this.rectTransform().sizeDelta.x / 2f + _anchorOffet.x, _anchorOffet.y);
			break;
		case RectAnchor.Center:
			this.rectTransform().anchorMin = new Vector2(0.5f, 0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f, 0.5f);
			this.rectTransform().anchoredPosition = new Vector2(_anchorOffet.x, _anchorOffet.y);
			break;
		case RectAnchor.CenterRight:
			this.rectTransform().anchorMin = new Vector2(1f, 0.5f);
			this.rectTransform().anchorMax = new Vector2(1f, 0.5f);
			this.rectTransform().anchoredPosition = new Vector2((0f - this.rectTransform().sizeDelta.x) / 2f - _anchorOffet.x, _anchorOffet.y);
			break;
		case RectAnchor.BottomLeft:
			this.rectTransform().anchorMin = new Vector2(0f, 0f);
			this.rectTransform().anchorMax = new Vector2(0f, 0f);
			this.rectTransform().anchoredPosition = new Vector2(this.rectTransform().sizeDelta.x / 2f + _anchorOffet.x, this.rectTransform().sizeDelta.y / 2f + _anchorOffet.y);
			break;
		case RectAnchor.BottomCenter:
			this.rectTransform().anchorMin = new Vector2(0.5f, 0f);
			this.rectTransform().anchorMax = new Vector2(0.5f, 0f);
			this.rectTransform().anchoredPosition = new Vector2(_anchorOffet.x, this.rectTransform().sizeDelta.y / 2f + _anchorOffet.y);
			break;
		case RectAnchor.BottonRight:
			this.rectTransform().anchorMin = new Vector2(1f, 0f);
			this.rectTransform().anchorMax = new Vector2(1f, 0f);
			this.rectTransform().anchoredPosition = new Vector2((0f - this.rectTransform().sizeDelta.x) / 2f - _anchorOffet.x, this.rectTransform().sizeDelta.y / 2f + _anchorOffet.y);
			break;
		}
	}

	protected GameObject GetFirstUIElement(Vector2 position)
	{
		uiEventSystem = EventSystem.current;
		if (uiEventSystem != null)
		{
			uiPointerEventData = new PointerEventData(uiEventSystem);
			uiPointerEventData.position = position;
			uiEventSystem.RaycastAll(uiPointerEventData, uiRaycastResultCache);
			if (uiRaycastResultCache.Count > 0)
			{
				return uiRaycastResultCache[0].gameObject;
			}
			return null;
		}
		return null;
	}

	protected void CameraSmoothFollow()
	{
		if ((bool)cameraTransform && (bool)cameraLookAt)
		{
			float y = cameraLookAt.eulerAngles.y;
			float b = cameraLookAt.position.y + followHeight;
			float y2 = cameraTransform.eulerAngles.y;
			float y3 = cameraTransform.position.y;
			y2 = Mathf.LerpAngle(y2, y, followRotationDamping * Time.deltaTime);
			y3 = Mathf.Lerp(y3, b, followHeightDamping * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
			Vector3 position = cameraLookAt.position;
			position -= quaternion * Vector3.forward * followDistance;
			position = new Vector3(position.x, y3, position.z);
			RaycastHit hitInfo;
			if (enableWallDetection && Physics.Linecast(new Vector3(cameraLookAt.position.x, cameraLookAt.position.y + 1f, cameraLookAt.position.z), position, out hitInfo))
			{
				position = new Vector3(hitInfo.point.x, y3, hitInfo.point.z);
			}
			cameraTransform.position = position;
			cameraTransform.LookAt(cameraLookAt);
		}
	}

	protected void CameraFollow()
	{
		if ((bool)cameraTransform && (bool)cameraLookAt)
		{
			Vector3 vector = followOffset;
			cameraTransform.position = cameraLookAt.position + vector;
			cameraTransform.LookAt(cameraLookAt.position);
		}
	}

	private IEnumerator UpdateVirtualControl()
	{
		DoActionBeforeEndOfFrame();
		yield return new WaitForEndOfFrame();
		UpdateControlState();
	}

	private IEnumerator FixedUpdateVirtualControl()
	{
		DoActionBeforeEndOfFrame();
		yield return new WaitForFixedUpdate();
		UpdateControlState();
	}

	protected virtual void DoActionBeforeEndOfFrame()
	{
	}
}
