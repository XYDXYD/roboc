using System;

internal struct Sanction
{
	public readonly SanctionType SanctionType;

	public readonly string Reason;

	public readonly DateTimeOffset Issued;

	public readonly int? Duration;

	public Sanction(SanctionType type, string reason)
	{
		this = new Sanction(type, reason, DateTimeOffset.Now, null);
	}

	public Sanction(SanctionType type, string reason, DateTimeOffset issued, int? duration)
	{
		SanctionType = type;
		Reason = (reason ?? string.Empty);
		Issued = issued;
		Duration = duration;
	}
}
