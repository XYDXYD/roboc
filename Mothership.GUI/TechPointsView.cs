using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI
{
	internal class TechPointsView : MonoBehaviour, IChainRoot, IChainListener, IInitialize
	{
		[SerializeField]
		private GameObject _techTreeButton;

		[SerializeField]
		private UILabel _techPointInfoLabel;

		[SerializeField]
		private UILabel _techPointCountLabel;

		[Inject]
		internal TechPointsPresenter presenter
		{
			get;
			set;
		}

		public TechPointsView()
			: this()
		{
		}

		private void Awake()
		{
			this.get_gameObject().SetActive(false);
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Close:
					presenter.Close();
					break;
				case ButtonType.Confirm:
					presenter.Hide();
					break;
				case ButtonType.OpenTechTree:
					presenter.OpenTechTree();
					break;
				}
			}
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		internal void SetTechPointCount(string v)
		{
			_techPointCountLabel.set_text(v);
		}

		internal void SetTechPointInfo(string v)
		{
			_techPointInfoLabel.set_text(v);
		}
	}
}
