using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using ReflectionStudio.Core.Helpers;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// A control that displays text, and when clicked, navigates somehwere else on the web.
	/// </summary>
	[TemplatePart(Name = "PART_Link", Type = typeof(Hyperlink))]
	public class WebLink : Control
	{
		#region --------------------CONSTRUCTORS--------------------

		/// <summary>
		/// Web link constructor.
		/// </summary>
		static WebLink()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WebLink),
				new FrameworkPropertyMetadata(typeof(WebLink)));
		}

		/// <summary>
		/// To retreive and link with parts
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Hyperlink elem = this.GetTemplateChild("PART_Link") as Hyperlink;
			elem.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(elem_RequestNavigate);
		}

		/// <summary>
		/// Navigates to the uri.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void elem_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.Uri.OriginalString))
			{
				ProcessHelper.LaunchWebUri(this.Uri);
			}
		}

		#endregion

		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// The text property.
		/// </summary>
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(WebLink), null);

		/// <summary>
		/// Gets or sets the link text.
		/// </summary>
		[System.ComponentModel.Category("Common Properties"), System.ComponentModel.Description("The text content property.")]
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		/// <summary>
		/// The text decorations property.
		/// </summary>
		public static readonly DependencyProperty TextDecorationsProperty =
			DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(WebLink), null);

		/// <summary>
		/// Gets or sets the text decorations.
		/// </summary>
		[System.ComponentModel.Category("Common Properties"), System.ComponentModel.Description("The text decoration property.")]
		public TextDecorationCollection TextDecorations
		{
			get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}

		/// <summary>
		/// The text wrapping property.
		/// </summary>
		public static readonly DependencyProperty TextWrappingProperty =
			DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(WebLink), null);

		/// <summary>
		/// Gets or sets the text wrapping.
		/// </summary>
		[System.ComponentModel.Category("Common Properties"), System.ComponentModel.Description("The text wrapping property.")]
		public TextWrapping TextWrapping
		{
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}

		/// <summary>
		/// The link Uri property.
		/// </summary>
		public static readonly DependencyProperty UriProperty =
			DependencyProperty.Register("Uri", typeof(Uri), typeof(WebLink), null);

		/// <summary>
		/// Gets or sets the uri.
		/// </summary>
		[System.ComponentModel.Category("Common Properties"), System.ComponentModel.Description("The web Uri property.")]
		public Uri Uri
		{
			get { return (Uri)GetValue(UriProperty); }
			set { SetValue(UriProperty, value); }
		}

		#endregion
	}
}
