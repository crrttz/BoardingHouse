//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BoardingHouse.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class History
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> AccountId { get; set; }
        public Nullable<System.Guid> RoomId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> Is_Delete { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Room Room { get; set; }
    }
}
