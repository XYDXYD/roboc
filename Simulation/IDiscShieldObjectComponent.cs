namespace Simulation
{
	internal interface IDiscShieldObjectComponent
	{
		ShieldEntity discShieldObject
		{
			get;
		}

		bool shieldActive
		{
			get;
			set;
		}
	}
}
