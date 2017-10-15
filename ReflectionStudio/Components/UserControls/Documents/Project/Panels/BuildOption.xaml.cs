using System.Windows.Controls;

using ReflectionStudio.Core.Project;
using ReflectionStudio.Core.Project.Settings;
using ReflectionStudio.Classes;

namespace ReflectionStudio.Components.UserControls
{
	/// <summary>
	/// Interaction logic for BuildOption.xaml
	/// </summary>
	public partial class BuildOption : UserControl
	{
		public BuildOption()
		{
			InitializeComponent();
		}

		public void Load()
		{
			ProjectEntity entity = ((BindingView)this.DataContext).Project;

			if (entity.Settings.Action == ProfilMode.CallCount)
				rbMethodCall.IsChecked = true;
			else
				rbMethodTime.IsChecked = true;

			this.sliderLogLevel.Value = (int)entity.Settings.LogLevel;

			if (entity.Settings.TransportMode == Transport.Http)
				this.cbTransportMode.SelectedIndex = 0;
			else
				this.cbTransportMode.SelectedIndex = 1;
		}

		public void Save()
		{
			ProjectEntity entity = ((BindingView)this.DataContext).Project;

			entity.Settings.Action = (bool)rbMethodCall.IsChecked ? ProfilMode.CallCount : ProfilMode.TimeSpent;

			entity.Settings.LogLevel = (ProfilerLogLevel)this.sliderLogLevel.Value;

			if (this.cbTransportMode.SelectedIndex == 0)
				entity.Settings.TransportMode = Transport.Http;
			else
				entity.Settings.TransportMode = Transport.Tcp;
		}
	}
}
