using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Academy.Repositories
{
    public interface IAcademyRepository
    {
        Task<List<AcademyVideos>> GetVideos(int UserID);
        Task<List<TourStep>> GetToursSteps();
        Task<int> SaveTourLog(int UserID, string TourName, string StepKey, string TimeZoneId);
    }
}
