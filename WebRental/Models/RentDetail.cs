//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebRental.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RentDetail
    {
        public int ID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public int BranchID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> CarID { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Car Car { get; set; }
        public virtual User User { get; set; }
    }
}
