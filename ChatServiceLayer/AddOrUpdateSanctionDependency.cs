namespace ChatServiceLayer
{
	internal struct AddOrUpdateSanctionDependency
	{
		public string UserName;

		public Sanction Sanction;

		public int Duration;

		public bool Remove;
	}
}
