namespace Services.Web.Photon.Tencent
{
	public class CreateOrderDependency
	{
		public string railID
		{
			get;
			private set;
		}

		public string sku
		{
			get;
			private set;
		}

		public string railSessionID
		{
			get;
			private set;
		}

		public CreateOrderDependency(string railID_, string sku_, string railSessionID_)
		{
			railID = railID_;
			sku = sku_;
			railSessionID = railSessionID_;
		}
	}
}
