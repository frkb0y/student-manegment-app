namespace plz_fix.Models
{
    public class TodoModel
    {
        public int OID { get; set; }
        public string WorkName { get; set; }
        public string WorkDescription { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsCompleted { get; set; }
        public string CreatedByName { get; set; } // ADDED for sender's name
    }
}