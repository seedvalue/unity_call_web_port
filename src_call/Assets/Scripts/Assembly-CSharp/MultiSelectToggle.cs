using System.Collections.Generic;
using UnityEngine;

public class MultiSelectToggle : MonoBehaviour
{
	private float xStartPos;

	private float yStartPos;

	private float xEndPos;

	private float yEndPos;

	private Material line_material;

	private ControllerType controllerType;

	private bool movementAllowed;

	private List<Transform> currently_selected = new List<Transform>();

	private void OnGUI()
	{
		if (!controllerType.Equals(ControllerType.RealTimeStratToggle))
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			xStartPos = Event.current.mousePosition.x;
			yStartPos = (float)Screen.height - Event.current.mousePosition.y;
		}
		if (Input.GetMouseButton(0))
		{
			xEndPos = Event.current.mousePosition.x;
			yEndPos = (float)Screen.height - Event.current.mousePosition.y;
			if (Mathf.Abs(xEndPos - xStartPos) > 10f && Mathf.Abs(yEndPos - yStartPos) > 10f)
			{
				movementAllowed = false;
				GL.PushMatrix();
				line_material.SetPass(0);
				GL.LoadPixelMatrix();
				GL.Begin(1);
				GL.Color(line_material.color);
				GL.Vertex3(xStartPos, yStartPos, 0f);
				GL.Vertex3(xEndPos, yStartPos, 0f);
				GL.Vertex3(xStartPos, yStartPos - 1f, 0f);
				GL.Vertex3(xStartPos, yEndPos, 0f);
				GL.Vertex3(xStartPos, yEndPos, 0f);
				GL.Vertex3(xEndPos, yEndPos, 0f);
				GL.Vertex3(xEndPos, yStartPos, 0f);
				GL.Vertex3(xEndPos, yEndPos, 0f);
				GL.End();
				GL.PopMatrix();
			}
		}
	}

	private void Update()
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		List<Transform> list = ControlObjHandler.DetermineInMultiSelect(new Vector2(xStartPos, (float)Screen.height - yStartPos), new Vector2(xEndPos, (float)Screen.height - yEndPos));
		if (list.Count > 0)
		{
			ClearSelectedTargets();
			foreach (Transform item in list)
			{
				item.Find("SelectedIndicator").gameObject.SetActive(true);
				currently_selected.Add(item);
			}
			if (currently_selected.Count > 0)
			{
				SendMessage("ChangeSelectedTarget", currently_selected[currently_selected.Count / 2]);
			}
		}
		movementAllowed = true;
	}

	public void SetInitialOptions(Material material, ControllerType gametype)
	{
		line_material = material;
		controllerType = gametype;
	}

	public void ClearSelectedTargets()
	{
		if (currently_selected.Count <= 0)
		{
			return;
		}
		foreach (Transform item in currently_selected)
		{
			item.Find("SelectedIndicator").gameObject.SetActive(false);
		}
		currently_selected.Clear();
	}

	public List<Transform> getCurrentlySelected()
	{
		return currently_selected;
	}

	public bool isMovementAllowed()
	{
		return movementAllowed;
	}
}
