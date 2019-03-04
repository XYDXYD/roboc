using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class SpectatorHintView : MonoBehaviour, IInitialize
	{
		public UITexture hintTexture;

		public UILabel hintLabel;

		[Inject]
		internal SpectatorHintPresenter presenter
		{
			get;
			set;
		}

		public SpectatorHintView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.RegisterView(this);
		}

		internal void SetCurrentHint(HintData hint)
		{
			hintTexture.set_mainTexture(hint.texture);
			hintLabel.set_text(StringTableBase<StringTable>.Instance.GetString(hint.text));
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
			if (presenter != null)
			{
				presenter.UpdateHintTexture();
			}
		}
	}
}
