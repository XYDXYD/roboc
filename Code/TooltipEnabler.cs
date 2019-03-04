using UnityEngine;

public class TooltipEnabler : MonoBehaviour
{
	[SerializeField]
	private GameObject tooltip;

	public TooltipEnabler()
		: this()
	{
	}

	private void OnHover(bool isOver)
	{
		tooltip.SetActive(isOver);
	}
}
