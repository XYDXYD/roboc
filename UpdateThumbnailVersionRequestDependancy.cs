internal struct UpdateThumbnailVersionRequestDependancy
{
	public UniqueSlotIdentifier slotIdentifier;

	public uint thumbnailVersion;

	public uint slotId;

	public UpdateThumbnailVersionRequestDependancy(UniqueSlotIdentifier garageSlot, uint slotIdLcl, uint version)
	{
		slotIdentifier = garageSlot;
		thumbnailVersion = version;
		slotId = slotIdLcl;
	}
}
