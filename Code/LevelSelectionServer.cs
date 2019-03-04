using System;
using UnityEngine;
using Utility;

internal class LevelSelectionServer
{
	private string _level = string.Empty;

	public string GetActiveLevel()
	{
		if (_level.Length > 0)
		{
			return _level;
		}
		return string.Empty;
	}

	public void SetLevelActive(string level)
	{
		Console.Log("Activating game objects for level " + level);
		LevelSelectionServerBehaviour levelSelectionServerBehaviour = Object.FindObjectOfType<LevelSelectionServerBehaviour>();
		_level = level;
		for (int i = 0; i < levelSelectionServerBehaviour.levelRoots.Length; i++)
		{
			if (levelSelectionServerBehaviour.levelRoots[i].get_name() == level)
			{
				levelSelectionServerBehaviour.levelRoots[i].SetActive(true);
				return;
			}
		}
		throw new Exception("Failed to find gameObject for " + level);
	}
}
