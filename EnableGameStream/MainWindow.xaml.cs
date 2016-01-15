using System.Collections.ObjectModel;
using System.Linq;
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
		public string DeviceToPatch
		{
			get;
			set;
		} = "13D9";

		public NVidiaService Service
		{
			get;
		} = new NVidiaService();

		public DeviceEnumerator Device
		{
			get;
		} = new DeviceEnumerator();

		public ObservableCollection<PotentialFile> PotentialFiles { get; } = new ObservableCollection<PotentialFile>(); 

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
			Task.Factory.StartNew(
				() => PotentialFile.FindFiles(Service, DeviceToPatch).ToList().ForEach(x => PotentialFiles.Add(x)),
				default(CancellationToken),
				TaskCreationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext()
			);
			Closed += (sender, args) => Service.Dispose();
		}

		private void PatchButtonClick(object sender, RoutedEventArgs e)
		{
			PotentialFiles.ToList().ForEach( x => x.PatchFile(Device));
		}

		private void StopServiceButtonClick(object sender, RoutedEventArgs e)
		{
			Service.StopService();
		}
		private void StartServiceButtonClick(object sender, RoutedEventArgs e)
		{
			Service.StartService();
		}
	}
}
