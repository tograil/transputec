using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class AppLanguages
    {
        [NotMapped]
        public LanguageItem languageItem { get; set; }
        public List<LanguageItem> languageItems { get; set; }
    }
}
