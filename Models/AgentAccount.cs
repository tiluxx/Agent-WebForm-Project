//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Agent_WebForm_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AgentAccount
    {
        public string AgentACID { get; set; }
        public string AgentID { get; set; }
    
        public virtual C_User C_User { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
