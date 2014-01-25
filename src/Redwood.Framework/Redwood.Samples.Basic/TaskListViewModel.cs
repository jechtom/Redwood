using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redwood.Samples.Basic
{
    public class TaskListViewModel
    {
        public List<Task> Tasks { get; set; }
    }

    public class Task
    {
        public string Title { get; set; }
    }
}