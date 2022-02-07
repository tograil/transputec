namespace CrisesControl.Core.Models
{
    public partial class TourStep
    {
        public int TourStepId { get; set; }
        public string? TourName { get; set; }
        public string? TourKey { get; set; }
        public string? StepKey { get; set; }
        public string? StepTitle { get; set; }
        public string? StepDesc { get; set; }
        public string? NextLabel { get; set; }
        public string? PrevLabel { get; set; }
        public string? NextAction { get; set; }
        public string? PrevAction { get; set; }
        public string? ActionType { get; set; }
        public string? OnEnterEvent { get; set; }
        public string? OnLeaveEvent { get; set; }
        public string? ModalType { get; set; }
        public string? HighlightKey { get; set; }
        public string? TipPosition { get; set; }
        public int TipWidth { get; set; }
        public int HorizontalOffset { get; set; }
        public int VerticalOffset { get; set; }
        public string? Margin { get; set; }
        public bool IsAccessible { get; set; }
        public bool? Overlay { get; set; }
        public int Status { get; set; }
        public int StepOrder { get; set; }
    }
}
