using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;

namespace EnableGameStream
{
	public class NVidiaService : IDisposable, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private bool _serviceRunning;
		private bool _serviceNotRunning = true;
		private string _imagePath;

		public bool ServiceRunning
		{
			get
			{
				return _serviceRunning;

			}
			set
			{
				if (_serviceRunning != value)
				{
					_serviceRunning = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServiceRunning)));
					ServiceNotRunning = !value;
				}
			}
		}
		public bool ServiceNotRunning
		{
			get
			{
				return _serviceNotRunning;

			}
			set
			{
				if (_serviceNotRunning != value)
				{
					_serviceNotRunning = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServiceNotRunning)));
				}
			}
		}

		public ServiceController NVidiaStreamService
		{
			get;
			private set;
		}

		public string ImagePath
		{
			get { return _imagePath; }
			private set
			{
				if (_imagePath != value)
				{
					_imagePath = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImagePath)));
				}
			}
		}

		/// <summary>
		/// Create the service patcher
		/// </summary>
		public NVidiaService()
		{
			RetrieveNVidiaService();
			if (NVidiaStreamService == null)
			{
				// Service not installed, nothing to do.
				return;
			}
			ServiceRunning = NVidiaStreamService.Status == ServiceControllerStatus.Running;
			ImagePath = Path.GetDirectoryName(GetImagePath(NVidiaStreamService.ServiceName));
		}

		/// <summary>
		/// Stop the service
		/// </summary>
		public void StopService()
		{
			NVidiaStreamService.Stop();
			NVidiaStreamService.WaitForStatus(ServiceControllerStatus.Stopped);
			ServiceRunning = false;
		}

		/// <summary>
		/// Start the service
		/// </summary>
		public void StartService()
		{
			NVidiaStreamService.Start();
			NVidiaStreamService.WaitForStatus(ServiceControllerStatus.Running);
			ServiceRunning = true;
		}


		/// <summary>
		/// Get the NVidia service
		/// </summary>
		private void RetrieveNVidiaService()
		{
			NVidiaStreamService = (from service in ServiceController.GetServices()
								   where service.ServiceName == "NvStreamSvc"
								   select service).FirstOrDefault();
		}

		/// <summary>
		/// Retrieve the commandline for the service
		/// </summary>
		/// <param name="serviceName"></param>
		/// <returns>string with the path</returns>
		private static string GetImagePath(string serviceName)
		{
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
			{
				using (var key = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName))
				{
					var imagePath = key?.GetValue("ImagePath")?.ToString();
					if (imagePath == null)
					{
						return null;
					}
					if (imagePath.StartsWith("\""))
					{
						imagePath = imagePath.Substring(1, imagePath.IndexOf("\"", 1, StringComparison.Ordinal) - 1);
					}
					else if (imagePath.Contains(" "))
					{
						imagePath = imagePath.Substring(0, imagePath.IndexOf(" ", StringComparison.Ordinal));
					}
					return Environment.ExpandEnvironmentVariables(imagePath);
				}
			}
		}

		public void Dispose()
		{
			((IDisposable)NVidiaStreamService).Dispose();
		}
	}
}
