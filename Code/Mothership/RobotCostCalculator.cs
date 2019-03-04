using Svelto.IoC;
using System.Collections.Generic;

namespace Mothership
{
	public class RobotCostCalculator
	{
		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		internal void GetListOfUnlockedCubes(HashSet<uint> cubeTypesInRobot, out Dictionary<uint, SunkCube> cubeTypesThatAreLockedForPlayer)
		{
			cubeTypesThatAreLockedForPlayer = new Dictionary<uint, SunkCube>();
			foreach (uint item in cubeTypesInRobot)
			{
				if (!cubeInventory.OwnedCubes.Contains(item))
				{
					CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(new CubeTypeID(item));
					cubeTypesThatAreLockedForPlayer[item] = new SunkCube(StringTableBase<StringTable>.Instance.GetString(cubeTypeData.nameStrKey));
				}
			}
		}

		internal Dictionary<CubeTypeID, SunkCube> GetRobotCubesInfoFromIDs(List<uint> lockedCubesList)
		{
			Dictionary<CubeTypeID, SunkCube> dictionary = new Dictionary<CubeTypeID, SunkCube>();
			int count = lockedCubesList.Count;
			for (int i = 0; i < count; i++)
			{
				CubeTypeID cubeTypeID = lockedCubesList[i];
				CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(cubeTypeID);
				string @string = StringTableBase<StringTable>.Instance.GetString(cubeTypeData.nameStrKey);
				SunkCube value = new SunkCube(@string);
				dictionary.Add(cubeTypeID, value);
			}
			return dictionary;
		}
	}
}
