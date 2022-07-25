namespace CrisesControl.Core.Incidents
{
    public class TaskPredecessorList
    {
        public int PredecessorTaskID { get; set; }
        public int TaskPredecessorID { get; set; }
        public int TaskSequence { get; set; }
        public string TaskTitle { get; set; }
        public int TaskStatus { get; set; }
    }  

 
}