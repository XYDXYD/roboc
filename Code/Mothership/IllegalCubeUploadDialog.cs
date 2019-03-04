using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class IllegalCubeUploadDialog : MonoBehaviour
	{
		public RobotShopInventoryItem templateItem;

		public UILabel errorUILabel;

		private GameObject _itemHolder;

		public IllegalCubeUploadDialog()
			: this()
		{
		}

		public void Show(List<SunkCube> usedCubes, string errorStr)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			this.get_gameObject().SetActive(true);
			errorUILabel.set_text(errorStr);
			Vector3 localPosition = templateItem.get_transform().get_localPosition();
			_itemHolder = new GameObject();
			_itemHolder.get_transform().set_parent(templateItem.get_transform().get_parent());
			_itemHolder.get_transform().set_localScale(Vector3.get_one());
			_itemHolder.get_transform().set_localPosition(localPosition);
			foreach (SunkCube usedCube in usedCubes)
			{
				CreateInventoryItem(usedCube, localPosition);
				localPosition.y -= templateItem.VerticalSpacing;
			}
			templateItem.get_gameObject().SetActive(false);
		}

		private void CreateInventoryItem(SunkCube sunkCube, Vector3 position)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			RobotShopInventoryItem robotShopInventoryItem = Object.Instantiate<RobotShopInventoryItem>(templateItem);
			robotShopInventoryItem.SetData(sunkCube.name);
			robotShopInventoryItem.get_transform().set_parent(templateItem.get_transform().get_parent());
			robotShopInventoryItem.get_transform().set_localScale(Vector3.get_one());
			robotShopInventoryItem.get_transform().set_localPosition(position);
			robotShopInventoryItem.get_transform().set_parent(_itemHolder.get_transform());
			robotShopInventoryItem.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			Object.Destroy(_itemHolder);
			this.get_gameObject().SetActive(false);
		}
	}
}
