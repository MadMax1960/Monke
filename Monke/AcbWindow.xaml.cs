using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Monke
{
	public partial class AcbWindow : Window
	{
		public AcbWindow()
		{
			InitializeComponent();
		}

		private void CompileAcbButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog folderDialog = new OpenFileDialog
			{
				ValidateNames = false,
				CheckFileExists = false,
				CheckPathExists = true,
				FileName = "Folder Selection.", // The user will type the folder or navigate into it
				Filter = "Folder|*.none" // Dummy filter
			};

			if (folderDialog.ShowDialog() == true)
			{
				string folderPath = Path.GetDirectoryName(folderDialog.FileName);
				RunAcbEditor(folderPath);
			}
		}

		private void DecompileAcbButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "ACB Files (*.acb)|*.acb",
				Title = "Select ACB File"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string acbFilePath = openFileDialog.FileName;
				RunAcbEditor(acbFilePath);
			}
		}

		private void RunAcbEditor(string path)
		{
			string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SonicAudioTools", "AcbEditor.exe");

			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = exePath,
					Arguments = $"\"{path}\"", // Pass the folder path directly
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
				MessageBox.Show("Error in launching AcbEditor: " + ex.Message);
			}
		}
	}
}
