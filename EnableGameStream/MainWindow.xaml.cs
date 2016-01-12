using System.Windows;

namespace EnableGameStream
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private NVidiaServicePatcher patcher;

		public MainWindow()
		{
			InitializeComponent();
			patcher = new NVidiaServicePatcher();
			//MessageBox.Show(patcher.DeviceId);
			patcher.PatchFiles();
		}
	}
}
