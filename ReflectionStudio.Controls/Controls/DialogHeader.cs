using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// DialogHeader is a header control for dialog that define title, description and image properties
	/// </summary>
	public class DialogHeader : ContentControl
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static DialogHeader()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogHeader),
				new FrameworkPropertyMetadata(typeof(DialogHeader)));
		}

		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// Title DependencyProperty
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
			   DependencyProperty.Register("Title", typeof(string), typeof(DialogHeader));

		/// <summary>
		/// Title property
		/// </summary>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Description DependencyProperty
		/// </summary>
		public static readonly DependencyProperty DescriptionProperty =
			   DependencyProperty.Register("Description", typeof(string), typeof(DialogHeader));

		/// <summary>
		/// Description property
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}

		/// <summary>
		/// Image DependencyProperty
		/// </summary>
		public static readonly DependencyProperty ImageProperty =
			   DependencyProperty.Register("Image", typeof(ImageSource), typeof(DialogHeader));

		/// <summary>
		/// Image property
		/// </summary>
		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		/// <summary>
		/// HasSeparator DependencyProperty
		/// </summary>
		public static readonly DependencyProperty HasSeparatorProperty =
			   DependencyProperty.Register("HasSeparator", typeof(Visibility), typeof(DialogHeader));

		/// <summary>
		/// HasSeparator property
		/// </summary>
		public Visibility HasSeparator
		{
			get { return (Visibility)GetValue(HasSeparatorProperty); }
			set { SetValue(HasSeparatorProperty, value); }
		}
		#endregion
	}
}
