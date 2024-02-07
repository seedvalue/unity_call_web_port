using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathFinder
{
	private class PathCalculator
	{
		private Transform moveObject;

		private GridGenerator.GridSquare init;

		private GridGenerator.GridSquare final;

		private bool ActiveDraw;

		private float drawTime;

		private List<GridGenerator.GridSquare> pathTiles = new List<GridGenerator.GridSquare>();

		private bool Calculated;

		private float elapsedTime;

		public float ElapsedTime
		{
			get
			{
				return elapsedTime;
			}
		}

		public PathCalculator(GridGenerator.GridSquare i, GridGenerator.GridSquare f)
		{
			init = i;
			final = f;
		}

		public PathCalculator(Transform t, GridGenerator.GridSquare i, GridGenerator.GridSquare f)
		{
			moveObject = t;
			init = i;
			final = f;
		}

		public void ThreadRun()
		{
			CalculatePath(init, final);
		}

		public bool isCalculated()
		{
			return Calculated;
		}

		public bool isActiveDraw()
		{
			return ActiveDraw;
		}

		public List<GridGenerator.GridSquare> getCalculatedPathTiles()
		{
			return pathTiles.GetRange(0, pathTiles.Count);
		}

		public void SendTransformCalculatedPath()
		{
			if (moveObject != null)
			{
				moveObject.SendMessage("ReceiveCalculatedPath", getCalculatedPathTiles());
			}
			Calculated = false;
		}

		public void DrawPath(GridGenerator gridGen, float sTime)
		{
			if (drawTime > sTime)
			{
				drawTime = 0f;
				ActiveDraw = false;
			}
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
			drawTime += Time.deltaTime;
		}

		public void CalculatePath(GridGenerator.GridSquare start, GridGenerator.GridSquare end)
		{
			BinaryHeap<GridGenerator.GridSquare> binaryHeap = new BinaryHeap<GridGenerator.GridSquare>(new GridGenerator.GridComparer());
			List<GridGenerator.GridSquare> list = new List<GridGenerator.GridSquare>();
			GridGenerator.GridSquare gridSquare = start;
			GridGenerator.GridSquare gridSquare2 = null;
			float num = DateTime.Now.Millisecond;
			Calculated = false;
			pathTiles.Clear();
			gridSquare.setGMovementCost(0);
			gridSquare.setHMovementCost((Mathf.Abs(end.getRow() - start.getRow()) + Mathf.Abs(end.getColumn() - start.getColumn())) * 10);
			gridSquare.setFMovementCost(gridSquare.getGMovementCost() + gridSquare.getHMovementCost());
			binaryHeap.Insert(gridSquare);
			while (binaryHeap.Count > 0)
			{
				gridSquare = binaryHeap.RemoveRoot();
				gridSquare.isOnOpenList = false;
				if (gridSquare == end)
				{
					break;
				}
				gridSquare.isOnCloseList = true;
				list.Add(gridSquare);
				List<GridGenerator.GridSquare> list2 = new List<GridGenerator.GridSquare>(gridSquare.getAdjacentSquares());
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].isOnCloseList)
					{
						continue;
					}
					int num2 = 12;
					if (Mathf.Abs(gridSquare.getRow() - list2[i].getRow()) + Mathf.Abs(gridSquare.getColumn() - list2[i].getColumn()) == 2)
					{
						num2 = 14;
					}
					int hMovementCost = (Mathf.Abs(end.getRow() - list2[i].getRow()) + Mathf.Abs(end.getColumn() - list2[i].getColumn())) * 10;
					int num3 = gridSquare.getGMovementCost() + num2;
					bool isOnOpenList = list2[i].isOnOpenList;
					if (!isOnOpenList || num3 <= list2[i].getGMovementCost())
					{
						list2[i].ParentObject = gridSquare;
						list2[i].setGMovementCost(num3);
						list2[i].setHMovementCost(hMovementCost);
						list2[i].setFMovementCost(list2[i].getGMovementCost() + list2[i].getHMovementCost());
						if (gridSquare2 == null || gridSquare2.getHMovementCost() > gridSquare.getHMovementCost())
						{
							gridSquare2 = gridSquare;
						}
						if (!isOnOpenList)
						{
							list2[i].isOnOpenList = true;
							binaryHeap.Insert(list2[i]);
						}
					}
				}
			}
			if (gridSquare == end)
			{
				pathTiles.Add(gridSquare);
				while (gridSquare != start)
				{
					pathTiles.Add(gridSquare.ParentObject);
					gridSquare = gridSquare.ParentObject;
				}
			}
			else
			{
				pathTiles.Add(gridSquare2);
				while (gridSquare2 != start)
				{
					pathTiles.Add(gridSquare2.ParentObject);
					gridSquare2 = gridSquare2.ParentObject;
				}
			}
			pathTiles.Reverse();
			elapsedTime = (float)DateTime.Now.Millisecond - num;
			Calculated = true;
			ActiveDraw = true;
		}
	}

	private GridGenerator mainGrid;

	private PathCalculator newPath;

	private Thread pathCalcThread;

	private Queue<PathStructQueue> calcQueue = new Queue<PathStructQueue>();

	public PathFinder(GridGenerator grid)
	{
		mainGrid = grid;
	}

	public void CalculateNewPath(GridGenerator.GridSquare init, GridGenerator.GridSquare final)
	{
		InitiateThreadCalculation(new PathCalculator(init, final));
	}

	public void CalculateNewPath(Transform moveobj, GridGenerator.GridSquare init, GridGenerator.GridSquare final)
	{
		PathStructQueue item = default(PathStructQueue);
		item.trans = moveobj;
		item.i = init;
		item.f = final;
		calcQueue.Enqueue(item);
		if (calcQueue.Count == 1)
		{
			InitiateThreadCalculation(new PathCalculator(item.trans, item.i, item.f));
		}
	}

	private void InitiateThreadCalculation(PathCalculator d_path)
	{
		newPath = d_path;
		pathCalcThread = new Thread(newPath.ThreadRun);
		try
		{
			pathCalcThread.Start();
		}
		catch (ThreadStateException ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public void CheckPathCalculation()
	{
		if (newPath != null && newPath.isCalculated() && pathCalcThread != null)
		{
			pathCalcThread.Join();
			pathCalcThread = null;
			mainGrid.ResetGrid();
			newPath.SendTransformCalculatedPath();
			calcQueue.Dequeue();
			if (calcQueue.Count > 0)
			{
				PathStructQueue pathStructQueue = calcQueue.Peek();
				InitiateThreadCalculation(new PathCalculator(pathStructQueue.trans, pathStructQueue.i, pathStructQueue.f));
			}
		}
	}

	public void DrawPath()
	{
		if (newPath != null && newPath.isActiveDraw())
		{
			newPath.DrawPath(mainGrid, 10f);
		}
	}

	public List<GridGenerator.GridSquare> getCalculatedTiles()
	{
		return newPath.getCalculatedPathTiles();
	}
}
