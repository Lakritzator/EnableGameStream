using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;

namespace EnableGameStream
{
	public class NVidiaServicePatcher : IDisposable, INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;
		private bool _serviceRunning;
		private bool _serviceNotRunning = true;
		private string _pnpDeviceId;
		private string _deviceId;
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

		public string DeviceToPatch { get; set; } = "13D9";

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

		public string PnpDeviceId
		{
			get { return _pnpDeviceId; }
			private set
			{
				if (_pnpDeviceId != value)
				{
					_pnpDeviceId = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PnpDeviceId)));

				}
			}
		}

		public string DeviceId
		{
			get { return _deviceId; }
			private set
			{
				if (_deviceId != value)
				{
					_deviceId = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceId)));
				}
			}
		}

		/// <summary>
		/// Create the service patcher
		/// </summary>
		public NVidiaServicePatcher()
		{
			RetrieveNVidiaService();
			if (NVidiaStreamService == null)
			{
				// Service not installed, nothing to do.
				return;
			}
			ServiceRunning = NVidiaStreamService.Status == ServiceControllerStatus.Running;
		}

		/// <summary>
		/// Initialize some of the values
		/// </summary>
		public void Initialize()
		{
			ImagePath = GetImagePath(NVidiaStreamService.ServiceName);

			RetrieveNVidiaGraphicsDeviceId();
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
		/// Start the patching
		/// </summary>
		public void PatchFiles()
		{
			var deviceToReplace = long.Parse(DeviceToPatch, System.Globalization.NumberStyles.HexNumber);
			var patternToLocate = BitConverter.GetBytes(deviceToReplace);
			var path = Path.GetDirectoryName(ImagePath);

			if (path == null)
			{
				return;
			}
			foreach (var file in Directory.GetFiles(path))
			{
				var filePatcher = new FilePatcher(file);
				var indexes = filePatcher.LocateBytes(patternToLocate);

				var deviceToReplaceWith = long.Parse(DeviceId, System.Globalization.NumberStyles.HexNumber);
				var patchPattern = BitConverter.GetBytes(deviceToReplaceWith);
				if (indexes.Count > 0)
				{
					var result = MessageBox.Show(file + " - " + indexes.Count, "Patch this?", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						foreach (var location in indexes)
						{
							filePatcher.ReplaceBytes(location, patchPattern);
						}
						filePatcher.WritePatched();
					}
				}
			}
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
		/// Get the NVidia graphics device ID
		/// </summary>
		private void RetrieveNVidiaGraphicsDeviceId()
		{
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

			PnpDeviceId = (from managementObject in searcher.Get().Cast<ManagementObject>()
						   where ((string)managementObject.Properties["Caption"].Value).ToLower().Contains("nvidia")
						   select (string)managementObject.Properties["PNPDeviceID"].Value).FirstOrDefault();
			if (PnpDeviceId != null)
			{
				DeviceId = Regex.Replace(PnpDeviceId, ".*DEV_([A-F0-9]+)&.*", "$1");
			}
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
