using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
	public Color32 topColor = Color.white;

	public Color32 bottomColor = Color.black;

	public override void ModifyMesh(VertexHelper helper)
	{
		if (!IsActive() || helper.currentVertCount == 0)
		{
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		helper.GetUIVertexStream(list);
		float num = list[0].position.y;
		float num2 = list[0].position.y;
		for (int i = 1; i < list.Count; i++)
		{
			float y = list[i].position.y;
			if (y > num2)
			{
				num2 = y;
			}
			else if (y < num)
			{
				num = y;
			}
		}
		float num3 = num2 - num;
		UIVertex vertex = default(UIVertex);
		for (int j = 0; j < helper.currentVertCount; j++)
		{
			helper.PopulateUIVertex(ref vertex, j);
			vertex.color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - num) / num3);
			helper.SetUIVertex(vertex, j);
		}
	}
}
