using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Application.Commands.Assets.UpdateAssets {
    public class UpdateAssetsRequest : IRequest<UpdateAssetsResponse> {
        [FromRoute]
        public int AssetId { get; set; }
        public int CompanyId { get; set; }
        public string AssetTitle { get; set; } = null!;
        public string? AssetDescription { get; set; }
        public string? AssetType { get; set; }
        public string? AssetPath { get; set; }
        public string FilePath { get; set; }
        public double AssetSize { get; set; }
        public int Status { get; set; }
        public int SourceObjectId { get; set; }
        public int AssetTypeId { get; set; }
        public string? SourceFileName { get; set; }
        public int? AssetOwner { get; set; }
        public int? ReminderCount { get; set; }
        public DateTimeOffset? ReviewDate { get; set; }
        public string? ReviewFrequency { get; set; }
    }
}
