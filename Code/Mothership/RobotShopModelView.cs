using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace Mothership
{
	internal sealed class RobotShopModelView : MonoBehaviour, IInitialize, IChainListener
	{
		public GameObject communityPanel;

		public UILabel communityName;

		public UILabel communityDescription;

		public UILabel communityAddedBy;

		public UIButton getButton;

		public UIButton communityReportButton;

		public UILabel communityRobotCpu;

		public RobotShopRating communityStyleRating;

		public RobotShopRating communityCombatRating;

		public UILabel buyCount;

		public UILabel tierLabel;

		public GameObject communityDesignInfos;

		public GameObject communityRemoveButton;

		public GameObject communityRemoveShopPopup;

		public GameObject communityReportPopup;

		public UILabel communityReportReason;

		public UIEventListener communityReportListener;

		public UILabel communitySubmissionExpiryTimer;

		public UIButton communityConfirmReportButton;

		public UIButton communityPreviousRobotButton;

		public UIButton communityNextRobotButton;

		public GameObject FeaturedRosette;

		public GameObject StandardBanner;

		public UILabel StandardBannerLabel;

		public UIButton DevOnlySetFeatured;

		public UIButton DevOnlyHideFeatured;

		public UIButton DevOnlyRestoreFeatured;

		public static readonly string COST_NUMBER_FORMAT = "N0";

		public static readonly CultureInfo COST_CULTURE_INFO = CultureInfo.InvariantCulture;

		private ITaskRoutine _updateRemainingTimeTask;

		private bool _taskRunning;

		private bool _robotExpired;

		private DateTime _submissionExpiryDate;

		private UIInput _reportAbuseUIInput;

		private string _reportAbuseString;

		[Inject]
		internal RobotShopController shopController
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopCommunityController communityController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public event Action<bool> OnCloseRequestedEvent;

		public event Action OnDevOnlySetFeaturedRequestedEvent;

		public event Action OnDevOnlyHideFeaturedRequestedEvent;

		public event Action OnDevOnlyRestoreFeaturedRequestedEvent;

		public event Action OnRemovePlayerShopSubmissionRequestedEvent;

		public event Action<string> OnReportAbuseRequestedEvent;

		public event Action OnPreviousRobotRequestedEvent;

		public event Action OnNextRobotRequestedEvent;

		public event Action OnCopyRobotRequestedEvent;

		public RobotShopModelView()
			: this()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			shopController.SetupModelView(this);
			communityController.SetupModelView(this);
			communityPreviousRobotButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			communityNextRobotButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private unsafe void Start()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			_reportAbuseUIInput = communityReportListener.GetComponent<UIInput>();
			_reportAbuseUIInput.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_reportAbuseUIInput.onSubmit.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
			communityReportPopup.SetActive(false);
			communityRemoveShopPopup.SetActive(false);
			if (_updateRemainingTimeTask != null)
			{
				_updateRemainingTimeTask.Pause();
				_taskRunning = false;
			}
		}

		internal void ShowItem(CRFItem item, bool hasMultipleRobots)
		{
			_robotExpired = false;
			communityPanel.SetActive(true);
			communityName.set_text(item.robotShopItem.name);
			communityDescription.set_text(item.robotShopItem.description.Replace('&', '\n'));
			communityAddedBy.set_text(item.robotShopItem.addedByDisplayName);
			tierLabel.set_text(item.TierStr);
			buyCount.set_text(GetSalesCountString(item.robotShopItem.rentCount));
			if (item.isMyRobot)
			{
				communityReportButton.get_gameObject().SetActive(false);
			}
			else
			{
				communityReportButton.get_gameObject().SetActive(!item.robotShopItem.featured);
			}
			getButton.get_gameObject().SetActive(!item.isMyRobot);
			bool active = item.isMyRobot && !item.robotShopItem.featured;
			communityRemoveButton.SetActive(active);
			communityRobotCpu.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strCRFPflops"), item.robotCPUToPlayer));
			communityStyleRating.Set(item.robotShopItem.styleRating);
			communityCombatRating.Set(item.robotShopItem.combatRating);
			UIButton obj = communityPreviousRobotButton;
			bool isEnabled = hasMultipleRobots;
			communityNextRobotButton.set_isEnabled(isEnabled);
			obj.set_isEnabled(isEnabled);
			FeaturedRosette.SetActive(item.robotShopItem.featured);
			if (item.robotShopItem.bannerMessage != string.Empty)
			{
				StandardBannerLabel.set_text(StringTableBase<StringTable>.Instance.GetString(item.robotShopItem.bannerMessage));
			}
			StandardBanner.SetActive(!string.IsNullOrEmpty(item.robotShopItem.bannerMessage));
			if (item.isMyRobot && !item.robotShopItem.featured)
			{
				_submissionExpiryDate = item.robotShopItem.submissionExpiryDate;
				if (_updateRemainingTimeTask == null)
				{
					_updateRemainingTimeTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)UpdateRemainingTime);
					_updateRemainingTimeTask.Start((Action<PausableTaskException>)null, (Action)null);
					_taskRunning = true;
				}
				if (!_taskRunning)
				{
					_updateRemainingTimeTask.Resume();
				}
			}
			else if (_updateRemainingTimeTask != null)
			{
				_updateRemainingTimeTask.Pause();
				_taskRunning = false;
			}
			if (shopController.IsDeveloper())
			{
				if (item.robotShopItem.featured)
				{
					DevOnlySetFeatured.get_gameObject().SetActive(false);
					DevOnlyHideFeatured.get_gameObject().SetActive(item.robotShopItem.buyable);
					DevOnlyRestoreFeatured.get_gameObject().SetActive(!item.robotShopItem.buyable);
				}
				else
				{
					DevOnlySetFeatured.get_gameObject().SetActive(true);
					DevOnlyHideFeatured.get_gameObject().SetActive(false);
					DevOnlyRestoreFeatured.get_gameObject().SetActive(false);
				}
			}
			else
			{
				DevOnlySetFeatured.get_gameObject().SetActive(false);
				DevOnlyHideFeatured.get_gameObject().SetActive(false);
				DevOnlyRestoreFeatured.get_gameObject().SetActive(false);
			}
		}

		private IEnumerator UpdateRemainingTime()
		{
			while (this.get_gameObject().get_activeInHierarchy())
			{
				TimeSpan duration = _submissionExpiryDate - DateTime.UtcNow;
				if (duration.TotalMilliseconds > 0.0)
				{
					communitySubmissionExpiryTimer.set_text(RobotShopCommunityItemView.FormatRemainingTime(duration));
					yield return (object)new WaitForSecondsEnumerator(1f);
				}
				else if (!_robotExpired)
				{
					_robotExpired = true;
					this.OnCloseRequestedEvent(obj: false);
				}
			}
		}

		public void Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			switch ((ButtonType)message)
			{
			case ButtonType.Close:
				if (this.OnCloseRequestedEvent != null)
				{
					this.OnCloseRequestedEvent(obj: false);
				}
				break;
			case ButtonType.Cancel:
				communityReportPopup.SetActive(false);
				communityRemoveShopPopup.SetActive(false);
				guiInputController.SetShortCutMode(ShortCutMode.OnlyGUINoSwitching);
				break;
			case ButtonType.RemovePlayerShopSubmission:
				guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				communityRemoveShopPopup.SetActive(true);
				break;
			case ButtonType.DevOnlyCRFSetFeatured:
				if (this.OnDevOnlySetFeaturedRequestedEvent != null)
				{
					this.OnDevOnlySetFeaturedRequestedEvent();
				}
				break;
			case ButtonType.DevOnlyCRFHideFeatured:
				if (this.OnDevOnlyHideFeaturedRequestedEvent != null)
				{
					this.OnDevOnlyHideFeaturedRequestedEvent();
				}
				break;
			case ButtonType.DevOnlyCRFRestoreFeatured:
				if (this.OnDevOnlyRestoreFeaturedRequestedEvent != null)
				{
					this.OnDevOnlyRestoreFeaturedRequestedEvent();
				}
				break;
			case ButtonType.Confirm:
				communityRemoveShopPopup.SetActive(false);
				if (this.OnRemovePlayerShopSubmissionRequestedEvent != null)
				{
					this.OnRemovePlayerShopSubmissionRequestedEvent();
				}
				guiInputController.SetShortCutMode(ShortCutMode.OnlyGUINoSwitching);
				break;
			case ButtonType.StartReportAbuse:
				communityReportReason.set_text(StringTableBase<StringTable>.Instance.GetString("strReportRobotInfo"));
				_reportAbuseString = string.Empty;
				_reportAbuseUIInput.set_value(string.Empty);
				guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				communityConfirmReportButton.set_isEnabled(false);
				communityReportPopup.SetActive(true);
				break;
			case ButtonType.ConfirmReportAbuse:
				if (this.OnReportAbuseRequestedEvent != null)
				{
					this.OnReportAbuseRequestedEvent(_reportAbuseString);
				}
				guiInputController.SetShortCutMode(ShortCutMode.OnlyGUINoSwitching);
				communityReportPopup.SetActive(false);
				break;
			case ButtonType.GetRobot:
				this.OnCopyRobotRequestedEvent();
				break;
			}
		}

		private string GetSalesCountString(int count)
		{
			if (count < 1000)
			{
				return $"{count:n0}";
			}
			if ((float)count < 1000000f)
			{
				float num = Mathf.Floor((float)count / 1000f);
				return $"{num:n0}k+";
			}
			float num2 = Mathf.Floor((float)count / 1000000f);
			return $"{num2:n0}m+";
		}

		private void OnPreviousRobotRequested()
		{
			if (this.OnPreviousRobotRequestedEvent != null)
			{
				this.OnPreviousRobotRequestedEvent();
			}
		}

		private void OnNextRobotRequested()
		{
			if (this.OnNextRobotRequestedEvent != null)
			{
				this.OnNextRobotRequestedEvent();
			}
		}

		private void StoreReportAbuseText()
		{
			_reportAbuseString = _reportAbuseUIInput.get_value();
			communityConfirmReportButton.set_isEnabled(_reportAbuseString != null && _reportAbuseString.Length != 0);
		}

		private void RemoveReportAbuseFocus()
		{
			_reportAbuseUIInput.RemoveFocus();
		}
	}
}
