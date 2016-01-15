using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace EnableGameStream
{
	/// <summary>
	/// This contains the logic to find the video controller
	/// </summary>
	public class DeviceEnumerator
	{
		public string Id { get; private set; }

		public string Name { get; private set; }

		public DeviceEnumerator()
		{
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

			var device = (from managementObject in searcher.Get().Cast<ManagementObject>()
							   where ((string)managementObject.Properties["Caption"].Value).ToLower().Contains("nvidia")
							   select new KeyValuePair<string, string>((string)managementObject.Properties["Caption"].Value, (string)managementObject.Properties["PNPDeviceID"].Value)
							   ).FirstOrDefault();
			if (device.Key != null)
			{
				Name = device.Key;
				Id = Regex.Replace(device.Value, ".*DEV_([A-F0-9]+)&.*", "$1");
			}
		}
	}
}
