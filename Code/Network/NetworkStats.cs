using LiteNetLib;
using System;

namespace Network
{
	internal class NetworkStats
	{
		private class Stat
		{
			public string _name;

			public ulong _delta;

			private ulong _lastVal;

			public Stat(string name)
			{
				_name = name;
			}

			public void Update(ulong value)
			{
				_delta = value - _lastVal;
				_lastVal = value;
			}
		}

		private class AverageStat
		{
			private const int HistorySize = 100;

			public string _name;

			private long[] _history = new long[100];

			private int _count;

			public AverageStat(string name)
			{
				_name = name;
			}

			public void Update(long value)
			{
				if (_count == 100)
				{
					Array.Copy(_history, 1, _history, 0, 99);
					_count--;
				}
				_history[_count] = value;
				_count++;
			}

			public override string ToString()
			{
				string text = _name + ": ";
				long num = 0L;
				long val = 0L;
				long val2 = 0L;
				if (_count != 0)
				{
					val = (val2 = _history[0]);
					for (int i = 0; i < _count; i++)
					{
						long num2 = _history[i];
						val = Math.Min(val, num2);
						val2 = Math.Max(val2, num2);
						num += num2;
					}
					num /= _count;
				}
				string text2 = text;
				return text2 + "(" + val.ToString() + "/" + val2.ToString() + "/" + num.ToString() + ")";
			}
		}

		private BetterList<Stat> _stats = new BetterList<Stat>();

		private ulong _updateMicrosec;

		private ulong _lastMicrosec;

		private ulong _lastOutgoing;

		private ulong _lastPending;

		private Stat _time = new Stat("TimeUsec");

		private Stat _rxPackets = new Stat("RXPackets");

		private Stat _txPackets = new Stat("TXPackets");

		private Stat _rxBytes = new Stat("RXBytes");

		private Stat _txBytes = new Stat("TXBytes");

		private BetterList<AverageStat> _avgStats = new BetterList<AverageStat>();

		private AverageStat _avgOutgoing = new AverageStat("Outgoing");

		private AverageStat _avgPending = new AverageStat("Pending");

		private AverageStat _avgTotalPacketsSent = new AverageStat("Sent");

		private AverageStat _avgUS = new AverageStat("US");

		private DateTime _lastReport;

		public NetworkStats()
		{
			_lastReport = DateTime.UtcNow;
			AddValues();
		}

		public void Update(NetManager manager, long updateMicrosec, long totalOutgoing, long totalPending, long totalPacketsSent)
		{
			_updateMicrosec += (ulong)updateMicrosec;
			_avgOutgoing.Update(totalOutgoing);
			_avgPending.Update(totalPending);
			_avgTotalPacketsSent.Update(totalPacketsSent);
			_avgUS.Update(updateMicrosec);
			DateTime utcNow = DateTime.UtcNow;
			double totalMilliseconds = (utcNow - _lastReport).TotalMilliseconds;
			if (totalMilliseconds >= 1000.0)
			{
				_lastReport = utcNow;
				LogValues(manager);
				Report(manager, (ulong)totalMilliseconds);
			}
		}

		private void Report(NetManager manager, ulong timeDelta)
		{
			string text = string.Empty;
			float num = 1000f / (float)(double)timeDelta;
			foreach (Stat stat in _stats)
			{
				int num2 = (int)((float)(double)stat._delta * num);
				string text2 = text;
				text = text2 + stat._name + ": " + num2.ToString() + "/sec ";
			}
			text += "min/max/avg ";
			foreach (AverageStat avgStat in _avgStats)
			{
				text = text + avgStat.ToString() + " ";
			}
			NetworkLogger.Log(manager.get_LocalEndPoint() + ": " + text);
		}

		private void AddValues()
		{
			_stats.Add(_time);
			_stats.Add(_rxPackets);
			_stats.Add(_rxBytes);
			_stats.Add(_txPackets);
			_stats.Add(_txBytes);
			_avgStats.Add(_avgOutgoing);
			_avgStats.Add(_avgPending);
			_avgStats.Add(_avgTotalPacketsSent);
			_avgStats.Add(_avgUS);
		}

		private void LogValues(NetManager manager)
		{
			_time.Update(_updateMicrosec);
			_rxPackets.Update(manager.get_PacketsReceived());
			_txPackets.Update(manager.get_PacketsSent());
			_rxBytes.Update(manager.get_BytesReceived());
			_txBytes.Update(manager.get_BytesSent());
		}
	}
}
