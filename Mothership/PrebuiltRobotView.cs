using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class PrebuiltRobotView : MonoBehaviour, IInitialize, IChainListener, IChainRoot
	{
		[SerializeField]
		private UIWidget _uiWidget;

		[SerializeField]
		private PrebuiltRobotOptionView _classContainer;

		[SerializeField]
		private PrebuiltRobotOptionView[] _categoriesContainer;

		[SerializeField]
		private UISprite[] _colourSprites;

		[Inject]
		private PrebuiltRobotPresenter presenter
		{
			get;
			set;
		}

		public PrebuiltRobotOptionView classContainer => _classContainer;

		public PrebuiltRobotOptionView[] categoriesContainer => _categoriesContainer;

		public PrebuiltRobotView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Show(bool enable)
		{
			this.get_gameObject().SetActive(enable);
			if (enable)
			{
				HalfScreenGUIHelper.Show(GUIPosition.Left, _uiWidget);
			}
			else
			{
				HalfScreenGUIHelper.Hide();
			}
		}

		public void Listen(object message)
		{
			if (message is PrebuiltRobotOption)
			{
				PrebuiltRobotOption selectedOption = (PrebuiltRobotOption)message;
				presenter.OptionChanged(selectedOption);
			}
			else if (message is ButtonType)
			{
				presenter.ButtonClicked((ButtonType)message);
			}
		}

		public void ShowColors(PaletteColor[] colors)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < colors.Length; i++)
			{
				_colourSprites[i].set_color(Color32.op_Implicit(colors[i].diffuse));
			}
		}
	}
}
