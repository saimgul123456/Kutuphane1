using System.Collections.Generic;
using System.ComponentModel;

namespace Kutuphane1.Models
{
    public class KitapAdi : BaseObject
    {

        [DisplayName("Kitabın Adı")]
        public string KİTABİNADİ { get; set; }

        public virtual ICollection<SayisalKitap> SayisalKitaplar { get; set; }
        public virtual ICollection<OrtakRaf> OrtakRafKitaplar { get; set; }
    }
}