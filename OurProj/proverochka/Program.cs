using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurProj;

namespace proverochka
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new TaskReader();

            try
            {
                // Чтение из файла
                reader.ReadFromFile("tasks.txt");

                // Или чтение из строки
                // string text = @"КОЛИЧЕСТВО ЗАДАНИЙ = 2
                // ЗАДАНИЕ №1
                // Когда распался СССР?
                // Ответ: 1989-1990 годах
                // 
                // ЗАДАНИЕ №2
                // Где брать комплектующие для компьютера?
                // Ответ: Сайт ДНС";
                // reader.ReadFromText(text);

                // Отображение всех заданий
                reader.DisplayTasks();

                // Получение конкретного задания
                var task1 = reader.GetTaskByNumber(1);
                if (task1 != null)
                {
                    Console.WriteLine($"Задание 1: {task1.Question}");
                    Console.WriteLine($"Ответ: {task1.Answer}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
