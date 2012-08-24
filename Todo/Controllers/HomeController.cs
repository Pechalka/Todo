using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Driver;

namespace Todo.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskDataService _taskDataService;

        public HomeController()
        {
            _taskDataService = new TaskDataService();
        }

        public ActionResult Index()
        {
            return View(_taskDataService.GetCurrentTasks());
        }

        public ActionResult Update(List<Task> tasks)
        {
            _taskDataService.Update(tasks);
            return new EmptyResult();
        }

    }
}


public class TaskDataService
{
    private readonly MongoCollection<Task> _tasks;

    public TaskDataService()
    {
        var server = MongoServer.Create("mongodb://localhost:27020/");
        var database = server.GetDatabase("todos");
        _tasks = database.GetCollection<Task>("tasks");
    }

    public List<Task> GetCurrentTasks()
    {
        //return new List<Task>
        //           {
        //               new Task{ title = "test"},
        //               new Task{ title = "test1"},
        //               new Task{ title = "test2"},
        //               new Task{ title = "test3"}
        //           };

        return _tasks.FindAll().ToList();
    }

    public void Update(List<Task> tasks)
    {
        if (tasks == null || tasks.Count == 0)
            return; 
        _tasks.RemoveAll();
        _tasks.InsertBatch(tasks);
    }
}

public class Task
{
    public Guid Id { get; set; }
    public string title { get; set; }

    public bool Day1 { get; set; }
    public bool Day2 { get; set; }
    public bool Day3 { get; set; }
    public bool Day4 { get; set; }
    public bool Day5 { get; set; }
    public bool Day6 { get; set; }
    public bool Day7 { get; set; }
}