using Svelto.UI.Comms.SignalChain;
using System;
using System.Text;
using UnityEngine;

internal sealed class SettingsCountDownResetter : MonoBehaviour
{
	private const float COUNT_DOWN_TIME_SECS = 10f;

	public Transform listener;

	public UILabel label;

	private DateTime _endTime = DateTime.UtcNow;

	private TimeSpan _elapse = default(TimeSpan);

	private readonly StringBuilder _sb = new StringBuilder();

	public SettingsCountDownResetter()
		: this()
	{
	}

	private void OnEnable()
	{
		_endTime = DateTime.UtcNow.AddSeconds(10.0);
	}

	private void Update()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (_endTime > DateTime.UtcNow)
		{
			if (label != null)
			{
				_elapse = _endTime - DateTime.UtcNow;
				_sb.Length = 0;
				_sb.AppendFormat("{0} {1}", Math.Round(_elapse.TotalSeconds), StringTableBase<StringTable>.Instance.GetString("strSeconds"));
				label.set_text(_sb.ToString());
			}
			return;
		}
		if (listener != null)
		{
			new SignalChain(listener).Send<ButtonType>(ButtonType.Cancel);
		}
		if (label != null)
		{
			_sb.Length = 0;
			_sb.AppendFormat("0 {0}", StringTableBase<StringTable>.Instance.GetString("strSeconds"));
			label.set_text(_sb.ToString());
		}
	}
}
