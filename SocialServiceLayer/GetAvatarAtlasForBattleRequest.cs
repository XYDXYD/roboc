using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace SocialServiceLayer
{
	internal class GetAvatarAtlasForBattleRequest : IGetAvatarAtlasForBattleRequest, IServiceRequest<GetAvatarAtlasRequestDependancy>, IAnswerOnComplete<AvatarAtlasForBattle>, ITask, IServiceRequest, IAbstractTask
	{
		private Dictionary<string, Texture2D> _allAvatars = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);

		private IServiceAnswer<AvatarAtlasForBattle> _answer;

		private PresetAvatarMap _presetAvatars;

		private IDictionary<string, AvatarInfo> _playerAvatarInfos;

		private Dictionary<string, Texture2D> _loadedAvatarTextures;

		public bool isDone
		{
			get;
			private set;
		}

		public GetAvatarAtlasForBattleRequest()
		{
			_presetAvatars = (Resources.Load("PresetAvatarMap") as PresetAvatarMap);
		}

		public void Inject(GetAvatarAtlasRequestDependancy dependancy)
		{
			Console.Log("GetAvatarAtlasForBattleRequest try to get an avatar atlas for:" + dependancy.avatarInfos.Count + " avatars");
			_playerAvatarInfos = dependancy.avatarInfos;
			_loadedAvatarTextures = dependancy.loadedCustomAvatarTextures;
			foreach (KeyValuePair<string, Texture2D> loadedAvatarTexture in _loadedAvatarTextures)
			{
			}
		}

		public void Execute()
		{
			_allAvatars = new Dictionary<string, Texture2D>();
			foreach (KeyValuePair<string, AvatarInfo> playerAvatarInfo in _playerAvatarInfos)
			{
				if (playerAvatarInfo.Value.UseCustomAvatar)
				{
					Texture2D val = _loadedAvatarTextures[playerAvatarInfo.Key];
					if (val == null)
					{
						_allAvatars.Add(playerAvatarInfo.Key, AvatarUtils.FailedToLoadTexture);
					}
					else
					{
						_allAvatars.Add(playerAvatarInfo.Key, val);
					}
				}
				else
				{
					Texture2D texture = _presetAvatars.Avatars[playerAvatarInfo.Value.AvatarId].Texture;
					_allAvatars.Add(playerAvatarInfo.Key, texture);
				}
			}
			BuildAtlasAndContinue();
		}

		public IServiceRequest SetAnswer(IServiceAnswer<AvatarAtlasForBattle> answer)
		{
			_answer = answer;
			return this;
		}

		private void BuildAtlasAndContinue()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Console.Log("Building avatar atlas now");
			AvatarUtils.CreateAvatarAtlas(_allAvatars, out Texture2D atlasTexture, out ReadOnlyDictionary<string, Rect> atlasRects);
			AvatarAtlasForBattle obj = new AvatarAtlasForBattle(atlasTexture, (IDictionary<string, Rect>)(object)atlasRects);
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
				isDone = true;
				return;
			}
			throw new Exception("No success path specified");
		}
	}
}
