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
    
    public partial class Fact
    {
        public int key1 { get; set; }
        public int key2 { get; set; }
        public int key3 { get; set; }
        public int measure1 { get; set; }
        public int measure2 { get; set; }
        public int measure3 { get; set; }
        public byte[] filler { get; set; }
    
        public virtual Dim1 Dim1 { get; set; }
        public virtual Dim2 Dim2 { get; set; }
        public virtual Dim3 Dim3 { get; set; }
    }
}