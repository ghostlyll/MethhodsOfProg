using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace OurProj
{
    public class Victorine : Game
    {
        private List<Question> questions = new List<Question>();
        private Random random = new Random();
        private QuizResult currentResult;
        private Student currentStudent;

        public class Student
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Group { get; set; }
        }

        public List<Question> Questions => questions;
        public QuizResult CurrentResult => currentResult;
        public Student CurrentStudent => currentStudent;

        public bool IsPossibleToConstruct()
        {
            return questions != null && questions.Count > 0;
        }

        public void LoadFromXML(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"XML файл не найден: {filePath}");

            questions.Clear();

            var xmlDoc = XDocument.Load(filePath);

            foreach (var questionElement in xmlDoc.Root.Elements("question"))
            {
                var question = new Question
                {
                    Text = questionElement.Element("text")?.Value?.Trim() ?? "",
                    Points = int.TryParse(questionElement.Element("points")?.Value, out int points) ? points : 1
                };

                var answersElement = questionElement.Element("answers");
                if (answersElement != null)
                {
                    foreach (var answerElement in answersElement.Elements("answer"))
                    {
                        bool isCorrect = answerElement.Attribute("correct")?.Value == "true";

                        question.Answers.Add(new Answer
                        {
                            Text = answerElement.Value?.Trim() ?? "",
                            IsCorrect = isCorrect
                        });
                    }
                }

                if (!string.IsNullOrEmpty(question.Text) && question.Answers.Count > 0)
                {
                    questions.Add(question);
                }
            }
        }

        // Альтернативный метод загрузки из TXT файла в формате вашего TaskReader
        public void LoadFromTXT(string filePath)
        {
            var taskReader = new TaskReader();
            taskReader.ReadFromFile(filePath);

            questions.Clear();

            foreach (var task in taskReader.Tasks)
            {
                var question = new Question
                {
                    Text = task.Question
                };

                // Добавляем правильный ответ
                question.Answers.Add(new Answer
                {
                    Text = task.Answer,
                    IsCorrect = true
                });

                // Добавляем неправильные ответы
                foreach (var wrongAnswer in task.WrongAnswers)
                {
                    question.Answers.Add(new Answer
                    {
                        Text = wrongAnswer,
                        IsCorrect = false
                    });
                }

                questions.Add(question);
            }
        }

        public void ShuffleQuestions()
        {
            questions = questions.OrderBy(q => random.Next()).ToList();

            foreach (var question in questions)
            {
                question.Answers = question.Answers.OrderBy(a => random.Next()).ToList();
            }
        }

        public void SetStudent(string firstName, string lastName, string group)
        {
            currentStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                Group = group
            };
        }

        public void StartNewQuiz()
        {
            currentResult = new QuizResult
            {
                StudentName = $"{currentStudent.FirstName} {currentStudent.LastName}",
                StudentGroup = currentStudent.Group,
                TotalQuestions = questions.Count,
                CorrectAnswers = 0,
                CompletionTime = DateTime.Now,
                QuestionResults = new List<QuestionResult>()
            };
        }

        public void PlayGame()
        {
            // Этот метод будет вызываться из UI
            ShuffleQuestions();
            StartNewQuiz();
        }

        public void SaveAnswers()
        {
            if (currentResult == null || currentStudent == null)
                throw new InvalidOperationException("Нет данных для сохранения");

            // Сохраняем результаты в текстовый файл
            string fileName = $"{currentStudent.LastName}_{currentStudent.FirstName}_{currentStudent.Group}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"ВИКТОРИНА - РЕЗУЛЬТАТЫ");
                writer.WriteLine(new string('=', 50));
                writer.WriteLine($"Студент: {currentStudent.FirstName} {currentStudent.LastName}");
                writer.WriteLine($"Группа: {currentStudent.Group}");
                writer.WriteLine($"Дата прохождения: {currentResult.CompletionTime:dd.MM.yyyy HH:mm:ss}");
                writer.WriteLine(new string('-', 50));
                writer.WriteLine($"Всего вопросов: {currentResult.TotalQuestions}");
                writer.WriteLine($"Правильных ответов: {currentResult.CorrectAnswers}");
                writer.WriteLine($"Процент правильных: {currentResult.Percentage:F1}%");
                writer.WriteLine(new string('-', 50));

                if (currentResult.QuestionResults.Any())
                {
                    writer.WriteLine("\nДЕТАЛЬНЫЕ РЕЗУЛЬТАТЫ:");
                    writer.WriteLine(new string('-', 50));

                    for (int i = 0; i < currentResult.QuestionResults.Count; i++)
                    {
                        var result = currentResult.QuestionResults[i];
                        writer.WriteLine($"\nВопрос {i + 1}: {result.QuestionText}");
                        writer.WriteLine($"Ваш ответ: {result.SelectedAnswer} {(result.IsCorrect ? "✓" : "✗")}");
                        writer.WriteLine($"Правильный ответ: {result.CorrectAnswer}");
                    }
                }
            }
        }

        public void SaveAsXML()
        {
            if (currentResult == null)
                throw new InvalidOperationException("Нет результатов для сохранения");

            string fileName = $"{currentStudent.LastName}_{currentStudent.FirstName}_results.xml";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, fileName);

            var xmlDoc = new XDocument(
                new XElement("quizResults",
                    new XElement("student",
                        new XElement("firstName", currentStudent.FirstName),
                        new XElement("lastName", currentStudent.LastName),
                        new XElement("group", currentStudent.Group)
                    ),
                    new XElement("quizInfo",
                        new XElement("completionTime", currentResult.CompletionTime.ToString("o")),
                        new XElement("totalQuestions", currentResult.TotalQuestions),
                        new XElement("correctAnswers", currentResult.CorrectAnswers),
                        new XElement("percentage", currentResult.Percentage)
                    ),
                    new XElement("detailedResults",
                        currentResult.QuestionResults.Select((r, index) =>
                            new XElement("questionResult",
                                new XAttribute("number", index + 1),
                                new XElement("question", r.QuestionText),
                                new XElement("selectedAnswer", r.SelectedAnswer),
                                new XElement("correctAnswer", r.CorrectAnswer),
                                new XElement("isCorrect", r.IsCorrect)
                            )
                        )
                    )
                )
            );

            xmlDoc.Save(filePath);
        }

        public void SubmitAnswer(Question question, Answer selectedAnswer)
        {
            var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);

            var questionResult = new QuestionResult
            {
                QuestionText = question.Text,
                SelectedAnswer = selectedAnswer?.Text ?? "Не отвечено",
                CorrectAnswer = correctAnswer?.Text ?? "",
                IsCorrect = selectedAnswer?.IsCorrect ?? false
            };

            currentResult.QuestionResults.Add(questionResult);

            if (questionResult.IsCorrect)
            {
                currentResult.CorrectAnswers++;
            }

            // Пересчитываем процент
            currentResult.Percentage = (double)currentResult.CorrectAnswers / currentResult.TotalQuestions * 100;
        }

        public List<Question> GetShuffledQuestions()
        {
            var shuffled = questions.ToList();
            shuffled = shuffled.OrderBy(q => random.Next()).ToList();

            foreach (var question in shuffled)
            {
                question.Answers = question.Answers.OrderBy(a => random.Next()).ToList();
            }

            return shuffled;
        }
    }
}