using UnityEngine;

namespace Mothership.GUI.Inventory
{
	internal sealed class InventoryItemDetails : MonoBehaviour
	{
		[SerializeField]
		private ItemDetailsRow[] detailsRows = new ItemDetailsRow[10];

		private const string VALUE_FORMAT = "{0}{1}";

		private int _firstFreeRow;

		private GameObject[] _rows;

		public InventoryItemDetails()
			: this()
		{
		}

		public void ResetAllStats()
		{
			for (int i = 0; i < _rows.Length; i++)
			{
				_rows[i].SetActive(false);
			}
			_firstFreeRow = 0;
		}

		public void AddStat(string name, string value, string suffix = null)
		{
			if (_firstFreeRow >= _rows.Length)
			{
				RemoteLogger.Error("Error adding stat to item: no more rows avaible in stat table.", "StatName = " + name, null);
				return;
			}
			detailsRows[_firstFreeRow].statName.set_text(name);
			if (suffix != null)
			{
				string text = $"{value}{suffix}";
				detailsRows[_firstFreeRow].statValue.set_text(text);
			}
			else
			{
				detailsRows[_firstFreeRow].statValue.set_text(value);
			}
			if (!_rows[_firstFreeRow].get_activeSelf())
			{
				_rows[_firstFreeRow].SetActive(true);
			}
			_firstFreeRow++;
		}

		private void Awake()
		{
			_rows = (GameObject[])new GameObject[detailsRows.Length];
			for (int i = 0; i < _rows.Length; i++)
			{
				_rows[i] = detailsRows[i].statName.get_transform().get_parent().get_gameObject();
				_rows[i].SetActive(false);
			}
		}
	}
}
