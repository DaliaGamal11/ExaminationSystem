
namespace Examination_System
{
        public enum QuestionType
        {
            TrueFalse,
            ChooseOne,
            ChooseAll
        }

        // Delegate for exam completed event
        public delegate void ExamCompletedHandler(object sender, EventArgs e);

        public abstract class Question
        {
            public string Header { get; set; }
            public string Body { get; set; }
            public int Marks { get; set; }
            public QuestionType Type { get; protected set; }

            public abstract void Show();
            public abstract bool EvaluateAnswer(string[] userAnswers); // Multi-choice support
        }
        public class TrueFalseQuestion : Question
        {
            public bool CorrectAnswer { get; set; }

            public TrueFalseQuestion()
            {
                Type = QuestionType.TrueFalse;
            }

            public override void Show()
            {
                Console.WriteLine($"{Header} - {Marks} Marks\n{Body}\n1. True\n2. False");
            }

            public override bool EvaluateAnswer(string[] userAnswers)
            {
                return bool.TryParse(userAnswers[0], out bool ans) && ans == CorrectAnswer;
            }
        }
        public class ChooseOneQuestion : Question
        {
            public string[] Options { get; set; }
            public string CorrectAnswer { get; set; }

            public ChooseOneQuestion()
            {
                Type = QuestionType.ChooseOne;
            }

            public override void Show()
            {
                Console.WriteLine($"{Header} - {Marks} Marks\n{Body}");
                for (int i = 0; i < Options.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Options[i]}");
                }
            }

            public override bool EvaluateAnswer(string[] userAnswers)
            {
                return userAnswers.Length > 0 && userAnswers[0].Trim().Equals(CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            }
        }
        public class ChooseAllQuestion : Question
        {
            public string[] Options { get; set; }
            public string[] CorrectAnswers { get; set; }

            public ChooseAllQuestion()
            {
                Type = QuestionType.ChooseAll;
            }

            public override void Show()
            {
                Console.WriteLine($"{Header} - {Marks} Marks\n{Body}");
                for (int i = 0; i < Options.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Options[i]}");
                }
            }

            public override bool EvaluateAnswer(string[] userAnswers)
            {
                if (userAnswers.Length != CorrectAnswers.Length) return false;

                foreach (var ans in userAnswers)
                {
                    if (!CorrectAnswers.Contains(ans.Trim(), StringComparer.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            }
        }



        public abstract class Exam
        {
            public int TimeInMinutes { get; set; }
            public Question[] Questions { get; set; }
            public Subject Subject { get; set; }

            public event ExamCompletedHandler ExamCompleted;

            public Exam(int time, Question[] questions, Subject subject)
            {
                TimeInMinutes = time;
                Questions = questions;
                Subject = subject;
            }

            public abstract void ShowExam();

            protected void OnExamCompleted()
            {
                ExamCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public class PracticeExam : Exam
        {
            public PracticeExam(int time, Question[] questions, Subject subject)
                : base(time, questions, subject) { }

            public override void ShowExam()
            {
                int score = 0;
                for (int i = 0; i < Questions.Length; i++)
                {
                    Questions[i].Show();
                    Console.Write("Your Answer: ");
                    string userInput = Console.ReadLine();
                    string[] answers = userInput.Split(',');

                    if (Questions[i].EvaluateAnswer(answers))
                    {
                        score += Questions[i].Marks;
                        Console.WriteLine("Correct!\n");
                    }
                    else
                    {
                        Console.WriteLine("Wrong Answer.\n");
                    }
                }
                Console.WriteLine($"Your Score: {score}");
                OnExamCompleted();
            }
        }

        public class FinalExam : Exam
        {
            public FinalExam(int time, Question[] questions, Subject subject)
                : base(time, questions, subject) { }

            public override void ShowExam()
            {
                int score = 0;
                for (int i = 0; i < Questions.Length; i++)
                {
                    Questions[i].Show();
                    Console.Write("Your Answer: ");
                    string userInput = Console.ReadLine();
                    string[] answers = userInput.Split(',');

                    if (Questions[i].EvaluateAnswer(answers))
                        score += Questions[i].Marks;

                    Console.WriteLine(); // no feedback
                }
                Console.WriteLine($"Your Final Score: {score}");
                OnExamCompleted();
            }
        }


        public class Answer
        {
            public string[] SelectedAnswers { get; set; } // Supports multiple for Choose All
        }
        public class Subject
        {
            public string Name { get; set; }
            public string Code { get; set; }

            public Subject(string name, string code)
            {
                Name = name;
                Code = code;
            }
        }



    class Program
    {
        static void Main()
        {
            Subject subject = new Subject("Computer Science", "CS101");

            Question[] questions = new Question[3];

            questions[0] = new TrueFalseQuestion
            {
                Header = "Q1",
                Body = "C# is a statically typed language.",
                Marks = 5,
                CorrectAnswer = true
            };

            questions[1] = new ChooseOneQuestion
            {
                Header = "Q2",
                Body = "Which language is primarily used for web development?",
                Marks = 5,
                Options = new string[] { "C#", "Java", "JavaScript", "Python" },
                CorrectAnswer = "JavaScript"
            };

            questions[2] = new ChooseAllQuestion
            {
                Header = "Q3",
                Body = "Select all object-oriented languages:",
                Marks = 10,
                Options = new string[] { "C#", "HTML", "Python", "CSS" },
                CorrectAnswers = new string[] { "C#", "Python" }
            };

            Console.WriteLine("Choose Exam Type:\n1. Practice Exam\n2. Final Exam");
            string choice = Console.ReadLine();

            Exam exam;

            if (choice == "1")
                exam = new PracticeExam(30, questions, subject);
            else
                exam = new FinalExam(30, questions, subject);

            exam.ExamCompleted += OnExamFinished;

            Console.WriteLine($"\n--- {subject.Name} Exam ---\n");
            exam.ShowExam();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void OnExamFinished(object sender, EventArgs e)
        {
            Console.WriteLine("\n--- Exam Completed Successfully! ---");
        }
    }

}

