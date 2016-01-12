using System;
using System.Collections.Generic;
using System.IO;

namespace EnableGameStream
{
	public class FilePatcher
	{
		private readonly byte[] _fileBytes;
		private readonly string _filepath;

		public FilePatcher(string filepath)
		{
			_filepath = filepath;
			_fileBytes = File.ReadAllBytes(filepath);
		}

		public IList<int> LocateBytes(byte[]pattern)
		{
			return _fileBytes.IndexOfSequence(pattern);
		}

		public void ReplaceBytes(int location, byte[] newValues)
		{
			Buffer.BlockCopy(newValues, 0, _fileBytes, location, newValues.Length);
		}

		/// <summary>
		/// Rename the original file (this
		/// </summary>
		/// <param name="fileBackupPath">Name of the backup file, null to use the original file with a .bkp extension</param>
		/// <returns>Backup filename</returns>
		public string WritePatched(string fileBackupPath = null)
		{
			if (!File.Exists(_filepath))
			{
				throw new ApplicationException("Original file is gone!");
			}
			fileBackupPath = fileBackupPath ?? _filepath + ".bkp";
			File.Move(_filepath, fileBackupPath);
			if (!File.Exists(fileBackupPath) || File.Exists(_filepath))
			{
				// original still there, or no backup
				throw new ApplicationException("Problem moving the original fle!");
			}
			File.WriteAllBytes(_filepath, _fileBytes);
			return fileBackupPath;
		}
	}
}
