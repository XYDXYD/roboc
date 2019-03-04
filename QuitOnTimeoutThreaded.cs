using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.Utilities;
using System;
using System.Collections;
using System.Threading;
using Utility;

internal class QuitOnTimeoutThreaded : IDisposable
{
	private const int PreQuitTimeoutSeconds = 3300;

	private const int QuitTimeoutSeconds = 300;

	private const int RepeatIntervalSeconds = 300;

	private bool _hasApplicationQuit;

	private DateTime _futureTime;

	private WaitForSecondsEnumerator _waitForIt;

	private ITaskRoutine _task;

	public event Action WillQuitEvent;

	public QuitOnTimeoutThreaded()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		_futureTime = DateTime.Now.AddSeconds(3300.0);
		_waitForIt = new WaitForSecondsEnumerator(300f);
		MultiThreadRunner scheduler = new MultiThreadRunner("GameServerQuitThread", true);
		_task = TaskRunner.get_Instance().AllocateNewTaskRoutine();
		_task.SetScheduler(scheduler).SetEnumerator(QuitOnTimeout()).ThreadSafeStart((Action<PausableTaskException>)null, (Action)null);
	}

	public void Reset()
	{
		_futureTime = DateTime.Now.AddSeconds(3300.0);
		this.WillQuitEvent = null;
	}

	public void Dispose()
	{
		Console.Log("Application Quit Requested");
		_hasApplicationQuit = true;
		ThreadUtility.MemoryBarrier();
	}

	private IEnumerator QuitOnTimeout()
	{
		while (!_hasApplicationQuit && DateTime.Now < _futureTime)
		{
			Thread.Sleep(1000);
			yield return null;
		}
		if (!_hasApplicationQuit && this.WillQuitEvent != null)
		{
			MainThreadCaller.get_Instance().PushAction((Action)delegate
			{
				if (this.WillQuitEvent != null)
				{
					this.WillQuitEvent();
				}
				else
				{
					Console.LogWarning("WillQuitEvent is null, but it shouldn't have been!");
				}
			});
		}
		DateTime newFuture = DateTime.Now.AddSeconds(300.0);
		while (!_hasApplicationQuit && DateTime.Now < newFuture)
		{
			Thread.Sleep(1000);
			yield return null;
		}
		if (!_hasApplicationQuit)
		{
			Console.Log("[quit timeout thread] timed out");
			ExitApplication();
		}
	}

	private void ExitApplication()
	{
		_task.Stop();
		Console.Log("[quit timeout thread] exiting application");
		Environment.Exit(0);
	}
}
