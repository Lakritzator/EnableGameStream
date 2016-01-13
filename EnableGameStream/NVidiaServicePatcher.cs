using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;

namespace EnableGameStream
{
	public class NVidiaServicePatcher : IDisposable
	{
		public ServiceController NVidiaStreamService
		{
			get;
			private set;
		}

		public string ImagePath
		{
			get;
			private set;
		}

		public string PnpDeviceId
		{
			get;
			private set;
		}

		public string DeviceId
		{
			get
			{
				if (PnpDeviceId != null)
				{
					return Regex.Replace(PnpDeviceId, ".*DEV_([A-F0-9]+)&.*", "$1");
				}
				return null;
			}
		}

		public NVidiaServicePatcher()
		{
			RetrieveNVidiaService();
			if (NVidiaStreamService == null)
			{
				// Service not installed, nothing to do.
				return;
			}
			ImagePath = GetImagePath(NVidiaStreamService.ServiceName);

			RetrieveNVidiaGraphicsDeviceId();
		}

		public void PatchFiles()
		{

			var deviceToReplace = long.Parse("13D9", System.Globalization.NumberStyles.HexNumber);
			var patternToLocate = BitConverter.GetBytes(deviceToReplace);
			var path = Path.GetDirectoryName(ImagePath);
			bool serviceRunning = NVidiaStreamService.Status == ServiceControllerStatus.Running;

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
						if (serviceRunning)
						{
							NVidiaStreamService.Stop();
							NVidiaStreamService.WaitForStatus(ServiceControllerStatus.Stopped);
							serviceRunning = false;
						}
						foreach (var location in indexes)
						{
							filePatcher.ReplaceBytes(location, patchPattern);
						}
						filePatcher.WritePatched();
					}
				}
			}
			if (!serviceRunning)
			{
				NVidiaStreamService.Start();
			}
		}

		private void RetrieveNVidiaService()
		{
			NVidiaStreamService = (from service in ServiceController.GetServices()
								   where service.ServiceName == "NvStreamSvc"
								   select service).FirstOrDefault();
		}

		private void RetrieveNVidiaGraphicsDeviceId()
		{
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

			PnpDeviceId = (from managementObject in searcher.Get().Cast<ManagementObject>()
						   where ((string)managementObject.Properties["Caption"].Value).ToLower().Contains("nvidia")
						   select (string)managementObject.Properties["PNPDeviceID"].Value).FirstOrDefault();
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
						imagePath = imagePath.Substring(1, imagePath.IndexOf("\"", 1, StringComparison.Ordinal)-1);
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
