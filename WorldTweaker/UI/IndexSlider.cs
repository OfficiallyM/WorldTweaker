using System.Collections.Generic;
using UnityEngine;
using WorldTweaker.Core;

namespace WorldTweaker.UI
{
	internal sealed class IndexSlider<T>
	{
		public string Title { get; }
		public string Description { get; }
		public List<OptionSlider<T>> Options { get; }
		public int SelectedIndex { get; private set; }

		public T Value => Options[SelectedIndex].Value;

		public IndexSlider(string title, List<OptionSlider<T>> options, int defaultIndex)
		{
			Title = title;
			Options = options;
			SelectedIndex = defaultIndex;
		}

		public IndexSlider(string title, string description, List<OptionSlider<T>> options, int defaultIndex)
		{
			Title = title;
			Description = description;
			Options = options;
			SelectedIndex = defaultIndex;
		}

		public void Render()
		{
			GUILayout.Label(Title, "LabelSubHeaderCenter");
			if (!string.IsNullOrEmpty(Description))
				GUILayout.Label(Description);
			SelectedIndex = Mathf.RoundToInt(GUILayout.HorizontalSlider(SelectedIndex, 0, Options.Count - 1));
			GUILayout.Label(Options[SelectedIndex].Label, "LabelCenter");
			GUILayout.Space(10);
		}

		public void SetValue(T value)
		{
			for (int i = 0; i < Options.Count; i++)
			{
				if (EqualityComparer<T>.Default.Equals(Options[i].Value, value))
				{
					SelectedIndex = i;
					return;
				}
			}
		}
	}
}
