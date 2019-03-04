namespace Robocraft.GUI
{
	public abstract class GenericListEntryViewBase : GenericComponentViewBase, IGenericListEntryView
	{
		public virtual void Default()
		{
		}

		public abstract void UpdateData(object data);

		public override void Setup()
		{
			base.Setup();
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public override void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					genericComponentMessage.Consume();
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "EmbeddedListButton", new ListItemComponentDataContainer(this)));
				}
			}
		}
	}
}
