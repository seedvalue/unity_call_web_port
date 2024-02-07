using UnityEngine;

public class terroristMaterial : MonoBehaviour
{
	public Renderer[] AllMat;

	public void MaterialChange(Material mat)
	{
		Renderer[] allMat = AllMat;
		foreach (Renderer renderer in allMat)
		{
			renderer.material = mat;
		}
	}
}
