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
    public partial class StudentRegistrationPage : Page
    {
        private string _filePath;
        private OurProj.Victorine _victorine;

        public StudentRegistrationPage(string filePath, OurProj.Victorine victorine)
        {
            InitializeComponent();
            _filePath = filePath;
            _victorine = victorine;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(GroupTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Устанавливаем данные студента
            _victorine.SetStudent(
                FirstNameTextBox.Text.Trim(),
                LastNameTextBox.Text.Trim(),
                GroupTextBox.Text.Trim()
            );

            // Начинаем викторину
            _victorine.PlayGame();

            // Переходим на страницу викторины
            NavigationService.Navigate(new QuizPage(_victorine));
        }
    }
}