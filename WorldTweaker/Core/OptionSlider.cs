namespace WorldTweaker.Core
{
	internal sealed class OptionSlider<T>
	{
		public T Value { get; }
		public string Label { get; }

		public OptionSlider(T value, string label)
		{
			Value = value;
			Label = label;
		}
	}
}
