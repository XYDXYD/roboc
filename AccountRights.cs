internal struct AccountRights
{
	public readonly bool Moderator;

	public readonly bool Developer;

	public readonly bool Admin;

	public AccountRights(bool moderator, bool developer, bool admin)
	{
		Moderator = moderator;
		Developer = developer;
		Admin = admin;
	}
}
