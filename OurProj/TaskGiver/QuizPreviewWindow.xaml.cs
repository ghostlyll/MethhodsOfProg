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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace TaskGiver
{
    public partial class QuizPreviewWindow : Window
    {
        public class QuestionPreview
        {
            public int Number { get; set; }
            public string Question { get; set; }
            public string CorrectAnswer { get; set; }
            public List<string> WrongAnswers { get; set; }
        }

        public QuizPreviewWindow(OurProj.TaskReader taskReader)
        {
            InitializeComponent();
            LoadQuizPreview(taskReader);
        }

        private void LoadQuizPreview(OurProj.TaskReader taskReader)
        {
            // Устанавливаем информацию о викторине
            QuizInfoText.Text = $"Всего вопросов: {taskReader.Tasks.Count} | " +
                               $"Создано: {DateTime.Now:dd.MM.yyyy HH:mm}";

            // Создаем список для предпросмотра
            var previewQuestions = new ObservableCollection<QuestionPreview>();

            for (int i = 0; i < taskReader.Tasks.Count; i++)
            {
                var task = taskReader.Tasks[i];
                previewQuestions.Add(new QuestionPreview
                {
                    Number = i + 1,
                    Question = task.Question,
                    CorrectAnswer = task.Answer,
                    WrongAnswers = task.WrongAnswers
                });
            }

            QuestionsList.ItemsSource = previewQuestions;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}