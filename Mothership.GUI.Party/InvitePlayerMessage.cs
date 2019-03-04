namespace Mothership.GUI.Party
{
	internal struct InvitePlayerMessage
	{
		public readonly string name;

		public InvitePlayerMessage(string pname)
		{
			name = pname;
		}
	}
}
