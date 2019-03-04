using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

internal class ImprovedPlayerListElementSimulation : PlayerListElementSimulation
{
	[SerializeField]
	private UISprite[] loadoutSprites;

	[SerializeField]
	private UISprite[] loadoutSpriteBackgrounds;

	[SerializeField]
	private Color32 emptySlotLoadoutBackgroundTint = new Color32((byte)195, (byte)195, (byte)195, (byte)45);

	[SerializeField]
	private Color32 filledSlotLoadoutBackgroundTint = new Color32((byte)195, (byte)195, (byte)195, byte.MaxValue);

	[SerializeField]
	private UISprite loadingBar;

	[SerializeField]
	private UILabel _tierLabel;

	private bool _aiPlayer;

	[Inject]
	internal ItemDescriptorSpriteUtility itemDescriptorSpriteUtility
	{
		private get;
		set;
	}

	internal override void SetPlayer(PlayerDataDependency player, string filteredRobotName, int maxCPU, bool isMegabot)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		base.SetPlayer(player, filteredRobotName, maxCPU, isMegabot);
		_aiPlayer = player.AiPlayer;
		if (player.WeaponOrder != null)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < player.WeaponOrder.Count(); i++)
			{
				int itemDescriptorKeyByIndex = player.WeaponOrder.GetItemDescriptorKeyByIndex(i);
				if (itemDescriptorKeyByIndex != 0)
				{
					list.Add(itemDescriptorKeyByIndex);
				}
			}
			list.Sort();
			for (int j = 0; j < loadoutSprites.Length; j++)
			{
				if (j < list.Count)
				{
					loadoutSprites[j].get_gameObject().SetActive(true);
					loadoutSprites[j].set_spriteName(itemDescriptorSpriteUtility.GetSprite(list[j]));
					loadoutSpriteBackgrounds[j].set_color(Color32.op_Implicit(filledSlotLoadoutBackgroundTint));
				}
				else
				{
					loadoutSprites[j].get_gameObject().SetActive(false);
					loadoutSpriteBackgrounds[j].set_color(Color32.op_Implicit(emptySlotLoadoutBackgroundTint));
				}
			}
		}
		_tierLabel.set_text(RRAndTiers.ConvertTierIndexToTierString((uint)player.Tier, isMegabot));
		SetLoadProgress(0f);
	}

	public override void SetColour(Color colour)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.SetColour(colour);
	}

	public override void SetLoadProgress(float progress)
	{
		base.SetLoadProgress(progress);
		loadingBar.set_fillAmount((!_aiPlayer) ? progress : 1f);
	}
}
