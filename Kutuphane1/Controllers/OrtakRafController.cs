using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Kutuphane1.Context;
using Kutuphane1.Helpers;
using Kutuphane1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane1.Controllers
{
	public class OrtakRafController : Controller
	{
		private readonly DataContext _dataContext; //burayı sor***?
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
			//Alt satırı sor????????*
			//ViewBag.KitapAdi = new SelectList(await _dataContext.OrtakRaf.Select(t => t.KitapAdi).Distinct().ToListAsync(), "Id", "");


			var sl = await _dataContext.OrtakRaf
					.Select(o => new SelectListItem { Value = o.KitapAdiId.ToString(), Text = o.KİTAPADİ }).Distinct()
					.ToListAsync();

			ViewBag.KitapAdi = new SelectList(sl, "Value", "Text");

			var ortakRaf = await _dataContext.OrtakRaf.Include(t => t.KitapAdi).Where(w=>w.Aktif.Equals(true)).ToListAsync();


			//TempData["notification"] = SweetAlertMessages.AlertDefault("Ortak Raf İşlemleri", "Kayıt başarıyla gerçekleşti.", NotificationType.success);
			//SweetAlertMessages.SuccesMessage = SweetAlertMessages.AlertDefaultIn("Kayıt İşlemi", "Ortak Raftaki kitaplar Sayısal Kitaplara kaydedildi.", NotificationType.success);
			SweetAlertMessages.ErrorMessage = SweetAlertMessages.AlertDefault("Kayıt İşlemi", "İşlem başarısız sonuçlandı.", NotificationType.error);
			//SweetAlertMessages.SavedMessage = SweetAlertMessages.AlertSavedIn("Kayıt İşlemi", "Ortak Raftaki Kitaplar");
			SweetAlertMessages.SuccesMessage = SweetAlertMessages.AlertPosition("Ortak Raftaki Sayisal Kitap Tablosuna Kaydedildi.", NotificationType.success, "top-end", 3000/*, "<script type='text/javascript'", "</script>"*/);
			//var ortakRaf = await _dataContext.OrtakRaf.ToListAsync();//değişti**
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

			//List<ViewModels.OrtakRafModel> gelenVeri = new List<ViewModels.ReklamListesiModel>();
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

		public async Task<IActionResult> Ekle()
		{
			var model = await _dataContext.KitapAdi
					.Select(x => new
					{
						Value = x.Id.ToString(),
						Text = x.KİTABİNADİ
					}).ToListAsync();

			ViewBag.ListKitap = new SelectList(model, "Value", "Text");
			ViewBag.RandomVeri = TempData["RandomSayi"];

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Ekle(OrtakRaf model) //değişti***
		{
			var kitapId = Convert.ToInt32(Request.Form["ddlKitaplar"].ToString());

			//gelen modelin kendisi direk veritabanı tablosuna ekleme modunda oluşturuldu.
			var kitapAdiGetir = await _dataContext.KitapAdi.FirstOrDefaultAsync(x => x.Id == kitapId);

			var addOrtakRaf = new OrtakRaf
			{
				KAYİTTARİHİ = model.KAYİTTARİHİ,
				KİTAPADİ = kitapAdiGetir.KİTABİNADİ,
				ADET = model.ADET,
				excelKodu = model.excelKodu,
				KitapAdiId = kitapId,
				OlusturmaTarihi = DateTime.Now,
				OlusturanKullanici = "userX"
			};

			await _dataContext.OrtakRaf.AddAsync(addOrtakRaf); //değişti***

			////veritabanı modellerinde bütün değişkenlerini uygula
			var result = await _dataContext.SaveChangesAsync();
			return RedirectToAction(nameof(Liste));
		}

		public async Task<IActionResult> Guncelle(int id)
		{
			var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == id); //değişti**

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
			catch (Exception)
			{
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
			//if (!TryValidateModel(model))
			//    return View(model);

			try
			{
				var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == model.Id); //değişti

				if (ortakRaf == null) //değişti***
					return NotFound();

				ortakRaf.Aktif = false; //değişti***

				_dataContext.Entry(ortakRaf).State = EntityState.Modified; //değişti***
				var result = await _dataContext.SaveChangesAsync();
			}
			catch (Exception hata)
			{
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

			var ortakRaf = await _dataContext.OrtakRaf.Where(o => o.KAYİTTARİHİ >= model.BaslangicTarihi
																														&& o.KAYİTTARİHİ <= model.BitisTarihi
																														&& model.KitapAdiId.Contains(o.KitapAdiId.ToString()))
					.ToListAsync();
			var temp = ortakRaf.ToList();
			//ortakRaf.Clear();
			//
			//foreach (var item2 in model.KitapAdiId) //**
			//{
			//	var a = temp.Where(t => t.KitapAdiId == Convert.ToInt32(item2)).ToList();//**
			//
			//	ortakRaf.AddRange(a);//**
			//											 // buarada sadece modelin ismi kulanılmıştır.
			//											 //if (model.KitapAdiId > 0)
			//											 //	ortakRaf = ortakRaf.Where(t => t.KitapAdiId == model.KitapAdiId).ToList();
			//
			//}
			//  return View(nameof(Liste), reklamSatisListesi);
			return PartialView("_ListePartial", ortakRaf);
		}

		[HttpPost]
		public async Task<IActionResult> Aktar([FromBody] List<JsonElement> model)
		{
			try
			{
				foreach (var item in model)
				{
					//var kitapId = Convert.ToInt32(Request.Form["ddlKitaplar"].ToString());
					var itemId = Convert.ToInt32(item[0].GetString());
					var adt = Convert.ToInt32(item[3].GetString());
					var brmFyt = Convert.ToDecimal(item[4].GetString(), CultureInfo.GetCultureInfo("tr-TR"));

					//OrtakRaf tablosundan okuduğumuz verileri Sayisalkitap tablosuna adım adım ekliyoruz.
					_dataContext.SayisalKitap.Add(
							new SayisalKitap
							{
								KAYİTTARİHİ = Convert.ToDateTime(item[1].GetString()!),
								KİTAPADİ = item[2].GetString(),
								ADET = adt,
								FİYAT = brmFyt,
								TUTAR = adt * brmFyt,
								KDV = adt * brmFyt * 0.18M,
								TOPLAMTUTAR = adt * brmFyt + adt * brmFyt * 0.18M,
								Aktif = true,
								Silindi = false,
								OlusturanKullanici = "userX",
								OlusturmaTarihi = DateTime.Now,
								GuncelleyenKullanici = "userX",
								GuncellemeTarihi = DateTime.Now,
								KitapAdiId = itemId
							});

					//döngü içerisinde her kaydı ayrı ayrı kaydediyoruz.
					var ortakRaf = await _dataContext.OrtakRaf.FirstOrDefaultAsync(t => t.Id == itemId);
					await using var transaction = await _dataContext.Database.BeginTransactionAsync();
					if (ortakRaf == null)
						return NotFound();
					ortakRaf.Aktif = !ortakRaf.Aktif;


					_dataContext.Entry(ortakRaf).State = EntityState.Modified;
					await _dataContext.SaveChangesAsync();
					await transaction.CommitAsync();
				}

				var content = new ExportDataToExcel().ExportToExcel(model);
				File(content, "application/force-download", "sayisalKitap.xlsx");
				return Ok(content);
			}
			catch (Exception hata)
			{
				return BadRequest(hata.Message);
			}
		}

		public record Filtre
		{
			public string[] KitapAdiId { get; set; } //**
			public DateTime? BaslangicTarihi { get; set; }
			public DateTime? BitisTarihi { get; set; }
		}
	}
}
