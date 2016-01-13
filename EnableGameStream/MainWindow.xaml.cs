using System.Windows;

namespace EnableGameStream
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly NVidiaServicePatcher patcher;

		public MainWindow()
		{
			InitializeComponent();
			patcher = new NVidiaServicePatcher();
			//MessageBox.Show(patcher.DeviceId);
			patcher.PatchFiles();
			Closed += (sender, args) => patcher.Dispose();
		}
	}
}
