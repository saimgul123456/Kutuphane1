using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kutuphane1.Models
{
    public class SayisalKitap : BaseObject
    {

        [DisplayName("Kayıt Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime KAYİTTARİHİ { get; set; }
        [DisplayName("Kitabın Adı")]
        public string KİTAPADİ { get; set; }
        [DisplayName("Adet")]
        public int ADET { get; set; }
        [DisplayName("Fiyat"), DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? FİYAT { get; set; }
        [DisplayName("Tutar"), DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? TUTAR { get; set; }
        [DisplayName("Kdv"), DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? KDV { get; set; }

        [DisplayName("Toplam Tutar"), DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? TOPLAMTUTAR { get; set; }

        public int KitapAdiId { get; set; }
        public KitapAdi KitapAdi { get; set; }
    }
}