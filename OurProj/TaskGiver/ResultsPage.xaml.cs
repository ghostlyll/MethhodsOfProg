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
using System.Windows.Media.Animation;

namespace TaskGiver
{
    public partial class ResultsPage : Page
    {
        private OurProj.Victorine _victorine;

        public ResultsPage(OurProj.Victorine victorine)
        {
            InitializeComponent();
            _victorine = victorine;
            LoadResults();
        }

        private void LoadResults()
        {
            var result = _victorine.CurrentResult;

            if (result == null)
                return;

            // Отображаем информацию о студенте
            StudentInfoText.Text = $"{result.StudentName}, группа {result.StudentGroup}";

            // Отображаем результаты
            CorrectAnswersText.Text = result.CorrectAnswers.ToString();
            TotalQuestionsText.Text = result.TotalQuestions.ToString();
            PercentageText.Text = $"{result.Percentage:F1}%";

            // Анимация прогресс-бара
            DoubleAnimation animation = new DoubleAnimation
            {
                To = result.Percentage,
                Duration = TimeSpan.FromSeconds(1.5),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            ProgressIndicator.BeginAnimation(Border.WidthProperty,
                new DoubleAnimation(0, result.Percentage * 4, TimeSpan.FromSeconds(1.5)));

            // Отображаем сообщение в зависимости от результата
            string message;
            if (result.Percentage >= 90)
                message = "Отличный результат! Вы прекрасно справились с викториной! 🎉";
            else if (result.Percentage >= 70)
                message = "Хороший результат! Вы хорошо знаете материал! 👍";
            else if (result.Percentage >= 50)
                message = "Удовлетворительный результат. Есть над чем поработать! 🤔";
            else
                message = "Попробуйте еще раз! Рекомендуется повторить материал. 📚";

            ResultMessageText.Text = message;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на главную страницу выбора роли
            NavigationService.Navigate(new RoleSelectionPage());
        }
    }
}