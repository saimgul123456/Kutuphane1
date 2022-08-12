using System;
using System.Collections.Generic;
using System.Linq;
using Kutuphane1.Models;
using Newtonsoft.Json.Linq;

namespace Kutuphane1.Helpers
{
    public static class EntityExtensions
    {
        public static List<SayisalKitap> ConvertJsonToEntity(IEnumerable<JToken> model)
        {
            return (from item in model
                    select new SayisalKitap
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        KitapAdiId = Convert.ToInt32(item["KitapAdiId"]),
                        KAYİTTARİHİ = Convert.ToDateTime(item["KAYİTTARİHİ"]!.ToString()),
                        KİTAPADİ = item["KİTAPADİ"]!.ToString(),
                        ADET = Convert.ToInt32(item["ADET"]!.ToString()),
                        FİYAT = Convert.ToDecimal(item["FİYAT"]!.ToString()),
                        TUTAR = Convert.ToDecimal(item["TUTAR"]!.ToString()),
                        KDV = Convert.ToDecimal(item["KDV"]!.ToString()),
                        TOPLAMTUTAR = Convert.ToDecimal(item["TOPLAMTUTAR"]!.ToString()),
                        Aktif = true,
                        Silindi = false,
                        OlusturanKullanici = "userX",
                        OlusturmaTarihi = DateTime.Now,
                        GuncelleyenKullanici = "userX",
                        GuncellemeTarihi = DateTime.Now
                    }).ToList();
        }
    }
}