using ExtensionMethods;
using Services.Local;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System.Collections;
using UnityEngine;
using WebServices;

namespace Services.Web
{
	internal class LoadTextureFromURLRequest : ILoadTextureFromURLRequest, IServiceRequest<string>, IAnswerOnComplete<Texture2D>, IServiceRequest
	{
		private string _url;

		private IServiceAnswer<Texture2D> _answer;

		public void Inject(string dependency)
		{
			_url = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<Texture2D> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (CacheDTO.robotShopThumbnails.ContainsKey(_url))
			{
				if (_answer != null && _answer.succeed != null)
				{
					_answer.succeed(CacheDTO.robotShopThumbnails.get_Item(_url));
				}
			}
			else
			{
				string[] array = _url.Split('/');
				array[array.Length - 1] = WWW.EscapeURL(array[array.Length - 1]);
				string url = string.Join("/", array);
				TaskRunner.get_Instance().Run(LoadTexture(url));
			}
		}

		private IEnumerator LoadTexture(string url)
		{
			WWW www = new WWW(url);
			yield return (object)new WWWEnumerator(www, 5f);
			if (www.get_isDone() && www.get_error().IsNullOrWhiteSpace())
			{
				Texture2D val = GameUtility.CreateRobotShopTexture();
				www.LoadImageIntoTexture(val);
				if (_answer != null && _answer.succeed != null)
				{
					_answer.succeed(val);
				}
			}
			else if (_answer != null && _answer.failed != null)
			{
				ServiceBehaviour serviceBehaviour = new ServiceBehaviour("strLoadingBundle", "strErrorLoadGameFilesHD");
				serviceBehaviour.exceptionThrown = new WWWException(www);
				_answer.failed(serviceBehaviour);
			}
		}
	}
}
