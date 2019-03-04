using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class SpectatorHintPresenter : IInitialize
	{
		private readonly string CONFIG_FILENAME = (!WorldSwitching.IsMultiplayer()) ? "hintsSingleplayer.xml" : "hintsMultiplayer.xml";

		private const string SOURCE_ADDRESS = "https://s3-eu-west-1.amazonaws.com/hintcache/live/";

		private const string LOCAL_CACHE_FOLDER = "../HintCache";

		private List<HintData> _hints = new List<HintData>();

		private SpectatorHintView _view;

		public void RegisterView(SpectatorHintView view)
		{
			_view = view;
		}

		void IInitialize.OnDependenciesInjected()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateImages);
		}

		public void UpdateHintTexture()
		{
			SetCurrentHint();
		}

		private void SetCurrentHint()
		{
			if (_hints.Count > 0)
			{
				_view.SetCurrentHint(_hints[Random.Range(0, _hints.Count)]);
			}
		}

		private IEnumerator UpdateImages()
		{
			WWW configData = new WWW("https://s3-eu-west-1.amazonaws.com/hintcache/live/" + CONFIG_FILENAME);
			try
			{
				yield return (object)new WWWEnumerator(configData, -1f);
				if (configData.get_error() == null)
				{
					yield return CreateHints(CONFIG_FILENAME, configData.get_text());
				}
				else
				{
					Console.LogError(string.Format("Error downloading hint config [SourceAddress: {0}, Error {1}]", "https://s3-eu-west-1.amazonaws.com/hintcache/live/" + CONFIG_FILENAME, configData.get_error()));
				}
			}
			finally
			{
				base._003C_003E__Finally0();
			}
		}

		private IEnumerator CreateHints(string fileName, string text)
		{
			if (!ConfigCacheValid(fileName, text))
			{
				if (Directory.Exists(GetLocalCacheFolder()))
				{
					DeleteCache();
				}
				CacheConfig(fileName, text);
			}
			XmlDocument config = new XmlDocument();
			config.LoadXml(text);
			XmlNodeList nodes = config.SelectNodes("//hint");
			string defaultLanguage = config.SelectSingleNode("//DefaultLanguage").InnerText;
			IEnumerator enumerator = nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode node = (XmlNode)enumerator.Current;
					yield return DownloadHintImage(node, defaultLanguage);
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable2 = disposable = (enumerator as IDisposable);
				if (disposable != null)
				{
					disposable2.Dispose();
				}
			}
		}

		private void CacheConfig(string fileName, string text)
		{
			string localCacheFolder = GetLocalCacheFolder();
			if (!Directory.Exists(localCacheFolder))
			{
				Directory.CreateDirectory(localCacheFolder);
			}
			File.WriteAllText(localCacheFolder + "/" + fileName, text);
		}

		private void DeleteCache()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(GetLocalCacheFolder());
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.Delete();
			}
		}

		private bool ConfigCacheValid(string filename, string newConfig)
		{
			if (Directory.Exists(GetLocalCacheFolder()) && File.Exists(GetLocalCacheFolder() + "/" + filename))
			{
				string a = File.ReadAllText(GetLocalCacheFolder() + "/" + filename);
				return a == newConfig;
			}
			return false;
		}

		private IEnumerator DownloadHintImage(XmlNode node, string defaultLanguage)
		{
			string text = node.SelectSingleNode("text").InnerText;
			XmlNode imageNode = node.SelectSingleNode($"imageurl/{StringTableBase<StringTable>.Instance.language}");
			if (imageNode == null)
			{
				imageNode = node.SelectSingleNode($"imageurl/{defaultLanguage}");
			}
			string fileName = imageNode.InnerText;
			string localName = GetLocalCacheFolder() + "/" + fileName;
			if (File.Exists(localName))
			{
				byte[] array = File.ReadAllBytes(localName);
				Texture2D val = new Texture2D(2, 2, 12, false);
				ImageConversion.LoadImage(val, array);
				OnTextureLoaded(val, text);
			}
			else
			{
				string url = "https://s3-eu-west-1.amazonaws.com/hintcache/live/" + fileName;
				WWW www = new WWW(url);
				try
				{
					yield return (object)new WWWEnumerator(www, -1f);
					if (www.get_error() == null)
					{
						string localCacheFolder = GetLocalCacheFolder();
						if (!Directory.Exists(localCacheFolder))
						{
							Directory.CreateDirectory(localCacheFolder);
						}
						File.WriteAllBytes(localName, www.get_bytes());
						OnTextureLoaded(www.get_texture(), text);
					}
					else
					{
						Console.LogError($"Error downloading hint image [SourceAddress: {url}, Error {www.get_error()}]");
					}
				}
				finally
				{
					base._003C_003E__Finally0();
				}
			}
		}

		private string GetLocalCacheFolder()
		{
			return Application.get_dataPath() + "/../HintCache";
		}

		private void OnTextureLoaded(Texture2D texture, string text)
		{
			_hints.Add(new HintData(texture, text));
		}
	}
}
