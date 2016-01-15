using System;
using System.Collections.Generic;
using System.IO;

namespace EnableGameStream
{
	public class PotentialFile
	{

		public static IList<PotentialFile> FindFiles(NVidiaService service, string deviceToPatch)
		{
			var result = new List<PotentialFile>();
			var deviceToReplace = long.Parse(deviceToPatch, System.Globalization.NumberStyles.HexNumber);
			var patternToLocate = BitConverter.GetBytes(deviceToReplace);
			if (service.ImagePath != null)
			{
				foreach (var file in Directory.GetFiles(service.ImagePath))
				{
					var filePatcher = new FilePatcher(file, patternToLocate);
					if (filePatcher.Count != 2)
					{
						continue;
					}
					var potentialFile = new PotentialFile
					{
						Patcher = filePatcher,
					};
					result.Add(potentialFile);
				}
			}


			return result;
		}

		public FilePatcher Patcher
		{
			get;
			private set;
		}

		/// <summary>
		/// Start the patching
		/// </summary>
		public void PatchFile(DeviceEnumerator device)
		{
			var deviceToReplaceWith = long.Parse(device.Id, System.Globalization.NumberStyles.HexNumber);
			var patchPattern = BitConverter.GetBytes(deviceToReplaceWith);
			if (Patcher.Indexes.Count > 0)
			{
				foreach (var location in Patcher.Indexes)
				{
					Patcher.ReplaceBytes(location, patchPattern);
				}
				Patcher.WritePatched();
			}
		}
	}
}
