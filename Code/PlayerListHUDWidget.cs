using System.Text;
using UnityEngine;

internal sealed class PlayerListHUDWidget : MonoBehaviour
{
	public UILabel killsLabel;

	public UILabel nameLabel;

	public UITexture AvatarTexture;

	public UITexture ClanAvatarTexture;

	private StringBuilder _stringBuilder = new StringBuilder();

	public PlayerListHUDWidget()
		: this()
	{
	}

	private void Awake()
	{
		killsLabel.set_text(string.Empty);
		nameLabel.set_text(string.Empty);
	}

	public void SetPlayerKills(string kills, string playerName)
	{
		if (kills == "0")
		{
			kills = "-";
		}
		killsLabel.set_text(kills);
		nameLabel.set_text(playerName);
		nameLabel.UpdateNGUIText();
		string text = default(string);
		if (nameLabel.Wrap(playerName, ref text))
		{
			return;
		}
		_stringBuilder.Length = 0;
		if (text.Length - 3 > 0)
		{
			for (int i = 0; i < text.Length - 3; i++)
			{
				_stringBuilder.Append(text[i]);
			}
			_stringBuilder.Append("...");
			nameLabel.set_text(_stringBuilder.ToString());
		}
	}

	public void SetColour(bool isPlayer, bool isDead, bool isInPlatoon)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (isPlayer)
		{
			killsLabel.set_color(PlayerColours.playerColor);
			nameLabel.set_color(PlayerColours.playerColor);
		}
		else if (isDead)
		{
			killsLabel.set_color(PlayerColours.deadColor);
			nameLabel.set_color(PlayerColours.deadColor);
		}
		else if (isInPlatoon)
		{
			killsLabel.set_color(PlayerColours.platoonColor);
			nameLabel.set_color(PlayerColours.platoonColor);
		}
		else
		{
			killsLabel.set_color(PlayerColours.enemyColor);
			nameLabel.set_color(PlayerColours.enemyColor);
		}
	}
}
