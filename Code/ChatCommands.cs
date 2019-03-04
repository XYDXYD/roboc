using ExtensionMethods;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Utility;

internal class ChatCommands
{
	internal struct CommandData
	{
		public enum AccessLevel
		{
			DevOnly,
			AdminOrDev,
			ModAdminOrDev,
			All
		}

		public readonly string UsageInstructions;

		private readonly AccessLevel _level;

		private readonly WeakReference _targetReference;

		private readonly MethodInfo _methodInfo;

		private readonly int _minArgs;

		private readonly int _maxArgs;

		public CommandData(AccessLevel accessLevel, Func<string, bool> callBack, string usageInstructions, int minArgs, int maxArgs)
		{
			_level = accessLevel;
			_targetReference = new WeakReference(callBack.Target);
			_methodInfo = callBack.Method;
			UsageInstructions = usageInstructions;
			_minArgs = minArgs;
			_maxArgs = maxArgs;
		}

		public bool CanUseCommand(AccountRights accountRights)
		{
			switch (_level)
			{
			case AccessLevel.ModAdminOrDev:
				return accountRights.Moderator || accountRights.Admin || accountRights.Developer;
			case AccessLevel.AdminOrDev:
				return accountRights.Admin || accountRights.Developer;
			case AccessLevel.DevOnly:
				return accountRights.Developer;
			default:
				return true;
			}
		}

		public bool Execute(string input)
		{
			if (!NumParamsValid(input))
			{
				return false;
			}
			object target = _targetReference.Target;
			return (bool)_methodInfo.Invoke(target, new string[1]
			{
				input
			});
		}

		private bool NumParamsValid(string input)
		{
			int num = (!input.IsNullOrWhiteSpace()) ? input.Split(' ').Length : 0;
			return num >= _minArgs && num <= _maxArgs;
		}
	}

	private readonly Dictionary<string, CommandData> _registeredCommands = new Dictionary<string, CommandData>(StringComparer.OrdinalIgnoreCase);

	[CompilerGenerated]
	private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	[Inject]
	public IServiceRequestFactory serviceRequestFactory
	{
		private get;
		set;
	}

	public ChatCommands()
	{
		RegisterCommand("help", CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strNoInstructions"), DisplayHelp);
	}

	public static void GetParam(string input, out string param, out string remainder)
	{
		int num = input.IndexOf(" ", StringComparison.Ordinal);
		if (num != -1)
		{
			param = input.Substring(0, num);
			remainder = input.Substring(num + 1);
		}
		else
		{
			param = input;
			remainder = string.Empty;
		}
	}

	public void ProcessCommand(string input)
	{
		GetAccountRights(delegate(AccountRights accountRights)
		{
			if (!ProcessCommand_Internal(input, accountRights))
			{
				chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strInvalidCommand"));
			}
		});
	}

	public void RegisterCommand(string commandName, CommandData.AccessLevel accessLevel, string instructions, Func<string, bool> callBack, int minArgs = 0, int maxArgs = int.MaxValue)
	{
		commandName = commandName.ToLower();
		CommandData value = new CommandData(accessLevel, callBack, instructions, minArgs, maxArgs);
		_registeredCommands.Add(commandName, value);
	}

	public void DeregisterCommand(string commandName)
	{
		if (_registeredCommands.ContainsKey(commandName))
		{
			_registeredCommands.Remove(commandName);
		}
		else
		{
			Console.LogWarning($"Command is not currently registered: {commandName}");
		}
	}

	internal void CustomError(string error)
	{
		chatPresenter.SystemMessage(error);
	}

	private bool DisplayHelp(string input)
	{
		GetAccountRights(delegate(AccountRights accountRights)
		{
			StringBuilder stringBuilder = new StringBuilder(StringTableBase<StringTable>.Instance.GetString("strAvailableCommands"));
			stringBuilder.Append("\n");
			foreach (KeyValuePair<string, CommandData> registeredCommand in _registeredCommands)
			{
				if (registeredCommand.Value.CanUseCommand(accountRights))
				{
					stringBuilder.Append(registeredCommand.Key);
					stringBuilder.Append(", ");
				}
			}
			chatPresenter.SystemMessage(stringBuilder.ToString());
		});
		return true;
	}

	private bool ProcessCommand_Internal(string input, AccountRights accountRights)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		string param = null;
		string remainder = null;
		GetParam(input, out param, out remainder);
		param = param.ToLower();
		if (_registeredCommands.TryGetValue(param, out CommandData value))
		{
			if (value.CanUseCommand(accountRights))
			{
				if (!value.Execute(remainder))
				{
					chatPresenter.SystemMessage(StringTableBase<StringTable>.Instance.GetString("strCommandUsage") + " " + value.UsageInstructions);
				}
				return true;
			}
			return false;
		}
		return false;
	}

	private void GetAccountRights(Action<AccountRights> onComplete)
	{
		serviceRequestFactory.Create<IGetAccountRightsRequest>().SetAnswer(new ServiceAnswer<AccountRights>(onComplete, ErrorWindow.ShowServiceErrorWindow)).Execute();
	}
}
