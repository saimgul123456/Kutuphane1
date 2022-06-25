using System;
using System.ComponentModel;

namespace Kutuphane1.Models
{
	public class SayisalKitap : BaseObject
	{

		[DisplayName("Kayıt Tarihi")]
		public DateTime KAYİTTARİHİ { get; set; }
		[DisplayName("Kitabın Adı")]
		public string KİTAPADİ { get; set; }
		[DisplayName("Adet")]
		public int ADET { get; set; }
		[DisplayName("Fiyat")]
		public decimal? FİYAT { get; set; }
		[DisplayName("Tutar")]
		public decimal? TUTAR { get; set; }
		[DisplayName("Kdv")]
		public decimal? KDV { get; set; }
		[DisplayName("Toplam Tutar")]
		public decimal? TOPLAMTUTAR { get; set; }

		public int KitapAdiId { get; set; }
		public KitapAdi KitapAdi { get; set; }
	}
}