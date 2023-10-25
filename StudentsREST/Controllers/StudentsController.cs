using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace StudentsREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private List<Student> _students; // список студентов, с которым мы работаем

        private string _pathToJson; // хранит путь до json файла со студентами

        public StudentsController()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            // получаем конфигурацию проекта где есть путь до файла
            var config = new ConfigurationBuilder().AddJsonFile(filePath).Build();
            _pathToJson = config["Path"]; // путь до файла со студентами, который мы указали в appsettings.json

            if (System.IO.File.Exists(_pathToJson)) //проверка если файл существует
            {
                string jsonString = System.IO.File.ReadAllText(_pathToJson); // считываем все данные с файла в виде json строки
                List<Student> students = JsonSerializer.Deserialize<List<Student>>(jsonString); // преобразуем json в список студентов

                if (students != null) 
                {
                    _students = students; 
                }
                else
                    throw new Exception("No students, list is null");
            }
            else // если файл не найден то Exception
            {
                throw new Exception("Can't find file");
            }
        }

        //с помощью тройного слэша можно делать автодокументирование в сваггере

        /// <summary>
        /// return list of students
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("students")]
        public List<Student> GetStudents()
        {
            return _students;
        }

        /// <summary>
        /// return student by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet, Route("students/{id}")]
        public Student GetStudentById(int id) 
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null) throw new Exception("Student with id not found");
            return student;
        }

        /// <summary>
        /// add new student
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        [HttpPost, Route("addstudents/{id}/{name}")]
        public void AddStudent(int id, string name)
        {
            Student student = new Student { Id = id, Name = name }; //создаем нового студента 

            _students.Add(student); //добавляем его в наш список
            string json = JsonSerializer.Serialize(_students); // делаем из списка json строку
            System.IO.File.WriteAllText(_pathToJson, json); //перезаписываем json в файл

        }

    }
}
