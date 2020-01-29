using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.DualScreen
{
    public interface ILayoutService
	{
		Point? GetLocationOnScreen(VisualElement visualElement);
	}
}