//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projeto_DA
{
    using System;
    using System.Collections.Generic;
    
    public partial class CasaArrendavel : Casa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CasaArrendavel()
        {
            this.Arrendamentos = new HashSet<Arrendamento>();
        }
    
        public decimal ValorBaseMes { get; set; }
        public decimal Comissao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Arrendamento> Arrendamentos { get; set; }
    }
}
