using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	public class TutorialButton : MonoBehaviour
	{
		[Inject]
		internal TutorialButtonPresenter tutorialButtonPresenter
		{
			get;
			set;
		}

		public event Action<string> OnButtonPressed = delegate
		{
		};

		public TutorialButton()
			: this()
		{
		}

		private void OnClick()
		{
			this.OnButtonPressed("Button_Tutorial");
			tutorialButtonPresenter.ButtonPressed();
		}
	}
}
