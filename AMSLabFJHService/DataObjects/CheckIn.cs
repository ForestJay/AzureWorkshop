using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace AMSLabFJHService.DataObjects
{
    public class CheckIn : EntityData
    {
        public string PersonId { get; set; }
        public string Location { get; set; }
        public string CheckInTime { get; set; }
    }
}