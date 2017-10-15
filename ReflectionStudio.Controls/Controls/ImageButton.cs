using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ReflectionStudio.Controls
{
    /// <summary>
    /// ImagePosition
    /// </summary>
    public enum ImagePosition
    {
        BeforeText, AfterText
    }

    /// <summary>
    /// Templated button with image
    /// </summary>
    [Description("ImageButton")]
    public class ImageButton : Button
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static ImageButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton),
                    new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        #region --------------------DEPENDENCY PROPERTIES--------------------

        /// <summary>
        /// Layout DependencyProperty
        /// </summary>
        public static readonly DependencyProperty LayoutProperty =
               DependencyProperty.Register("Layout", typeof(Orientation), typeof(ImageButton));

        /// <summary>
        /// Layout Property
        /// </summary>
        public Orientation Layout
        {
            get { return (Orientation)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        /// <summary>
        /// Image DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
               DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton));

        /// <summary>
        /// Image Property
        /// </summary>
        [Description("The image displayed in the button if there is an Image control in the template whose Source property is template-bound to the ImageButton Source property."),
        Category("Common Properties")]
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Order DependencyProperty
        /// </summary>
        public static readonly DependencyProperty OrderProperty =
               DependencyProperty.Register("Order", typeof(ImagePosition), typeof(ImageButton));

        /// <summary>
        /// Order Property
        /// </summary>
        public ImagePosition Order
        {
            get { return (ImagePosition)GetValue(OrderProperty); }
            set { SetValue(OrderProperty, value); }
        }
        #endregion
    }

    /// <summary>
    /// FlatImageButton for explorer bars
    /// </summary>
    [Description("FlatImageButton")]
    public class FlatImageButton : Button
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static FlatImageButton()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatImageButton),
                    new FrameworkPropertyMetadata(typeof(FlatImageButton)));
        }

        #region --------------------DEPENDENCY PROPERTIES--------------------
        /// <summary>
        /// Image DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
               DependencyProperty.Register("Image", typeof(ImageSource), typeof(FlatImageButton));

        /// <summary>
        /// Image property
        /// </summary>
        [Description("The image displayed in the button if there is an Image control in the template whose Source property is template-bound to the ImageButton Source property."), Category("Common Properties")]
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        #endregion
    }

    /// <summary>
    /// A button that opens a drop-down menu when clicked.
    /// </summary>
    public class DropDownButton : ButtonBase
    {
        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        //public static readonly DependencyProperty DropDownMenuProperty
        //    = DependencyProperty.Register("DropDownMenu", typeof(ContextMenu),
        //                                  typeof(DropDownButton), new FrameworkPropertyMetadata(null));

        //public ContextMenu DropDownMenu
        //{
        //    get { return (ContextMenu)GetValue(DropDownMenuProperty); }
        //    set { SetValue(DropDownMenuProperty, value); }
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected static readonly DependencyPropertyKey IsDropDownMenuOpenPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDropDownMenuOpen", typeof(bool),
                                                  typeof(DropDownButton), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsDropDownMenuOpenProperty = IsDropDownMenuOpenPropertyKey.DependencyProperty;

        public bool IsDropDownMenuOpen
        {
            get { return (bool)GetValue(IsDropDownMenuOpenProperty); }
            protected set { SetValue(IsDropDownMenuOpenPropertyKey, value); }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            //base.OnMouseRightButtonUp(e);
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.ContextMenu != null && !this.ContextMenu.IsOpen &&
                this.ContextMenu.Items.Count > 0)
            {
                {
                    ContextMenu.Placement = PlacementMode.Bottom;
                    ContextMenu.PlacementTarget = this;
                    ContextMenu.IsOpen = true;
                    ContextMenu.Closed += DropDownMenu_Closed;
                    this.IsDropDownMenuOpen = true;
                }
            }
        }

        void DropDownMenu_Closed(object sender, RoutedEventArgs e)
        {
            ((ContextMenu)sender).Closed -= DropDownMenu_Closed;
            this.IsDropDownMenuOpen = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                e.Handled = true;
            }
        }
    }
}
