using System;

namespace Tubes_KPL.Model
{
    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Deadline Deadline { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
        public Status Status { get; set; } = Status.Incompleted; // default

        public Task(string name, string description, Deadline deadline, string userId) // Tambah userId di constructor
        {
            Name = name;
            Description = description;
            Deadline = deadline;
            UserId = userId; // Set userId
        }

        public override string ToString()
        {
            return $"Nama: {Name}, Deskripsi: {Description}, Deadline: {Deadline}, Dibuat oleh: {UserId}";
        }
    }
}
