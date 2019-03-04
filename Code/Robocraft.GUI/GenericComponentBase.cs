namespace Robocraft.GUI
{
	internal abstract class GenericComponentBase : IGenericComponent
	{
		private string _name;

		private IDataSource _dataSource;

		public abstract IGenericComponentView View
		{
			get;
		}

		protected IDataSource DataSource => _dataSource;

		public string Name => _name;

		public void SetName(string name)
		{
			_name = name;
		}

		public void SetDataSource(IDataSource dataSource)
		{
			if (_dataSource != null && _dataSource is INotifyDataChanged)
			{
				INotifyDataChanged notifyDataChanged = (INotifyDataChanged)_dataSource;
				notifyDataChanged.onDataItemChanged -= OnDataItemChanged;
				notifyDataChanged.onAllDataChanged -= OnAllDataChanged;
			}
			_dataSource = dataSource;
			if (_dataSource != null && _dataSource is INotifyDataChanged)
			{
				INotifyDataChanged notifyDataChanged2 = (INotifyDataChanged)_dataSource;
				notifyDataChanged2.onDataItemChanged += OnDataItemChanged;
				notifyDataChanged2.onAllDataChanged += OnAllDataChanged;
			}
		}

		protected virtual void OnDataItemChanged(int index1, int index2)
		{
		}

		protected virtual void OnAllDataChanged()
		{
		}

		public abstract void SetView(IGenericComponentView view);

		public virtual void Activate()
		{
			View.Setup();
			View.Show();
		}

		public virtual void DeActivate()
		{
			View.Hide();
		}

		public virtual void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Message)
			{
			case MessageType.Show:
				View.Show();
				break;
			case MessageType.Hide:
				View.Hide();
				break;
			}
		}
	}
}
