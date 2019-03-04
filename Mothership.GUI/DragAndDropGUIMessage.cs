namespace Mothership.GUI
{
	internal class DragAndDropGUIMessage
	{
		public readonly DragAndDropGUIMessageType MessageType;

		public readonly object Data;

		public DragAndDropGUIMessage(DragAndDropGUIMessageType Message_, object data_ = null)
		{
			Data = data_;
			MessageType = Message_;
		}
	}
}
