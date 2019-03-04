using System.Collections.Generic;
using UnityEngine;

namespace NGUIExtensions
{
	[ExecuteInEditMode]
	internal class UIStretchTable : MonoBehaviour
	{
		public enum Orientation
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private AnchorUpdate _updateMode;

		[SerializeField]
		private Orientation _orientation;

		private List<UIWidget> _items;

		public UIStretchTable()
			: this()
		{
		}

		private void Start()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)_updateMode == 2)
			{
				UpdateLayout();
			}
		}

		private void Update()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)_updateMode == 1)
			{
				UpdateLayout();
			}
		}

		private void OnEnable()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)_updateMode == 0)
			{
				UpdateLayout();
			}
		}

		[ContextMenu("Execute")]
		public void UpdateLayout()
		{
			UIWidget component = this.GetComponent<UIWidget>();
			if (component == null)
			{
				return;
			}
			if (_items == null)
			{
				_items = new List<UIWidget>();
			}
			else
			{
				_items.Clear();
			}
			Transform transform = this.get_transform();
			for (int i = 0; i < transform.GetChildCount(); i++)
			{
				Transform child = transform.GetChild(i);
				if (child.get_gameObject().get_activeSelf())
				{
					UIWidget component2 = child.GetComponent<UIWidget>();
					if (component2 != null)
					{
						_items.Add(component2);
					}
				}
			}
			if (_items.Count == 0)
			{
				return;
			}
			GetLen(component, out int len, out int otherLen);
			int num = len / _items.Count;
			float num2;
			float num3;
			if (_orientation == Orientation.Horizontal)
			{
				num2 = component.get_localCorners()[0].x;
				num3 = component.get_localCorners()[0].y;
			}
			else
			{
				num2 = component.get_localCorners()[1].x;
				num3 = component.get_localCorners()[1].y - (float)num;
			}
			for (int j = 0; j < _items.Count; j++)
			{
				UIWidget val = _items[j];
				if (_orientation == Orientation.Horizontal)
				{
					val.SetRect(num2, num3, (float)num, (float)otherLen);
					num2 += (float)num;
				}
				else
				{
					val.SetRect(num2, num3, (float)otherLen, (float)num);
					num3 -= (float)num;
				}
			}
			_items.Clear();
		}

		private void GetLen(UIWidget w, out int len, out int otherLen)
		{
			if (_orientation == Orientation.Horizontal)
			{
				len = w.get_width();
				otherLen = w.get_height();
			}
			else
			{
				len = w.get_height();
				otherLen = w.get_width();
			}
		}
	}
}
