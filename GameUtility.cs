using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

internal static class GameUtility
{
	private const float TRUNCATED_TO_THOUSAND_LIMIT = 10000f;

	private const float TRUNCATED_TO_MILLION_LIMIT = 1000000f;

	private const string STR_FMT = "N0";

	public static void SetLayerRecursively(Transform trans, int layer)
	{
		if (!(trans == null))
		{
			Transform[] componentsInChildren = trans.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].get_gameObject().set_layer(layer);
			}
		}
	}

	public static GameObject FindInGameObject(GameObject go, string name)
	{
		return FindInGameObject(go, name, mustBeEnabled: false);
	}

	public static GameObject FindInGameObject(GameObject go, string name, bool mustBeEnabled)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		IEnumerator enumerator = go.get_transform().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform val = enumerator.Current;
				if (val.get_name() == name && (!mustBeEnabled || val.get_gameObject().get_activeSelf()))
				{
					return val.get_gameObject();
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		IEnumerator enumerator2 = go.get_transform().GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Transform val2 = enumerator2.Current;
				GameObject val3 = FindInGameObject(val2.get_gameObject(), name, mustBeEnabled);
				if (val3 != null)
				{
					return val3;
				}
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		return null;
	}

	public static T GetComponentInAllChildren<T>(GameObject go) where T : Component
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		T component = go.GetComponent<T>();
		if ((object)component != null)
		{
			return component;
		}
		IEnumerator enumerator = go.get_transform().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform val = enumerator.Current;
				component = GetComponentInAllChildren<T>(val.get_gameObject());
				if ((object)component != null)
				{
					return component;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return (T)(object)null;
	}

	public static T[] GetComponentsInAllChildren<T>(GameObject go) where T : Component
	{
		List<T> components = new List<T>();
		GetComponentsInAllChildren(go, ref components);
		return components.ToArray();
	}

	private static void GetComponentsInAllChildren<T>(GameObject go, ref List<T> components) where T : Component
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		T[] components2 = go.GetComponents<T>();
		T[] array = components2;
		foreach (T item in array)
		{
			components.Add(item);
		}
		IEnumerator enumerator = go.get_transform().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform val = enumerator.Current;
				GetComponentsInAllChildren(val.get_gameObject(), ref components);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static T GetComponentInAllParents<T>(Transform transform) where T : Component
	{
		if (transform == null)
		{
			return (T)(object)null;
		}
		T component = transform.GetComponent<T>();
		if ((object)component != null)
		{
			return component;
		}
		return GetComponentInAllParents<T>(transform.get_parent());
	}

	public static string GetRobotRatingString(float rating)
	{
		if (rating < 10000f)
		{
			return $"{rating:n0}";
		}
		if (rating < 1000000f)
		{
			float num = Mathf.Floor(rating / 1000f);
			return $"{num:n0}k";
		}
		float num2 = Mathf.Floor(rating / 1000000f);
		return $"{num2:n0}m";
	}

	public static string UrlWithPath(string url)
	{
		Uri uri = new Uri(url);
		string text = uri.Scheme + "://" + uri.Authority + uri.AbsolutePath;
		int num = text.LastIndexOf("/") + 1;
		return text.Remove(num, text.Length - num);
	}

	public static Vector3 VectorFromPointToLine(Vector3 A, Vector3 B, Vector3 p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = B - A;
		float num = Vector3.SqrMagnitude(val);
		if (num == 0f)
		{
			return A - p;
		}
		float num2 = Vector3.Dot(p - A, val) / num;
		if (num2 < 0f)
		{
			return A - p;
		}
		if (num2 > 1f)
		{
			return B - p;
		}
		Vector3 val2 = A + num2 * val;
		return val2 - p;
	}

	public static GameObject GetMachineBoard(Transform t)
	{
		while (t.get_parent() != null)
		{
			t = t.get_parent();
		}
		return t.get_gameObject();
	}

	public static GameObject GetMachineRoot(Transform t)
	{
		t = t.get_parent();
		while (t != null)
		{
			if (t.GetComponent<Rigidbody>() != null)
			{
				return t.get_gameObject();
			}
			t = t.get_parent();
		}
		return null;
	}

	public static Rigidbody GetMachineRigidbody(Transform t)
	{
		while (t != null)
		{
			Rigidbody component = t.GetComponent<Rigidbody>();
			if (component != null)
			{
				return component;
			}
			t = t.get_parent();
		}
		return null;
	}

	public static Transform GetCubeRoot(Transform t)
	{
		while (t != null)
		{
			CubeInstance component = t.GetComponent<CubeInstance>();
			if (component != null)
			{
				return component.get_transform();
			}
			t = t.get_parent();
		}
		return null;
	}

	public static CubeInstance GetCubeInstance(Transform t)
	{
		while (t != null)
		{
			CubeInstance component = t.GetComponent<CubeInstance>();
			if (component != null)
			{
				return component;
			}
			t = t.get_parent();
		}
		return null;
	}

	public static void UnorderredListRemoveAt<T>(this List<T> list, int index)
	{
		int index2 = list.Count - 1;
		list[index] = list[index2];
		list.RemoveAt(index2);
	}

	public static string ToPrettyString<TKey, TValue>(this IDictionary<TKey, TValue> dict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		foreach (KeyValuePair<TKey, TValue> item in dict)
		{
			stringBuilder.Append($" {item.Key}={item.Value} ");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public static void Shuffle<T>(this FasterList<T> list)
	{
		for (int num = list.get_Count() - 1; num >= 1; num--)
		{
			int num2 = Random.Range(0, num);
			T val = list.get_Item(num2);
			list.set_Item(num2, list.get_Item(num));
			list.set_Item(num, val);
		}
	}

	public static string GetUTF8String(string utf16Str)
	{
		Encoding uTF = Encoding.UTF8;
		byte[] bytes = uTF.GetBytes(utf16Str);
		return uTF.GetString(bytes);
	}

	public static string CommaSeparate(int value)
	{
		return value.ToString("N0", CultureInfo.InvariantCulture);
	}

	public static string CommaSeparate(long value)
	{
		return value.ToString("N0", CultureInfo.InvariantCulture);
	}

	public static Texture2D CreateRobotShopTexture()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		return new Texture2D(216, 116, 3, false);
	}

	public static bool MachineIsOnGround(Rigidbody hitMachine, TargetType targetType, float groundClearance, MachineRootContainer machineRootContainer, MachinePreloader machinePreloader)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (targetType != 0)
		{
			return true;
		}
		Transform transform = hitMachine.get_transform();
		GameObject machineBoard = GetMachineBoard(transform);
		int machineIdFromRoot = machineRootContainer.GetMachineIdFromRoot(targetType, machineBoard);
		PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(machineIdFromRoot);
		if (preloadedMachine == null)
		{
			return true;
		}
		Vector3 machineSize = preloadedMachine.machineInfo.MachineSize;
		float num = groundClearance + machineSize.y * 0.5f;
		bool flag = Physics.Raycast(hitMachine.get_worldCenterOfMass(), Vector3.get_down(), num, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
		if (!flag)
		{
			flag = Physics.Raycast(hitMachine.get_worldCenterOfMass(), -transform.get_up(), num, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
		}
		return flag;
	}

	public static bool IsPlayerInRange(Vector3 playerWorldCOM, Vector3 targetCentre, float targetSqRadius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetCentre - playerWorldCOM;
		float sqrMagnitude = val.get_sqrMagnitude();
		return sqrMagnitude <= targetSqRadius;
	}
}
