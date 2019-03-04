using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class FloatingNumber : MonoBehaviour
	{
		[SerializeField]
		private UILabel _normalLabel;

		[SerializeField]
		private UILabel _criticaLabel;

		[SerializeField]
		private UITexture _background;

		public GameObject numberGameobject
		{
			get;
			private set;
		}

		public Transform numberTransform
		{
			get;
			private set;
		}

		public Animation labelAnimation
		{
			get;
			private set;
		}

		public string text
		{
			set
			{
				_normalLabel.set_text(value);
				_criticaLabel.set_text(value);
			}
		}

		public Color textColor
		{
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				_normalLabel.set_color(value);
				_background.set_color(value);
			}
		}

		public FloatingNumber()
			: this()
		{
		}

		private void Awake()
		{
			numberGameobject = this.get_gameObject();
			numberTransform = this.get_transform();
			labelAnimation = this.GetComponent<Animation>();
		}
	}
}
