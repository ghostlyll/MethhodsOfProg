using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskGiver
{
    /// <summary>
    /// Логика взаимодействия для FileSelectionPage.xaml
    /// </summary>
    public partial class FileSelectionPage : Page
    {
        private string _userRole;

        public FileSelectionPage(string role)
        {
            InitializeComponent();
            _userRole = role;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_userRole == "teacher")
            {
                TitleText.Text = "Учитель: выберите TXT файл";
                this.Background = System.Windows.Media.Brushes.LightCoral;
            }
            else
            {
                TitleText.Text = "Ученик: выберите XML файл";
                this.Background = System.Windows.Media.Brushes.LightGreen;
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (_userRole == "teacher")
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите текстовый файл с заданиями";
            }
            else
            {
                openFileDialog.Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите XML файл с заданиями";
            }

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                SelectedFileText.Text = $"Выбран файл: {System.IO.Path.GetFileName(selectedFilePath)}";

                // Здесь можно добавить логику обработки файла
                ProcessSelectedFile(selectedFilePath);
            }
        }

        private void ProcessSelectedFile(string filePath)
        {
            try
            {
                if (_userRole == "teacher")
                {
                    // Обработка TXT файла для учителя
                    MessageBox.Show($"TXT файл выбран: {filePath}\n" +
                                   "Здесь будет логика обработки заданий для учителя",
                                   "Файл выбран",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                }
                else
                {
                    // Обработка XML файла для ученика
                    MessageBox.Show($"XML файл выбран: {filePath}\n" +
                                   "Здесь будет логика обработки заданий для ученика",
                                   "Файл выбран",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на предыдущую страницу
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
