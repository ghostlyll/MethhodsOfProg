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
    public partial class QuizPage : Page
    {
        private OurProj.Victorine _victorine;
        private List<OurProj.Question> _shuffledQuestions;
        private int _currentQuestionIndex = 0;

        public QuizPage(OurProj.Victorine victorine)
        {
            InitializeComponent();
            _victorine = victorine;
            _shuffledQuestions = _victorine.GetShuffledQuestions();
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (_currentQuestionIndex >= _shuffledQuestions.Count)
                return;

            var question = _shuffledQuestions[_currentQuestionIndex];

            // Обновляем прогресс
            ProgressText.Text = $"Вопрос {_currentQuestionIndex + 1} из {_shuffledQuestions.Count}";
            ProgressBar.Maximum = _shuffledQuestions.Count;
            ProgressBar.Value = _currentQuestionIndex + 1;

            // Показываем вопрос
            QuestionText.Text = question.Text;

            // Показываем варианты ответов
            AnswersList.ItemsSource = null;
            AnswersList.ItemsSource = question.Answers;

            // Обновляем кнопки навигации
            PreviousButton.IsEnabled = _currentQuestionIndex > 0;
            NextButton.Content = _currentQuestionIndex == _shuffledQuestions.Count - 1 ?
                "Завершить викторину" : "Следующий вопрос";
        }

        private OurProj.Question GetCurrentQuestion()
        {
            return _shuffledQuestions[_currentQuestionIndex];
        }

        private OurProj.Answer GetSelectedAnswer()
        {
            var question = GetCurrentQuestion();
            return question.Answers.FirstOrDefault(a => a.IsSelected);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                LoadQuestion();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем текущий ответ
            var selectedAnswer = GetSelectedAnswer();

            if (selectedAnswer == null && _currentQuestionIndex < _shuffledQuestions.Count - 1)
            {
                MessageBox.Show("Пожалуйста, выберите ответ!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Отправляем ответ в викторину
            _victorine.SubmitAnswer(GetCurrentQuestion(), selectedAnswer);

            if (_currentQuestionIndex < _shuffledQuestions.Count - 1)
            {
                // Переходим к следующему вопросу
                _currentQuestionIndex++;
                LoadQuestion();
            }
            else
            {
                // Завершаем викторину
                CompleteQuiz();
            }
        }

        private void CompleteQuiz()
        {
            // Сохраняем результаты
            _victorine.SaveAnswers();
            _victorine.SaveAsXML();

            // Переходим на страницу результатов
            NavigationService.Navigate(new ResultsPage(_victorine));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите прервать викторину? Все результаты будут потеряны.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }
    }
}