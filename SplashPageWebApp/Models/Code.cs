//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SplashPageWebApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Code
    {
        public int id { get; set; }
        public string code1 { get; set; }
        public string sponsorEmail { get; set; }
        public bool isUsed { get; set; }
        public System.DateTime startTime { get; set; }
        public System.DateTime expiredTime { get; set; }
    }
}
