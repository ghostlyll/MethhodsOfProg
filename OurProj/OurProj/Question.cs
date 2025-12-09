using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurProj
{
    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public int Points { get; set; } = 1;
    }

    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }

    public class QuizResult
    {
        public string StudentName { get; set; }
        public string StudentGroup { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double Percentage { get; set; }
        public DateTime CompletionTime { get; set; }
        public List<QuestionResult> QuestionResults { get; set; } = new List<QuestionResult>();
    }

    public class QuestionResult
    {
        public string QuestionText { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}