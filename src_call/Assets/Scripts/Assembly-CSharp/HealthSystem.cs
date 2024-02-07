using UnityEngine;

internal class HealthSystem : ScrollBarEssentials
{
	public HealthSystem(Rect sb_dimen, bool vbar, Texture sb_bt, Texture st, float rot)
		: base(sb_dimen, vbar, sb_bt, st, rot)
	{
	}

	public void Initialize()
	{
		current_value = 200;
		max_value = 200;
	}

	public void Update(int x, int y)
	{
		if (current_value < 0)
		{
			current_value = 0;
		}
		else if (current_value >= max_value)
		{
			current_value = max_value;
		}
		ScrollBarDimens = new Rect((float)x - ScrollBarDimens.width / 2f, (float)y - ScrollBarDimens.height, ScrollBarDimens.width, ScrollBarDimens.height);
		pivotVector.x = ScrollBarDimens.x + ScrollBarDimens.width / 2f;
		pivotVector.y = ScrollBarDimens.y + ScrollBarDimens.height / 2f;
	}

	public override void IncrimentBar(int value)
	{
		ProcessValue(value);
	}
}
