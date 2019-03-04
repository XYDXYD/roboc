using RCNetwork.UNet.Client;
using Svelto.Command;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using Utility;

namespace Mothership
{
	internal sealed class TestConnection : ITickable, ITickableBase
	{
		private enum TestState
		{
			Connecting,
			Connected,
			Failed,
			Passed
		}

		private const int MESSAGE_COUNT = 10;

		private const int REPEAT_COUNT = 4;

		private const int TOTAL_MESSAGE_COUNT = 40;

		private const float TIMEOUT = 10f;

		private float _timer;

		private float _currentTimeout = 10f;

		private float _connectionTime;

		private int _messageCount;

		private TestState _currentState = TestState.Failed;

		private string _hostIp;

		private int _hostPort;

		[Inject]
		internal INetworkInitialisationTestClient networkTest
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		internal void ConnectionResult(bool _result)
		{
			if (_result)
			{
				_connectionTime = _timer;
				if (_currentState == TestState.Failed)
				{
					networkTest.Stop();
				}
				else if (_currentState == TestState.Connecting)
				{
					_currentState = TestState.Connected;
					commandFactory.Build<SendTestDataClientCommand>().Inject(10).Execute();
				}
			}
			else if (_currentState == TestState.Connecting)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["id"] = _hostIp + ":" + _hostPort;
				RemoteLogger.Error("Test Connection : player unable to connect", string.Empty, null, dictionary);
				TestFailed();
			}
			else if (_currentState == TestState.Connected)
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2["id"] = _hostIp + ":" + _hostPort;
				dictionary2["timeSinceConnected"] = (_timer - _connectionTime).ToString();
				dictionary2["timeSinceTestStarted"] = _timer.ToString();
				dictionary2["messageSentCount"] = _messageCount.ToString();
				RemoteLogger.Error("Test Connection : player disconnected", string.Empty, null, dictionary2);
				TestFailed();
			}
		}

		internal void MessageReceived()
		{
			if (_currentState == TestState.Failed)
			{
				networkTest.Stop();
				return;
			}
			_messageCount++;
			if (_messageCount >= 40)
			{
				TestPassed();
			}
			else if (_messageCount % 10 == 0)
			{
				commandFactory.Build<SendTestDataClientCommand>().Inject(10).Execute();
			}
		}

		public void Tick(float deltaSec)
		{
			if (IsTestRunning())
			{
				_timer += deltaSec;
				if (_timer >= _currentTimeout)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["id"] = _hostIp + ":" + _hostPort;
					dictionary["state"] = _currentState.ToString();
					dictionary["timeToConnect"] = _connectionTime.ToString();
					dictionary["messageSentCount"] = _messageCount.ToString();
					RemoteLogger.Error("Test Connection : timeout connection test", string.Empty, null, dictionary);
					Console.Log("Test Connection : timeout connection test");
					TestFailed();
				}
			}
		}

		internal void StartTest(string hostIP, int hostPort, NetworkConfig networkConfig, byte[] encryptionParams)
		{
			if (string.IsNullOrEmpty(hostIP))
			{
				string text = "Starting connection test against invalid server address";
				if (hostIP != null)
				{
					Console.LogError($"{text} Address was: {hostIP}");
				}
				else
				{
					Console.LogError($"{text} Address was null");
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("address", hostIP);
				Dictionary<string, string> data = dictionary;
				RemoteLogger.Error(text, null, null, data);
			}
			inputController.OnScreenStateChange += InputController_OnScreenStateChange;
			_currentState = TestState.Connecting;
			_messageCount = 0;
			_connectionTime = 0f;
			_timer = 0f;
			networkTest.SetEncryptionParams(encryptionParams);
			networkTest.Start(hostIP, hostPort, networkConfig);
			_hostIp = hostIP;
			_hostPort = hostPort;
			if (ClientConfigData.TryGetValue("GameServerConnectionTestTimeout", out long value))
			{
				_currentTimeout = value;
			}
		}

		private void InputController_OnScreenStateChange()
		{
			if (IsTestRunning())
			{
				networkTest.Stop();
				_currentState = TestState.Failed;
				inputController.OnScreenStateChange -= InputController_OnScreenStateChange;
			}
		}

		private bool IsTestRunning()
		{
			return _currentState == TestState.Connecting || _currentState == TestState.Connected;
		}

		private void TestPassed()
		{
			commandFactory.Build<NotifyTestResultClientCommand>().Inject(dependency: true).Execute();
			_currentState = TestState.Passed;
			networkTest.Stop();
			inputController.OnScreenStateChange -= InputController_OnScreenStateChange;
			Console.Log("timeToConnect" + _connectionTime.ToString());
			Console.Log("timeToCompleteTest" + _timer.ToString());
			Console.Log("Test Connection passed");
		}

		private void TestFailed()
		{
			Console.LogError("Connection test failed");
			commandFactory.Build<NotifyTestResultClientCommand>().Inject(dependency: false).Execute();
			_currentState = TestState.Failed;
			networkTest.Stop();
			inputController.OnScreenStateChange -= InputController_OnScreenStateChange;
		}
	}
}
