using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Redwood.Framework.ViewModel;

namespace Redwood.Samples.Basic
{
    public class TaskListViewModel : ViewModelBase
    {
        public List<Task> Tasks { get; set; }
    }

    public class Task
    {
        public string Title { get; set; }
    }
}