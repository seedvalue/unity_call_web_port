using UnityEngine;

public class SelectionToggle : MonoBehaviour
{
	private GameObject main_selection_toggle;

	private GameObject selection_toggle;

	private bool new_toggle_created;

	private float time_counter;

	private void Start()
	{
		main_selection_toggle = (GameObject)Resources.Load("SelectionToggle");
	}

	private void Update()
	{
		if (!new_toggle_created)
		{
			return;
		}
		time_counter += Time.deltaTime;
		if ((double)time_counter >= 0.06)
		{
			time_counter = 0f;
			Light component = selection_toggle.transform.Find("toggle_light").GetComponent<Light>();
			component.intensity -= 1f;
			if (component.intensity <= 0f)
			{
				new_toggle_created = false;
			}
		}
	}

	public void createSelectionToggle(Vector3 position, Quaternion rotation)
	{
		if (selection_toggle != null)
		{
			Object.DestroyImmediate(selection_toggle);
		}
		selection_toggle = Object.Instantiate(main_selection_toggle, position, rotation);
		new_toggle_created = true;
		if (time_counter != 0f)
		{
			time_counter = 0f;
		}
	}
}
