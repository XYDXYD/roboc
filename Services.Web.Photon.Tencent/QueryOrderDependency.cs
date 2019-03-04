namespace Services.Web.Photon.Tencent
{
	public class QueryOrderDependency
	{
		public string railID
		{
			get;
			private set;
		}

		public string orderID
		{
			get;
			private set;
		}

		public QueryOrderDependency(string railID_, string orderID_)
		{
			railID = railID_;
			orderID = orderID_;
		}
	}
}
