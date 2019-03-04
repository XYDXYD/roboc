using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetSpecialItemListRequest : WebServicesRequest<ReadOnlyDictionary<uint, SpecialItemListData>>, ILoadSpecialItemListRequest, ITask, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<uint, SpecialItemListData>>, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		protected override byte OperationCode => 6;

		private event Action<bool> _onComplete;

		public GetSpecialItemListRequest()
			: base("strRobocloudError", "strUnableLoadSpecialItemList", 3)
		{
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete += action;
			return this;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		public override void Execute()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (CacheDTO.specialItemList != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new ReadOnlyDictionary<uint, SpecialItemListData>(CacheDTO.specialItemList));
				}
				isDone = true;
				progress = 1f;
				if (this._onComplete != null)
				{
					this._onComplete(obj: true);
				}
			}
			else
			{
				base.Execute();
			}
		}

		public void ClearCache()
		{
			CacheDTO.specialItemList = null;
		}

		protected override ReadOnlyDictionary<uint, SpecialItemListData> ProcessResponse(OperationResponse response)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<uint, SpecialItemListData> dictionary = new Dictionary<uint, SpecialItemListData>();
			foreach (KeyValuePair<string, Hashtable> item in (Dictionary<string, Hashtable>)response.Parameters[1])
			{
				uint num = Convert.ToUInt32(item.Key, 16);
				Hashtable value = item.Value;
				SpecialItemListData value2 = ParseNode(num, value);
				dictionary.Add(num, value2);
			}
			CacheDTO.specialItemList = dictionary;
			isDone = true;
			progress = 1f;
			if (this._onComplete != null)
			{
				this._onComplete(obj: true);
			}
			return new ReadOnlyDictionary<uint, SpecialItemListData>(CacheDTO.specialItemList);
		}

		private SpecialItemListData ParseNode(uint id, Hashtable value)
		{
			string name = string.Empty;
			if (((Dictionary<object, object>)value).ContainsKey((object)"name"))
			{
				name = Convert.ToString(value.get_Item((object)"name"));
			}
			string spriteName = string.Empty;
			if (((Dictionary<object, object>)value).ContainsKey((object)"spriteName"))
			{
				spriteName = Convert.ToString(value.get_Item((object)"spriteName"));
			}
			uint motherhsipSize = 0u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"mothershipSize"))
			{
				motherhsipSize = Convert.ToUInt32(value.get_Item((object)"mothershipSize"));
			}
			return new SpecialItemListData(name, spriteName, motherhsipSize);
		}
	}
}
