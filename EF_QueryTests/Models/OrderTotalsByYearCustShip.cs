//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EF_QueryTests.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderTotalsByYearCustShip
    {
        public string customercompany { get; set; }
        public string shippercompany { get; set; }
        public Nullable<int> orderyear { get; set; }
        public Nullable<int> qty { get; set; }
        public Nullable<decimal> val { get; set; }
    }
}
