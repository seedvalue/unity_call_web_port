using UnityEngine;

internal class ScrollBarEssentials
{
	protected GUIStyle style = new GUIStyle();

	protected Vector2 string_size;

	protected Vector2 pivotVector = Vector2.zero;

	protected bool MouseInRect;

	protected int current_value;

	protected int max_value;

	protected Rect ScrollBarDimens = default(Rect);

	protected Rect ScrollBarTextureDimens;

	protected bool VerticleBar;

	protected float texture_rotation;

	protected Texture ScrollBarBubbleTexture;

	protected Texture ScrollTexture;

	public ScrollBarEssentials(Rect sb_dimen, bool vbar, Texture sb_bt, Texture st, float rot)
	{
		ScrollBarDimens = sb_dimen;
		VerticleBar = vbar;
		ScrollBarBubbleTexture = sb_bt;
		ScrollTexture = st;
		texture_rotation = rot;
		pivotVector.x = ScrollBarDimens.x + ScrollBarDimens.width / 2f;
		pivotVector.y = ScrollBarDimens.y + ScrollBarDimens.height / 2f;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
	}

	public ScrollBarEssentials(Rect sb_dimen, Rect sbv_dimen, bool vbar, Texture sb_bt, Texture st, float rot)
	{
		ScrollBarDimens = sb_dimen;
		ScrollBarTextureDimens = sbv_dimen;
		VerticleBar = vbar;
		ScrollBarBubbleTexture = sb_bt;
		ScrollTexture = st;
		texture_rotation = rot;
		pivotVector.x = ScrollBarDimens.x + ScrollBarDimens.width / 2f;
		pivotVector.y = ScrollBarDimens.y + ScrollBarDimens.height / 2f;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
	}

	protected virtual int DetermineMaxVal(int value)
	{
		return value;
	}

	protected void ProcessValue(int value)
	{
		current_value += value;
	}

	public virtual void DrawBar()
	{
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(texture_rotation, pivotVector);
		if (!VerticleBar)
		{
			if (ScrollBarTextureDimens.width != 0f && ScrollBarTextureDimens.height != 0f)
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x + ScrollBarTextureDimens.x, ScrollBarDimens.y + ScrollBarTextureDimens.y, (float)current_value * (ScrollBarTextureDimens.width / (float)max_value), ScrollBarTextureDimens.height), ScrollTexture);
			}
			else
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x, ScrollBarDimens.y, (float)current_value * (ScrollBarDimens.width / (float)max_value), ScrollBarBubbleTexture.height), ScrollTexture);
			}
			for (int i = 0; (float)i < ScrollBarDimens.width / (float)ScrollBarBubbleTexture.width; i++)
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x + (float)(i * ScrollBarBubbleTexture.width), ScrollBarDimens.y, ScrollBarBubbleTexture.width, ScrollBarBubbleTexture.height), ScrollBarBubbleTexture);
			}
		}
		else
		{
			if (ScrollBarTextureDimens.width != 0f && ScrollBarTextureDimens.height != 0f)
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x + ScrollBarTextureDimens.x, ScrollBarDimens.y + ScrollBarTextureDimens.y + ScrollBarTextureDimens.height, ScrollBarTextureDimens.width, (float)(-current_value) * (ScrollBarTextureDimens.height / (float)max_value)), ScrollTexture);
			}
			else
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x, ScrollBarDimens.y + ScrollBarDimens.height, ScrollBarBubbleTexture.width, (float)(-current_value) * (ScrollBarDimens.height / (float)max_value)), ScrollTexture);
			}
			for (int j = 0; (float)j < ScrollBarDimens.height / (float)ScrollBarBubbleTexture.height; j++)
			{
				GUI.DrawTexture(new Rect(ScrollBarDimens.x, ScrollBarDimens.y + (float)(j * ScrollBarBubbleTexture.height), ScrollBarBubbleTexture.width, ScrollBarBubbleTexture.height), ScrollBarBubbleTexture);
			}
		}
		if (ScrollBarDimens.Contains(Event.current.mousePosition))
		{
			MouseInRect = true;
		}
		else
		{
			MouseInRect = false;
		}
		if (MouseInRect)
		{
			GUIUtility.RotateAroundPivot(0f - texture_rotation, pivotVector);
			string_size = style.CalcSize(new GUIContent(current_value + " / " + max_value));
			GUI.Label(new Rect(ScrollBarDimens.x + ScrollBarDimens.width / 2f - string_size.x / 2f, ScrollBarDimens.y + ScrollBarDimens.height / 2f - string_size.y / 2f, string_size.x, string_size.y + string_size.y / 2f), current_value + " / " + max_value, style);
		}
		GUI.matrix = matrix;
	}

	public virtual void IncrimentBar(int value)
	{
		ProcessValue(value);
	}

	public int getCurrentValue()
	{
		return current_value;
	}

	public int getMaxValue(int value)
	{
		return DetermineMaxVal(value);
	}

	public Rect getScrollBarRect()
	{
		return ScrollBarDimens;
	}

	public Rect getScrollBarTextureDimens()
	{
		return ScrollBarTextureDimens;
	}
}
