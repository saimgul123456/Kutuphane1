using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Kutuphane1.Helpers
{
    public class ExportDataToExcel : ControllerBase
    {
        public byte[] ExportToExcel(List<JsonElement> model)
        {
            //const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            const string fileName = "SayisalKitap.xlsx";

            // Excel dosyası (excel çalışma kitabı) oluşturuyoruz 
            using (var workbook = new XLWorkbook())
            {
                // Çalışma kitabına bir sayfa ekliyoruz, sayfa ismini istediğimiz gibi verebiliriz
                var worksheet = workbook.Worksheets.Add("Sayfa1");

                // Hücreye değer atıyoruz.
                worksheet.Cell("A1").Value = "Kitap Adı";
                worksheet.Cell("B1").Value = "Adet";
                worksheet.Cell("C1").Value = "Birim Fiyat";
                worksheet.Cell("D1").Value = "Tutar";
                worksheet.Cell("E1").Value = "Kdv";
                worksheet.Cell("F1").Value = "Toplam Tutar";

                //Başlıkları ortaladık.
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                //Başlık Rengini belirledik.
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Font.SetFontColor(XLColor.FromHtml("#FF0000"));

                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
                //worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thick);
                //worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetLeftBorder(XLBorderStyleValues.Medium);
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Border.SetLeftBorderColor(XLColor.BluePigment);
                worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 6)).Style.Font.SetBold();

                var i = 2;
                foreach (var item in model)
                {
                    var adt = Convert.ToInt32(item[3].GetString());
                    var brmFyt = Convert.ToDecimal(item[4].GetString(), CultureInfo.GetCultureInfo("tr-TR"));

                    worksheet.Cell(i, 1).SetValue(item[2].GetString());
                    worksheet.Cell(i, 2).SetValue(adt);

                    //Hücreyi biçimlendiriyoruz, binlik ayraçları ve virgülden sonraki hane sayısını belirtmiş olduk
                    worksheet.Cell(i, 2).Style.NumberFormat.Format = "#,##0.00";
                    //Hücrenin veri tipini belirtiyoruz
                    worksheet.Cell(i, 2).DataType = XLDataType.Number;
                    worksheet.Cell(i, 3).SetValue(brmFyt);
                    worksheet.Cell(i, 3).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 3).DataType = XLDataType.Number;
                    worksheet.Cell(i, 4).SetValue(adt * brmFyt);
                    worksheet.Cell(i, 4).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 4).DataType = XLDataType.Number;
                    worksheet.Cell(i, 5).SetValue(adt * brmFyt * 0.18M);
                    worksheet.Cell(i, 5).DataType = XLDataType.Number;
                    worksheet.Cell(i, 5).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 6).SetValue(adt * brmFyt + adt * brmFyt * 0.18M);
                    worksheet.Cell(i, 6).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(i, 6).DataType = XLDataType.Number;
                    i++;
                }

                //Hücreye formül tanımlıyoruz, Toplama işlemi yapıyoruz
                worksheet.Cell($"B{i}").SetFormulaA1($"=SUM(B2:B{i - 1})");
                //worksheet.Cell($"C{i}").SetFormulaA1($"=SUM(C2:C{i - 1})");
                worksheet.Cell($"D{i}").SetFormulaA1($"=SUM(D2:D{i - 1})");
                worksheet.Cell($"E{i}").SetFormulaA1($"=SUM(E2:E{i - 1})");
                worksheet.Cell($"F{i}").SetFormulaA1($"=SUM(F2:F{i - 1})");
                worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, 6)).Style.Font.SetFontColor(XLColor.Red);
                worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, 6)).Style.Font.SetBold();
                worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, 6)).DataType = XLDataType.Number;
                worksheet.Range(worksheet.Cell(i, 1), worksheet.Cell(i, 6)).Style.NumberFormat.Format = "#,##0.00";

                // Bu metod ise sayfadaki tüm formüllerin tekrar hesaplanmasını sağlıyor
                worksheet.RecalculateAllFormulas();

                var totalCell = worksheet.Cell(i, 1);
                totalCell.Value = "TOPLAM";

                // Hücreye arka plan rengi veriyoruz, FromColor ya da FromArgb fonksiyonu ile de özel renkler oluşturulabilir
                totalCell.Style.Fill.SetBackgroundColor(XLColor.GreenPigment);

                // Hücrenin fontunu kalın yapıyoruz
                totalCell.Style.Font.SetBold();
                // Alternatif kullanım
                // totalCell.Style.Font.Bold = true; 

                // Hücrenin yazı rengini, tipini ve boyutunu belirtiyoruz
                totalCell.Style.Font.SetFontColor(XLColor.FromColor(Color.White));
                totalCell.Style.Font.SetFontName("Calibri");
                totalCell.Style.Font.SetFontSize(9);

                // Hücredeki veriyi yatay olarak sağa yasladık
                totalCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                // Hücredeki veriyi dikey olarak ortaladık
                totalCell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Metni Kaydır olarak belirttik 
                totalCell.Style.Alignment.SetWrapText();

                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Font.SetBold();
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
                worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(i - 1, 1)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

                //Hücreye kenarlık oluşturduk ve kenarlık rengi belirttik
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

                // Hücreyi yatayda sağa yasladık.
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                // C4 hücresini dikeyde ortaladık
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                // Hücreyi bold yaptık
                //worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Font.SetBold();

                // Hücrenin rengini html hex color code ile kırmızı olarak belirttik
                worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(i - 1, 6)).Style.Font.SetFontColor(XLColor.FromHtml("#538DD5"));

                //// Hücreye noktalı alt kenarlık oluşturduk
                //worksheet.Cell("A1").Style.Border.SetBottomBorder(XLBorderStyleValues.Dotted);

                //// Hücreyi genişliğinin içeriğe göre ayarlanmasını sağladık
                //worksheet.Column($"A").AdjustToContents();
                worksheet.Columns("A:F").AdjustToContents();

                //// Sütun genişliğini belirtiyoruz
                //worksheet.Column(3).Width = 20;

                // Satır yüksekliğini belirtiyoruz 
                //worksheet.Row(4).Height = 30;


                using var memoryStream = new MemoryStream();
                var uplodFile = Path.Combine($@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\Downloads\{fileName}");
                workbook.SaveAs(uplodFile);


                //workbook.SaveAs(fileName);
                var content = memoryStream.ToArray();
                return content;

                //return File(content, "application/force-download", fileName);
                //return Ok(content);
            }
        }
    }
}