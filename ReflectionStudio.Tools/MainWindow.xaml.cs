using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReflectionStudio.Controls.Helpers;

namespace ReflectionStudio.Tools
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool bCapturing;
		private System.Windows.Forms.PropertyGrid _prop = null;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CreatePropertyGrid();

			//MenuItemThemes.DataContext = ThemeManager.Instance.Themes;
			//ThemeManager.Instance.LoadThemeSkin(Application.Current, ThemeManager.Instance.Themes.Skins[0]);
			//ThemeManager.Instance.LoadThemeColor(Application.Current, ThemeManager.Instance.Themes.Colors[0]);
		}

		void CreatePropertyGrid()
		{
			_prop = new System.Windows.Forms.PropertyGrid() { SelectedObject = this };
			windowsFormsHost1.Child = _prop;
		}

		private void MenuItem_ClickQuit(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MenuItem_ClickSkin(object sender, RoutedEventArgs e)
		{
			//DictionaryDefinition res = (DictionaryDefinition)((MenuItem)sender).DataContext;
			//ThemeManager.Instance.LoadThemeSkin(Application.Current, res);
		}

		private void MenuItem_ClickColor(object sender, RoutedEventArgs e)
		{
			//DictionaryDefinition res = (DictionaryDefinition)((MenuItem)sender).DataContext;
			//ThemeManager.Instance.LoadThemeColor(Application.Current, res);
		}

		private void buttonCapture_Click(object sender, RoutedEventArgs e)
		{
			bCapturing = true;
		}

		private void windowsFormsHost1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (bCapturing)
			{
				IInputElement elem = Mouse.DirectlyOver;
				_prop.SelectedObject = elem;

				FrameworkElement selection = elem as FrameworkElement;
				this.textBlockName.Text = selection.GetType().ToString() + selection.Name;

				bCapturing = false;
			}
		}
	}
}
