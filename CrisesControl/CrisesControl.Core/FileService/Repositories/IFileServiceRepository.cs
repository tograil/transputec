using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.FileService.Repositories
{
    public interface IFileServiceRepository
    {
        Task<bool> UploadFileToFileShare(string shareName, string dirName, string fileName, Stream fileStream);
    }
}
