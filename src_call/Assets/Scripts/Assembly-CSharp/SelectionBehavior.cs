using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBehavior : MonoBehaviour
{
	private class UpdatePositionToggle
	{
		private static List<UpdatePositionToggle> updater_list = new List<UpdatePositionToggle>();

		private Transform update_target;

		private bool toggled_movement;

		private int toggle_count;

		public bool MovementToggle
		{
			get
			{
				return toggled_movement;
			}
			set
			{
				toggled_movement = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return update_target.position;
			}
			set
			{
				update_target.position = value;
			}
		}

		public Vector3 Rotation
		{
			get
			{
				return update_target.localEulerAngles;
			}
			set
			{
				update_target.localEulerAngles = value;
			}
		}

		public UpdatePositionToggle(Transform ref_obj, int c)
		{
			update_target = ref_obj;
			toggle_count = c;
			toggled_movement = true;
		}

		public static void AddUpdaterToggle(UpdatePositionToggle new_toggle)
		{
			new_toggle.MovementToggle = true;
			if (updater_list.Count > 0)
			{
				for (int i = 0; i < updater_list.Count; i++)
				{
					UpdatePositionToggle updatePositionToggle = updater_list[i];
					if (updatePositionToggle.getName().Equals(new_toggle.getName()))
					{
						updater_list[i] = new_toggle;
						break;
					}
					if (i == updater_list.Count - 1)
					{
						updater_list.Add(new_toggle);
					}
				}
			}
			else
			{
				updater_list.Add(new_toggle);
			}
		}

		public static void UpdateTogglePositions()
		{
			if (updater_list.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < updater_list.Count; i++)
			{
				UpdatePositionToggle updatePositionToggle = updater_list[i];
				if (updatePositionToggle.isMoveable())
				{
					updatePositionToggle.Position += (updatePositionToggle.destinationPosition() - updatePositionToggle.Position).normalized * (30f * Time.deltaTime);
					float num = 0f;
					if (updatePositionToggle.destinationPosition().z <= updatePositionToggle.Position.z)
					{
						num = updatePositionToggle.destinationPosition().z - updatePositionToggle.Position.z;
						num = ((num == 0f) ? updatePositionToggle.Rotation.y : (57.29578f * ((float)Math.PI + Mathf.Atan((updatePositionToggle.destinationPosition().x - updatePositionToggle.Position.x) / num))));
					}
					else
					{
						num = updatePositionToggle.Position.z - updatePositionToggle.destinationPosition().z;
						num = ((num == 0f) ? updatePositionToggle.Rotation.y : (57.29578f * Mathf.Atan((updatePositionToggle.Position.x - updatePositionToggle.destinationPosition().x) / num)));
					}
					updatePositionToggle.Rotation = new Vector3(updatePositionToggle.Rotation.x, num, updatePositionToggle.Rotation.z);
					if (!updatePositionToggle.MovementToggle)
					{
						updater_list.RemoveAt(i);
					}
				}
			}
		}

		private bool ActiveCollision()
		{
			return update_target.GetComponent<ControlObjHandler>().isActiveCollision();
		}

		public bool isMoveable()
		{
			return update_target.GetComponent<ControlObjHandler>().Moveable;
		}

		public Vector3 destinationPosition()
		{
			if (update_target.GetComponent<ControlObjHandler>().isValidMovePathTileCount())
			{
				Vector3 gridSquareCentralOrigin = update_target.GetComponent<ControlObjHandler>().getMovePathTile().getGridSquareCentralOrigin();
				gridSquareCentralOrigin.y += update_target.localScale.y;
				return gridSquareCentralOrigin;
			}
			MovementToggle = false;
			return update_target.position;
		}

		public string getName()
		{
			return update_target.name;
		}
	}

	private CamMovementBehavior camMovementBehavior;

	private MultiSelectToggle multiselect_toggle;

	private SelectionToggle selection_toggle;

	private RaycastHit TargetCastInfo;

	private Transform selectedTarget;

	private Transform activeTarget;

	private Vector3 moveto_pos = Vector3.zero;

	private Vector3 previous_moveto_pos = Vector3.zero;

	private Texture2D[] mouseCursor = new Texture2D[3];

	private Texture2D[] mouseDirection = new Texture2D[8];

	private Texture2D activeCursorGfx;

	private Vector2 CursorPosMod = Vector2.zero;

	private string ControlAssetName = "ControlObj";

	private string NPCAssetName = "NPCObj";

	private string CreatureAssetName = "CreatureObj";

	private bool similar_selected;

	public GridGenerator MainGridGenerator;

	public KeyCode AddAdditionalKey = KeyCode.LeftAlt;

	public Texture2D MouseCursor;

	public Texture2D MouseDirections;

	public Material MultiSelectMaterial;

	public bool SimilarSelected
	{
		get
		{
			return similar_selected;
		}
		set
		{
			similar_selected = value;
		}
	}

	private void Start()
	{
		if (MouseCursor != null)
		{
			for (int i = 0; i < mouseCursor.Length; i++)
			{
				Color[] pixels = MouseCursor.GetPixels(32 * i, 0, 32, 32);
				mouseCursor[i] = new Texture2D(32, 32, TextureFormat.ARGB32, true);
				mouseCursor[i].SetPixels(0, 0, 32, 32, pixels);
				mouseCursor[i].Apply();
			}
			activeCursorGfx = mouseCursor[0];
		}
		if (MouseDirections != null)
		{
			for (int j = 0; j < mouseDirection.Length; j++)
			{
				Color[] pixels2 = MouseDirections.GetPixels(32 * j, 0, 32, 32);
				mouseDirection[j] = new Texture2D(32, 32, TextureFormat.ARGB32, true);
				mouseDirection[j].SetPixels(0, 0, 32, 32, pixels2);
				mouseDirection[j].Apply();
			}
		}
		camMovementBehavior = base.transform.GetComponent<CamMovementBehavior>();
		multiselect_toggle = base.gameObject.AddComponent<MultiSelectToggle>();
		selection_toggle = base.gameObject.AddComponent<SelectionToggle>();
		multiselect_toggle.SetInitialOptions(MultiSelectMaterial, camMovementBehavior.ControllerToggleType);
	}

	private void OnGUI()
	{
		if (activeCursorGfx != null)
		{
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x + ((!(activeCursorGfx == mouseCursor[0])) ? CursorPosMod.x : 0f), Event.current.mousePosition.y + ((!(activeCursorGfx == mouseCursor[0])) ? CursorPosMod.y : 0f), 32f, 32f), activeCursorGfx);
		}
		if (TargetCastInfo.transform != null)
		{
			GUI.Label(new Rect(0f, 0f, Screen.width / 2, 20f), "Target: " + TargetCastInfo.transform.name);
		}
		if (selectedTarget != null)
		{
			GUI.color = Color.white;
			GUI.Label(new Rect(0f, 20f, Screen.width / 2, 20f), "Selected: " + selectedTarget.transform.name);
			GUI.Label(new Rect(0f, 40f, Screen.width / 2, 20f), "Count: " + multiselect_toggle.getCurrentlySelected().Count);
		}
		if (activeTarget != null)
		{
			GUI.Label(new Rect(0f, 60f, Screen.width / 2, 20f), "Active: " + activeTarget.transform.name);
		}
	}

	private void Update()
	{
		if (!camMovementBehavior.isMouseMovement() && activeCursorGfx != mouseCursor[0])
		{
			if (mouseCursor[0] != null)
			{
				activeCursorGfx = mouseCursor[0];
			}
			else
			{
				activeCursorGfx = null;
			}
		}
		Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
		if (Physics.Raycast(ray, out TargetCastInfo) && Input.GetMouseButtonUp(0) && TargetCastInfo.transform != null)
		{
			if (TargetCastInfo.transform.name.Contains(ControlAssetName))
			{
				if (camMovementBehavior.ControllerToggleType.Equals(ControllerType.RealTimeStratToggle) && !Input.GetKey(AddAdditionalKey))
				{
					multiselect_toggle.ClearSelectedTargets();
				}
				ChangeSelectedTarget(TargetCastInfo.transform);
			}
			if (TargetCastInfo.transform.name.Contains(CreatureAssetName) || TargetCastInfo.transform.name.Contains(NPCAssetName))
			{
				if (activeTarget != null)
				{
					if (activeTarget.name.Contains(NPCAssetName))
					{
						NPCHandler component = activeTarget.GetComponent<NPCHandler>();
						component.setActiveTarget(false);
					}
					else if (activeTarget.name.Contains(CreatureAssetName))
					{
						CreatureHandler component2 = activeTarget.GetComponent<CreatureHandler>();
						component2.setActiveTarget(false);
					}
					activeTarget.Find("SelectedIndicator").gameObject.SetActive(false);
				}
				activeTarget = TargetCastInfo.transform;
				activeTarget.Find("SelectedIndicator").gameObject.SetActive(true);
				if (activeTarget.name.Contains(NPCAssetName))
				{
					NPCHandler component3 = activeTarget.GetComponent<NPCHandler>();
					component3.setActiveTarget(true);
				}
				else
				{
					CreatureHandler component4 = activeTarget.GetComponent<CreatureHandler>();
					component4.setActiveTarget(true);
				}
			}
		}
		if (selectedTarget != null && TargetCastInfo.transform != null && ((multiselect_toggle.isMovementAllowed() && Input.GetMouseButtonUp(0) && camMovementBehavior.ControllerToggleType.Equals(ControllerType.RealTimeStratToggle)) || (Input.GetMouseButton(0) && camMovementBehavior.ControllerToggleType.Equals(ControllerType.DungeonCrawlerToggle))) && selectedTarget.name.Contains(ControlAssetName) && !TargetCastInfo.transform.name.Contains(ControlAssetName))
		{
			moveto_pos = TargetCastInfo.point;
			moveto_pos.y += selectedTarget.transform.localScale.y;
			selection_toggle.createSelectionToggle(moveto_pos, Quaternion.FromToRotation(Vector3.up, TargetCastInfo.normal));
			if (camMovementBehavior.ControllerToggleType.Equals(ControllerType.RealTimeStratToggle))
			{
				int num = 0;
				foreach (Transform item in multiselect_toggle.getCurrentlySelected())
				{
					if (moveto_pos != previous_moveto_pos)
					{
						MainGridGenerator.CalculateNewPath(item, moveto_pos);
					}
					UpdatePositionToggle.AddUpdaterToggle(new UpdatePositionToggle(item, num++));
				}
			}
			else
			{
				if (moveto_pos != previous_moveto_pos)
				{
					MainGridGenerator.CalculateNewPath(selectedTarget, moveto_pos);
				}
				UpdatePositionToggle.AddUpdaterToggle(new UpdatePositionToggle(selectedTarget, 1));
			}
			previous_moveto_pos = moveto_pos;
		}
		UpdatePositionToggle.UpdateTogglePositions();
	}

	private void ChangeSelectedTarget(Transform trans)
	{
		if (!camMovementBehavior.isFollowToggle() && selectedTarget != null)
		{
			ControlObjHandler component = selectedTarget.GetComponent<ControlObjHandler>();
			if (selectedTarget.name.Contains(ControlAssetName))
			{
				similar_selected = true;
			}
			if (multiselect_toggle.getCurrentlySelected().Count == 0)
			{
				selectedTarget.Find("SelectedIndicator").gameObject.SetActive(false);
			}
			component.setActiveTarget(false);
		}
		else if (camMovementBehavior.isFollowToggle() && selectedTarget != null)
		{
			ControlObjHandler component2 = selectedTarget.GetComponent<ControlObjHandler>();
			if (multiselect_toggle.getCurrentlySelected().Count == 0)
			{
				selectedTarget.Find("SelectedIndicator").gameObject.SetActive(false);
			}
			component2.setActiveTarget(false);
		}
		selectedTarget = trans;
		if (camMovementBehavior.ControllerToggleType.Equals(ControllerType.RealTimeStratToggle) && !multiselect_toggle.getCurrentlySelected().Contains(selectedTarget))
		{
			multiselect_toggle.getCurrentlySelected().Add(selectedTarget);
		}
		selectedTarget.Find("SelectedIndicator").gameObject.SetActive(true);
		ControlObjHandler component3 = selectedTarget.GetComponent<ControlObjHandler>();
		component3.setActiveTarget(true);
	}

	private void ExecuteEscapeSequence()
	{
		if (camMovementBehavior.ControllerToggleType.Equals(ControllerType.RealTimeStratToggle) && activeTarget == null && multiselect_toggle.getCurrentlySelected().Count > 0)
		{
			multiselect_toggle.ClearSelectedTargets();
		}
		if (activeTarget != null)
		{
			if (activeTarget.name.Contains(NPCAssetName))
			{
				activeTarget.GetComponent<NPCHandler>().setActiveTarget(false);
			}
			else
			{
				activeTarget.GetComponent<CreatureHandler>().setActiveTarget(false);
			}
			activeTarget.Find("SelectedIndicator").gameObject.SetActive(false);
			activeTarget = null;
		}
		else if (selectedTarget != null)
		{
			selectedTarget.Find("SelectedIndicator").gameObject.SetActive(false);
			selectedTarget.GetComponent<ControlObjHandler>().setActiveTarget(false);
			selectedTarget = null;
			camMovementBehavior.SendMessage("ExecuteEscapeSequence");
		}
	}

	public void setActiveCursorScrollValues(int c_index, float dx, float dy)
	{
		activeCursorGfx = mouseDirection[c_index];
		CursorPosMod = new Vector2(dx, dy);
	}

	public Transform getSelectedTarget()
	{
		return selectedTarget;
	}

	public Transform getActiveTarget()
	{
		return activeTarget;
	}
}
