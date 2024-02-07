namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/PPIViewer")]
	[RequireComponent(typeof(Text))]
	public class PPIViewer : MonoBehaviour
	{
		private Text label;

		private void Awake()
		{
			label = GetComponent<Text>();
		}

		private void Start()
		{
			if (label != null)
			{
				label.text = "PPI: " + Screen.dpi;
			}
		}
	}
}
