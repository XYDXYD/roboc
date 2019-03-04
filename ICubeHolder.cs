using System;

internal interface ICubeHolder
{
	CubeTypeID selectedCubeID
	{
		get;
		set;
	}

	int currentRotation
	{
		get;
		set;
	}

	byte currentPaletteId
	{
		get;
		set;
	}

	PaletteColor currentColor
	{
		get;
		set;
	}

	event Action<CubeTypeID> OnCubeSelectedChanged;

	event Action<PaletteColor> OnColorChanged;

	event Action<int> OnRotationChanged;
}
