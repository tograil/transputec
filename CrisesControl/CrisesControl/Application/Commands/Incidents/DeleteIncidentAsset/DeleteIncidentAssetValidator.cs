using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteIncidentAsset
{
    public class DeleteIncidentAssetValidator:AbstractValidator<DeleteIncidentAssetRequest>
    {
        public DeleteIncidentAssetValidator()
        {
            RuleFor(x => x.IncidentAssetId).GreaterThan(0);
            RuleFor(x => x.IncidentId).GreaterThan(0);
            RuleFor(x => x.AssetObjMapId).GreaterThan(0);
        }
    }
}
