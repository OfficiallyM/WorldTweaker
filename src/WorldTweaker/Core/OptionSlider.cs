namespace WorldTweaker.Core
{
	internal sealed class OptionSlider<T>
	{
		public T Value { get; }
		public string Label { get; }
		public string Description { get; }

		public OptionSlider(T value, string label, string description = null)
		{
			Value = value;
			Label = label;
			Description = description;
		}
	}
}
