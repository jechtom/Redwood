using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redwood.Framework.ViewModel;

namespace Redwood.Samples.Basic
{
    public class TaskListViewModel : ViewModelBase
    {
        public string NewTaskText { get; set; }

        public List<Task> Tasks { get; set; }


        public TaskListViewModel()
        {
            Tasks = new List<Task>();
            Tasks.Add(new Task() { Id = 1, Title = "Hello!" });
        }

        public void AddTask()
        {
            Tasks.Add(new Task() { Title = NewTaskText });
            NewTaskText = "";
        }

        public void Delete(int id)
        {
            Tasks.RemoveAll(t => t.Id == id);
        }
    }

    public class Task
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsFinished { get; set; }

        public void SetFinished()
        {
            IsFinished = true;
        }
    }
}