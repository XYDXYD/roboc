using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Local
{
	internal static class CacheDTO
	{
		internal static byte[] hintProgresses = null;

		internal static RobotDragData robotDragData = null;

		internal static CircularBufferIndexer<string, Texture2D> robotShopThumbnails = new CircularBufferIndexer<string, Texture2D>(150);

		internal static Dictionary<UniqueSlotIdentifier, uint> slotVersionInfoCache = new Dictionary<UniqueSlotIdentifier, uint>();

		internal static bool lastMatchResult = false;

		internal static bool lastMatchResultIsValid = false;

		internal static HashSet<uint> newInventoryCubes = new HashSet<uint>();
	}
}
