using UnityEngine;

public class FormationSwitches
{
	public int FormationCount = 9;

	private Texture2D baseFormations;

	private Texture2D[] inactiveBaseFormations;

	private Texture2D[] activeBaseFormations;

	public void Init()
	{
		inactiveBaseFormations = new Texture2D[FormationCount];
		activeBaseFormations = new Texture2D[FormationCount];
		baseFormations = (Texture2D)Resources.Load("gfx/Formations_296x32");
		activeBaseFormations = new Texture2D[FormationCount];
		for (int i = 0; i < FormationCount; i++)
		{
			Color[] pixels = baseFormations.GetPixels(32 * i + i, 32, 32, 32);
			inactiveBaseFormations[i] = new Texture2D(32, 32, TextureFormat.ARGB32, true);
			inactiveBaseFormations[i].SetPixels(pixels);
			inactiveBaseFormations[i].Apply();
			pixels = baseFormations.GetPixels(32 * i + i, 0, 32, 32);
			activeBaseFormations[i] = new Texture2D(32, 32, TextureFormat.ARGB32, true);
			activeBaseFormations[i].SetPixels(pixels);
			activeBaseFormations[i].Apply();
		}
	}

	public void OnGUI()
	{
		for (int i = 0; i < FormationCount; i++)
		{
			GUI.DrawTexture(new Rect(Mathf.Floor(baseFormations.width / FormationCount) * (float)i + (float)i, Screen.height - baseFormations.height / 2, 32f, 32f), inactiveBaseFormations[i]);
			GUI.DrawTexture(new Rect(Mathf.Floor(baseFormations.width / FormationCount) * (float)i + (float)i, Screen.height - baseFormations.height, 32f, 32f), activeBaseFormations[i]);
		}
	}
}
