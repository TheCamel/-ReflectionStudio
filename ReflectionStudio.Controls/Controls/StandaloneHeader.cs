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
	/// StandaloneHeader is a header control that define title, description and image properties
	/// </summary>
	public class StandaloneHeader : ContentControl
	{
		/// <summary>
		/// Constructor
		/// </summary>
		static StandaloneHeader()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(StandaloneHeader),
				new FrameworkPropertyMetadata(typeof(StandaloneHeader)));
		}

		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// Title DependencyProperty
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
			   DependencyProperty.Register("Title", typeof(string), typeof(StandaloneHeader));

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
			   DependencyProperty.Register("Description", typeof(string), typeof(StandaloneHeader));

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
			   DependencyProperty.Register("Image", typeof(ImageSource), typeof(StandaloneHeader));

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
			   DependencyProperty.Register("HasSeparator", typeof(Visibility), typeof(StandaloneHeader));

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
