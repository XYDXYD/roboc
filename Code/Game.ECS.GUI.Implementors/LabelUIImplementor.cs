using Game.ECS.GUI.Components;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class LabelUIImplementor : MonoBehaviour, ILabelUIComponent
	{
		[SerializeField]
		private UILabel _label;

		public string label
		{
			get
			{
				return _label.get_text();
			}
			set
			{
				_label.set_text(value);
			}
		}

		public LabelUIImplementor()
			: this()
		{
		}
	}
}
