using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class RobotLockedDialog : MonoBehaviour
	{
		[SerializeField]
		private GameObject okButton;

		[SerializeField]
		private UILabel bodyLabel;

		[SerializeField]
		private UILabel titleLabel;

		[SerializeField]
		private RobotShopInventoryItem robotShopInventoryItemTemplate;

		private GameObject _itemHolder;

		private UIScrollView _uiScrollView;

		public RobotLockedDialog()
			: this()
		{
		}

		internal void Show(string titleStr, string bodyStr, Dictionary<CubeTypeID, SunkCube> lockedCubes)
		{
			bodyLabel.set_text(bodyStr);
			titleLabel.set_text(titleStr);
			CreateItemsList(lockedCubes);
			if (_uiScrollView == null)
			{
				_uiScrollView = this.GetComponentInChildren<UIScrollView>();
			}
			_uiScrollView.verticalScrollBar.Set(0f, false);
			this.get_gameObject().SetActive(true);
		}

		internal void Hide()
		{
			Object.Destroy(_itemHolder);
			this.get_gameObject().SetActive(false);
		}

		private void Awake()
		{
			_uiScrollView = this.GetComponentInChildren<UIScrollView>();
		}

		private void CreateItemsList(Dictionary<CubeTypeID, SunkCube> neededCubes)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			List<SunkCube> list = new List<SunkCube>();
			list.AddRange(neededCubes.Values);
			Transform transform = robotShopInventoryItemTemplate.get_transform();
			Vector3 localPosition = transform.get_localPosition();
			_itemHolder = new GameObject();
			_itemHolder.get_transform().set_parent(transform.get_parent());
			_itemHolder.get_transform().set_localScale(Vector3.get_one());
			_itemHolder.get_transform().set_localPosition(localPosition);
			foreach (SunkCube item in list)
			{
				CreateInventoryItem(item, localPosition);
				localPosition.y -= robotShopInventoryItemTemplate.VerticalSpacing;
			}
			robotShopInventoryItemTemplate.get_gameObject().SetActive(false);
		}

		private void CreateInventoryItem(SunkCube neededCube, Vector3 position)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			RobotShopInventoryItem robotShopInventoryItem = Object.Instantiate<RobotShopInventoryItem>(robotShopInventoryItemTemplate);
			robotShopInventoryItem.SetData(neededCube.name);
			robotShopInventoryItem.get_transform().set_parent(robotShopInventoryItemTemplate.get_transform().get_parent());
			robotShopInventoryItem.get_transform().set_localScale(Vector3.get_one());
			robotShopInventoryItem.get_transform().set_localPosition(position);
			robotShopInventoryItem.get_transform().set_parent(_itemHolder.get_transform());
			robotShopInventoryItem.get_gameObject().SetActive(true);
		}
	}
}
