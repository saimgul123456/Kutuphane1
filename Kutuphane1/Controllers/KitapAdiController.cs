using System;
using System.Threading.Tasks;
using Kutuphane1.Context;
using Kutuphane1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Controllers
{
	[Authorize]
	public class KitapAdiController : Controller
	{
		private readonly DataContext _dataContext;
		public KitapAdiController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public async Task<IActionResult> Liste()
		{
			var kitapAdiListe = await _dataContext.KitapAdi.ToListAsync();
			return View(kitapAdiListe);
		}

		public IActionResult Ekle()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Ekle(KitapAdi model)
		{
			if (!TryValidateModel(model))
				return View(model);

			//gelen modelin kendisi direk veritabanı tablosuna ekleme modunda oluşturuldu.
			await _dataContext.KitapAdi.AddAsync(model);

			//veritabanı modellerinde bütün değişkenlerini uygula
			var result = await _dataContext.SaveChangesAsync();
			return View(model);
		}

		public async Task<IActionResult> Guncelle(int id)
		{
			var kitapAdi = await _dataContext.KitapAdi.FirstOrDefaultAsync(t => t.Id == id);

			if (kitapAdi == null)
				return NotFound();

			return View(kitapAdi);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Guncelle(KitapAdi model)
		{
			if (!TryValidateModel(model))
				return View(model);

			try
			{
				var kitapAdi = await _dataContext.KitapAdi.FirstOrDefaultAsync(t => t.Id == model.Id);

				if (kitapAdi == null)
					return NotFound();

				kitapAdi.KİTABİNADİ = model.KİTABİNADİ;

				kitapAdi.Aktif = model.Aktif;


				_dataContext.Entry(kitapAdi).State = EntityState.Modified;
				var result = await _dataContext.SaveChangesAsync();
			}
			catch (Exception)
			{
			}

			return View(model);
		}

		public async Task<IActionResult> Sil(int id)
		{
			var kitapAdi = await _dataContext.KitapAdi.FirstOrDefaultAsync(t => t.Id == id);

			if (kitapAdi == null)
				return NotFound();

			return View(kitapAdi);
		}

		//sayfa üzerinden tetiklenecek
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Sil(KitapAdi model)
		{
			//if (!TryValidateModel(model))
			//    return View(model);

			try
			{
				var kitapAdi = await _dataContext.KitapAdi.FirstOrDefaultAsync(t => t.Id == model.Id);

				if (kitapAdi == null)
					return NotFound();

				kitapAdi.Aktif = false;

				_dataContext.Entry(kitapAdi).State = EntityState.Modified;
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
			var kitapAdi = await _dataContext.KitapAdi.FirstOrDefaultAsync(t => t.Id == id);

			await using var transaction = await _dataContext.Database.BeginTransactionAsync();

			if (kitapAdi != null)
			{
				kitapAdi.Aktif = !kitapAdi.Aktif;

				_dataContext.Entry(kitapAdi).State = EntityState.Modified;
				await _dataContext.SaveChangesAsync();
				await transaction.CommitAsync();
			}

			return RedirectToAction(nameof(Liste));
		}
	}
}
