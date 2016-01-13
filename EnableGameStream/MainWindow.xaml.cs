using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EnableGameStream
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly NVidiaServicePatcher _patcher;

		public MainWindow()
		{
			InitializeComponent();
			_patcher = new NVidiaServicePatcher();
			DataContext = _patcher;
			Task.Factory.StartNew(
				_patcher.Initialize,
				default(CancellationToken),
				TaskCreationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext()
			);

			Closed += (sender, args) => _patcher.Dispose();
		}

		private void PatchButtonClick(object sender, RoutedEventArgs e)
		{
			_patcher.PatchFiles();
		}

		private void StopServiceButtonClick(object sender, RoutedEventArgs e)
		{
			_patcher.StopService();
		}
		private void StartServiceButtonClick(object sender, RoutedEventArgs e)
		{
			_patcher.StartService();
		}
	}
}
