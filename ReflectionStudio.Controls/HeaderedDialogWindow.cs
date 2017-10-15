using System.Windows;
using System.Windows.Media;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// Add a header that contains title, description and image to the DialogWindow class
	/// </summary>
	public class HeaderedDialogWindow : DialogWindow
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static HeaderedDialogWindow()
        {
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
						typeof(HeaderedDialogWindow), new FrameworkPropertyMetadata(typeof(HeaderedDialogWindow)));
        }

		/// <summary>
		/// Constructor
		/// </summary>
		public HeaderedDialogWindow() : base()
		{
		}

		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// DialogDescription DependencyProperty
		/// </summary>
		public static readonly DependencyProperty DialogDescriptionProperty =
			   DependencyProperty.Register("DialogDescription", typeof(string), typeof(HeaderedDialogWindow));

		/// <summary>
		/// DialogDescription property
		/// </summary>
		public string DialogDescription
		{
			get { return (string)GetValue(DialogDescriptionProperty); }
			set { SetValue(DialogDescriptionProperty, value); }
		}

		/// <summary>
		/// DialogImage DependencyProperty
		/// </summary>
		public static readonly DependencyProperty DialogImageProperty =
			   DependencyProperty.Register("DialogImage", typeof(ImageSource), typeof(HeaderedDialogWindow));

		/// <summary>
		/// DialogImage property
		/// </summary>
		public ImageSource DialogImage
		{
			get { return (ImageSource)GetValue(DialogImageProperty); }
			set { SetValue(DialogImageProperty, value); }
		}

		#endregion
	}
}
