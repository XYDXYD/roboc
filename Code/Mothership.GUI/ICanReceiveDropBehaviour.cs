namespace Mothership.GUI
{
	internal interface ICanReceiveDropBehaviour
	{
		void ReceiveDrop(object data);

		bool CanReceiveObject(object data);
	}
}
