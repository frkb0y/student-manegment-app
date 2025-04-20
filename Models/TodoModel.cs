namespace plz_fix.Models
{
    public class TodoModel
    {
        public int OID { get; set; } // Primary Key
        public string WorkName { get; set; }
        public string WorkDescription { get; set; }
        public DateTime StartDate { get; set; }

        // Add these if you want them later
        // public DateTime EndDate { get; set; }
        // public bool IsCompleted { get; set; }
        // etc...
    }


}
