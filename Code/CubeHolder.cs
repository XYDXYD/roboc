using System;
using UnityEngine;

internal class CubeHolder : ICubeHolder
{
	private static CubeTypeID _persistentSelectedCube = CubeTypeID.StandardCubeID;

	protected CubeTypeID _selectedCube;

	private PaletteColor _currentColor = new PaletteColor(0, Color.get_white(), Color.get_white(), Color.get_white(), premium: false);

	private byte _currentPaletteId;

	private int _currentRotation;

	public CubeTypeID selectedCubeID
	{
		get
		{
			return _selectedCube;
		}
		set
		{
			if (_selectedCube != value)
			{
				SetCubeID(value);
				this.OnCubeSelectedChanged(value);
			}
		}
	}

	public int currentRotation
	{
		get
		{
			return _currentRotation;
		}
		set
		{
			_currentRotation = value;
			this.OnRotationChanged(value);
		}
	}

	public byte currentPaletteId
	{
		get
		{
			return _currentPaletteId;
		}
		set
		{
			_currentPaletteId = value;
		}
	}

	public PaletteColor currentColor
	{
		get
		{
			return _currentColor;
		}
		set
		{
			if (_currentColor != value)
			{
				_currentColor = value;
				this.OnColorChanged(value);
			}
		}
	}

	public event Action<CubeTypeID> OnCubeSelectedChanged = delegate
	{
	};

	public event Action<PaletteColor> OnColorChanged = delegate
	{
	};

	public event Action<int> OnRotationChanged = delegate
	{
	};

	public CubeHolder()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		_selectedCube = _persistentSelectedCube;
	}

	protected virtual void SetCubeID(CubeTypeID id)
	{
		_selectedCube = id;
		_persistentSelectedCube = _selectedCube;
	}
}
