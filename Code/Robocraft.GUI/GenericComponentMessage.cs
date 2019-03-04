namespace Robocraft.GUI
{
	public class GenericComponentMessage
	{
		public MessageType Message;

		public string Target;

		public string Originator;

		public IGenericComponentDataContainer Data;

		public bool Consumed;

		public GenericComponentMessage(MessageType messageType_, string originator_, string target_ = "")
		{
			Message = messageType_;
			Target = target_;
			Originator = originator_;
			Data = null;
		}

		public GenericComponentMessage(MessageType messageType_, string target_, string originator_, IGenericComponentDataContainer data_)
		{
			Message = messageType_;
			Originator = originator_;
			Target = target_;
			Data = data_;
		}

		public void Consume()
		{
			Consumed = true;
		}
	}
}
