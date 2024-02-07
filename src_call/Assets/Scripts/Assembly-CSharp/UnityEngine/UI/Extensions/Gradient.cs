using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/Gradient")]
	public class Gradient : BaseMeshEffect
	{
		public GradientMode gradientMode;

		public GradientDir gradientDir;

		public bool overwriteAllColor;

		public Color vertex1 = Color.white;

		public Color vertex2 = Color.black;

		private Graphic targetGraphic;

		protected override void Start()
		{
			targetGraphic = GetComponent<Graphic>();
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			int currentVertCount = vh.currentVertCount;
			if (!IsActive() || currentVertCount == 0)
			{
				return;
			}
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			UIVertex vertex = default(UIVertex);
			if (gradientMode == GradientMode.Global)
			{
				if (gradientDir == GradientDir.DiagonalLeftToRight || gradientDir == GradientDir.DiagonalRightToLeft)
				{
					gradientDir = GradientDir.Vertical;
				}
				float num = ((gradientDir != 0) ? list[list.Count - 1].position.x : list[list.Count - 1].position.y);
				float num2 = ((gradientDir != 0) ? list[0].position.x : list[0].position.y);
				float num3 = num2 - num;
				for (int i = 0; i < currentVertCount; i++)
				{
					vh.PopulateUIVertex(ref vertex, i);
					if (overwriteAllColor || !(vertex.color != targetGraphic.color))
					{
						vertex.color *= Color.Lerp(vertex2, vertex1, (((gradientDir != 0) ? vertex.position.x : vertex.position.y) - num) / num3);
						vh.SetUIVertex(vertex, i);
					}
				}
				return;
			}
			for (int j = 0; j < currentVertCount; j++)
			{
				vh.PopulateUIVertex(ref vertex, j);
				if (overwriteAllColor || CompareCarefully(vertex.color, targetGraphic.color))
				{
					switch (gradientDir)
					{
					case GradientDir.Vertical:
						vertex.color *= ((j % 4 != 0 && (j - 1) % 4 != 0) ? vertex2 : vertex1);
						break;
					case GradientDir.Horizontal:
						vertex.color *= ((j % 4 != 0 && (j - 3) % 4 != 0) ? vertex2 : vertex1);
						break;
					case GradientDir.DiagonalLeftToRight:
						vertex.color *= ((j % 4 == 0) ? vertex1 : (((j - 2) % 4 != 0) ? Color.Lerp(vertex2, vertex1, 0.5f) : vertex2));
						break;
					case GradientDir.DiagonalRightToLeft:
						vertex.color *= (((j - 1) % 4 == 0) ? vertex1 : (((j - 3) % 4 != 0) ? Color.Lerp(vertex2, vertex1, 0.5f) : vertex2));
						break;
					}
					vh.SetUIVertex(vertex, j);
				}
			}
		}

		private bool CompareCarefully(Color col1, Color col2)
		{
			if (Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
			{
				return true;
			}
			return false;
		}
	}
}
