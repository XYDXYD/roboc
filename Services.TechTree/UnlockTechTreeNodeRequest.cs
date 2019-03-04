using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.TechTree
{
	internal class UnlockTechTreeNodeRequest : WebServicesRequest<Dictionary<string, TechTreeItemData>>, IUnlockTechTreeNodeRequest, IServiceRequest<string>, IAnswerOnComplete<Dictionary<string, TechTreeItemData>>, IServiceRequest
	{
		private string _nodeToUnlockId;

		protected override byte OperationCode => 184;

		public UnlockTechTreeNodeRequest()
			: base("strRobocloudError", "strFailedToUnlockTeckTreeNode", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public void Inject(string dependency)
		{
			_nodeToUnlockId = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			if (val.Parameters == null)
			{
				val.Parameters = new Dictionary<byte, object>();
			}
			val.Parameters[211] = _nodeToUnlockId;
			return val;
		}

		protected override Dictionary<string, TechTreeItemData> ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[210];
			Dictionary<string, TechTreeItemData> dictionary2 = new Dictionary<string, TechTreeItemData>();
			using (Dictionary<string, Hashtable>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string key = enumerator.Current.Key;
					Hashtable value = enumerator.Current.Value;
					TechTreeItemData value2 = new TechTreeItemData(Convert.ToUInt32((string)value.get_Item((object)"mainCubeId"), 16), Convert.ToInt32(value.get_Item((object)"positionX")), Convert.ToInt32(value.get_Item((object)"positionY")), Convert.ToBoolean(value.get_Item((object)"isUnlocked")), Convert.ToBoolean(value.get_Item((object)"isUnlockable")), (uint)Convert.ToInt32(value.get_Item((object)"tp")), (string[])value.get_Item((object)"neighbours"));
					dictionary2.Add(key, value2);
				}
				return dictionary2;
			}
		}
	}
}
