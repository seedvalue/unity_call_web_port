using System;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
	public class GridComparer : IComparer<GridSquare>
	{
		public int Compare(GridSquare a, GridSquare b)
		{
			if (a.getFMovementCost() < b.getFMovementCost())
			{
				return -1;
			}
			if (a.getFMovementCost() > b.getFMovementCost())
			{
				return 1;
			}
			return 0;
		}
	}

	public class GridSquareQuadrant
	{
		private static GridSquare[,] gridSquares;

		private static int QuadSize = -1;

		private List<GridSquareQuadrant> adjacentQuadrants = new List<GridSquareQuadrant>();

		private int quad_row;

		private int quad_col = -1;

		public GridSquareQuadrant(int quad_size, int t_rows, int t_cols, int q_row, int q_col)
		{
			if (gridSquares == null)
			{
				gridSquares = new GridSquare[t_rows, t_cols];
				QuadSize = quad_size;
			}
			quad_row = q_row;
			quad_col = q_col;
		}

		public GridSquareQuadrant(int q_row, int q_col)
		{
			quad_row = q_row;
			quad_col = q_col;
		}

		public static void resetQuadrantSquares(int t_r, int t_c)
		{
			for (int i = 0; i < t_r; i++)
			{
				for (int j = 0; j < t_c; j++)
				{
					if (gridSquares[i, j] != null)
					{
						gridSquares[i, j].isOnOpenList = false;
						gridSquares[i, j].isOnCloseList = false;
					}
				}
			}
		}

		public static GridSquare retrieveSpecificSquare(int r, int c)
		{
			return gridSquares[r, c];
		}

		public static void removeSpecificSquare(int r, int c)
		{
			gridSquares[r, c] = null;
		}

		public static void addSquaretoQuad(GridSquare newSquare, int r, int c)
		{
			gridSquares[r, c] = newSquare;
		}

		public void determineAdjacentQuadrants(GridSquareQuadrant[,] quads)
		{
			for (int i = 0; i < QuadSize; i++)
			{
				try
				{
					if (gridSquares[quad_row * QuadSize + i, quad_col * QuadSize] != null && gridSquares[quad_row * QuadSize + i, quad_col * QuadSize - 1] != null)
					{
						adjacentQuadrants.Add(quads[quad_row, quad_col - 1]);
						break;
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
			}
			for (int j = 0; j < QuadSize; j++)
			{
				try
				{
					if (gridSquares[quad_row * QuadSize, quad_col * QuadSize + j] != null && gridSquares[quad_row * QuadSize - 1, quad_col * QuadSize + j] != null)
					{
						adjacentQuadrants.Add(quads[quad_row - 1, quad_col]);
						break;
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
			}
			for (int k = 0; k < QuadSize; k++)
			{
				try
				{
					if (gridSquares[quad_row * QuadSize + k, quad_col * QuadSize + QuadSize - 1] != null && gridSquares[quad_row * QuadSize + k, quad_col * QuadSize + QuadSize - 1] != null)
					{
						adjacentQuadrants.Add(quads[quad_row, quad_col + 1]);
						break;
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
			}
			for (int l = 0; l < QuadSize; l++)
			{
				try
				{
					if (gridSquares[quad_row * QuadSize + QuadSize - 1, quad_col * QuadSize + l] != null && gridSquares[quad_row * QuadSize + QuadSize, quad_col * QuadSize + l] != null)
					{
						adjacentQuadrants.Add(quads[quad_row + 1, quad_col]);
						break;
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
			}
			try
			{
				if (gridSquares[quad_row * QuadSize, quad_col * QuadSize] != null && gridSquares[quad_row * QuadSize - 1, quad_col * QuadSize - 1] != null)
				{
					adjacentQuadrants.Add(quads[quad_row - 1, quad_col - 1]);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
			try
			{
				if (gridSquares[quad_row * QuadSize, quad_col * QuadSize + QuadSize - 1] != null && gridSquares[quad_row * QuadSize - 1, quad_col * QuadSize + QuadSize] != null)
				{
					adjacentQuadrants.Add(quads[quad_row - 1, quad_col + 1]);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
			try
			{
				if (gridSquares[quad_row * QuadSize + QuadSize - 1, quad_col * QuadSize] != null && gridSquares[quad_row * QuadSize + QuadSize, quad_col * QuadSize - 1] != null)
				{
					adjacentQuadrants.Add(quads[quad_row + 1, quad_col - 1]);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
			try
			{
				if (gridSquares[quad_row * QuadSize + QuadSize - 1, quad_col * QuadSize + QuadSize - 1] != null && gridSquares[quad_row * QuadSize + QuadSize, quad_col * QuadSize + QuadSize] != null)
				{
					adjacentQuadrants.Add(quads[quad_row + 1, quad_col + 1]);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
		}

		public GridSquare[] retrieveQuadrantSquares()
		{
			List<GridSquare> list = new List<GridSquare>();
			for (int i = quad_row * QuadSize; i < quad_row * QuadSize + QuadSize; i++)
			{
				for (int j = quad_col * QuadSize; j < quad_col * QuadSize + QuadSize; j++)
				{
					try
					{
						if (gridSquares[i, j] != null)
						{
							list.Add(gridSquares[i, j]);
						}
					}
					catch (IndexOutOfRangeException)
					{
					}
				}
			}
			return list.ToArray();
		}

		public void resetQuadrant()
		{
		}

		public int getRow()
		{
			return quad_row;
		}

		public int getColumn()
		{
			return quad_col;
		}

		public List<GridSquareQuadrant> getAdjacentQuadrants()
		{
			return adjacentQuadrants;
		}
	}

	public class GridSquare
	{
		private static GridGenerator parentGenerator;

		private int row;

		private int col = -1;

		private float slopedAngle;

		private int[] quadrant;

		private Vector3[] positions = new Vector3[4];

		private List<GridSquare> adjacentSquares = new List<GridSquare>();

		private int f_val;

		private int g_val;

		private int h_val;

		private GridSquare parentSquare;

		private bool onOpenList;

		private bool onCloseList;

		public GridSquare ParentObject
		{
			get
			{
				return parentSquare;
			}
			set
			{
				parentSquare = value;
			}
		}

		public bool isOnOpenList
		{
			get
			{
				return onOpenList;
			}
			set
			{
				onOpenList = value;
			}
		}

		public bool isOnCloseList
		{
			get
			{
				return onCloseList;
			}
			set
			{
				onCloseList = value;
			}
		}

		public GridSquare(GridGenerator parent, int r, int c, float slope, int[] quad, Vector3 LL, Vector3 LR, Vector3 UL, Vector3 UR)
		{
			parentGenerator = parent;
			row = r;
			col = c;
			slopedAngle = slope;
			quadrant = quad;
			positions[0] = LL;
			positions[1] = LR;
			positions[2] = UL;
			positions[3] = UR;
		}

		private GridSquare DeterminedValidAdjacent(int r, int c)
		{
			if (r <= parentGenerator.Rows - 1 && c <= parentGenerator.Columns - 1 && r >= 0 && c >= 0)
			{
				return parentGenerator.getDesiredGridSquare(r, c);
			}
			return null;
		}

		public void calculateAdjacentSquares()
		{
			int[] array = new int[3]
			{
				row - 1,
				row,
				row + 1
			};
			int[] array2 = new int[3]
			{
				col - 1,
				col,
				col + 1
			};
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					if (i != 1 || j != 1)
					{
						GridSquare gridSquare = DeterminedValidAdjacent(array[i], array2[j]);
						if (gridSquare != null)
						{
							adjacentSquares.Add(gridSquare);
						}
					}
				}
			}
		}

		public GridSquareQuadrant getQuadrant()
		{
			return parentGenerator.getDesiredQuadrant(quadrant[0], quadrant[1]);
		}

		public Vector3 getGridSquareLowerLeft()
		{
			return parentGenerator.transform.position + positions[0];
		}

		public Vector3 getGridSquareLowerRight()
		{
			return parentGenerator.transform.position + positions[1];
		}

		public Vector3 getGridSquareUpperLeft()
		{
			return parentGenerator.transform.position + positions[2];
		}

		public Vector3 getGridSquareUpperRight()
		{
			return parentGenerator.transform.position + positions[3];
		}

		public Vector3 getGridSquareCentralOrigin()
		{
			return parentGenerator.transform.position + (positions[0] + (positions[3] - positions[0]) / 2f);
		}

		public float getSlopeValue()
		{
			return slopedAngle;
		}

		public void setFMovementCost(int val)
		{
			f_val = val;
		}

		public void setGMovementCost(int val)
		{
			g_val = val;
		}

		public void setHMovementCost(int val)
		{
			h_val = val;
		}

		public int getFMovementCost()
		{
			return f_val;
		}

		public int getGMovementCost()
		{
			return g_val;
		}

		public int getHMovementCost()
		{
			return h_val;
		}

		public int getRow()
		{
			return row;
		}

		public int getColumn()
		{
			return col;
		}

		public List<GridSquare> getAdjacentSquares()
		{
			return adjacentSquares;
		}
	}

	private GridSquareQuadrant[,] gridQuadrants;

	private PathFinder pathFinder;

	private RaycastHit raycastInfo = default(RaycastHit);

	public LayerMask IgnoreMask;

	public int Rows = 16;

	public int Columns = 16;

	public int GridWidth = 10;

	public int GridHeight = 20;

	public int GridDepth = 10;

	public int GridQuadSize = 8;

	public float MinSlopeAngle = 5f;

	public float MaxSlopeAngle = 30f;

	public float Rigidity = 0.1f;

	public float AngularRigidity = 0.01f;

	private void Start()
	{
		gridQuadrants = new GridSquareQuadrant[Mathf.FloorToInt(Rows / GridQuadSize) + 1, Mathf.FloorToInt(Columns / GridQuadSize) + 1];
		Vector3 vector = new Vector3(GridWidth, 0f, GridDepth);
		Vector3 vector2 = new Vector3(vector.x + (float)GridWidth, 0f, vector.z);
		Vector3 vector3 = new Vector3(vector.x, 0f, vector.z + (float)GridDepth);
		Vector3 vector4 = new Vector3(vector2.x, 0f, vector2.z + (float)GridDepth);
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				vector = new Vector3(i * GridWidth, 0f, j * GridDepth);
				vector2 = new Vector3(vector.x + (float)GridWidth, 0f, vector.z);
				vector3 = new Vector3(vector.x, 0f, vector.z + (float)GridDepth);
				vector4 = new Vector3(vector2.x, 0f, vector2.z + (float)GridDepth);
				int[] array = DetermineActiveGridQuad(i, j);
				try
				{
					if (gridQuadrants[array[0], array[1]] == null)
					{
						if (gridQuadrants[0, 0] == null)
						{
							gridQuadrants[array[0], array[1]] = new GridSquareQuadrant(GridQuadSize, Rows, Columns, array[0], array[1]);
						}
						else
						{
							gridQuadrants[array[0], array[1]] = new GridSquareQuadrant(array[0], array[1]);
						}
					}
				}
				catch (IndexOutOfRangeException)
				{
				}
				if (!Physics.Raycast(new Ray(base.transform.position + vector + new Vector3(0f, GridHeight, 0f), -base.transform.up), GridHeight + GridHeight / 2, ~IgnoreMask.value) || !Physics.Raycast(new Ray(base.transform.position + vector2 + new Vector3(0f, GridHeight, 0f), -base.transform.up), GridHeight + GridHeight / 2, ~IgnoreMask.value) || !Physics.Raycast(new Ray(base.transform.position + vector3 + new Vector3(0f, GridHeight, 0f), -base.transform.up), GridHeight + GridHeight / 2, ~IgnoreMask.value) || !Physics.Raycast(new Ray(base.transform.position + vector4 + new Vector3(0f, GridHeight, 0f), -base.transform.up), GridHeight + GridHeight / 2, ~IgnoreMask.value))
				{
					continue;
				}
				RaycastHit[] array2 = new RaycastHit[4];
				Physics.Raycast(new Ray(base.transform.position + vector + new Vector3(0f, GridHeight, 0f), -base.transform.up), out raycastInfo, GridHeight + GridHeight / 2, ~IgnoreMask.value);
				array2[0] = raycastInfo;
				Physics.Raycast(new Ray(base.transform.position + vector2 + new Vector3(0f, GridHeight, 0f), -base.transform.up), out raycastInfo, GridHeight + GridHeight / 2, ~IgnoreMask.value);
				array2[1] = raycastInfo;
				Physics.Raycast(new Ray(base.transform.position + vector3 + new Vector3(0f, GridHeight, 0f), -base.transform.up), out raycastInfo, GridHeight + GridHeight / 2, ~IgnoreMask.value);
				array2[2] = raycastInfo;
				Physics.Raycast(new Ray(base.transform.position + vector4 + new Vector3(0f, GridHeight, 0f), -base.transform.up), out raycastInfo, GridHeight + GridHeight / 2, ~IgnoreMask.value);
				array2[3] = raycastInfo;
				float[] array3 = new float[2]
				{
					Mathf.Atan((array2[2].point.y - array2[0].point.y) / (array2[2].point.z - array2[0].point.z)),
					Mathf.Atan((array2[1].point.y - array2[0].point.y) / (array2[1].point.x - array2[0].point.x))
				};
				if (Mathf.Abs(array2[0].distance - array2[1].distance) <= Rigidity && Mathf.Abs(array2[0].distance - array2[2].distance) <= Rigidity && Mathf.Abs(array2[0].distance - array2[3].distance) <= Rigidity)
				{
					if (j - 1 >= 0 && GridSquareQuadrant.retrieveSpecificSquare(i, j - 1) != null)
					{
						vector3 = new Vector3(GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareLowerLeft().x - base.transform.position.x, vector.y + (array2[2].point.y - vector.y), vector.z + (float)GridDepth);
						vector4 = new Vector3(GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperRight().x - base.transform.position.x, vector3.y, vector2.z + (float)GridDepth);
						vector = new Vector3(GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperLeft().x - base.transform.position.x, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperLeft().y - base.transform.position.y, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperLeft().z - base.transform.position.z);
						vector2 = new Vector3(vector.x + (float)GridWidth, vector.y, vector.z);
					}
					else
					{
						vector = ((array2[2].distance < array2[0].distance) ? new Vector3(array2[0].point.x - base.transform.position.x, vector.y + (array2[2].point.y - vector.y), array2[0].point.z - base.transform.position.z) : ((!(array2[0].distance < array2[2].distance)) ? new Vector3(array2[0].point.x - base.transform.position.x, vector.y + (array2[2].point.y - vector.y), array2[0].point.z - base.transform.position.z) : new Vector3(array2[0].point.x - base.transform.position.x, vector.y + (array2[0].point.y - vector.y), array2[0].point.z - base.transform.position.z)));
						vector2 = new Vector3(vector.x + (float)GridWidth, vector.y, vector.z);
						vector3 = new Vector3(vector.x, vector.y, vector.z + (float)GridDepth);
						vector4 = new Vector3(vector2.x, vector.y, vector2.z + (float)GridDepth);
					}
					if (!Physics.Raycast(new Ray(base.transform.position + vector, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, base.transform.right), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, -base.transform.right), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, (vector3 - vector).normalized), GridDepth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, (vector4 - vector2).normalized), GridDepth, ~IgnoreMask.value))
					{
						GridSquare newSquare = new GridSquare(this, i, j, 0f, array, vector, vector2, vector3, vector4);
						GridSquareQuadrant.addSquaretoQuad(newSquare, i, j);
					}
				}
				else if ((array3[0] >= (float)Math.PI / 180f * MinSlopeAngle && array3[0] <= (float)Math.PI / 180f * MaxSlopeAngle) || (array3[0] >= -(float)Math.PI / 180f * MaxSlopeAngle && array3[0] <= -(float)Math.PI / 180f * MinSlopeAngle))
				{
					float num = Mathf.Atan((array2[2].point.y - array2[0].point.y) / (array2[2].point.z - array2[0].point.z));
					float num2 = Mathf.Atan((array2[3].point.y - array2[1].point.y) / (array2[3].point.z - array2[1].point.z));
					if (Mathf.Abs(num - num2) <= AngularRigidity)
					{
						if (j - 1 >= 0 && GridSquareQuadrant.retrieveSpecificSquare(i, j - 1) != null)
						{
							vector = new Vector3(vector.x, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperLeft().y - base.transform.position.y, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperLeft().z - base.transform.position.z);
							vector2 = new Vector3(vector2.x, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperRight().y - base.transform.position.y, GridSquareQuadrant.retrieveSpecificSquare(i, j - 1).getGridSquareUpperRight().z - base.transform.position.z);
						}
						else
						{
							vector = new Vector3(array2[0].point.x - base.transform.position.x, array2[0].point.y - base.transform.position.y, array2[0].point.z - base.transform.position.z);
							vector2 = new Vector3(array2[1].point.x - base.transform.position.x, vector2.y + (array2[1].point.y - vector2.y), vector2.z);
						}
						vector3 = new Vector3(vector.x, vector3.y + (array2[2].point.y - vector3.y), array2[2].point.z - base.transform.position.z);
						vector4 = new Vector3(vector2.x, vector4.y + (array2[3].point.y - vector4.y), array2[3].point.z - base.transform.position.z);
						if (!Physics.Raycast(new Ray(base.transform.position + vector, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, base.transform.right), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, -base.transform.right), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, (vector3 - vector).normalized), GridDepth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector2, (vector4 - vector2).normalized), GridDepth, ~IgnoreMask.value))
						{
							GridSquare newSquare2 = new GridSquare(this, i, j, array3[0], array, vector, vector2, vector3, vector4);
							GridSquareQuadrant.addSquaretoQuad(newSquare2, i, j);
						}
					}
				}
				else
				{
					if ((!(array3[1] >= (float)Math.PI / 180f * MinSlopeAngle) || !(array3[1] <= (float)Math.PI / 180f * MaxSlopeAngle)) && (!(array3[1] >= -(float)Math.PI / 180f * MaxSlopeAngle) || !(array3[1] <= -(float)Math.PI / 180f * MinSlopeAngle)))
					{
						continue;
					}
					float num3 = Mathf.Atan((array2[1].point.y - array2[0].point.y) / (array2[1].point.x - array2[0].point.x));
					float num4 = Mathf.Atan((array2[3].point.y - array2[2].point.y) / (array2[3].point.x - array2[2].point.x));
					if (Mathf.Abs(num3 - num4) <= AngularRigidity)
					{
						if (i - 1 > 0 && GridSquareQuadrant.retrieveSpecificSquare(i - 1, j) != null)
						{
							vector = new Vector3(array2[0].point.x - base.transform.position.x, GridSquareQuadrant.retrieveSpecificSquare(i - 1, j).getGridSquareLowerRight().y - base.transform.position.y, GridSquareQuadrant.retrieveSpecificSquare(i - 1, j).getGridSquareLowerRight().z - base.transform.position.z);
							vector3 = new Vector3(array2[2].point.x - base.transform.position.x, GridSquareQuadrant.retrieveSpecificSquare(i - 1, j).getGridSquareUpperRight().y - base.transform.position.y, GridSquareQuadrant.retrieveSpecificSquare(i - 1, j).getGridSquareUpperRight().z - base.transform.position.z);
						}
						else
						{
							vector = new Vector3(array2[0].point.x - base.transform.position.x, array2[0].point.y - base.transform.position.y, array2[0].point.z - base.transform.position.z);
						}
						vector2 = new Vector3(array2[1].point.x - base.transform.position.x, vector2.y + (array2[1].point.y - vector2.y), vector2.z);
						vector4 = new Vector3(array2[3].point.x - base.transform.position.x, vector4.y + (array2[3].point.y - vector4.y), vector4.z);
						if (!Physics.Raycast(new Ray(base.transform.position + vector, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector3, base.transform.up), GridHeight, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, base.transform.forward), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector3, -base.transform.forward), GridWidth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector, (vector2 - vector).normalized), GridDepth, ~IgnoreMask.value) && !Physics.Raycast(new Ray(base.transform.position + vector3, (vector4 - vector3).normalized), GridDepth, ~IgnoreMask.value))
						{
							GridSquare newSquare3 = new GridSquare(this, i, j, array3[1], array, vector, vector2, vector3, vector4);
							GridSquareQuadrant.addSquaretoQuad(newSquare3, i, j);
						}
					}
				}
			}
		}
		for (int k = 0; k < Mathf.FloorToInt(Rows / GridQuadSize) + 1; k++)
		{
			for (int l = 0; l < Mathf.FloorToInt(Columns / GridQuadSize) + 1; l++)
			{
				if (gridQuadrants[k, l] != null)
				{
					gridQuadrants[k, l].determineAdjacentQuadrants(gridQuadrants);
				}
			}
		}
		for (int m = 0; m < Rows; m++)
		{
			for (int n = 0; n < Columns; n++)
			{
				if (GridSquareQuadrant.retrieveSpecificSquare(m, n) != null)
				{
					if (DetermineEmptyAdjacentCount(m, n) >= 7)
					{
						GridSquareQuadrant.removeSpecificSquare(m, n);
					}
					else
					{
						GridSquareQuadrant.retrieveSpecificSquare(m, n).calculateAdjacentSquares();
					}
				}
			}
		}
		pathFinder = new PathFinder(this);
	}

	private void Update()
	{
		pathFinder.CheckPathCalculation();
	}

	private int DetermineEmptyAdjacentCount(int row, int col)
	{
		int[] array = new int[3]
		{
			row - 1,
			row,
			row + 1
		};
		int[] array2 = new int[3]
		{
			col - 1,
			col,
			col + 1
		};
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				try
				{
					if (GridSquareQuadrant.retrieveSpecificSquare(array[i], array2[j]) == null)
					{
						num++;
					}
				}
				catch (IndexOutOfRangeException)
				{
					num++;
				}
			}
		}
		return num;
	}

	private int[] DetermineGridCheckPoints(Vector3 moveToPoint)
	{
		int[] array = new int[2] { -1, -1 };
		int num = (int)base.transform.position.x;
		int num2 = (int)base.transform.position.z;
		while ((float)num < moveToPoint.x)
		{
			if (moveToPoint.x > base.transform.position.x + (float)GridWidth)
			{
				num += GridWidth;
				array[0]++;
				continue;
			}
			array[0] = 0;
			break;
		}
		while ((float)num2 < moveToPoint.z)
		{
			if (moveToPoint.z > base.transform.position.z + (float)GridDepth)
			{
				num2 += GridDepth;
				array[1]++;
				continue;
			}
			array[1] = 0;
			break;
		}
		if (GridSquareQuadrant.retrieveSpecificSquare(array[0], array[1]) == null)
		{
			Debug.Log("Grid Coord: (" + array[0] + ", " + array[1] + ") Does Not Exist");
			array[0] = -1;
			array[1] = -1;
		}
		return array;
	}

	private int[] DetermineActiveGridQuad(int r, int c)
	{
		return new int[2]
		{
			Mathf.FloorToInt(r / GridQuadSize),
			Mathf.FloorToInt(c / GridQuadSize)
		};
	}

	private RaycastHit CollectedRaycastInfo(Ray ray, float dist, int layerMask)
	{
		RaycastHit hitInfo = default(RaycastHit);
		Physics.Raycast(ray, out hitInfo, dist, layerMask);
		return hitInfo;
	}

	public void CalculateNewPath(Vector3 init, Vector3 final)
	{
		int[] array = DetermineGridCheckPoints(init);
		int[] array2 = DetermineGridCheckPoints(final);
		if (array[0] != -1 && array[1] != -1 && array2[0] != -1 && array2[1] != -1)
		{
			pathFinder.CalculateNewPath(getDesiredGridSquare(array[0], array[1]), getDesiredGridSquare(array2[0], array2[1]));
		}
	}

	public void CalculateNewPath(Transform moveObj, Vector3 final)
	{
		int[] array = DetermineGridCheckPoints(moveObj.position);
		int[] array2 = DetermineGridCheckPoints(final);
		if (array[0] != -1 && array[1] != -1 && array2[0] != -1 && array2[1] != -1)
		{
			pathFinder.CalculateNewPath(moveObj, getDesiredGridSquare(array[0], array[1]), getDesiredGridSquare(array2[0], array2[1]));
		}
	}

	public GridSquare getDesiredGridSquare(int r, int c)
	{
		return GridSquareQuadrant.retrieveSpecificSquare(r, c);
	}

	public GridSquareQuadrant getDesiredQuadrant(int qr, int qc)
	{
		return gridQuadrants[qr, qc];
	}

	public List<GridSquare> getCalculatedPathTiles()
	{
		return pathFinder.getCalculatedTiles();
	}

	public void ResetGrid()
	{
		for (int i = 0; i < Mathf.FloorToInt(Rows / GridQuadSize) + 1; i++)
		{
			for (int j = 0; j < Mathf.FloorToInt(Columns / GridQuadSize) + 1; j++)
			{
				if (gridQuadrants[i, j] != null)
				{
					gridQuadrants[i, j].resetQuadrant();
				}
			}
		}
		GridSquareQuadrant.resetQuadrantSquares(Rows, Columns);
	}

	public int getRows()
	{
		return Rows;
	}

	public int getColumns()
	{
		return Columns;
	}
}
