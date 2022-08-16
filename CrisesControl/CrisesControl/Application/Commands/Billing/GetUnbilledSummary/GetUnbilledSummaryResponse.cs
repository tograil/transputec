using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryResponse
    {
        public string TransactionMonth { get; set; }
        public int TransactionYear { get; set; }
        public int TotalMessages { get; set; }
        public int TotalRecipients { get; set; }
        public decimal TotalUnBilledWithVAT { get; set; }
        public decimal TotalBilledWithVAT { get; set; }
        public int MonthNumber { get; set; }
        public int MessageId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string ToFormatted { get; set; }
        public string Status { get; set; }
        public DateTimeOffset DateDelivered { get; set; }
        public int MessageAckStatus { get; set; }
        public DateTimeOffset DateAcknowledge { get; set; }
        public string AckMethod { get; set; }
        public int NumSegments { get; set; }
    }
}
