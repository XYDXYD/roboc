using UnityEngine;

public class VictoryLabelSetter : MonoBehaviour
{
	[SerializeField]
	private UILabel victoryLabel;

	public VictoryLabelSetter()
		: this()
	{
	}

	public void SetVictoryLabel(string text)
	{
		victoryLabel.set_text(text);
	}
}
