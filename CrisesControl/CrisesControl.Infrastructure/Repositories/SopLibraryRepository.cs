using CrisesControl.Core.Models;
using CrisesControl.Core.SopLibrary;
using CrisesControl.Core.SopLibrary.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SopLibraryRepository: ISopLibraryRepository
    {
        private readonly CrisesControlContext _context;
        public SopLibraryRepository(CrisesControlContext context)
        {
            _context = context;
        }
        
        public async Task<int> AU_Section(int SOPHeaderID, int IncidentID, string SOPVersion, DateTimeOffset NextReviewDate,
            int ContentID, int ContentSectionID, string SectionName, string SectionDescription, int SectionStatus, List<int> Tags,int CurrentUserId, int CompanyID, string TimeZoneId)
        {
            int rtContentSectionID = 0;
            try
            {
                int rtContentID = 0;
                int rtSOPDetailID = 0;

                SOPHeaderID = await AU_LibSOPHeader(SOPHeaderID, IncidentID, SOPVersion, NextReviewDate, SectionStatus, CurrentUserId, CompanyID, TimeZoneId);
                if (SOPHeaderID > 0)
                {

                    rtContentSectionID = await AU_LibContentSection(ContentSectionID, SOPHeaderID, SectionName, SectionStatus);

                    if (rtContentSectionID > 0)
                        rtContentID = await AU_LibContent(ContentID, SectionDescription, 1,CurrentUserId, TimeZoneId);

                    if (rtContentID > 0 && rtContentSectionID > 0)
                        rtSOPDetailID = await AU_LibSOPDetail(SOPHeaderID, rtContentID, rtContentSectionID);

                    if (Tags.Count > 0 && rtContentID > 0)
                       await AU_LibContentTag(rtContentID, Tags,CurrentUserId, TimeZoneId);
                }
                return rtContentSectionID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           

        }

        public async Task<int> AU_LibSOPHeader(int SOPHeaderID, int IncidentID, string SOPVersion, DateTimeOffset NextReviewDate, int currentUserId, int CompanyID, int Status = 1,  string TimeZoneId = "GMT Standard Time")
        {
            int Rt_SopHeaderId = 0;
            try
            {
                if (SOPHeaderID > 0)
                {
                    var sop_head =await _context.Set<LibSopheader>().Where(SH=> SH.LibSopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                    if (sop_head != null)
                    {

                        sop_head.ReviewDate = NextReviewDate;
                        sop_head.Sopversion = SOPVersion;
                        sop_head.UpdatedBy = currentUserId;
                        sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                        sop_head.IncidentId = IncidentID;
                        _context.Update(sop_head);
                        await _context.SaveChangesAsync();

                        Rt_SopHeaderId = sop_head.LibSopheaderId;
                    }
                }
                else
                {
                    LibSopheader sop_head = new LibSopheader();
                    sop_head.ReviewDate = NextReviewDate;
                    sop_head.CompanyId = CompanyID;
                    sop_head.Sopversion = SOPVersion;
                    sop_head.IncidentId = IncidentID;
                    sop_head.CreatedBy = currentUserId;
                    sop_head.CreatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                    sop_head.UpdatedBy = currentUserId;
                    sop_head.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await _context.AddAsync(sop_head);
                    await _context.SaveChangesAsync();

                    Rt_SopHeaderId = sop_head.LibSopheaderId;
                }
                return Rt_SopHeaderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<int> AU_LibContentSection(int ContentSectionID, int SOPHeaderID, string SectionName, int Status)
        {
            try
            {
                int libSectionID = 0;
                if (ContentSectionID > 0)
                {
                    var content = await _context.Set<LibContentSection>().Where(CS=> CS.LibSectionId == ContentSectionID && CS.LibSopheaderId == SOPHeaderID).FirstOrDefaultAsync();
                    if (content != null)
                    {
                        content.SectionName = SectionName;
                        content.Status = Status;
                        _context.Update(content);
                      await  _context.SaveChangesAsync();
                        libSectionID= content.LibSectionId;
                    }
                }
                else
                {
                    LibContentSection content = new LibContentSection();
                    content.SectionName = SectionName;
                    content.Status = Status;
                    content.LibSopheaderId = SOPHeaderID;
                    await _context.AddAsync(content);
                    await _context.SaveChangesAsync();
                    libSectionID= content.LibSectionId;
                }
                return libSectionID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<int> AU_LibContent(int ContentID, string ContentText, int Status,int currentUserId, string TimeZoneId)
        {
            try
            {
                int PrimaryContentID = 0;
                string old_checksum = "";
                string Checksum = ContentText.Trim().PWDencrypt();

                if (ContentID > 0)
                {
                    var old_content = await _context.Set<LibContent>().Where(C=> C.LibContentId == ContentID).FirstOrDefaultAsync();
                    if (old_content != null)
                    {
                        old_checksum = old_content.Checksum;
                        if (old_checksum != Checksum)
                        {
                            PrimaryContentID = old_content.PrimaryContentId == 0 ? old_content.LibContentId : old_content.LibContentId;
                            old_content.Status = 2;
                            //CreateContentVersion(PrimaryContentID, ContentID);
                        }
                    }
                }

                if (old_checksum.Trim() != Checksum.Trim())
                {
                    LibContent content = new LibContent();
                    content.ContentText = ContentText;
                    content.Status = Status;
                    content.CreatedBy = currentUserId;
                    content.Checksum = Checksum;
                    content.PrimaryContentId = PrimaryContentID;
                    content.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    content.UpdatedBy = currentUserId;
                    content.UpdatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);
                   await _context.AddAsync(content);
                    await _context.SaveChangesAsync();
                    ContentID= content.LibContentId;
                }
                return ContentID;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AU_LibSOPDetail(int SOPHeaderID, int ContentID, int ContentSectionID)
        {
            int sop_detail_id = 0;
            try
            {
                var old_content = await _context.Set<LibSopdetail>()
                                   .Where(SD=> SD.LibSopheaderId == SOPHeaderID && SD.LibContentSectionId == ContentSectionID
                                   ).FirstOrDefaultAsync();
                if (old_content != null)
                {
                    old_content.LibContentId = ContentID;
                    _context.Update(old_content);
                    await _context.SaveChangesAsync();
                    sop_detail_id = old_content.LibSopdetailId;
                    //return old_content.SOPDetailID;
                }
                else
                {
                    LibSopdetail sop_detail = new LibSopdetail();
                    sop_detail.LibSopheaderId = SOPHeaderID;
                    sop_detail.LibContentId = ContentID;
                    sop_detail.LibContentSectionId = ContentSectionID;
                    await _context.AddAsync(sop_detail);
                    await _context.SaveChangesAsync();
                    sop_detail_id = sop_detail.LibSopdetailId;
                }
                return sop_detail_id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task AU_LibContentTag(int ContentID, List<int> Tags,int currentUserId,  string TimeZoneId)
        {
            try
            {
                var existing =await  _context.Set<LibContentTag>().Where(c => c.LibContentId == ContentID).Select(s => new { s.TagId, s.LibContentTagId }).ToListAsync();
                var existingtags = existing.Select(s => s.TagId).ToList();
                var todelete = existing.Where(c => !Tags.Contains(c.TagId)).Select(s => s.LibContentTagId).ToList();

                var newitems = Tags.Where(tw => !existingtags.Contains(tw)).Select(s => s).ToList();

                if (todelete.Count > 0)
                {
                    _context.RemoveRange(_context.Set<LibContentTag>().Where(_ => todelete.Contains(_.LibContentTagId)));
                   await _context.SaveChangesAsync();
                }

                foreach (int TagID in newitems)
                {
                    LibContentTag tags = new LibContentTag();
                    tags.TagId = TagID;
                    tags.LibContentId = ContentID;
                    tags.CreatedBy = currentUserId;
                    tags.CreatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    tags.UpdatedBy = currentUserId;
                    tags.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                    await  _context.AddAsync(tags);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<dynamic> RecordLibraryUsage(int LibSOPHeaderID)
        {
            try
            {
                var section = await _context.Set<LibSopheader>().Where(SH => SH.LibSopheaderId == LibSOPHeaderID).FirstOrDefaultAsync();
                if (section != null)
                {
                    section.NoOfUse = section.NoOfUse + 1;
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                    var getsection = await GetSOPLibrarySection(section.LibSopheaderId, section.CompanyId);
                    return getsection;
                }
                return section;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public async Task<bool> DeleteSOPLib(int LibSOPHeaderID)
        {

            try
            {
                var section = await  _context.Set<LibSopheader>().Where(SH=> SH.LibSopheaderId == LibSOPHeaderID).FirstOrDefaultAsync();
                if (section != null)
                {
                    section.Status = 3;
                    _context.Update(section);
                  await  _context.SaveChangesAsync();
                    return true;
                }
              
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
               
            }
        }

      public async  Task<List<SopSectionList>> GetSOPLibrarySections(int CompanyID)
        {
            var sections = (from SH in _context.Set<LibSopheader>()
                            join I in _context.Set<LibIncident>() on SH.IncidentId equals I.LibIncidentId
                            join IT in _context.Set<LibIncidentType>() on I.LibIncidentTypeId equals IT.LibIncidentTypeId
                            join CS in _context.Set<LibContentSection>() on SH.LibSopheaderId equals CS.LibSopheaderId
                            where SH.CompanyId == CompanyID
                            select new SopSectionList
                            { 
                             Header= SH, 
                              IncidentID = I.LibIncidentId, 
                              Name= I.Name,
                              IncidentTypeName = IT.Name,
                              SectionName=CS.SectionName, 
                              Status=SH.Status 
                            }).ToList();

            return sections;
        }

        public async Task<SopSection> GetSOPLibrarySection(int SOPHeaderID, int CompanyID)
        {
            try {
               
            var section = (
                          from SH in _context.Set<LibSopheader>()
                          join I in _context.Set<LibIncident>() on SH.IncidentId equals I.LibIncidentId
                          join IT in _context.Set<LibIncidentType>() on I.LibIncidentTypeId equals IT.LibIncidentTypeId
                          join CS in _context.Set<LibContentSection>() on SH.LibSopheaderId equals CS.LibSopheaderId
                          join SD in _context.Set<LibSopdetail>() on CS.LibSectionId equals SD.LibContentSectionId
                          join C in _context.Set<LibContent>() on SD.LibContentId equals C.LibContentId
                          where CS.LibSopheaderId == SOPHeaderID
                          select  new SopSection()
                          {
                              Name=I.Name,
                              IncidentTypeName = IT.Name,
                             LibSopheaderId= CS.LibSopheaderId,
                              IncidentId=SH.IncidentId,
                              ReviewDate=SH.ReviewDate,
                              Sopversion=SH.Sopversion,
                              LibSectionId=CS.LibSectionId,
                              LibContentId=C.LibContentId,
                              SectionDescription = C.ContentText,
                              SectionName=CS.SectionName,
                              //todo:int TagId List
                              //TagId = new List<int>(_context.Set<Tag>().Include(c=>c.LibContentTag).Where(CT => CT.LibContentId == C.LibContentId).Select(a => a.TagId).ToList()),
                          }).FirstOrDefault();

            return section;
            }
            catch(Exception ex)
            {
                throw ex;
            }




        }
    }
}
