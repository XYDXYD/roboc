using System;
using UnityEngine;

namespace Mothership
{
	[RequireComponent(typeof(UITable))]
	public class UITableLabelsReposition : MonoBehaviour
	{
		private UITable _table;

		private UILabel[] _labels;

		public UITableLabelsReposition()
			: this()
		{
		}

		private void Awake()
		{
			_table = this.GetComponent<UITable>();
			_labels = this.GetComponentsInChildren<UILabel>(true);
		}

		private unsafe void OnEnable()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			for (int i = 0; i < _labels.Length; i++)
			{
				UILabel obj = _labels[i];
				obj.onChange = Delegate.Combine((Delegate)obj.onChange, (Delegate)new OnDimensionsChanged((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private unsafe void OnDisable()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			for (int i = 0; i < _labels.Length; i++)
			{
				UILabel obj = _labels[i];
				obj.onChange = Delegate.Remove((Delegate)obj.onChange, (Delegate)new OnDimensionsChanged((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void OnLabelChanged()
		{
			_table.set_repositionNow(true);
		}
	}
}
