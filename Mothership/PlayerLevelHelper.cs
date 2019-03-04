using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class PlayerLevelHelper
	{
		public static IEnumerator LoadCurrentPlayerLevel(IServiceRequestFactory serviceRequestFactory, Action<PlayerLevelAndProgress> OnSuccess, Action OnFailure)
		{
			ILoadTotalXPRequest experienceRequest = serviceRequestFactory.Create<ILoadTotalXPRequest>();
			TaskService<uint[]> experienceTaskService = new TaskService<uint[]>(experienceRequest);
			yield return new HandleTaskServiceWithError(experienceTaskService, delegate
			{
				Console.Log("Load Player Level And Progress: retrieved experience");
			}, delegate
			{
				OnFailure();
			}).GetEnumerator();
			if (experienceTaskService.succeeded)
			{
				int totalPlayerXp = (int)experienceTaskService.result[0];
				ILoadPlayerLevelDataRequest levelDataRequest = serviceRequestFactory.Create<ILoadPlayerLevelDataRequest>();
				TaskService<IDictionary<uint, uint>> levelDataTaskService = new TaskService<IDictionary<uint, uint>>(levelDataRequest);
				yield return new HandleTaskServiceWithError(levelDataTaskService, delegate
				{
					Console.Log("Load Player Level And Progress: retrieved player level ranges");
				}, delegate
				{
					OnFailure();
				}).GetEnumerator();
				if (levelDataTaskService.succeeded)
				{
					IDictionary<uint, uint> levelData = levelDataTaskService.result;
					int playerLevel = CalculateLevelFromXP(levelData, totalPlayerXp);
					float playerLevelProgress = CalculateFractionalProgressToNextLevel(levelData, totalPlayerXp);
					OnSuccess(new PlayerLevelAndProgress((uint)playerLevel, playerLevelProgress));
				}
			}
		}

		private static float CalculateFractionalProgressToNextLevel(IDictionary<uint, uint> levelData, int experience)
		{
			int num = 0;
			for (uint num2 = 1u; num2 < levelData.Count; num2++)
			{
				if (experience < levelData[num2])
				{
					uint num3 = levelData[num2];
					uint num4 = levelData[num2 - 1];
					return Mathf.InverseLerp((float)(double)num4, (float)(double)num3, (float)experience);
				}
				num++;
			}
			return 0.999999f;
		}

		private static int CalculateLevelFromXP(IDictionary<uint, uint> levelData, int experience)
		{
			int num = 0;
			for (uint num2 = 1u; num2 < levelData.Count; num2++)
			{
				if (experience < levelData[num2])
				{
					return num;
				}
				num++;
			}
			return num;
		}
	}
}
