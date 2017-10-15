using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ReflectionStudio.Controls;

namespace ReflectionStudio.Controls
{
    /// <summary>
    /// MessageBoxDlg act like the Winform version
    /// </summary>
	public partial class MessageBoxDlg : HeaderedDialogWindow
    {
		#region --------------------CONSTRUCTOR--------------------
		/// <summary>
		/// Constructor
		/// </summary>
		public MessageBoxDlg()
        {
            InitializeComponent();
		}

		#endregion

		#region --------------------PROPERTIES--------------------

		/// <summary>
		/// The resulted button
		/// </summary>
		private Button ButtonClicked
        {
            get;
            set;
        }

		/// <summary>
		/// The message box result taken from the button tag
		/// </summary>
		private MessageBoxResult MessageBoxResult
		{
			get
			{
				if (ButtonClicked != null)
					return (MessageBoxResult)ButtonClicked.Tag;
				else
					return MessageBoxResult.Cancel;
			}
		}

        #endregion

		#region --------------------PUBLIC FUNCTIONS --------------------
		/// <summary>
		/// First public call, simple
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		public static MessageBoxResult Show(string message, string title)
        {
            return MessageBoxDlg.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.None);
        }

		/// <summary>
		/// Second public call with more options
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		/// <param name="button"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
        public static MessageBoxResult Show(string message, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            MessageBoxDlg msgBox = new MessageBoxDlg();

            msgBox.Title = title;
            msgBox.textBlockMessage.Text = message;
            msgBox.DisplayButton(button);
            msgBox.DisplayIcon(icon);
            msgBox.ShowDialog();

            return msgBox.MessageBoxResult;
		}
		#endregion

		#region --------------------PRIVATE FUNCTIONS --------------------
		/// <summary>
		/// Deternime which button to show, set the corresponding values
		/// </summary>
		/// <param name="button"></param>
		private void DisplayButton(MessageBoxButton button)
        {
            this.BtnLeft.Visibility = Visibility.Hidden;
            this.BtnMidle.Visibility = Visibility.Hidden;
            this.BtnRight.Visibility = Visibility.Hidden;

            switch (button)
            {
                case MessageBoxButton.OK:
					DisplayButton(BtnMidle, "Ok", "Ok", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
					DisplayButton(BtnLeft, "Cancel", "Cancel", MessageBoxResult.Cancel);
					DisplayButton(BtnRight, "OK", "OK", MessageBoxResult.OK); 
                    break;
                case MessageBoxButton.YesNo:
					DisplayButton(BtnLeft, "Yes", "Yes", MessageBoxResult.Yes);
					DisplayButton(BtnRight, "No", "No", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
					DisplayButton(BtnLeft, "Yes", "Yes", MessageBoxResult.Yes);
					DisplayButton(BtnMidle, "No", "No", MessageBoxResult.No);
					DisplayButton(BtnRight, "Cancel", "Cancel", MessageBoxResult.Cancel);
                    break;
            }
        }

		/// <summary>
		/// Shorcut to update buttons
		/// </summary>
		/// <param name="btn"></param>
		/// <param name="label"></param>
		/// <param name="tooltip"></param>
		/// <param name="code"></param>
		private void DisplayButton( Button btn, string label, string tooltip, MessageBoxResult code)
		{
			btn.Content = label;
			btn.ToolTip = tooltip;
			btn.Tag = code;
			btn.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Display the requested icon
		/// </summary>
		/// <param name="icon"></param>
		private void DisplayIcon(MessageBoxImage icon)
        {
            if (icon != MessageBoxImage.None)
            {
				string iconfilename = "pack://application:,,,/ReflectionStudio.Controls;component/Resources/Images/32x32/messagebox-" + icon + ".png";
                this.DialogImage = new BitmapImage(new Uri(iconfilename));
            }
        }

		/// <summary>
		/// Get the buttons click's
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
			this.ButtonClicked = (Button)sender;
            this.Close();
		}
		#endregion
	}
}
