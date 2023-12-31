﻿using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.CreateAsset
{
    public class CreateAssetRequest: IRequest<CreateAssetResponse>
    {
        public int CompanyId { get; set; }
        public string AssetTitle { get; set; } = null!;
        public string? AssetDescription { get; set; }
        public string? AssetType { get; set; }
        public string? AssetPath { get; set; }
        public double AssetSize { get; set; }
        public int Status { get; set; }
        public int SourceObjectId { get; set; }
        public int AssetTypeId { get; set; }
        public string? FilePath { get; set; }
        public string? SourceFileName { get; set; }
        public int? AssetOwner { get; set; }
        public int? ReminderCount { get; set; }
        public DateTimeOffset ReviewDate { get; set; }
        public string? ReviewFrequency { get; set; }
    }
}
