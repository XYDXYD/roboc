namespace Simulation
{
	internal class IgnoreListSimulation : IgnoreList
	{
		protected override void BlockFriend(string user)
		{
			RemoveAndBlockFriend(user);
		}
	}
}
