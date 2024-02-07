using System.Collections.Generic;
using UnityEngine;

public class ControlObjHandler : MonoBehaviour
{
	private static List<Transform> control_handlers = new List<Transform>();

	private bool active_target;

	private bool active_collision;

	private List<GridGenerator.GridSquare> pathTiles = new List<GridGenerator.GridSquare>();

	private bool MovementToggle;

	private int indexMovetoCount = 1;

	private int ID = -1;

	private HealthSystem healthSystem;

	private int lineWidth = 2;

	public string Controller_Name = "NEWCONTROLLER";

	public Color ActiveColor = Color.white;

	public bool ToggleIndicatorSelect = true;

	public bool DisplayHealth = true;

	public Material IndicatorMaterial;

	public Rect HealthBarDimens;

	public bool VerticleHealthBar;

	public Texture HealthBubbleTexture;

	public Texture HealthTexture;

	public float BarRotationVal;

	public bool Moveable
	{
		get
		{
			return MovementToggle;
		}
		set
		{
			MovementToggle = value;
		}
	}

	private void Start()
	{
		base.transform.Find("SelectedIndicator").gameObject.SetActive(false);
		base.transform.Find("SelectedIndicator").GetComponent<Renderer>().material.SetColor("_Color", new Color(ActiveColor.r + ActiveColor.r / 2f, ActiveColor.g + ActiveColor.g / 2f, ActiveColor.b + ActiveColor.b / 2f));
		base.transform.Find("SelectedIndicator").GetComponent<Renderer>().material.SetColor("_Emission", ActiveColor);
		base.transform.Find("SelectedIndicator").localScale = new Vector3(0.1f + base.transform.localScale.x * 0.03f, 1f, 0.1f + base.transform.localScale.z * 0.03f);
		control_handlers.Add(base.transform);
		healthSystem = new HealthSystem(HealthBarDimens, VerticleHealthBar, HealthBubbleTexture, HealthTexture, BarRotationVal);
		healthSystem.Initialize();
	}

	private void OnDestroy()
	{
		control_handlers.Remove(base.transform);
	}

	private void OnGUI()
	{
		if (!base.transform.Find("SelectedIndicator").gameObject.activeSelf)
		{
			return;
		}
		if (DisplayHealth)
		{
			healthSystem.DrawBar();
		}
		if (!ToggleIndicatorSelect)
		{
			return;
		}
		Vector3 worldPos = Camera.main.WorldToScreenPoint(base.transform.position + (base.transform.localScale.magnitude / 2f * -Camera.main.transform.right - new Vector3(0f, base.transform.localScale.y + base.transform.localScale.y / 2f, 0f)));
		Vector3 worldPos2 = Camera.main.WorldToScreenPoint(base.transform.position + (base.transform.localScale.magnitude / 2f * -Camera.main.transform.right + new Vector3(0f, base.transform.localScale.y + base.transform.localScale.y / 2f, 0f)));
		Vector3 worldPos3 = Camera.main.WorldToScreenPoint(base.transform.position + (base.transform.localScale.magnitude / 2f * Camera.main.transform.right - new Vector3(0f, base.transform.localScale.y + base.transform.localScale.y / 2f, 0f)));
		Vector3 worldPos4 = Camera.main.WorldToScreenPoint(base.transform.position + (base.transform.localScale.magnitude / 2f * Camera.main.transform.right + new Vector3(0f, base.transform.localScale.y + base.transform.localScale.y / 2f, 0f)));
		if (isInFOV(worldPos) || isInFOV(worldPos2) || isInFOV(worldPos3) || isInFOV(worldPos4))
		{
			GL.PushMatrix();
			IndicatorMaterial.SetPass(0);
			GL.LoadPixelMatrix();
			GL.Begin(1);
			GL.Color(IndicatorMaterial.color);
			for (int i = 0; i < lineWidth; i++)
			{
				GL.Vertex3(worldPos.x - (float)i, worldPos.y, 0f);
				GL.Vertex3(worldPos2.x - (float)i, worldPos2.y, 0f);
				GL.Vertex3(worldPos3.x + (float)i, worldPos3.y, 0f);
				GL.Vertex3(worldPos4.x + (float)i, worldPos4.y, 0f);
				GL.Vertex3(worldPos2.x, worldPos2.y - (float)i, 0f);
				GL.Vertex3(worldPos2.x + (worldPos4.x - worldPos2.x) / 4f, worldPos2.y - (float)i, 0f);
				GL.Vertex3(worldPos4.x, worldPos4.y - (float)i, 0f);
				GL.Vertex3(worldPos4.x + (worldPos2.x - worldPos4.x) / 4f, worldPos4.y - (float)i, 0f);
				GL.Vertex3(worldPos.x, worldPos.y + (float)i, 0f);
				GL.Vertex3(worldPos.x + (worldPos3.x - worldPos.x) / 4f, worldPos.y + (float)i, 0f);
				GL.Vertex3(worldPos3.x, worldPos3.y + (float)i, 0f);
				GL.Vertex3(worldPos3.x + (worldPos.x - worldPos3.x) / 4f, worldPos3.y + (float)i, 0f);
			}
			GL.End();
			GL.PopMatrix();
		}
	}

