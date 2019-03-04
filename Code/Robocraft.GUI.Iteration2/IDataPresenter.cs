namespace Robocraft.GUI.Iteration2
{
	internal interface IDataPresenter : IPresenter
	{
		void SetDataSource(IDataSource ds);

		void UpdateFromSource();
	}
}
