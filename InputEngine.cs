using Svelto.ES.Legacy;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;

internal sealed class InputEngine : IEngine, ITickable, ITickableBase
{
	private readonly InputController _inputController;

	private List<IInputPlugin> _inputPlugins;

	public InputEngine(InputController inputController, params IInputPlugin[] plugins)
	{
		_inputController = inputController;
		_inputPlugins = new List<IInputPlugin>(plugins);
	}

	public Type[] AcceptedComponents()
	{
		return new Type[1]
		{
			typeof(IInputComponent)
		};
	}

	public void Add(IComponent component)
	{
		if (component is IInputComponent)
		{
			for (int i = 0; i < _inputPlugins.Count; i++)
			{
				IInputPlugin inputPlugin = _inputPlugins[i];
				inputPlugin.RegisterComponent(component as IInputComponent);
			}
		}
	}

	public void Remove(IComponent component)
	{
		if (component is IInputComponent)
		{
			for (int i = 0; i < _inputPlugins.Count; i++)
			{
				IInputPlugin inputPlugin = _inputPlugins[i];
				inputPlugin.UnregisterComponent(component as IInputComponent);
			}
		}
	}

	public void Tick(float delta)
	{
		if (_inputController.Enabled)
		{
			for (int i = 0; i < _inputPlugins.Count; i++)
			{
				IInputPlugin inputPlugin = _inputPlugins[i];
				inputPlugin.Execute();
			}
		}
	}
}
