using Svelto.IoC;

namespace Mothership
{
	internal class CubePrerequisites : ICubePrerequisites
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

		public bool CanUseCube(CubeTypeID cubeId, out string errorString)
		{
			CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(cubeId);
			CubeTypeID requiredCubeId = cubeTypeData.requiredCubeId;
			if (requiredCubeId != 0u && !cubeInventory.IsCubeOwned(requiredCubeId))
			{
				string @string = StringTableBase<StringTable>.Instance.GetString(cubeList.CubeTypeDataOf(requiredCubeId).nameStrKey);
				errorString = StringTableBase<StringTable>.Instance.GetReplaceString("strCubeLocked", "{BaseItem}", @string);
				return false;
			}
			errorString = null;
			return true;
		}

		public bool CanUseCube(CubeTypeID cubeId)
		{
			string errorString;
			return CanUseCube(cubeId, out errorString);
		}
	}
}
