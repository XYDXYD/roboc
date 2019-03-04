using Services.Web;

namespace WebServices
{
	internal class SinglePlayerWebStorageRequestFactory : WebStorageRequestFactoryDefault
	{
		public SinglePlayerWebStorageRequestFactory()
		{
			AddRelation<ISetRobotInGameRequest, SinglePlayer_SetRobotInGameRequest, float>();
		}
	}
}
