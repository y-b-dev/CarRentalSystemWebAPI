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
    
    public partial class spGetRents_Result
    {
        public int ID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTime> ActualEndDate { get; set; }
        public int BranchID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> CarID { get; set; }
    }
}