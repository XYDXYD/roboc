using UnityEngine;
using Utility;

internal static class WwwExt
{
	public static int GetResponseCode(this WWW request)
	{
		int result = 0;
		if (request.get_responseHeaders() == null)
		{
			Console.LogError("no response headers - error: " + request.get_error() + " trying to reach " + request.get_url());
		}
		else if (!request.get_responseHeaders().ContainsKey("STATUS"))
		{
			Console.LogError("response headers has no STATUS. headers: " + request.get_responseHeaders().ToPrettyString());
		}
		else
		{
			result = ParseResponseCode(request.get_responseHeaders()["STATUS"]);
		}
		return result;
	}

	private static int ParseResponseCode(string statusString)
	{
		int result = 0;
		string[] array = statusString.Split(' ');
		if (array.Length < 3)
		{
			Console.LogError("invalid response status: " + statusString);
		}
		else if (!int.TryParse(array[1], out result))
		{
			Console.LogError("invalid response code: " + array[1]);
		}
		return result;
	}
}
