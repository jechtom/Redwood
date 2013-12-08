using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redwood.Framework.ViewModel;

namespace Redwood.Samples.Basic
{
    public class IndexViewModel : ViewModelBase
    {

        public string NewTaskText { get; set; }

        public List<Task> Tasks { get; set; } 


        public void AddTask()
        {
            Tasks.Add(new Task() { Title = NewTaskText });
            NewTaskText = string.Empty;
        }

        public void FinishTask(Task task)
        {
            Tasks.Remove(task);
        }

    }

    public class Task
    {
        public string Title { get; set; }
    }
}