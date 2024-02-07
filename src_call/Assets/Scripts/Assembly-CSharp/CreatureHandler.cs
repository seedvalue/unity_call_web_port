using System.Collections.Generic;
using UnityEngine;

public class CreatureHandler : MonoBehaviour
{
	private static List<Transform> creature_handlers = new List<Transform>();

	public string Creature_Name = "NEWCREATURE";

	public Color ActiveColor = Color.magenta;

	private bool active_target;

	private GUIStyle handler_style = new GUIStyle();

	private List<GridGenerator.GridSquare> pathTiles = new List<GridGenerator.GridSquare>();

	private bool MovementToggle;

	private int indexMovetoCount = 1;

	private int ID = -1;

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
		creature_handlers.Add(base.transform);
	}

	private void OnDestroy()
	{
		creature_handlers.Remove(base.transform);
	}

	private void OnGUI()
	{
		if (active_target)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position + new Vector3(0f, base.transform.localScale.y, 0f));
			Vector2 vector2 = handler_style.CalcSize(new GUIContent(Creature_Name));
			GUI.Label(new Rect(vector.x - vector2.x / 2f, (float)Screen.height - vector.y - vector2.y, vector2.x, vector2.y * 2f), Creature_Name);
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
		if (ID != creature_handlers.IndexOf(base.transform))
		{
			ID = creature_handlers.IndexOf(base.transform);
			base.name = "CreatureObj" + ID;
		}
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

	public override string ToString()
	{
		return Creature_Name.ToString();
	}
}
