using System;

internal sealed class ForceFlushBonusObserver
{
	public event Action ForceFlush = delegate
	{
	};

	public void Flush()
	{
		this.ForceFlush();
	}
}
