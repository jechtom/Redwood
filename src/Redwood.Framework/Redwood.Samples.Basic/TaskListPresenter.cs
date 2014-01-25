using Redwood.Framework.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Samples.Basic
{
    public class TaskListPresenter : RedwoodPresenter
    {
        public void Get()
        {
            /*
            var page = Page("TaskListView.rwhtml");
            page.DataContext = new TaskListViewModel();
            return page;
            */
            //return View("TaskListView");
        }

        //[ClientCommand]
        public object AddTask(TaskListViewModel model, string newTaskText)
        {
            model.Tasks.Add(new Task() { Title = newTaskText });
            return model;
        }


        //[ClientCommand]
        public object FinishTask(TaskListViewModel model, int taskIndex)
        {
            model.Tasks.RemoveAt(taskIndex);
            return model;
        }
    }
}