namespace Robocraft.GUI
{
	internal interface IGenericComponent
	{
		IGenericComponentView View
		{
			get;
		}

		string Name
		{
			get;
		}

		void SetName(string name);

		void SetDataSource(IDataSource dataSource);

		void SetView(IGenericComponentView view);

		void Activate();

		void DeActivate();

		void HandleMessage(GenericComponentMessage message);
	}
}
