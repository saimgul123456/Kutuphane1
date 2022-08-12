using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kutuphane1.Models
{
    public class OrtakRaf : BaseObject
    {
        [DisplayName("Kayıt Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime KAYİTTARİHİ { get; set; }
        [DisplayName("Kitap Adı")]
        public string KİTAPADİ { get; set; }
        [DisplayName("Adet")]
        public int ADET { get; set; }

        public int excelKodu { get; set; }
        public int KitapAdiId { get; set; }
        public KitapAdi KitapAdi { get; set; }
    }
}