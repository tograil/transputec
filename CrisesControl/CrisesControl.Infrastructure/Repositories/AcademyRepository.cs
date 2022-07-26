using CrisesControl.Core.Academy;
using CrisesControl.Core.Academy.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class AcademyRepository: IAcademyRepository
    {
        private readonly CrisesControlContext _context;

        public AcademyRepository(CrisesControlContext context)
        {
            _context = context;
        }

        public async Task<List<AcademyVideos>> GetVideos(int UserID)
        {
            try
            {
                var pUserID = new SqlParameter("@UserID", UserID);
                var result = await  _context.Set<AcademyVideos>().FromSqlRaw("exec Pro_Get_Academy_Video @UserID", pUserID).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TourStep>> GetToursSteps()
        {
            try
            {
                //Use: EXEC [dbo].[Pro_Academy_GetActiveTourSteps]
                var steps = await _context.Set<TourStep>().Where(TR=> TR.Status == 1).OrderBy(o => o.StepOrder).ToListAsync();
                return steps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<int> SaveTourLog(int UserID, string TourName, string StepKey, string TimeZoneId)
        {
            try
            {
                var step =  await _context.Set<TourStep>().Where(TS=> TS.StepKey == StepKey).FirstOrDefaultAsync();
                if (step != null)
                {
                    TourLog TL = new TourLog();
                    TL.TourName = TourName;
                    TL.UserId = UserID;
                    TL.TourStepId = step.TourStepId;
                    TL.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(TL);
                    await _context.SaveChangesAsync();
                    return TL.Id;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
    }
}
