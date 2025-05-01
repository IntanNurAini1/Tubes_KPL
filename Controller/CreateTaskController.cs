using System;
using System.Collections.Generic;
using Tubes_KPL.Model;
using Tubes_KPL.Manager;

namespace Tubes_KPL.Controller
{
    public class CreateTaskController<T> where T : Model.Task
    {
        private readonly CreateTaskManager<T> taskManager;

        public CreateTaskController(CreateTaskManager<T> taskManager)
        {
            this.taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
        }

        public void CreateTask(string name, string description, Deadline deadline, string userId)
        {
            if (name == null || description == null || deadline == null || userId == null)
            {
                Console.WriteLine("Nama, deskripsi, deadline, dan User ID tugas tidak boleh kosong.");
                return;
            }

            T newTask = (T)Activator.CreateInstance(typeof(T), name, description, deadline, userId); // Tambah userId ke instance

            taskManager.AddTask(newTask);
            Console.WriteLine("Tugas berhasil dibuat!");
        }

        public List<T> GetTasks(string userId)
        {
            return taskManager.GetTasks(userId);
        }
    }
}
