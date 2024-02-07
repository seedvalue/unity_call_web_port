using UnityEngine;

public class GridRenderer : MonoBehaviour
{
	private GridGenerator gridGenerator;

	private Matrix4x4 gridTransformation;

	public GameObject Grid;

	public bool ActiveGridDraw;

	public bool ActivePathDraw;

	private void Start()
	{
		gridGenerator = Grid.GetComponent<GridGenerator>();
		gridTransformation = gridGenerator.transform.worldToLocalMatrix * Matrix4x4.TRS(gridGenerator.transform.position, gridGenerator.transform.rotation, gridGenerator.transform.localScale);
	}

	private void OnPostRender()
	{
		GL.PushMatrix();
		gridGenerator.GetComponent<Renderer>().material.SetPass(0);
		GL.MultMatrix(gridTransformation);
		GL.Begin(1);
		GL.Color(gridGenerator.GetComponent<Renderer>().material.color);
		if (ActiveGridDraw)
		{
			for (int i = 0; i < gridGenerator.getRows(); i++)
			{
				for (int j = 0; j < gridGenerator.getColumns(); j++)
				{
					if (gridGenerator.getDesiredGridSquare(i, j) != null && isInFOV(gridGenerator.getDesiredGridSquare(i, j).getGridSquareCentralOrigin()))
					{
						Vector3 gridSquareLowerLeft = gridGenerator.getDesiredGridSquare(i, j).getGridSquareLowerLeft();
						Vector3 gridSquareLowerRight = gridGenerator.getDesiredGridSquare(i, j).getGridSquareLowerRight();
						Vector3 gridSquareUpperLeft = gridGenerator.getDesiredGridSquare(i, j).getGridSquareUpperLeft();
						Vector3 gridSquareUpperRight = gridGenerator.getDesiredGridSquare(i, j).getGridSquareUpperRight();
						GL.Vertex3(gridSquareLowerLeft.x, gridSquareLowerLeft.y, gridSquareLowerLeft.z);
						GL.Vertex3(gridSquareUpperLeft.x, gridSquareUpperLeft.y, gridSquareUpperLeft.z);
						GL.Vertex3(gridSquareLowerLeft.x, gridSquareLowerLeft.y, gridSquareLowerLeft.z);
						GL.Vertex3(gridSquareLowerRight.x, gridSquareLowerRight.y, gridSquareLowerRight.z);
						GL.Vertex3(gridSquareLowerRight.x, gridSquareLowerRight.y, gridSquareLowerRight.z);
						GL.Vertex3(gridSquareUpperRight.x, gridSquareUpperRight.y, gridSquareUpperRight.z);
						GL.Vertex3(gridSquareUpperLeft.x, gridSquareUpperLeft.y, gridSquareUpperLeft.z);
						GL.Vertex3(gridSquareUpperRight.x, gridSquareUpperRight.y, gridSquareUpperRight.z);
					}
				}
			}
		}
		if (ActivePathDraw)
		{
			Object[] array = Object.FindObjectsOfType(typeof(ControlObjHandler));
			for (int k = 0; k < array.Length; k++)
			{
				ControlObjHandler controlObjHandler = (ControlObjHandler)array[k];
				if (controlObjHandler.Moveable)
				{
					controlObjHandler.SendMessage("DisplayPath", gridGenerator);
				}
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	private bool isInFOV(Vector3 worldPos)
	{
		worldPos = GetComponent<Camera>().WorldToScreenPoint(worldPos);
		if (worldPos.x >= 0f && worldPos.x <= (float)Screen.width && worldPos.y >= 0f && worldPos.y <= (float)Screen.height)
		{
			return true;
		}
		return false;
	}
}
