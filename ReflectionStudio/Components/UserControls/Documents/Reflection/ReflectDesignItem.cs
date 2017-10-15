using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReflectionStudio.Controls.Diagram.Components;
using System.Windows.Controls;
using System.Windows;

namespace ReflectionStudio.Components.UserControls
{
	class ReflectDesignItem : DesignerItem
	{
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Control clearFilterButton = base.GetTemplateChild("PART_ContentPresenter") as Control;
			if (clearFilterButton != null)
			{
				clearFilterButton.Template = (ControlTemplate)Application.Current.FindResource("ClassItemTemplate");
			}
		}
	}
}
