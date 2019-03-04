using System;
using UnityEngine;

namespace RCNetwork.Events
{
	internal sealed class HostListEventArgs : EventArgs
	{
		public HostData[] hostData;

		public HostListEventArgs(HostData[] _hostData)
		{
			hostData = _hostData;
		}
	}
}
