using System.IO;

public class AutoRegenHealthSettingsData
{
	public float secondsToWaitForHeal;

	public float secondsToFullHeal;

	public float thresholdToStartSound;

	public bool enableAutoHeal;

	public AutoRegenHealthSettingsData(float secondsToWaitForHeal, float secondsToFullHeal, float thresholdToStartSound, bool enabled)
	{
		this.secondsToWaitForHeal = secondsToWaitForHeal;
		this.secondsToFullHeal = secondsToFullHeal;
		this.thresholdToStartSound = thresholdToStartSound;
		enableAutoHeal = enabled;
	}

	public AutoRegenHealthSettingsData(AutoRegenHealthSettingsData toCopy)
	{
		secondsToWaitForHeal = toCopy.secondsToWaitForHeal;
		secondsToFullHeal = toCopy.secondsToFullHeal;
		thresholdToStartSound = toCopy.thresholdToStartSound;
		enableAutoHeal = toCopy.enableAutoHeal;
	}

	public AutoRegenHealthSettingsData(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				secondsToWaitForHeal = binaryReader.ReadSingle();
				secondsToFullHeal = binaryReader.ReadSingle();
				thresholdToStartSound = binaryReader.ReadSingle();
				enableAutoHeal = binaryReader.ReadBoolean();
			}
		}
	}
}
