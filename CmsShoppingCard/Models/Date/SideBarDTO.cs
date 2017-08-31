using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCard.Models.Date
{   [Table("tblSideBar")]
    public class SideBarDTO
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }
    }
}