using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal sealed class ColorPaletteData
{
	private List<PaletteColor> _colors;

	private byte[] _order;

	private int _regularCount;

	private int _premiumCount;

	public PaletteColor this[int key]
	{
		get
		{
			return _colors[key];
		}
	}

	public int Count => _colors.Count;

	public ColorPaletteData(ColorPaletteData otherPalette)
	{
		_colors = new List<PaletteColor>(otherPalette.Count);
		_regularCount = (_premiumCount = 0);
		for (int i = 0; i < otherPalette.Count; i++)
		{
			PaletteColor paletteColor = otherPalette[i];
			_colors.Add(paletteColor);
			if (paletteColor.isPremium)
			{
				_premiumCount++;
			}
			else
			{
				_regularCount++;
			}
		}
		_order = new byte[otherPalette._order.Length];
		for (int j = 0; j < _order.Length; j++)
		{
			_order[j] = otherPalette._order[j];
		}
	}

	public ColorPaletteData(byte[] serialisedData, byte[] order)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		_regularCount = (_premiumCount = 0);
		_order = order;
		using (MemoryStream input = new MemoryStream(serialisedData))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadInt32();
				_colors = new List<PaletteColor>(num);
				for (int i = 0; i < num; i++)
				{
					byte b = binaryReader.ReadByte();
					byte b2 = binaryReader.ReadByte();
					byte b3 = binaryReader.ReadByte();
					byte b4 = binaryReader.ReadByte();
					byte b5 = binaryReader.ReadByte();
					byte b6 = binaryReader.ReadByte();
					byte b7 = binaryReader.ReadByte();
					byte b8 = binaryReader.ReadByte();
					byte b9 = binaryReader.ReadByte();
					bool flag = binaryReader.ReadBoolean();
					if (flag)
					{
						_premiumCount++;
					}
					else
					{
						_regularCount++;
					}
					_colors.Add(new PaletteColor((byte)i, Color32.op_Implicit(new Color32(b, b2, b3, byte.MaxValue)), Color32.op_Implicit(new Color32(b4, b5, b6, byte.MaxValue)), Color32.op_Implicit(new Color32(b7, b8, b9, byte.MaxValue)), flag));
				}
			}
		}
	}

	public byte GetVisualIndexFromColorIndex(int colorIndex)
	{
		return _order[colorIndex];
	}

	public PaletteColor GetFromVisualIndex(int vi)
	{
		for (int i = 0; i < _order.Length; i++)
		{
			if (_order[i] == vi)
			{
				return this[i];
			}
		}
		throw new Exception("Invalid color visual index " + vi);
	}
}
