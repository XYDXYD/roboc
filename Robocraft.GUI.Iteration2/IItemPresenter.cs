namespace Robocraft.GUI.Iteration2
{
	internal interface IItemPresenter : IDataPresenter, IPresenter
	{
		void SetDataSourceIndex(int index);

		void SetSiblingIndex(int index);
	}
}
