using UnityEngine;
using Utility;

internal sealed class MultiplayerLoadingScreen : GenericLoadingScreen
{
	[SerializeField]
	private GameObject[] scenePanels;

	[SerializeField]
	private GameObject[] gameModeHintPanels;

	[SerializeField]
	private GameObject[] panelsNotToShowInCampaignMode;

	public void SetSceneName(string sceneName)
	{
		bool flag = false;
		for (int i = 0; i < scenePanels.Length; i++)
		{
			scenePanels[i].SetActive(scenePanels[i].get_name() == sceneName);
			if (scenePanels[i].get_name() == sceneName)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			Console.LogWarning("Loading scene not found for " + sceneName);
		}
	}

	public void SetGameModeHints(GameModeType gameModeType)
	{
		string b = gameModeType.ToString();
		for (int i = 0; i < gameModeHintPanels.Length; i++)
		{
			string name = gameModeHintPanels[i].get_name();
			gameModeHintPanels[i].SetActive(name == b);
		}
	}

	public void CustomHidePanels(GameModeType gameModeType)
	{
		bool flag = gameModeType == GameModeType.Campaign;
		for (int i = 0; i < panelsNotToShowInCampaignMode.Length; i++)
		{
			panelsNotToShowInCampaignMode[i].SetActive(!flag);
		}
	}
}
