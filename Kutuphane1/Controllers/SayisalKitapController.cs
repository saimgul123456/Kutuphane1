using System;
using System.Linq;
using System.Threading.Tasks;
using Kutuphane1.Context;
using Kutuphane1.Helpers;
using Kutuphane1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Kutuphane1.Controllers
{
  [Authorize]
  public class SayisalKitapController : Controller
    {
        private readonly DataContext _dataContext;
        public SayisalKitapController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Liste()
        {
            var sk = await _dataContext.SayisalKitap
                .Select(o => new SelectListItem
                {
                    Value = o.KitapAdiId.ToString(),
                    Text = o.KİTAPADİ
                })
                .Distinct().OrderBy(x => x.Text)
                .ToListAsync();

            ViewBag.KitapAdi = new SelectList(sk, "Value", "Text");

            var sayisalKitap = await _dataContext.SayisalKitap.Include(t => t.KitapAdi).Where(w => w.Aktif.Equals(true)).ToListAsync();


            return View(sayisalKitap);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(SayisalKitap model)
        {
            if (!TryValidateModel(model))
                return View(model);

            var kitapId = Convert.ToInt32(Request.Form["ddlKitaplar"].ToString());

            //gelen modelin kendisi direk veritabanı tablosuna ekleme modunda oluşturuldu.
            var sayisalKitap = new SayisalKitap
            {
                KAYİTTARİHİ = model.KAYİTTARİHİ,
                KİTAPADİ = model.KİTAPADİ,
                ADET = model.ADET,
                FİYAT = model.FİYAT,
                TUTAR = (decimal)model.FİYAT * model.ADET,
                KDV = (decimal)model.FİYAT * model.ADET * 0.18M
            };
            sayisalKitap.TOPLAMTUTAR = sayisalKitap.TUTAR + sayisalKitap.KDV;
            sayisalKitap.KitapAdiId = kitapId;
            await _dataContext.SayisalKitap.AddAsync(sayisalKitap);

            //veritabanı modellerinde bütün değişkenlerini uygula
            var result = await _dataContext.SaveChangesAsync();
            return View(model);
        }

        public async Task<IActionResult> Guncelle(int id)
        {
            var sayisalKitap = await _dataContext.SayisalKitap.FirstOrDefaultAsync(t => t.Id == id);

            if (sayisalKitap == null)
                return NotFound();

            return View(sayisalKitap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SayisalKitap model)
        {
            if (!TryValidateModel(model))
                return View(model);

            try
            {
                var sayisalKitap = await _dataContext.SayisalKitap.FirstOrDefaultAsync(t => t.Id == model.Id);

                if (sayisalKitap == null)
                    return NotFound();

                sayisalKitap.KAYİTTARİHİ = model.KAYİTTARİHİ;
                sayisalKitap.KİTAPADİ = model.KİTAPADİ;
                sayisalKitap.ADET = model.ADET;
                sayisalKitap.FİYAT = model.FİYAT;
                sayisalKitap.TUTAR = model.FİYAT * model.ADET;
                sayisalKitap.KDV = model.FİYAT * model.ADET * 0.18M;
                sayisalKitap.TOPLAMTUTAR = sayisalKitap.TUTAR + sayisalKitap.KDV;


                _dataContext.Entry(sayisalKitap).State = EntityState.Modified;
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
            var sayisalKitap = await _dataContext.SayisalKitap.FirstOrDefaultAsync(t => t.Id == id);

            if (sayisalKitap == null)
                return NotFound();

            return View(sayisalKitap);
        }

        //sayfa üzerinden tetiklenecek
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(SayisalKitap model)
        {
            try
            {
                var sayisalKitap = await _dataContext.SayisalKitap.FirstOrDefaultAsync(t => t.Id == model.Id);

                if (sayisalKitap == null)
                    return NotFound();

                sayisalKitap.Aktif = false;

                _dataContext.Entry(sayisalKitap).State = EntityState.Modified;
                var result = await _dataContext.SaveChangesAsync();
            }
            catch (Exception hata)
            {
                throw new Exception(hata.Message);
            }

            return RedirectToAction(nameof(Liste));
        }

        public async Task<IActionResult> DurumDegistir(int id)
        {
            var sayisalKitap = await _dataContext.SayisalKitap.FirstOrDefaultAsync(t => t.Id == id);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (sayisalKitap == null) return RedirectToAction(nameof(Liste));
            sayisalKitap.Aktif = !sayisalKitap.Aktif;

            _dataContext.Entry(sayisalKitap).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction(nameof(Liste));
        }

        [HttpPost]
        public async Task<IActionResult> Filtrele([FromBody] Filtre model)
        {
            //todo: diğer ekranların tüm controller işlemleri bittikten sonra filtreleme işlemini yapacağız.
            var sayisalKitap = model.KitapAdiId.Length != 0 && model.BaslangicTarihi != null && model.BitisTarihi != null
                ? await _dataContext.SayisalKitap
                    .Where(o =>
                        model.KitapAdiId.Contains(o.KitapAdiId.ToString())
                        && o.KAYİTTARİHİ >= model.BaslangicTarihi
                        & o.KAYİTTARİHİ <= model.BitisTarihi
                        && o.Aktif.Equals(true))
                    .ToListAsync()
                : model.KitapAdiId.Length != 0 && model.BaslangicTarihi == null || model.BitisTarihi == null
                    ? await _dataContext.SayisalKitap
                        .Where(o =>
                            model.KitapAdiId.Contains(o.KitapAdiId.ToString())
                            && o.Aktif.Equals(true))
                        .ToListAsync()
                    : await _dataContext.SayisalKitap
                        .Where(o =>
                            o.KAYİTTARİHİ >= model.BaslangicTarihi
                            & o.KAYİTTARİHİ <= model.BitisTarihi
                            && o.Aktif.Equals(true))
                        .ToListAsync();

            return PartialView("_ListePartial", sayisalKitap);
        }

        public IActionResult ExcelExport()
        {
            var selectedRows = Request.Query["checkedRow"].Where(x => !x.Equals("false")).ToList();
            var sayisalKitap = selectedRows.Select(item => item.Select(s => _dataContext.SayisalKitap.FirstOrDefault(x => x.Id == Convert.ToInt32(item))).First()).ToList();
            var excelData = new ExportDataToExcel().ExportToExcel(sayisalKitap);

            var document = new Document
            {
                FileName = "SayısalKitap.xlsx",
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
