using Authentication;
using Svelto.Command;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal abstract class BattleLoadProgress
{
	public delegate void ProgressUpdateHandler(string playerName, float progress);

	protected const int MapWeight = 5;

	protected const int MachinesWeight = 3;

	private float _latestProgress;

	private double _lastUpdate;

	private const float UpdateIntervalSecs = 0.5f;

	private readonly Dictionary<string, float> _playerProgressDictionary = new Dictionary<string, float>();

	protected UpdateMonoRunner _monoRunner;

	[Inject]
	internal ICommandFactory CommandFactory
	{
		get;
		set;
	}

	public float CurrentProgress
	{
		get;
		private set;
	}

	public bool IsComplete => MapLoaded && MachinesLoaded;

	protected abstract bool MapLoaded
	{
		get;
	}

	protected abstract bool MachinesLoaded
	{
		get;
	}

	protected abstract float MapLoadProgress
	{
		get;
	}

	protected abstract float MachineLoadProgress
	{
		get;
	}

	public event ProgressUpdateHandler PlayerLoadProgressUpdateEvent;

	public void SetPlayerProgress(string userName, float progress)
	{
		_playerProgressDictionary[userName] = progress;
		if (this.PlayerLoadProgressUpdateEvent != null)
		{
			this.PlayerLoadProgressUpdateEvent(userName, progress);
		}
	}

	protected void StartPollingProgress()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		_monoRunner = new UpdateMonoRunner("BattleLoadProgress");
		TaskRunner.get_Instance().RunOnSchedule(_monoRunner, CalculateProgressAndBroadcast());
	}

	protected IEnumerator CalculateProgressAndBroadcast()
	{
		while (_latestProgress < 1f)
		{
			if ((double)Time.get_time() - _lastUpdate > 0.5)
			{
				_latestProgress = CalculateTotalProgress();
				if (_latestProgress >= CurrentProgress)
				{
					BroadcastUpdate(_latestProgress);
					CurrentProgress = _latestProgress;
					SetPlayerProgress(User.Username, CurrentProgress);
				}
				_lastUpdate = Time.get_time();
			}
			yield return null;
		}
		Console.Log("Loading complete");
		_monoRunner.StopAllCoroutines();
	}

	private void BroadcastUpdate(float latestProgress)
	{
		Console.Log($"Loading progress at {latestProgress * 100f}%");
		CommandFactory.Build<BroadcastLoadingProgressClientCommand>().Inject(new LoadingProgressDependency(latestProgress)).Execute();
	}

	private float CalculateTotalProgress()
	{
		Console.Log("MapLoadProgress: " + MapLoadProgress);
		Console.Log("MachineLoadProgress: " + MachineLoadProgress);
		int num = 8;
		float num2 = 5f * MapLoadProgress;
		float num3 = 3f * MachineLoadProgress;
		float num4 = num2 + num3;
		return num4 / (float)num;
	}
}
