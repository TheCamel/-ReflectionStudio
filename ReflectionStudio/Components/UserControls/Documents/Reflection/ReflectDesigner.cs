using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using ReflectionStudio.Controls.Diagram;
using ReflectionStudio.Controls.Helpers;
using ReflectionStudio.Controls.Diagram.Components;

namespace ReflectionStudio.Components.UserControls
{
	public partial class ReflectDesigner : DesignerCanvas
    {
		protected override void OnDrop(DragEventArgs e)
		{
			//base.OnDrop(e);
			DragDropContainer dragObject = e.Data.GetData(typeof(DragDropContainer)) as DragDropContainer;
			if (dragObject != null && dragObject.Data != null )
			{
				ReflectDesignItem newItem = null;
				Object content = dragObject.Data;

				if (content != null)
				{
					newItem = new ReflectDesignItem();
					newItem.Style = (Style)Application.Current.FindResource(typeof(DesignerItem));
					newItem.DataContext = content;

					Point position = e.GetPosition(this);

					if (dragObject.DesiredSize.HasValue)
					{
						Size desiredSize = dragObject.DesiredSize.Value;
						newItem.Width = desiredSize.Width;
						newItem.Height = desiredSize.Height;

						DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
						DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
					}
					else
					{
						DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
						DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
					}

					Canvas.SetZIndex(newItem, this.Children.Count);
					this.Children.Add(newItem);
					SetConnectorDecoratorTemplate(newItem);

					//update selection
					this.SelectionService.SelectItem(newItem);
					newItem.Focus();
				}

				e.Handled = true;
			}
		}
    }
}
