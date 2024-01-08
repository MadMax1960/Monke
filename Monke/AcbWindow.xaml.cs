using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.VisualBasic; // Add this using directive

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

		private void AdjustAcbVolumeButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "ACB Files (*.acb)|*.acb",
				Title = "Select ACB File"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string acbFilePath = openFileDialog.FileName;
				string volumeInput = Interaction.InputBox("Enter the volume level:", "Volume Level", "100", -1, -1);

				if (float.TryParse(volumeInput, out float volumeLevel))
				{
					AdjustVolumeInAcbFile(acbFilePath, volumeLevel);
					MessageBox.Show($"Adjusted volume level to {volumeLevel} in file: {acbFilePath}");
				}
				else
				{
					MessageBox.Show("Invalid volume level entered.");
				}
			}
		}

		private void AdjustVolumeInAcbFile(string filePath, float volumeLevel)
		{
			byte[] originalSequence = { 0x3F, 0x80, 0x00, 0x00 }; // Original byte sequence for float 1.0
			byte[] newSequence = BitConverter.GetBytes(volumeLevel); // Convert new volume level to byte sequence

			// Reverse the byte array if the system is little-endian
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(newSequence);
			}

			byte[] fileContent = File.ReadAllBytes(filePath);

			int index = FindSequence(fileContent, originalSequence);
			if (index >= 0)
			{
				Array.Copy(newSequence, 0, fileContent, index, newSequence.Length);
				File.WriteAllBytes(filePath, fileContent);
			}
			else
			{
				MessageBox.Show("The specified sequence was not found in the file.");
			}
		}


		private int FindSequence(byte[] array, byte[] sequence)
		{
			for (int i = 0; i < array.Length - sequence.Length + 1; i++)
			{
				bool found = true;
				for (int j = 0; j < sequence.Length; j++)
				{
					if (array[i + j] != sequence[j])
					{
						found = false;
						break;
					}
				}
				if (found)
				{
					return i;
				}
			}
			return -1;
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
