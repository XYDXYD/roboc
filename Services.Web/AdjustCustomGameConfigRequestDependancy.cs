namespace Services.Web
{
	public class AdjustCustomGameConfigRequestDependancy
	{
		public readonly string Field;

		public readonly string NewValue;

		public AdjustCustomGameConfigRequestDependancy(string field_, string newValue_)
		{
			Field = field_;
			NewValue = newValue_;
		}
	}
}
