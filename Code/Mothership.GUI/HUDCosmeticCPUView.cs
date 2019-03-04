using Svelto.IoC;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDCosmeticCPUView : MonoBehaviour, IInitialize
	{
		public UILabel _valueLabel;

		private StringBuilder _stringBuilder = new StringBuilder();

		[Inject]
		private HUDCosmeticCPUPresenter _presenter
		{
			get;
			set;
		}

		public HUDCosmeticCPUView()
			: this()
		{
		}

		public void SetValue(uint CCPU, uint maxCCPU)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append(CCPU.ToString("N0", CultureInfo.InvariantCulture));
			_stringBuilder.Append(" / ");
			_stringBuilder.Append(maxCCPU.ToString("N0", CultureInfo.InvariantCulture));
			_valueLabel.set_text(_stringBuilder.ToString());
		}

		public void OnDependenciesInjected()
		{
			_presenter.SetView(this);
		}
	}
}
