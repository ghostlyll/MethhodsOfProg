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
using System.IO;
using System.Xml.Linq;

namespace TaskGiver
{
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
                TitleText.Text = "Учитель: создание викторины из TXT файла";
                this.Background = Brushes.LightCoral;
                SelectedFileText.Text = "Выберите TXT файл с вопросами";
            }
            else
            {
                TitleText.Text = "Ученик: прохождение викторины";
                this.Background = Brushes.LightGreen;
                SelectedFileText.Text = "Выберите XML файл викторины";
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (_userRole == "teacher")
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите текстовый файл с заданиями для учителя";
            }
            else
            {
                openFileDialog.Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите XML файл викторины для ученика";
            }

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(selectedFilePath);
                SelectedFileText.Text = $"Выбран файл: {fileName}";

                // Обрабатываем выбранный файл
                ProcessSelectedFile(selectedFilePath);
            }
        }

        private void ProcessSelectedFile(string filePath)
        {
            try
            {
                if (_userRole == "teacher")
                {
                    // Учитель: конвертируем TXT в XML
                    ConvertTxtToXml(filePath);
                }
                else
                {
                    // Ученик: загружаем XML и начинаем викторину
                    StartQuizFromXml(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}\n\nДетали: {ex.StackTrace}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private void ConvertTxtToXml(string txtFilePath)
        {
            try
            {
                // Читаем TXT файл с помощью TaskReader
                var taskReader = new OurProj.TaskReader();
                taskReader.ReadFromFile(txtFilePath);

                if (taskReader.Tasks == null || taskReader.Tasks.Count == 0)
                {
                    MessageBox.Show("Не удалось загрузить задания из файла. Проверьте формат файла.",
                                   "Ошибка",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);
                    return;
                }

                // Проверяем, что у каждого вопроса есть хотя бы один неверный ответ
                var invalidQuestions = new List<int>();
                for (int i = 0; i < taskReader.Tasks.Count; i++)
                {
                    var task = taskReader.Tasks[i];
                    if (task.WrongAnswers == null || task.WrongAnswers.Count == 0)
                    {
                        invalidQuestions.Add(i + 1);
                    }
                }

                if (invalidQuestions.Count > 0)
                {
                    throw new InvalidQuizFormatException(
                        "Для создания викторины каждый вопрос должен иметь хотя бы один неверный ответ.\n\n" +
                        $"Вопросы без неверных ответов: {string.Join(", ", invalidQuestions)}\n\n" +
                        "Пример правильного формата:\n" +
                        "ЗАДАНИЕ №1\n" +
                        "Какого цвета небо?\n" +
                        "Ответ: Синего\n" +
                        "Неверные ответы: Зелёного, Красного, Желтого");
                }

                // Создаем XML документ
                var xmlDoc = new XDocument(
                    new XElement("quiz",
                        new XElement("info",
                            new XElement("title", "Викторина из TXT файла"),
                            new XElement("sourceFile", System.IO.Path.GetFileName(txtFilePath)),
                            new XElement("creationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            new XElement("totalQuestions", taskReader.Tasks.Count)
                        ),
                        taskReader.Tasks.Select(task =>
                            new XElement("question",
                                new XElement("text", task.Question),
                                new XElement("points", "1"),
                                new XElement("answers",
                                    // Правильный ответ
                                    new XElement("answer",
                                        new XAttribute("correct", "true"),
                                        task.Answer
                                    ),
                                    // Неправильные ответы
                                    task.WrongAnswers.Select(wrongAnswer =>
                                        new XElement("answer",
                                            new XAttribute("correct", "false"),
                                            wrongAnswer
                                        )
                                    )
                                )
                            )
                        )
                    )
                );

                // Предлагаем сохранить XML файл
                var saveDialog = new SaveFileDialog();
                saveDialog.Filter = "XML файлы (*.xml)|*.xml|Все файлы (*.*)|*.*";
                saveDialog.Title = "Сохранить викторину как XML";
                saveDialog.FileName = $"quiz_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                saveDialog.DefaultExt = ".xml";

                if (saveDialog.ShowDialog() == true)
                {
                    xmlDoc.Save(saveDialog.FileName);

                    MessageBox.Show($"Викторина успешно создана!\n\n" +
                                   $"Загружено заданий: {taskReader.Tasks.Count}\n" +
                                   $"Сохранено в файл: {System.IO.Path.GetFileName(saveDialog.FileName)}\n\n" +
                                   $"Теперь этот XML файл можно использовать для проведения викторины.",
                                   "Успех",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);

                    // Показываем предпросмотр созданной викторины
                    ShowQuizPreview(taskReader);
                }
            }
            catch (InvalidQuizFormatException ex)
            {
                MessageBox.Show(ex.Message,
                               "Некорректный формат викторины",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании XML: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private void ShowQuizPreview(OurProj.TaskReader taskReader)
        {
            var previewWindow = new QuizPreviewWindow(taskReader);
            previewWindow.Owner = Window.GetWindow(this);
            previewWindow.ShowDialog();
        }

        private void StartQuizFromXml(string xmlFilePath)
        {
            try
            {
                // Проверяем, является ли файл валидным XML для викторины
                if (!IsValidQuizXml(xmlFilePath))
                {
                    MessageBox.Show("Выбранный XML файл не является валидной викториной.\n" +
                                   "Пожалуйста, выберите файл, созданный учителем.",
                                   "Ошибка",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);
                    return;
                }

                // Создаем и загружаем викторину
                var victorine = new OurProj.Victorine();
                victorine.LoadFromXML(xmlFilePath);

                if (!victorine.IsPossibleToConstruct())
                {
                    MessageBox.Show("Не удалось загрузить викторину из файла.",
                                   "Ошибка",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Error);
                    return;
                }

                // Переходим на страницу регистрации
                NavigationService.Navigate(new StudentRegistrationPage(xmlFilePath, victorine));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке викторины: {ex.Message}",
                               "Ошибка",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private bool IsValidQuizXml(string xmlFilePath)
        {
            try
            {
                var xmlDoc = XDocument.Load(xmlFilePath);

                // Проверяем базовую структуру
                return xmlDoc.Root != null &&
                       xmlDoc.Root.Name == "quiz" &&
                       xmlDoc.Root.Elements("question").Any();
            }
            catch
            {
                return false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
        public class InvalidQuizFormatException : Exception
        {
            public InvalidQuizFormatException() : base() { }

            public InvalidQuizFormatException(string message) : base(message) { }

            public InvalidQuizFormatException(string message, Exception innerException)
                : base(message, innerException) { }

            public InvalidQuizFormatException(string message, List<string> invalidQuestions)
                : base($"{message}\nНекорректные вопросы: {string.Join(", ", invalidQuestions)}") { }
        }
    }
}