	private void Update()
	{
		if (indexMovetoCount < pathTiles.Count && Mathf.Sqrt(Mathf.Pow(base.transform.position.x - pathTiles[indexMovetoCount].getGridSquareCentralOrigin().x, 2f) + Mathf.Pow(base.transform.position.z - pathTiles[indexMovetoCount].getGridSquareCentralOrigin().z, 2f)) < 0.5f)
		{
			indexMovetoCount++;
		}
		else if (indexMovetoCount == pathTiles.Count && Moveable)
		{
			indexMovetoCount = 1;
			Moveable = false;
		}
		if (ID != control_handlers.IndexOf(base.transform))
		{
			ID = control_handlers.IndexOf(base.transform);
			base.name = "ControlObj" + ID;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position + new Vector3(0f, base.transform.localScale.y + base.transform.localScale.y / 2f, 0f));
		vector.y = (float)Screen.height - vector.y;
		if (healthSystem.getScrollBarRect().x != vector.x && healthSystem.getScrollBarRect().y != vector.y)
		{
			healthSystem.Update((int)vector.x, (int)vector.y);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		active_collision = true;
	}

	private void OnCollisionExit(Collision collision)
	{
		active_collision = false;
	}

	private void ReceiveCalculatedPath(List<GridGenerator.GridSquare> path)
	{
		pathTiles = path;
		indexMovetoCount = 1;
		Moveable = true;
	}

	private void DisplayPath(GridGenerator gridGen)
	{
		for (int i = 0; i < pathTiles.Count; i++)
		{
			GL.Color(Color.yellow);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x, pathTiles[i].getGridSquareCentralOrigin().y, pathTiles[i].getGridSquareCentralOrigin().z);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x, pathTiles[i].getGridSquareCentralOrigin().y + (float)(gridGen.GridHeight / 2), pathTiles[i].getGridSquareCentralOrigin().z);
			GL.Color(Color.green);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x - 0.5f, pathTiles[i].getGridSquareCentralOrigin().y, pathTiles[i].getGridSquareCentralOrigin().z);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x - 0.5f, pathTiles[i].getGridSquareCentralOrigin().y + (float)(gridGen.GridHeight / 2), pathTiles[i].getGridSquareCentralOrigin().z);
			GL.Color(Color.blue);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x - 0.25f, pathTiles[i].getGridSquareCentralOrigin().y, pathTiles[i].getGridSquareCentralOrigin().z - 0.5f);
			GL.Vertex3(pathTiles[i].getGridSquareCentralOrigin().x - 0.25f, pathTiles[i].getGridSquareCentralOrigin().y + (float)(gridGen.GridHeight / 2), pathTiles[i].getGridSquareCentralOrigin().z - 0.5f);
		}
	}

	private bool isInFOV(Vector3 worldPos)
	{
		if (worldPos.x >= 0f && worldPos.x <= (float)Screen.width && worldPos.y >= 0f && worldPos.y <= (float)Screen.height)
		{
			return true;
		}
		return false;
	}

	public static List<Transform> DetermineInMultiSelect(Vector2 init, Vector2 end)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform control_handler in control_handlers)
		{
			Vector2 vector = Camera.main.WorldToScreenPoint(control_handler.position);
			vector.y = (float)Screen.height - vector.y;
			if (((vector.x >= init.x && vector.x <= end.x && vector.y >= init.y && vector.y <= end.y) || (vector.x <= init.x && vector.x >= end.x && vector.y <= init.y && vector.y >= end.y) || (vector.x >= end.x && vector.y <= end.y && vector.x <= init.x && vector.y >= init.y) || (vector.x >= init.x && vector.y <= init.y && vector.x <= end.x && vector.y >= end.y)) && !list.Contains(control_handler))
			{
				list.Add(control_handler);
			}
		}
		return list;
	}

	public bool isValidMovePathTileCount()
	{
		if (indexMovetoCount < pathTiles.Count)
		{
			return true;
		}
		return false;
	}

	public GridGenerator.GridSquare getMovePathTile()
	{
		return pathTiles[indexMovetoCount];
	}

	public void setActiveTarget(bool val)
	{
		active_target = val;
	}

	public bool isActiveTarget()
	{
		return active_target;
	}

	public bool isActiveCollision()
	{
		return active_collision;
	}

	public override string ToString()
	{
		return Controller_Name.ToString();
	}
}
