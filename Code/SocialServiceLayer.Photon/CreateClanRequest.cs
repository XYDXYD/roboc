using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class CreateClanRequest<T> : SocialRequest, ICreateClanRequest<T>, IServiceRequest<T>, IAnswerOnComplete, IServiceRequest where T : CreateClanRequestDependancyBase
	{
		private T _dependency;

		protected override byte OperationCode => 31;

		public CreateClanRequest()
			: base("strCreateClanErrorTitle", "strCreateClanErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(31, _dependency.clanName);
			dictionary.Add(32, _dependency.clanDescription);
			dictionary.Add(34, _dependency.clanType);
			dictionary.Add(33, (_dependency as CreateClanRequestDependancyTencent).clanDefaultAvatar);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(T dependency)
		{
			_dependency = dependency;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			CacheDTO.MyClanInfo = new ClanInfo(_dependency.clanName, _dependency.clanDescription, _dependency.clanType);
			Hashtable[] array = (Hashtable[])response.Parameters[36];
			ClanMember[] array2 = new ClanMember[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Hashtable clanMemberRaw = array[i];
				array2[i] = ClanMember.FromHashtable(clanMemberRaw);
			}
			CacheDTO.MyClanMembers = new Dictionary<string, ClanMember>();
			ClanMember[] array3 = array2;
			foreach (ClanMember clanMember in array3)
			{
				CacheDTO.MyClanMembers.Add(clanMember.Name, clanMember);
			}
			base.ProcessResponse(response);
		}
	}
}
