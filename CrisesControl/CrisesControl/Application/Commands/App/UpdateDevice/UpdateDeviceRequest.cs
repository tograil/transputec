using CrisesControl.SharedKernel.Enums;
using MediatR;

namespace CrisesControl.Api.Application.Commands.App.UpdateDevice
{
    public class UpdateDeviceRequest:IRequest<UpdateDeviceResponse>
    {
          public  bool IsSirenOn       {get;set;}
          public  bool OverrideSilent {get;set;}
          public  string SoundFile    {get;set;}
          public  UpdateType UpdateType   {get;set;}
          public  string Language     {get;set;}
          public string DeviceSerial { get; set; }
    }
}
