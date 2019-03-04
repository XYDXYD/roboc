using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace SocialServiceLayer
{
	internal class GetClanAvatarAtlasForBattleRequest : IGetClanAvatarAtlasForBattleRequest, IServiceRequest<GetClanAvatarAtlasRequestDependancy>, IAnswerOnComplete<AvatarAtlasForBattle>, ITask, IServiceRequest, IAbstractTask
	{
		private IServiceAnswer<AvatarAtlasForBattle> _answer;

		private PresetAvatarMap _presetAvatars;

		public Dictionary<string, string> _clanByPlayer;

		public Dictionary<string, Texture2D> _loadedClanAvatarTextures;

		public bool isDone
		{
			get;
			private set;
		}

		public GetClanAvatarAtlasForBattleRequest()
		{
			_presetAvatars = (Resources.Load("PresetAvatarMap") as PresetAvatarMap);
		}

		public void Inject(GetClanAvatarAtlasRequestDependancy dependancy)
		{
			Console.Log("GetAvatarClanAtlasForBattleRequest has:" + dependancy.loadedClanAvatarTextures.Count + " clan avatars");
			_loadedClanAvatarTextures = dependancy.loadedClanAvatarTextures;
			_clanByPlayer = dependancy.clanByPlayer;
			foreach (KeyValuePair<string, Texture2D> loadedClanAvatarTexture in _loadedClanAvatarTextures)
			{
			}
		}

		public void Execute()
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, Texture2D> dictionary = new Dictionary<string, Texture2D>();
			foreach (KeyValuePair<string, string> item in _clanByPlayer)
			{
				string key = item.Key;
				string value = item.Value;
				if (_loadedClanAvatarTextures.TryGetValue(value, out Texture2D value2))
				{
					dictionary[key] = value2;
				}
			}
			AvatarUtils.CreateAvatarAtlas(dictionary, out Texture2D atlasTexture, out ReadOnlyDictionary<string, Rect> atlasRects);
			AvatarAtlasForBattle obj = new AvatarAtlasForBattle(atlasTexture, (IDictionary<string, Rect>)(object)atlasRects);
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
				isDone = true;
				return;
			}
			throw new Exception("No success path specified");
		}

		public IServiceRequest SetAnswer(IServiceAnswer<AvatarAtlasForBattle> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
