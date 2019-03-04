namespace Services.Web.Photon
{
	public class GetBrawlRequestResult
	{
		public readonly BrawlClientParameters BrawlParameters;

		public readonly BrawlGameParameters Details;

		public GetBrawlRequestResult(BrawlClientParameters brawlParameters_, BrawlGameParameters details_)
		{
			BrawlParameters = brawlParameters_;
			Details = details_;
		}
	}
}
