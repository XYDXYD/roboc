using UnityEngine;

internal class PlayerListElementSimulation : MonoBehaviour
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel robotNameLabel;

	public UITexture avatarTexture;

	public UITexture clanAvatarTexture;

	public UISprite partySprite;

	protected string PlayerName;

	public PlayerListElementSimulation()
		: this()
	{
	}

	internal virtual void SetPlayer(PlayerDataDependency player, string filteredRobotName, int maxCPU, bool isMegabot)
	{
		PlayerName = player.PlayerName;
		nameLabel.set_text(player.DisplayName);
		int num = Mathf.Min(player.Cpu, maxCPU);
		robotNameLabel.set_text(string.Format("{0} [{1:n0} {2}]", filteredRobotName, num, StringTableBase<StringTable>.Instance.GetString("strCPU")));
	}

	public virtual void SetColour(Color colour)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		nameLabel.set_color(colour);
		robotNameLabel.set_color(colour);
	}

	public virtual void SetLoadProgress(float progress)
	{
	}
}
