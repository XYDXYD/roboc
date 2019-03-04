using JWT;
using UnityEngine;

namespace Assets.Scripts.Services
{
	internal class UnityJsonSerializer : IJsonSerializer
	{
		public string Serialize(object obj)
		{
			return JsonUtility.ToJson(obj);
		}

		public T Deserialize<T>(string json)
		{
			return JsonUtility.FromJson<T>(json);
		}
	}
}
