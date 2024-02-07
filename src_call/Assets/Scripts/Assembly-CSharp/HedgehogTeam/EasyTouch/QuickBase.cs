using UnityEngine;

namespace HedgehogTeam.EasyTouch
{
	public class QuickBase : MonoBehaviour
	{
		protected enum GameObjectType
		{
			Auto = 0,
			Obj_3D = 1,
			Obj_2D = 2,
			UI = 3
		}

		public enum DirectAction
		{
			None = 0,
			Rotate = 1,
			RotateLocal = 2,
			Translate = 3,
			TranslateLocal = 4,
			Scale = 5
		}

		public enum AffectedAxesAction
		{
			X = 0,
			Y = 1,
			Z = 2,
			XY = 3,
			XZ = 4,
			YZ = 5,
			XYZ = 6
		}

		public string quickActionName;

		public bool isMultiTouch;

		public bool is2Finger;

		public bool isOnTouch;

		public bool enablePickOverUI;

		public bool resetPhysic;

		public DirectAction directAction;

		public AffectedAxesAction axesAction;

		public float sensibility = 1f;

		public CharacterController directCharacterController;

		public bool inverseAxisValue;

		protected Rigidbody cachedRigidBody;

		protected bool isKinematic;

		protected Rigidbody2D cachedRigidBody2D;

		protected bool isKinematic2D;

		protected GameObjectType realType;

		protected int fingerIndex = -1;

		private void Awake()
		{
			cachedRigidBody = GetComponent<Rigidbody>();
			if ((bool)cachedRigidBody)
			{
				isKinematic = cachedRigidBody.isKinematic;
			}
			cachedRigidBody2D = GetComponent<Rigidbody2D>();
			if ((bool)cachedRigidBody2D)
			{
				isKinematic2D = cachedRigidBody2D.isKinematic;
			}
		}

		public virtual void Start()
		{
			EasyTouch.SetEnableAutoSelect(true);
			realType = GameObjectType.Obj_3D;
			if ((bool)GetComponent<Collider>())
			{
				realType = GameObjectType.Obj_3D;
			}
			else if ((bool)GetComponent<Collider2D>())
			{
				realType = GameObjectType.Obj_2D;
			}
			else if ((bool)GetComponent<CanvasRenderer>())
			{
				realType = GameObjectType.UI;
			}
			switch (realType)
			{
			case GameObjectType.Obj_3D:
			{
				LayerMask layerMask = EasyTouch.Get3DPickableLayer();
				layerMask = (int)layerMask | (1 << base.gameObject.layer);
				EasyTouch.Set3DPickableLayer(layerMask);
				break;
			}
			case GameObjectType.Obj_2D:
			{
				EasyTouch.SetEnable2DCollider(true);
				LayerMask layerMask = EasyTouch.Get2DPickableLayer();
				layerMask = (int)layerMask | (1 << base.gameObject.layer);
				EasyTouch.Set2DPickableLayer(layerMask);
				break;
			}
			case GameObjectType.UI:
				EasyTouch.instance.enableUIMode = true;
				EasyTouch.SetUICompatibily(false);
				break;
			}
			if (enablePickOverUI)
			{
				EasyTouch.instance.enableUIMode = true;
				EasyTouch.SetUICompatibily(false);
			}
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		protected Vector3 GetInfluencedAxis()
		{
			Vector3 result = Vector3.zero;
			switch (axesAction)
			{
			case AffectedAxesAction.X:
				result = new Vector3(1f, 0f, 0f);
				break;
			case AffectedAxesAction.Y:
				result = new Vector3(0f, 1f, 0f);
				break;
			case AffectedAxesAction.Z:
				result = new Vector3(0f, 0f, 1f);
				break;
			case AffectedAxesAction.XY:
				result = new Vector3(1f, 1f, 0f);
				break;
			case AffectedAxesAction.XYZ:
				result = new Vector3(1f, 1f, 1f);
				break;
			case AffectedAxesAction.XZ:
				result = new Vector3(1f, 0f, 1f);
				break;
			case AffectedAxesAction.YZ:
				result = new Vector3(0f, 1f, 1f);
				break;
			}
			return result;
		}

		protected void DoDirectAction(float value)
		{
			Vector3 influencedAxis = GetInfluencedAxis();
			switch (directAction)
			{
			case DirectAction.Rotate:
				base.transform.Rotate(influencedAxis * value, Space.World);
				break;
			case DirectAction.RotateLocal:
				base.transform.Rotate(influencedAxis * value, Space.Self);
				break;
			case DirectAction.Translate:
			{
				if (directCharacterController == null)
				{
					base.transform.Translate(influencedAxis * value, Space.World);
					break;
				}
				Vector3 motion2 = influencedAxis * value;
				directCharacterController.Move(motion2);
				break;
			}
			case DirectAction.TranslateLocal:
			{
				if (directCharacterController == null)
				{
					base.transform.Translate(influencedAxis * value, Space.Self);
					break;
				}
				Vector3 motion = directCharacterController.transform.TransformDirection(influencedAxis) * value;
				directCharacterController.Move(motion);
				break;
			}
			case DirectAction.Scale:
				base.transform.localScale += influencedAxis * value;
				break;
			}
		}

		public void EnabledQuickComponent(string quickActionName)
		{
			QuickBase[] components = GetComponents<QuickBase>();
			QuickBase[] array = components;
			foreach (QuickBase quickBase in array)
			{
				if (quickBase.quickActionName == quickActionName)
				{
					quickBase.enabled = true;
				}
			}
		}

		public void DisabledQuickComponent(string quickActionName)
		{
			QuickBase[] components = GetComponents<QuickBase>();
			QuickBase[] array = components;
			foreach (QuickBase quickBase in array)
			{
				if (quickBase.quickActionName == quickActionName)
				{
					quickBase.enabled = false;
				}
			}
		}

		public void DisabledAllSwipeExcepted(string quickActionName)
		{
			QuickSwipe[] array = Object.FindObjectsOfType(typeof(QuickSwipe)) as QuickSwipe[];
			QuickSwipe[] array2 = array;
			foreach (QuickSwipe quickSwipe in array2)
			{
				if (quickSwipe.quickActionName != quickActionName || (quickSwipe.quickActionName == quickActionName && quickSwipe.gameObject != base.gameObject))
				{
					quickSwipe.enabled = false;
				}
			}
		}
	}
}
