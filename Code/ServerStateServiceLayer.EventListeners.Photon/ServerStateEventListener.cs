using Services.Web.Photon;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal abstract class ServerStateEventListener<T, U> : PhotonEventListener<T, U>
	{
		public abstract WebServicesEventCode WebServicesEventCode
		{
			get;
		}

		public override int EventCode => (int)WebServicesEventCode;
	}
	internal abstract class ServerStateEventListener<T> : PhotonEventListener<T>
	{
		public abstract WebServicesEventCode WebServicesEventCode
		{
			get;
		}

		public override int EventCode => (int)WebServicesEventCode;
	}
	internal abstract class ServerStateEventListener : PhotonEventListener
	{
		public abstract WebServicesEventCode WebServicesEventCode
		{
			get;
		}

		public override int EventCode => (int)WebServicesEventCode;
	}
}
