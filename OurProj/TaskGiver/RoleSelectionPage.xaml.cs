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
    /// Логика взаимодействия для RoleSelectionPage.xaml
    public partial class RoleSelectionPage : Page
    {
        public RoleSelectionPage()
        {
            InitializeComponent();
        }

        private void TeacherButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу выбора файла для учителя
            this.NavigationService.Navigate(new FileSelectionPage("teacher"));
        }

        private void StudentButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу выбора файла для ученика
            this.NavigationService.Navigate(new FileSelectionPage("student"));
        }
    }
}
