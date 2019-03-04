using Fabric;
using Mothership.GUI;
using SinglePlayerCampaign.GUI.Mothership;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class EnterPlanetDialogue : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private BattleNormalModeButton normalModeButton;

		[SerializeField]
		private TutorialButton tutorialButton;

		[SerializeField]
		private EnterPlanetSinglePlayerButton enterPlanetSinglePlayerButton;

		[SerializeField]
		private CustomGameModeButton customGameButton;

		[SerializeField]
		private GameObject categoriesMainMenu;

		[SerializeField]
		private GameObject trainingSubmenu;

		[SerializeField]
		private GameObject customSubmenu;

		[SerializeField]
		private GameModePreferencesScreen _gameModePreferencesMenu;

		[SerializeField]
		private PlayMenuTierDisplayer _tierDisplayer;

		[SerializeField]
		private SinglePlayerCampaignListScreen _singlePlayerCampaignListScreen;

		[SerializeField]
		private UIButton[] backButts;

		[SerializeField]
		private GameObject earlyAccess;

		private const string ACCESSED_BATTLE_BUTTON_KEY = "AccessedBattleButton";

		private bool _isActive;

		[Inject]
		internal EnterPlanetDialogueController controller
		{
			private get;
			set;
		}

		public GameModePreferencesScreen gameModePreferencesMenu => _gameModePreferencesMenu;

		public PlayMenuTierDisplayer tierDisplayer => _tierDisplayer;

		public SinglePlayerCampaignListScreen singlePlayerCampaignListScreen => _singlePlayerCampaignListScreen;

		public EnterPlanetDialogue()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			controller.SetView(this);
		}

		private void Start()
		{
			earlyAccess.SetActive(false);
			Hide();
		}

		public void Show()
		{
			_isActive = true;
			this.get_gameObject().SetActive(true);
			PlayerPrefs.SetInt("AccessedBattleButton", 1);
			EventManager.get_Instance().PostEvent("GUI_Hexagons", 0);
			HalfScreenGUIHelper.ExactlyHalfScreenCameraOnRHS();
		}

		public void Hide()
		{
			_isActive = false;
			HalfScreenGUIHelper.Hide();
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void AddBackButtonEvent(Callback callback)
		{
			for (int i = 0; i < backButts.Length; i++)
			{
				UIButton val = backButts[i];
				EventDelegate.Add(val.onClick, callback);
			}
		}

		public void SetNormalModeAvailability(GameModeAvailabilityState state)
		{
			normalModeButton.SetButtonState(state);
		}

		public void SetCustomGamesAvailability(GameModeAvailabilityState state)
		{
			customGameButton.SetButtonState(state);
		}

		public void ChangeCategory(EnterPlanetCategory newCategory)
		{
			categoriesMainMenu.SetActive(newCategory == EnterPlanetCategory.None);
			trainingSubmenu.SetActive(false);
			customSubmenu.SetActive(false);
			gameModePreferencesMenu.isShown.set_value(false);
			singlePlayerCampaignListScreen.show.set_value(false);
			switch (newCategory)
			{
			case EnterPlanetCategory.Training:
				trainingSubmenu.SetActive(true);
				break;
			case EnterPlanetCategory.Custom:
				customSubmenu.SetActive(true);
				break;
			case EnterPlanetCategory.GameModePreferences:
				gameModePreferencesMenu.isShown.set_value(true);
				break;
			case EnterPlanetCategory.SinglePlayerCampaignList:
				singlePlayerCampaignListScreen.show.set_value(true);
				break;
			}
		}

		void IChainListener.Listen(object message)
		{
			controller.Listen(message);
		}
	}
}
