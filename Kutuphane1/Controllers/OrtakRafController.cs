using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Kutuphane1.Context;
using Kutuphane1.Helpers;
using Kutuphane1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Kutuphane1.Controllers
{
  [Authorize]
  public class OrtakRafController : BaseController
    {
        private readonly DataContext _dataContext; //burayı sor***?
        public static dynamic OrtakRafEkle { get; set; }
        public OrtakRafController(DataContext dataContext) //değişti***
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Liste()
        {
            ViewBag.BirimFiyat = "0,00";
            //todo: arayüzdeki textbox a random ve daha önce veritabanında olmayan bir değer göndereceğiz.
            var rnd = new Random();
            var sayi = rnd.Next(0, 999999);
            var result = _dataContext.OrtakRaf.FirstOrDefault(t => t.excelKodu == sayi);

            while (result != null)
            {
                sayi = rnd.Next(0, 99999);
                result = _dataContext.OrtakRaf.FirstOrDefault(t => t.excelKodu == sayi);
            }

            ViewBag.RandomVeri = sayi;
            TempData["RandomSayi"] = sayi;

            var sl = await _dataContext.OrtakRaf
                    .Select(o => new SelectListItem
                    {
                        Value = o.KitapAdiId.ToString(),
                        Text = o.KİTAPADİ
                    })
                    .Distinct().OrderBy(x => x.Text)
                    .ToListAsync();
            ViewBag.KitapAdi = new SelectList(sl, "Value", "Text");

            var ortakRaf = await _dataContext.OrtakRaf.Include(t => t.KitapAdi).ToListAsync();

            return View(ortakRaf);
        }

        [HttpPost]
        public IActionResult ImportExcel(IFormFile dosya, int excelKodu)
        {
            //excelden okunan verilerin biriktirileceği nesne
            var dt = new DataTable();

            //içerisine verilen nesnenin using in yaşam alanından çıktıktan sonra sonlandırılacağını garantiler
            using (var ms = new MemoryStream())
            {
                //client üzerinden gelen dosyanın server RAM inde açılmasını sağlar.
                dosya.CopyTo(ms);
                //asıl RAM e atılan verinin okuma işlemine başlanıyor.
                using (var excelDosyasi = new XLWorkbook(ms))
                {
                    var cSayfa = excelDosyasi.Worksheet(1);

                    //sayfadaki sutun sayısını okuyoruz.
                    int i, n = cSayfa.Columns().Count();
                    for (i = 1; i <= n; i++)
                        //bize ait olan dt nin içerisine excel den okuduğumuz kolon başlıklarını açmak.
                        dt.Columns.Add(cSayfa.Cell(1, i).Value.ToString());

                    //Excel sayfasındaki veri satır sayısını alıyoruz.
                    n = cSayfa.Rows().Count();
                    for (i = 2; i <= n; i++)
                    {
                        //excel de içi dolu olan satırı yakalıyoruz.
                        if (cSayfa.Cell(i, 1).Value == null)
                            continue; //sonraki satırdan devam et. for daki sonraki i değişkenine geç

                        if (cSayfa.Cell(i, 1).Value.ToString() == "")
                            continue; //sonraki satırdan devam et. for daki sonraki i değişkenine geç

                        //veri bizim dt mize ekleniyor.
                        var dr = dt.NewRow();

                        int j, k = cSayfa.Columns().Count();
                        for (j = 1; j <= k; j++) dr[j - 1] = cSayfa.Cell(i, j).Value;

                        dt.Rows.Add(dr);
                    }
                }
            }

            var gelenVeri = new List<OrtakRaf>();
            foreach (var item in dt.AsEnumerable())
            {
                if (string.IsNullOrWhiteSpace(item[0].ToString()))
                    continue;

                #region eğer KİTAP ADİ verisi bizim tablomuzda yoksa ekleme işlemi yapıyoruz.

                var kitapAdi = _dataContext.KitapAdi.FirstOrDefault(t => t.KİTABİNADİ == item[1].ToString());
                if (kitapAdi == null)
                    _dataContext.KitapAdi.Add(
                            new KitapAdi
                            {
                                KİTABİNADİ = item[1].ToString(),
                                FIYAT = Convert.ToDecimal(item[3]),
                                OlusturanKullanici = "Excelden",
                                OlusturmaTarihi = DateTime.Now
                            });

                #endregion

                //KitapAdi tablolarına kayıtları ekliyoruz.
                _dataContext.SaveChanges();

                gelenVeri.Add(new OrtakRaf
                {
                    KAYİTTARİHİ = Convert.ToDateTime(item[0].ToString()),
                    KİTAPADİ = item[1].ToString(),
                    ADET = Convert.ToInt32(item[2].ToString()),


                    KitapAdiId = _dataContext.KitapAdi.FirstOrDefault(t => t.KİTABİNADİ == item[1].ToString())!.Id,

                    OlusturanKullanici = "Excel import",
                    GuncelleyenKullanici = "",
                    OlusturmaTarihi = DateTime.Now,
                    GuncellemeTarihi = DateTime.Now,
                    Aktif = true,
                    Silindi = false,
                    excelKodu = excelKodu
                });
            }

            foreach (var item in gelenVeri) // OrtakRaf listesine Tekrarlı veri yüklenmemesi için method burdan başlar.
                _dataContext.OrtakRaf.Add(item);

            _dataContext.SaveChanges();
            var ortakRaf = _dataContext.OrtakRaf.Where(t => t.excelKodu == excelKodu).ToList();

            var result = ortakRaf
                    .GroupBy(p => new { p.KAYİTTARİHİ, p.KİTAPADİ }, p => p.ADET)
                    .Select(p => new { Name = p.Key, toplam = p.Sum() })
                    .OrderByDescending(p => p.toplam)
                    .ToList();

            return RedirectToAction(nameof(Liste));
        }

        public async Task<IActionResult> Ekle(string kitapId)
        {
            ViewBag.RandomVeri = TempData["RandomSayi"];
            ViewBag.KitapListesi = await _dataContext.OrtakRaf
                .Select(x =>
                    new OrtakRaf
                    {
                        Id = x.Id,
                        KitapAdiId = x.KitapAdiId,
                        KİTAPADİ = x.KİTAPADİ

                    })
                .OrderBy(x => x.KİTAPADİ)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(OrtakRaf model) //değişti***
        {
            #region KitapAdi tablosuna Ekleme
            // Veritabanında KitapAdi tablosunda yer almayan 
            // bir bilgiyi OrtakRaf tablosunda oluşturmak
            // veri uyuşmazlığına sebep olabileceği için
            // önce KitapAdi tablosuna kaydını sağlıyoruz,
            // sonrasında OrtakRaf tablosuna kaydını gerçekleştiriyoruz.
            // KitapAdi tablosuna eklediğimiz kitabın Id değerini
            // addKitapAdi tanımından KitapAdi tablosundan değer ataması yaparak elde etmiş oluyoruz. --># addKitapAdi.Id #
            var addKitapAdi = new KitapAdi
            {
                KİTABİNADİ = model.KİTAPADİ,
                FIYAT = Convert.ToDecimal(Request.Form["FIYAT"]),
                OlusturanKullanici = "userX",
                OlusturmaTarihi = model.KAYİTTARİHİ
            };
            await _dataContext.KitapAdi.AddAsync(addKitapAdi);
            await _dataContext.SaveChangesAsync();

            #endregion
            var addOrtakRaf = new OrtakRaf
            {
                KitapAdiId = addKitapAdi.Id,
                KAYİTTARİHİ = model.KAYİTTARİHİ,
                KİTAPADİ = model.KİTAPADİ,
                ADET = model.ADET,
                excelKodu = model.excelKodu,
                Aktif = true,
                OlusturmaTarihi = DateTime.Now,
                OlusturanKullanici = "userX"
            };

            await _dataContext.OrtakRaf.AddAsync(addOrtakRaf); //değişti***
                                                               //veritabanı modellerinde bütün değişkenlerini uygula
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Liste));
        }

        public async Task<IActionResult> Guncelle(int id)
        {
            ViewBag.RandomVeri = TempData["RandomSayi"];
            var ortakRaf = await _dataContext.OrtakRaf.Include(o => o.KitapAdi).FirstOrDefaultAsync(t => t.Id == id); //değişti**

            if (ortakRaf == null) //değişti***
                return NotFound();

            return View(ortakRaf); //değişti***
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(OrtakRaf model) //değişti
        {
            if (!TryValidateModel(model))
                return View(model);

            try
            {
                var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == model.Id); //değişti***

                if (ortakRaf == null) //değişti
                    return NotFound();

                ortakRaf.KAYİTTARİHİ = model.KAYİTTARİHİ; //değişti**
                ortakRaf.KİTAPADİ = model.KİTAPADİ; //değişti**
                ortakRaf.ADET = model.ADET; //değişti**


                _dataContext.Entry(ortakRaf).State = EntityState.Modified; //değişti**
                var result = await _dataContext.SaveChangesAsync();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }

            return View(model);
        }

        public async Task<IActionResult> Sil(int id)
        {
            var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == id); //değişti**

            if (ortakRaf == null) //değişti**
                return NotFound();

            return View(ortakRaf); //değişti***
        }

        //sayfa üzerinden tetiklenecek
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(OrtakRaf model) //değişti***
        {
            try
            {
                var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == model.Id); //değişti

                if (ortakRaf == null) //değişti***
                    return NotFound();

                ortakRaf.Aktif = false; //değişti***

                _dataContext.Entry(ortakRaf).State = EntityState.Modified; //değişti***
                var result = await _dataContext.SaveChangesAsync();
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message);
            }

            return RedirectToAction(nameof(Liste)); //Listeyi Sor?**
        }

        public async Task<IActionResult> DurumDegistir(int id)
        {
            var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == id);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (ortakRaf != null)
            {
                ortakRaf.Aktif = !ortakRaf.Aktif;

                _dataContext.Entry(ortakRaf).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            return RedirectToAction(nameof(Liste));
        }

        [HttpPost]
        public async Task<IActionResult> Filtrele([FromBody] Filtre model)
        {
            //todo: diğer ekranların tüm controller işlemleri bittikten sonra filtreleme işlemini yapacağız.

            //Filtrelemede işlemini tarih ve kitap bilgilerine göre listeler.
            var ortakRaf = model.KitapAdiId.Length != 0 && model.BaslangicTarihi != null && model.BitisTarihi != null
                ? await _dataContext.OrtakRaf
                    .Where(o =>
                        model.KitapAdiId.Contains(o.KitapAdiId.ToString())
                        && o.KAYİTTARİHİ >= model.BaslangicTarihi
                        & o.KAYİTTARİHİ <= model.BitisTarihi
                        && o.Aktif.Equals(true)).Include(x => x.KitapAdi)
                    .ToListAsync()
                //Filtreleme işleminde eğer kitap seçilmiş ancak tarih kısıtı verilmemişse sadece kitaba göre bilgiler listelenir.
                : model.KitapAdiId.Length != 0 && model.BaslangicTarihi == null || model.BitisTarihi == null
                    ? await _dataContext.OrtakRaf
                        .Where(o =>
                            model.KitapAdiId.Contains(o.KitapAdiId.ToString())
                            && o.Aktif.Equals(true)).Include(x => x.KitapAdi)
                        .ToListAsync()
                    //Filtreleme sadece tarih aralığında gerçekleşecekse filtreleme bu yönde gerçekleşir.
                    : await _dataContext.OrtakRaf
                        .Where(o =>
                            o.KAYİTTARİHİ >= model.BaslangicTarihi
                            & o.KAYİTTARİHİ <= model.BitisTarihi
                            && o.Aktif.Equals(true)).Include(x => x.KitapAdi)
                        .ToListAsync();
            return PartialView("_ListePartial", ortakRaf);
        }

        [HttpPost]
        public async Task<IActionResult> Aktar(string SayisalModel)
        {
            var model = JArray.Parse(SayisalModel).Children();
            try
            {
                var sayisalKitap = EntityExtensions.ConvertJsonToEntity(model);
                foreach (var item in sayisalKitap)
                {
                    var isState = _dataContext.OrtakRaf.Any(x => x.KitapAdiId != item.Id && x.Aktif.Equals(true));
                    if (!isState) continue;
                    _dataContext.SayisalKitap.Add(new SayisalKitap
                    {
                        KAYİTTARİHİ = item.KAYİTTARİHİ,
                        KİTAPADİ = item.KİTAPADİ,
                        ADET = item.ADET,
                        FİYAT = item.FİYAT,
                        TUTAR = item.TUTAR,
                        KDV = item.KDV,
                        TOPLAMTUTAR = item.TOPLAMTUTAR,
                        Aktif = true,
                        Silindi = false,
                        OlusturanKullanici = "userX",
                        OlusturmaTarihi = DateTime.Now,
                        GuncelleyenKullanici = "userX",
                        GuncellemeTarihi = DateTime.Now,
                        KitapAdiId = item.KitapAdiId
                    });

                    var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == item.Id);
                    await using var transaction = await _dataContext.Database.BeginTransactionAsync();
                    if (ortakRaf == null) return NotFound();
                    ortakRaf.Aktif = !ortakRaf.Aktif;

                    _dataContext.Entry(ortakRaf).State = EntityState.Modified;
                    await _dataContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception exp)
            {
                TempData["notification"] = AlertPositionTest($"{exp.Message} hata var bi bak yahu (; ! ;)", NotificationType.error, "bottom-end", 3000);
            }

            return Ok();
        }

        public IActionResult ExcelExport()
        {
            var reader = Request.Query["checkedRow"].Where(x => !x.Equals("false")).ToList();
            var ortakRaf = reader.Select(item => item.Select(s => _dataContext.OrtakRaf.FirstOrDefault(x => x.Id == Convert.ToInt32(item))).First()).ToList();
            var excelData = new ExportDataToExcel().ExportToExcel(ortakRaf);

            var document = new Document
            {
                FileName = "OrtakRaf.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Data = excelData
            };

            return File(document.Data, document.ContentType, document.FileName);
        }

        public IActionResult TumListeGetir()
        {
            return RedirectToAction(nameof(Liste));
        }
    }
}
