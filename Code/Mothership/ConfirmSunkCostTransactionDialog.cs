using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class ConfirmSunkCostTransactionDialog : MonoBehaviour
	{
		public UILabel sunkCubeCost;

		public UILabel sunkCubeCosmeticCreditsCost;

		public RobotShopInventoryItem templateItem;

		public UILabel titleLabel;

		public UILabel bodyLabel;

		public UILabel confirmButtLabel;

		private GameObject _itemHolder;

		public ConfirmSunkCostTransactionDialog()
			: this()
		{
		}

		public void DisplayConstruct(uint sunkCost, Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmConstructTitle"));
			bodyLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmConstructInfo"));
			confirmButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConstructRobot"));
			Show(sunkCost, usedCubes);
		}

		public void DisplayForge(uint sunkCost, Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmForgeTitle"));
			bodyLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmForgeInfo"));
			confirmButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strForgeGetRobot"));
			Show(sunkCost, usedCubes);
		}

		public void DisplayUnlock(uint robitsCost, uint cosmeticCreditsCost, Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmForgeTitle"));
			bodyLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmForgeInfo"));
			confirmButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strForgeGetRobot"));
			Show(robitsCost, cosmeticCreditsCost, usedCubes);
		}

		private void Show(uint sunkRobitsCost, uint sunkCosmeticCreditsCost, Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			sunkCubeCosmeticCreditsCost.set_text(sunkCosmeticCreditsCost.ToString("#,##0"));
			sunkCubeCost.set_text(sunkRobitsCost.ToString("#,##0"));
			this.get_gameObject().SetActive(true);
			Show(sunkRobitsCost, usedCubes);
		}

		private void Show(uint sunkRobitsCost, Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			sunkCubeCost.set_text(sunkRobitsCost.ToString("#,##0"));
			this.get_gameObject().SetActive(true);
			CreateItemsList(usedCubes);
		}

		private void CreateItemsList(Dictionary<CubeTypeID, SunkCube> usedCubes)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			List<SunkCube> list = new List<SunkCube>();
			list.AddRange(usedCubes.Values);
			Vector3 localPosition = templateItem.get_transform().get_localPosition();
			_itemHolder = new GameObject();
			_itemHolder.get_transform().set_parent(templateItem.get_transform().get_parent());
			_itemHolder.get_transform().set_localScale(Vector3.get_one());
			_itemHolder.get_transform().set_localPosition(localPosition);
			foreach (SunkCube item in list)
			{
				CreateInventoryItem(item, localPosition);
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
