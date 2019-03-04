internal static class UIAnchorUtility
{
	public class Anchors
	{
		public Pivot pivot;

		public AnchorPoint leftAnchor;

		public AnchorPoint rightAnchor;

		public AnchorPoint topAnchor;

		public AnchorPoint bottomAnchor;
	}

	public const int TOP_BOTTOM = 12;

	public static void CopyAnchors(UIWidget from, UIWidget to, int mask = 15)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		to.set_pivot(from.get_pivot());
		if ((mask & 1) != 0)
		{
			CopyAnchor(from.leftAnchor, to.leftAnchor);
		}
		if ((mask & 2) != 0)
		{
			CopyAnchor(from.rightAnchor, to.rightAnchor);
		}
		if ((mask & 4) != 0)
		{
			CopyAnchor(from.bottomAnchor, to.bottomAnchor);
		}
		if ((mask & 8) != 0)
		{
			CopyAnchor(from.topAnchor, to.topAnchor);
		}
		to.ResetAnchors();
		to.UpdateAnchors();
	}

	public static void CopyAnchors(Anchors from, UIWidget to, int mask = 15)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		to.set_pivot(from.pivot);
		if ((mask & 1) != 0)
		{
			CopyAnchor(from.leftAnchor, to.leftAnchor);
		}
		if ((mask & 2) != 0)
		{
			CopyAnchor(from.rightAnchor, to.rightAnchor);
		}
		if ((mask & 4) != 0)
		{
			CopyAnchor(from.bottomAnchor, to.bottomAnchor);
		}
		if ((mask & 8) != 0)
		{
			CopyAnchor(from.topAnchor, to.topAnchor);
		}
		to.ResetAnchors();
		to.UpdateAnchors();
	}

	public static Anchors CloneAnchors(UIWidget w)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Anchors anchors = new Anchors();
		anchors.pivot = w.get_pivot();
		anchors.leftAnchor = CloneAnchor(w.leftAnchor);
		anchors.rightAnchor = CloneAnchor(w.rightAnchor);
		anchors.topAnchor = CloneAnchor(w.topAnchor);
		anchors.bottomAnchor = CloneAnchor(w.bottomAnchor);
		return anchors;
	}

	private static AnchorPoint CloneAnchor(AnchorPoint a)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnchorPoint val = new AnchorPoint();
		val.target = a.target;
		val.absolute = a.absolute;
		val.relative = a.relative;
		return val;
	}

	private static void CopyAnchor(AnchorPoint from, AnchorPoint to)
	{
		to.target = from.target;
		to.relative = from.relative;
		to.absolute = from.absolute;
	}
}
