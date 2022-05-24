namespace CrisesControl.Core.Tasks;
public class Predecessor
{
    public int PredecessorTaskID { get; set; }
    public int TaskPredecessorID { get; set; }
    public int TaskSequence { get; set; }
    public string TaskTitle { get; set; }
}