using Game.ECS.GUI.Components;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class ProgressBarUIImplementor : MonoBehaviour, IProgressBarUIComponent
	{
		[SerializeField]
		private UISprite _progress;

		public float progress
		{
			get
			{
				return _progress.get_fillAmount();
			}
			set
			{
				_progress.set_fillAmount(value);
			}
		}

		public ProgressBarUIImplementor()
			: this()
		{
		}
	}
}
