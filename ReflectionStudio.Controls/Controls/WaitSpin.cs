using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ReflectionStudio.Controls
{
	/// <summary>
	/// Enumeration for representing state of an animation.
	/// </summary>
	public enum AnimationState
	{
		/// <summary>
		/// The animation is playing.
		/// </summary>
		Playing,

		/// <summary>
		/// The animation is paused.
		/// </summary>
		Paused,

		/// <summary>
		/// The animation is stopped.
		/// </summary>
		Stopped
	}

	/// <summary>
	/// A control that shows a loading animation.
	/// </summary>
	public class WaitSpin : Control
	{
		#region --------------------CONSTRUCTORS--------------------

		static WaitSpin()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitSpin),
				new FrameworkPropertyMetadata(typeof(WaitSpin)));
		}

		/// <summary>
        /// LoadingAnimation constructor.
        /// </summary>
		public WaitSpin()
        {
			this.DefaultStyleKey = typeof(WaitSpin);
        }

		#endregion

		#region --------------------DEPENDENCY PROPERTIES--------------------

		/// <summary>
		/// Ellipse fill property.
		/// </summary>
		public static readonly DependencyProperty EllipseFillProperty =
			DependencyProperty.Register("EllipseFill", typeof(Brush), typeof(WaitSpin), null);

		/// <summary>
		/// Gets or sets the ellipse fill.
		/// </summary>
		[System.ComponentModel.Category("Loading Animation Properties"), System.ComponentModel.Description("The fill for the little ellipses.")]
		public Brush EllipseFill
		{
			get { return (Brush)GetValue(EllipseFillProperty); }
			set { SetValue(EllipseFillProperty, value); }
		}

		/// <summary>
		/// Ellipse stroke property.
		/// </summary>
		public static readonly DependencyProperty EllipseStrokeProperty =
			DependencyProperty.Register("EllipseStroke", typeof(Brush), typeof(WaitSpin), null);

		/// <summary>
		/// Gets or sets the ellipse stroke.
		/// </summary>
		[System.ComponentModel.Category("Loading Animation Properties"), System.ComponentModel.Description("The stroke for the little ellipses.")]
		public Brush EllipseStroke
		{
			get { return (Brush)GetValue(EllipseStrokeProperty); }
			set { SetValue(EllipseStrokeProperty, value); }
		}

		/// <summary>
		/// Ellipse stroke property.
		/// </summary>
		public static readonly DependencyProperty ElemProperty =
			DependencyProperty.Register("Elem", typeof(UIElement), typeof(WaitSpin), null);

		/// <summary>
		/// Gets or sets the ellipse stroke.
		/// </summary>
		public UIElement Elem
		{
			get { return (UIElement)GetValue(ElemProperty); }
			set { SetValue(ElemProperty, value); }
		}

		#endregion

		#region --------------------PROPERTIES--------------------
		/// <summary>
		/// Stores the loading animation storyboard.
		/// </summary>
		private Storyboard _loadingAnimation;

		/// <summary>
		/// Stores whether the animation should play on load.
		/// </summary>
		private bool _autoPlay;

		/// <summary>
		/// Gets or sets a value indicating whether the animation should play on load.
		/// </summary>
		[System.ComponentModel.Category("Loading Animation Properties"), System.ComponentModel.Description("Whether the animation auto plays.")]
		public bool AutoPlay
		{
			get
			{
				return this._autoPlay;
			}

			set
			{
				this._autoPlay = value;

				if (this._loadingAnimation != null)
				{
					this._loadingAnimation.Stop();
					if (this._autoPlay)
					{
						this._loadingAnimation.Begin();
					}
				}
			}
		}

		/// <summary>
		/// Stores whether the animation is running.
		/// </summary>
		private AnimationState _animationState;

		/// <summary>
		/// Gets the animation state,
		/// </summary>
		public AnimationState AnimationState
		{
			get { return this._animationState; }
		}

		#endregion

		/// <summary>
		/// Gets the parts out of the template.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this._loadingAnimation = (Storyboard)this.GetTemplateChild("PART_LoadingAnimation");

			if (this._loadingAnimation != null && this._autoPlay)
			{
				this._loadingAnimation.Begin();
			}
		}

		/// <summary>
		/// Begins the loading animation.
		/// </summary>
		public void Begin()
		{
			if (this._loadingAnimation != null)
			{
				this._animationState = AnimationState.Playing;
				this._loadingAnimation.Begin();

				this.Visibility = System.Windows.Visibility.Visible;
				Elem.IsEnabled = false;
			}
		}

		/// <summary>
		/// Pauses the animation.
		/// </summary>
		public void Pause()
		{
			if (this._loadingAnimation != null)
			{
				this._animationState = AnimationState.Paused;
				this._loadingAnimation.Pause();
			}
		}

		/// <summary>
		/// Resumes the animation.
		/// </summary>
		public void Resume()
		{
			if (this._loadingAnimation != null)
			{
				this._animationState = AnimationState.Playing;
				this._loadingAnimation.Resume();
			}
		}

		/// <summary>
		/// Stops the animation.
		/// </summary>
		public void Stop()
		{
			if (this._loadingAnimation != null)
			{
				this._animationState = AnimationState.Stopped;
				this._loadingAnimation.Stop();

				this.Visibility = System.Windows.Visibility.Hidden;
				Elem.IsEnabled = true;
			}
		}
	}
}
