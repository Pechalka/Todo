using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MongoDB.Driver;

namespace Todo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaskDataService _taskDataService;

        public HomeController()
        {
            _taskDataService = new FileTaskDataService();
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

public interface ITaskDataService
{
    List<Task> GetCurrentTasks();
    void Update(List<Task> tasks);
}

class FileTaskDataService : ITaskDataService
{
    public string path {
        get { return HttpContext.Current.Server.MapPath("~/data.json"); }
    }

    public List<Task> GetCurrentTasks()
    {
        var data = File.ReadAllText(path);
        var serializer = new JavaScriptSerializer();
        var r = (List<Task>) serializer.Deserialize(data, typeof (List<Task>));
        return r ?? new List<Task>();
    }

    public void Update(List<Task> tasks)
    {
        if (tasks == null)
            return; 

        var serializer = new JavaScriptSerializer();
        var data = serializer.Serialize(tasks);

        File.WriteAllText(path, data);
    }
}


public class MongoDbTaskDataService : ITaskDataService
{
    private readonly MongoCollection<Task> _tasks;

    public MongoDbTaskDataService()
    {
        var server = MongoServer.Create("mongodb://localhost:27020/");
        var database = server.GetDatabase("todos");
        _tasks = database.GetCollection<Task>("tasks");
    }

    public List<Task> GetCurrentTasks()
    {
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