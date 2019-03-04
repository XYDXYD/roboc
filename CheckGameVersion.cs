using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using UnityEngine;
using Utility;

internal sealed class CheckGameVersion : MonoBehaviour
{
	public struct BuildVersionInfo
	{
		public readonly string VersionName;

		public readonly int VersionNumber;

		public BuildVersionInfo(string versionName, int versionNumber)
		{
			VersionName = versionName;
			VersionNumber = versionNumber;
		}
	}

	public string message = "strOutdatedClient";

	internal static CheckGameVersion instance;

	public CheckGameVersion()
		: this()
	{
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	internal void CheckLatestBuild(IServiceRequestFactory serviceRequestFactory, Action<int> onComplete)
	{
		Console.Log("Check Build Version - Querying room");
		ICheckGameVersionRequest checkGameVersionRequest = serviceRequestFactory.Create<ICheckGameVersionRequest>();
		checkGameVersionRequest.SetAnswer(new ServiceAnswer<CheckGameVersionData>(delegate(CheckGameVersionData checkVersionData)
		{
			OnCheckGameVersionSucceed(checkVersionData, onComplete);
		}, OnCheckGameVersionFail));
		checkGameVersionRequest.Execute();
	}

	private void OnCheckGameVersionSucceed(CheckGameVersionData checkGameVersionData, Action<int> onComplete)
	{
		CheckOnLogin(checkGameVersionData.versionNumber, delegate(int versionNumber)
		{
			onComplete(versionNumber);
		});
	}

	private void OnCheckGameVersionFail(ServiceBehaviour serviceBehaviour)
	{
		Console.LogException((Exception)new CheckGameVersionException(serviceBehaviour.errorBody));
	}

	private void CheckOnLogin(int correctVersionNumber, Action<int> onComplete)
	{
		if (IsCorrectVersion(correctVersionNumber))
		{
			onComplete?.Invoke(correctVersionNumber);
		}
		else
		{
			Console.LogException((Exception)new CheckGameVersionException("incorrect version"));
		}
	}

	public void CheckLatestBuild()
	{
		if (ClientConfigData.TryGetValue("MinimumVersion", out string value))
		{
			int correctVersionNumber = int.Parse(value);
			if (!IsCorrectVersion(correctVersionNumber))
			{
				Console.LogException((Exception)new CheckGameVersionException(StringTableBase<StringTable>.Instance.GetString(message)));
			}
		}
		else
		{
			Console.LogWarning("Failed to get minimum version, skipping check");
		}
	}

	private bool IsCorrectVersion(int correctVersionNumber)
	{
		int num = EmbeddedVersion();
		Console.LogWarning("current version: " + num + " - minimum required version: " + correctVersionNumber);
		return num >= correctVersionNumber;
	}

	public static int EmbeddedVersion()
	{
		BuildVersionInfo buildVersionInfo = GetBuildVersionInfo();
		return buildVersionInfo.VersionNumber;
	}

	public static BuildVersionInfo GetBuildVersionInfo()
	{
		TextAsset val = Resources.Load("build") as TextAsset;
		if (val != null)
		{
			string[] array = val.get_text().Split(',');
			string versionName = array[0];
			int versionNumber = Convert.ToInt32(array[1]);
			return new BuildVersionInfo(versionName, versionNumber);
		}
		return new BuildVersionInfo("Not found", -1);
	}
}
