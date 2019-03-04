using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class PrebuiltRobotRandomColorPicker
	{
		private PaletteColor[] _currentColourCombination;

		private FasterList<PaletteColor[]> _prebuiltRobotColorCombinations = new FasterList<PaletteColor[]>();

		private IServiceRequestFactory _serviceFactory;

		private PremiumMembership _premiumMembership;

		private ColorPaletteData _defaultColorPaletteData;

		public PrebuiltRobotRandomColorPicker(IServiceRequestFactory serviceFactory, PremiumMembership premiumMembership)
		{
			_serviceFactory = serviceFactory;
			_premiumMembership = premiumMembership;
		}

		public IEnumerator LoadData()
		{
			ColorPaletteData defaultColorPaletteData = null;
			ILoadDefaultColorPaletteRequest request = _serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			TaskService<ColorPaletteData> task = new TaskService<ColorPaletteData>(request);
			yield return task;
			if (task.succeeded)
			{
				defaultColorPaletteData = task.result;
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(task.behaviour);
			}
			ILoadPrebuiltRobotColorCombinationsRequest cRequest = _serviceFactory.Create<ILoadPrebuiltRobotColorCombinationsRequest>();
			TaskService<PrebuiltRobotColorCombinations> cTask = new TaskService<PrebuiltRobotColorCombinations>(cRequest);
			yield return cTask;
			if (cTask.succeeded)
			{
				FasterList<byte[]> colors = cTask.result.colors;
				for (int i = 0; i < colors.get_Count(); i++)
				{
					byte[] array = colors.get_Item(i);
					PaletteColor[] array2 = new PaletteColor[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array2[j] = defaultColorPaletteData[array[j]];
					}
					_prebuiltRobotColorCombinations.Add(array2);
				}
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(cTask.behaviour);
			}
		}

		public PaletteColor[] GetRandomColorCombination()
		{
			FasterList<PaletteColor[]> colourCombinationAvailableForPlayer = GetColourCombinationAvailableForPlayer();
			int num = Random.Range(0, colourCombinationAvailableForPlayer.get_Count());
			_currentColourCombination = colourCombinationAvailableForPlayer.get_Item(num);
			return ArrayUtils.Shuffle<PaletteColor>(_currentColourCombination);
		}

		private FasterList<PaletteColor[]> GetColourCombinationAvailableForPlayer()
		{
			FasterList<PaletteColor[]> val = new FasterList<PaletteColor[]>();
			for (int i = 0; i < _prebuiltRobotColorCombinations.get_Count(); i++)
			{
				PaletteColor[] array = _prebuiltRobotColorCombinations.get_Item(i);
				if (IsColourCombinationAvailableForPlayer(array))
				{
					val.Add(array);
				}
			}
			return val;
		}

		private bool IsColourCombinationAvailableForPlayer(PaletteColor[] colours)
		{
			if (_currentColourCombination != null && colours == _currentColourCombination)
			{
				return false;
			}
			for (int i = 0; i < colours.Length; i++)
			{
				if (colours[i].isPremium && !_premiumMembership.hasSubscription)
				{
					return false;
				}
			}
			return true;
		}
	}
}
