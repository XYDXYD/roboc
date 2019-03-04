using UnityEngine;

public class TutorialStartDialogue : MonoBehaviour
{
	public UIButton Submit;

	public UIButton Cancel;

	public UIPanel InitialPanel;

	public UIPanel ExtraHintPanel;

	public TutorialStartDialogue()
		: this()
	{
	}

	public void ShowInitialPanel()
	{
		this.get_gameObject().SetActive(true);
		InitialPanel.get_gameObject().SetActive(true);
		ExtraHintPanel.get_gameObject().SetActive(false);
	}

	public void ShowTutorialExtraHintPanel()
	{
		this.get_gameObject().SetActive(true);
		InitialPanel.get_gameObject().SetActive(false);
		ExtraHintPanel.get_gameObject().SetActive(true);
	}

	public void Hide()
	{
		this.get_gameObject().SetActive(false);
	}
}
