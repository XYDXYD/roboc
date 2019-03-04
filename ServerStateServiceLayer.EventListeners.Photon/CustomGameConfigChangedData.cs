namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal class CustomGameConfigChangedData
	{
		public readonly string FieldChanged;

		public readonly string NewValue;

		public CustomGameConfigChangedData(string fieldChanged_, string newValue_)
		{
			FieldChanged = fieldChanged_;
			NewValue = newValue_;
		}
	}
}
