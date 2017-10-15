using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using ReflectionStudio.Spy.Distant;
using ReflectionStudio.Spy.Internal;

namespace WcfServer
{
	class WcfHostService
	{
		private ServiceHost _Host = null;

		private Uri ServiceUri( Transport mode )
		{
			string format;
			if (mode == Transport.Http)
				format = "http://{0}:{1}";
			else
				format = "net.tcp://{0}:{1}";

			return new Uri
				(
				string.Format(format, SettingsManager.Instance.Settings.Machine, SettingsManager.Instance.Settings.DistantPort)
				);
		}

		public bool StartService()
		{
			try
			{
				if (_Host == null)
				{
					_Host = new ServiceHost(typeof(CaptureService), ServiceUri(SettingsManager.Instance.Settings.TransportMode) );

					if (SettingsManager.Instance.Settings.TransportMode == Transport.Http)
						_Host.AddServiceEndpoint(typeof(ICaptureService), new WSHttpBinding(), "CaptureService");
					else
						_Host.AddServiceEndpoint(typeof(ICaptureService), new NetTcpBinding(), "CaptureService");

					// TODO - supress if no more debug...
					ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
					metadataBehavior.HttpGetEnabled = true;
					_Host.Description.Behaviors.Add(metadataBehavior);

					Binding mexBinding = null;
					if (SettingsManager.Instance.Settings.TransportMode == Transport.Http)
						mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
					else
						mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();

					_Host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "mex");

					//ApplyCompression();
				}

				_Host.Open();
				return true;
			}
			catch (Exception error)
			{
				_Host.Close();
				_Host = null;

				return false;
			}
		}

		public void StopService()
		{
			if (_Host != null)
			{
				_Host.Close();
				_Host = null;
			}
		}

		//private Binding ApplyCompression(EndpointAddress endpointAdress)
		//{
		//    CustomBinding binding = new CustomBinding();

		//    if (SettingsManager.Instance.Settings.UseCompression == true)
		//        binding.Elements.Add(new CompressionMessageEncodingBindingElement());
		//    else
		//        binding.Elements.Add(new TextMessageEncodingBindingElement(System.ServiceModel.Channels.MessageVersion.Soap11, System.Text.Encoding.UTF8));

		//    if (SettingsManager.Instance.Settings.TransportMode == Transport.Http)
		//        binding.Elements.Add(new HttpTransportBindingElement());
		//    else
		//        binding.Elements.Add(new TcpTransportBindingElement());

		//    //transportBindingElement.MaxReceivedMessageSize = ParametresConfig.MaxWcfMessageSize;

		//    return binding;
		//}
	}
}
