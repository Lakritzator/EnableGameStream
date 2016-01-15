using System;
using System.Collections.Generic;
using System.IO;

namespace EnableGameStream
{
	public class FilePatcher
	{
		private readonly byte[] _fileBytes;

		public string Filepath { get; }

		public IList<int> Indexes { get; }
		public int Count
		{
			get { return Indexes.Count; }
		}

		public FilePatcher(string filepath, byte[] pattern)
		{
			Filepath = filepath;
			_fileBytes = File.ReadAllBytes(filepath);
			Indexes = _fileBytes.IndexOfSequence(pattern);
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
			if (!File.Exists(Filepath))
			{
				throw new ApplicationException("Original file is gone!");
			}
			fileBackupPath = fileBackupPath ?? Filepath + ".bkp";
			File.Move(Filepath, fileBackupPath);
			if (!File.Exists(fileBackupPath) || File.Exists(Filepath))
			{
				// original still there, or no backup
				throw new ApplicationException("Problem moving the original fle!");
			}
			File.WriteAllBytes(Filepath, _fileBytes);
			return fileBackupPath;
		}
	}
}
