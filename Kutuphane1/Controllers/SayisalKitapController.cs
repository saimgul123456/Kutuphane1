using System;
using System.Linq;
using System.Threading.Tasks;
using Kutuphane1.Context;
using Kutuphane1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Controllers
{
	public class SayisalKitapController : Controller
	{
		private readonly DataContext _dataContext;

		public SayisalKitapController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<IActionResult> Liste()
		{

			//*var sayisalKitapListe = await _dataContext.SayisalKitap.ToListAsync();
			var sk = await _dataContext.SayisalKitap.Select(o => new SelectListItem { Value = o.KitapAdiId.ToString(), Text = o.KİTAPADİ }).Distinct().ToListAsync();

			ViewBag.KitapAdi = new SelectList(sk, "Value", "Text");

			var sayisalKitap = await _dataContext.SayisalKitap.Include(t => t.KitapAdi).ToListAsync();



			return View(sayisalKitap);
		}

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
			var sayisalKitap = new SayisalKitap();
			sayisalKitap.KAYİTTARİHİ = model.KAYİTTARİHİ;
			sayisalKitap.KİTAPADİ = model.KİTAPADİ;
			sayisalKitap.ADET = model.ADET;
			sayisalKitap.FİYAT = model.FİYAT;
			sayisalKitap.TUTAR = (decimal)model.FİYAT * model.ADET;
			sayisalKitap.KDV = (decimal)model.FİYAT * model.ADET * 0.18M;
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
			catch (Exception)
			{
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
			//if (!TryValidateModel(model))
			//    return View(model);

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

			if (sayisalKitap != null)
			{
				sayisalKitap.Aktif = !sayisalKitap.Aktif;

				_dataContext.Entry(sayisalKitap).State = EntityState.Modified;
				await _dataContext.SaveChangesAsync();
				await transaction.CommitAsync();
			}

			return RedirectToAction(nameof(Liste));
		}
		public record Filtre
		{
			public string[] KitapAdiId { get; set; } //**
			public DateTime? BaslangicTarihi { get; set; }
			public DateTime? BitisTarihi { get; set; }
		}

		[HttpPost]
		public async Task<IActionResult> Filtrele([FromBody] Filtre model)
		{
			//todo: diğer ekranların tüm controller işlemleri bittikten sonra filtreleme işlemini yapacağız.

			var sayisalKitap = await _dataContext.SayisalKitap.Where(o => o.KAYİTTARİHİ >= model.BaslangicTarihi
			&& o.KAYİTTARİHİ <= model.BitisTarihi
			/*&& model.KitapAdiId.Contains(o.KitapAdiId.ToString())*/).ToListAsync();
			var temp = sayisalKitap.ToList();
			
			return PartialView("_ListePartial", sayisalKitap);
		}

		public IActionResult TumListeGetir(){
			return RedirectToAction(nameof(Liste));
		}
	}
}
