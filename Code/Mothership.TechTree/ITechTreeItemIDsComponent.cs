namespace Mothership.TechTree
{
	internal interface ITechTreeItemIDsComponent
	{
		string NodeID
		{
			get;
		}

		CubeTypeID CubeID
		{
			get;
		}
	}
}
