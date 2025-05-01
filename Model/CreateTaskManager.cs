using System;
using System.Collections.Generic;
using Tubes_KPL.Model;
using System.Linq;

namespace Tubes_KPL.Manager
{
    public class CreateTaskManager<T> where T : Model.Task
    {
        private List<T> tasks = new List<T>();

        public void AddTask(T task)
        {
            if (task == null)
            {
                Console.WriteLine("Tugas yang akan ditambahkan tidak boleh null.");
                return;
            }
            tasks.Add(task);
        }

        public List<T> GetTasks(string userId)
        {
            return tasks.Where(task => task.UserId == userId).ToList();
        }

        public List<T> GetAllTasks()
        {
            return tasks;
        }
    }
}
