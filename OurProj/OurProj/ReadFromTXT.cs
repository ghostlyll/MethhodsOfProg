using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurProj
{
    public class TaskReader
    {
        public class Task
        {
            public int Number { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public List<string> WrongAnswers { get; set; } = new List<string>();

            // Свойство для получения всех ответов (правильный + неверные)
            public List<string> AllAnswers
            {
                get
                {
                    var allAnswers = new List<string> { Answer };
                    if (WrongAnswers != null && WrongAnswers.Any())
                    {
                        allAnswers.AddRange(WrongAnswers);
                    }
                    return allAnswers;
                }
            }

            // Метод для проверки правильности ответа
            public bool IsCorrectAnswer(string answer)
            {
                return string.Equals(answer?.Trim(), Answer?.Trim(), StringComparison.OrdinalIgnoreCase);
            }

            // Метод для перемешивания всех ответов
            public List<string> GetShuffledAnswers()
            {
                var random = new Random();
                return AllAnswers.OrderBy(a => random.Next()).ToList();
            }
        }

        public List<Task> Tasks { get; private set; }
        public int TaskCount { get; private set; }

        public void ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var lines = File.ReadAllLines(filePath);
            Tasks = new List<Task>();
            TaskCount = 0;

            ParseLines(lines);
        }

        public void ReadFromText(string text)
        {
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Tasks = new List<Task>();
            TaskCount = 0;

            ParseLines(lines);
        }

        private void ParseLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                // Парсим количество заданий
                if (line.StartsWith("КОЛИЧЕСТВО ЗАДАНИЙ =", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(line.Split('=')[1].Trim(), out int count))
                    {
                        TaskCount = count;
                    }
                }
                // Парсим начало задания
                else if (line.StartsWith("ЗАДАНИЕ №"))
                {
                    // Извлекаем номер задания
                    string numberStr = line.Replace("ЗАДАНИЕ №", "").Trim();
                    if (int.TryParse(numberStr, out int taskNumber))
                    {
                        // Следующая строка - вопрос
                        string question = "";
                        if (i + 1 < lines.Length)
                        {
                            question = lines[i + 1].Trim();
                            i++; // Переходим к следующей строке
                        }

                        string answer = "";
                        List<string> wrongAnswers = new List<string>();

                        // Ищем строки с ответом и неверными ответами
                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            string currentLine = lines[j].Trim();

                            if (string.IsNullOrEmpty(currentLine))
                                continue;

                            // Проверяем правильный ответ
                            if (currentLine.StartsWith("Ответ:", StringComparison.OrdinalIgnoreCase))
                            {
                                answer = currentLine.Substring("Ответ:".Length).Trim();
                            }
                            // Проверяем неверные ответы
                            else if (currentLine.StartsWith("Неверные ответы:", StringComparison.OrdinalIgnoreCase))
                            {
                                string wrongAnswersStr = currentLine.Substring("Неверные ответы:".Length).Trim();
                                wrongAnswers = ParseWrongAnswers(wrongAnswersStr);
                            }
                            // Если начинается следующее задание, заканчиваем обработку
                            else if (currentLine.StartsWith("ЗАДАНИЕ №"))
                            {
                                i = j - 1; // Возвращаемся на одну строку назад
                                break;
                            }

                            // Если дошли до конца файла или следующей строки с ЗАДАНИЕ
                            if (j + 1 >= lines.Length || lines[j + 1].Trim().StartsWith("ЗАДАНИЕ №"))
                            {
                                i = j; // Переходим к последней обработанной строке
                                break;
                            }
                        }

                        Tasks.Add(new Task
                        {
                            Number = taskNumber,
                            Question = question,
                            Answer = answer,
                            WrongAnswers = wrongAnswers
                        });
                    }
                }
            }
        }

        private List<string> ParseWrongAnswers(string wrongAnswersStr)
        {
            // Разделяем неверные ответы по запятой
            var answers = wrongAnswersStr
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .ToList();

            return answers;
        }

        public void DisplayTasks()
        {
            Console.WriteLine($"Количество заданий: {TaskCount}");
            Console.WriteLine();

            foreach (var task in Tasks)
            {
                Console.WriteLine($"Задание №{task.Number}");
                Console.WriteLine($"Вопрос: {task.Question}");
                Console.WriteLine($"Ответ: {task.Answer}");

                if (task.WrongAnswers != null && task.WrongAnswers.Any())
                {
                    Console.WriteLine($"Неверные ответы: {string.Join(", ", task.WrongAnswers)}");

                    // Показываем все ответы в перемешанном виде
                    var shuffled = task.GetShuffledAnswers();
                    Console.WriteLine($"Все варианты (перемешанные): {string.Join(", ", shuffled)}");
                }
                else
                {
                    Console.WriteLine("Неверные ответы: отсутствуют");
                }
                Console.WriteLine();
            }
        }

        public Task GetTaskByNumber(int number)
        {
            return Tasks.FirstOrDefault(t => t.Number == number);
        }

        // Новые методы для работы с неверными ответами

        /// <summary>
        /// Получает все ответы для задания в перемешанном порядке
        /// </summary>
        public List<string> GetShuffledAnswersForTask(int taskNumber)
        {
            var task = GetTaskByNumber(taskNumber);
            return task?.GetShuffledAnswers() ?? new List<string>();
        }

        /// <summary>
        /// Проверяет, является ли ответ правильным для указанного задания
        /// </summary>
        public bool CheckAnswer(int taskNumber, string answer)
        {
            var task = GetTaskByNumber(taskNumber);
            return task?.IsCorrectAnswer(answer) ?? false;
        }

        /// <summary>
        /// Проверяет, является ли ответ неверным для указанного задания
        /// </summary>
        public bool IsWrongAnswer(int taskNumber, string answer)
        {
            var task = GetTaskByNumber(taskNumber);
            if (task == null) return false;

            // Проверяем, есть ли этот ответ в списке неверных ответов
            return task.WrongAnswers.Any(wa =>
                string.Equals(wa?.Trim(), answer?.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Получает список всех неверных ответов для задания
        /// </summary>
        public List<string> GetWrongAnswers(int taskNumber)
        {
            var task = GetTaskByNumber(taskNumber);
            return task?.WrongAnswers ?? new List<string>();
        }

        /// <summary>
        /// Получает количество неверных ответов для задания
        /// </summary>
        public int GetWrongAnswersCount(int taskNumber)
        {
            var task = GetTaskByNumber(taskNumber);
            return task?.WrongAnswers?.Count ?? 0;
        }
    }
}
