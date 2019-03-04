using DG.Tweening;
using UnityEngine;

internal sealed class CubeSelectorCategoryButton : MonoBehaviour
{
	public GameObject categoryIndicator;

	private static Tweener currentTween;

	public CubeSelectorCategoryButton()
		: this()
	{
	}

	public void OnHover(bool isOver)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (isOver)
		{
			if (currentTween != null && !TweenExtensions.IsComplete(currentTween))
			{
				TweenExtensions.Complete(currentTween);
			}
			Vector3 val = default(Vector3);
			val = categoryIndicator.get_transform().get_position();
			Vector3 position = this.get_transform().get_position();
			val.y = position.y;
			currentTween = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(categoryIndicator.get_transform(), val, 0.3f, false), 3);
		}
	}
}
