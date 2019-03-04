using Svelto.IoC;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDMassView : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private UILabel _valueLabel;

		private StringBuilder _stringBuilder = new StringBuilder();

		[Inject]
		private HUDMassPresenter _presenter
		{
			get;
			set;
		}

		public HUDMassView()
			: this()
		{
		}

		public void SetValue(float mass)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append(mass.ToString("N0", CultureInfo.InvariantCulture));
			_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strKilograms"));
			_valueLabel.set_text(_stringBuilder.ToString());
		}

		public void OnDependenciesInjected()
		{
			_presenter.SetView(this);
		}
	}
}
