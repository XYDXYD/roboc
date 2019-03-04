using Mothership.GUI;
using UnityEngine;

internal class TierHighlightWidget : MonoBehaviour, ITierComponent
{
	[SerializeField]
	private UILabel[] tierLabels;

	[SerializeField]
	private Color fontColorHighlighted = new Color(1f, 1f, 1f);

	[SerializeField]
	private Color fontColorNotHighlighted = new Color(0.294f, 0.294f, 0.294f);

	[SerializeField]
	private int fontSizeHighlighted = 26;

	[SerializeField]
	private int fontSizeNotHighlighted = 20;

	private int _tierIndex;

	public int tier
	{
		get
		{
			return _tierIndex;
		}
		set
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			_tierIndex = value;
			for (int i = 0; i < tierLabels.Length; i++)
			{
				UILabel val = tierLabels[i];
				if (i != _tierIndex)
				{
					val.set_color(fontColorNotHighlighted);
					val.set_fontSize(fontSizeNotHighlighted);
				}
				else
				{
					val.set_color(fontColorHighlighted);
					val.set_fontSize(fontSizeHighlighted);
				}
			}
		}
	}

	public TierHighlightWidget()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_002a: Unknown result type (might be due to invalid IL or missing references)
	//IL_002f: Unknown result type (might be due to invalid IL or missing references)


	private void Start()
	{
	}
}
