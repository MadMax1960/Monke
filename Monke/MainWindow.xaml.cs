using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace Monke
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadGif();
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = false;
			openFileDialog.CheckPathExists = true;
			openFileDialog.FileName = "Folder Selection."; 
			openFileDialog.Filter = "Folder|*.none"; // If this works kms

			if (openFileDialog.ShowDialog() == true)
			{
				string folderPath = Path.GetDirectoryName(openFileDialog.FileName);
				filePathTextBox.Text = folderPath;
				infoTextBlock.Text = "Selected folder: " + folderPath;
			}
		}

		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			string folderPath = filePathTextBox.Text;
			ProcessWavFiles(folderPath);
		}

		private void ProcessWavFiles(string folderPath)
		{
			string exePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vgaudiocli.exe");
			foreach (string file in Directory.EnumerateFiles(folderPath, "*.wav", SearchOption.AllDirectories))
			{
				string outputFileName = System.IO.Path.ChangeExtension(file, ".hca");
				RunVgAudioCli(exePath, file, outputFileName);
			}

			MessageBox.Show("Processing Complete.");
		}

		private void RunVgAudioCli(string exePath, string inputFile, string outputFile)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = exePath,
					Arguments = $"\"{inputFile}\" \"{outputFile}\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using (Process process = Process.Start(startInfo))
				{
					process.WaitForExit();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error in processing: " + ex.Message);
			}
		}

		private void LoadGif()
		{
			try
			{
				// Mmm Monke
				gifMediaElement.Source = new Uri("C:\\Users\\maxgo\\source\\repos\\Monke\\Monke\\animation.gif");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading GIF: " + ex.Message);
			}
		}

		private void UltraLazyButton_Click(object sender, RoutedEventArgs e)
		{
			AcbWindow acbWindow = new AcbWindow();
			acbWindow.Show();
		}

	}
}
