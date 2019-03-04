using Authentication;
using Svelto.IoC;
using System;

public class FeatureThrottler : IInitialize
{
	private bool _canPlay;

	private const string FEATURE_THROTTLER_JSON_KEY = "FeatureThrottlerOnPercent";

	void IInitialize.OnDependenciesInjected()
	{
		string value = string.Empty;
		if (ClientConfigData.TryGetValue("FeatureThrottlerOnPercent", out value))
		{
			double num = (float)Convert.ToInt32(value) / 100f;
			Random random = new Random(GetBaseHash());
			double num2 = random.NextDouble();
			_canPlay = (num2 < num);
		}
		if (ServEnv.Exists())
		{
			string value2 = null;
			if (ServEnv.TryGetValue("FeatureThrottlerOn", out value2) && value2 == "false")
			{
				_canPlay = true;
			}
		}
	}

	public bool UserCanPlay()
	{
		return _canPlay;
	}

	private int GetBaseHash()
	{
		return User.Username.GetHashCode();
	}

	private int GetBaseHash(string username)
	{
		return username.GetHashCode();
	}
}
