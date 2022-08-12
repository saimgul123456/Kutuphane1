using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using Kutuphane1.Models;

namespace Kutuphane1.Helpers
{
    public class ExportDataToExcel
    {
        public byte[] ExportToExcel(IEnumerable<SayisalKitap> model)
        {
            // Excel dosyası (excel çalışma kitabı) oluşturuyoruz 
            using var workbook = new XLWorkbook();
            // Çalışma kitabına bir sayfa ekliyoruz, sayfa ismini istediğimiz gibi verebiliriz
            var worksheet = workbook.Worksheets.Add("Sayfa1");
            var currentRow = 1;

            // Hücreye değer atıyoruz.
            worksheet.Row(currentRow).Height = 15.0;
            worksheet.Row(currentRow).Style.Font.Bold = true;

            //Başlıkları ortaladık.
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            //Başlık Rengini belirledik.
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Font.SetFontColor(XLColor.FromHtml("#FF0000"));

            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);

            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Border.SetLeftBorder(XLBorderStyleValues.Medium);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Border.SetLeftBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 7)).Style.Font.SetBold();

            worksheet.Cell(currentRow, 1).Value = "Kayıt Tarihi";
            worksheet.Cell(currentRow, 2).Value = "Kitap Adı";
            worksheet.Cell(currentRow, 3).Value = "Adet";
            worksheet.Cell(currentRow, 4).Value = "Birim Fiyat";
            worksheet.Cell(currentRow, 5).Value = "Tutar";
            worksheet.Cell(currentRow, 6).Value = "Kdv";
            worksheet.Cell(currentRow, 7).Value = "Toplam Tutar";

            foreach (var item in model)
            {
                currentRow++;
                worksheet.Row(currentRow).Height = 20.0;
                worksheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Cell(currentRow, 1).SetValue(item.KAYİTTARİHİ);
                worksheet.Cell(currentRow, 2).SetValue(item.KİTAPADİ);
                worksheet.Cell(currentRow, 3).SetValue(item.ADET);

                //Hücreyi biçimlendiriyoruz, binlik ayraçları ve virgülden sonraki hane sayısını belirtmiş olduk
                worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "dd.MM.yyyy";
                worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";

                //Hücrenin veri tipini belirtiyoruz
                worksheet.Cell(currentRow, 3).DataType = XLDataType.Number;
                worksheet.Cell(currentRow, 4).SetValue(Convert.ToDecimal(item.FİYAT, CultureInfo.GetCultureInfo("tr-TR")));
                worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(currentRow, 4).DataType = XLDataType.Number;
                worksheet.Cell(currentRow, 5).SetValue(item.TUTAR);
                worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(currentRow, 5).DataType = XLDataType.Number;
                worksheet.Cell(currentRow, 6).SetValue(item.KDV);
                worksheet.Cell(currentRow, 6).DataType = XLDataType.Number;
                worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(currentRow, 7).SetValue(item.TOPLAMTUTAR);
                worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(currentRow, 7).DataType = XLDataType.Number;
            }

            currentRow++;

            //Hücreye formül tanımlıyoruz, Toplama işlemi yapıyoruz
            worksheet.Cell(currentRow, 3).SetFormulaA1($"=SUM(C2:C{currentRow - 1})");
            worksheet.Cell(currentRow, 4).SetFormulaA1($"=SUM(D2:D{currentRow - 1})");
            worksheet.Cell(currentRow, 5).SetFormulaA1($"=SUM(E2:E{currentRow - 1})");
            worksheet.Cell(currentRow, 6).SetFormulaA1($"=SUM(F2:F{currentRow - 1})");
            worksheet.Cell(currentRow, 7).SetFormulaA1($"=SUM(G2:G{currentRow - 1})");

            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 7)).Style.Font.SetFontColor(XLColor.Red);
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 7)).Style.Font.SetBold();
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 7)).DataType = XLDataType.Number;
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 7)).Style.NumberFormat.Format = "#,##0.00";

            // Bu metod ise sayfadaki tüm formüllerin tekrar hesaplanmasını sağlıyor
            worksheet.RecalculateAllFormulas();

            var totalCell = worksheet.Cell(currentRow, 2);
            totalCell.Value = "TOPLAM";

            // Hücreye arka plan rengi veriyoruz, FromColor ya da FromArgb fonksiyonu ile de özel renkler oluşturulabilir
            totalCell.Style.Fill.SetBackgroundColor(XLColor.GreenPigment);

            // Hücrenin fontunu kalın yapıyoruz
            totalCell.Style.Font.SetBold();

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

            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Font.SetBold();

            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

            //Hücreye kenarlık oluşturduk ve kenarlık rengi belirttik
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

            // Hücreyi yatayda sağa yasladık.
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            // hücreyi dikeyde ortaladık
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            // Hücrenin rengini html hex color code ile kırmızı olarak belirttik
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 7)).Style.Font.SetFontColor(XLColor.FromHtml("#538DD5"));

            //// Hücreyi genişliğinin içeriğe göre ayarlanmasını sağladık
            worksheet.Columns("A:G").AdjustToContents();

            using var memStream = new MemoryStream();
            workbook.SaveAs(memStream);
            return memStream.ToArray();
        }

        public byte[] ExportToExcel(IEnumerable<OrtakRaf> model)
        {
            // Excel dosyası (excel çalışma kitabı) oluşturuyoruz 
            using var workbook = new XLWorkbook();
            // Çalışma kitabına bir sayfa ekliyoruz, sayfa ismini istediğimiz gibi verebiliriz
            var worksheet = workbook.Worksheets.Add("Sayfa1");
            var currentRow = 1;

            // Hücreye değer atıyoruz.
            worksheet.Row(currentRow).Height = 15.0;
            worksheet.Row(currentRow).Style.Font.Bold = true;

            //Başlıkları ortaladık.
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            //Başlık Rengini belirledik.
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Font.SetFontColor(XLColor.FromHtml("#FF0000"));

            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);

            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Border.SetLeftBorder(XLBorderStyleValues.Medium);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Border.SetLeftBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, 4)).Style.Font.SetBold();

            worksheet.Cell(currentRow, 1).Value = "Kayıt Tarihi";
            worksheet.Cell(currentRow, 2).Value = "Kitap Adı";
            worksheet.Cell(currentRow, 3).Value = "Adet";
            worksheet.Cell(currentRow, 4).Value = "Excel Kodu";

            foreach (var item in model)
            {
                currentRow++;
                worksheet.Row(currentRow).Height = 20.0;
                worksheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Cell(currentRow, 1).SetValue(item.KAYİTTARİHİ);
                worksheet.Cell(currentRow, 2).SetValue(item.KİTAPADİ);
                worksheet.Cell(currentRow, 3).SetValue(item.ADET);
                worksheet.Cell(currentRow, 4).SetValue(item.excelKodu);

                //Hücreyi biçimlendiriyoruz, binlik ayraçları ve virgülden sonraki hane sayısını belirtmiş olduk
                worksheet.Cell(currentRow, 1).Style.NumberFormat.Format = "dd.MM.yyyy";
                worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";

                //Hücrenin veri tipini belirtiyoruz
                worksheet.Cell(currentRow, 3).DataType = XLDataType.Number;
            }

            currentRow++;

            //Hücreye formül tanımlıyoruz, Toplama işlemi yapıyoruz
            worksheet.Cell(currentRow, 3).SetFormulaA1($"=SUM(C2:C{currentRow - 1})");

            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 3)).Style.Font.SetFontColor(XLColor.Red);
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 3)).Style.Font.SetBold();
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 3)).DataType = XLDataType.Number;
            worksheet.Range(worksheet.Cell(currentRow, 3), worksheet.Cell(currentRow, 3)).Style.NumberFormat.Format = "#,##0.00";

            // Bu metod ise sayfadaki tüm formüllerin tekrar hesaplanmasını sağlıyor
            worksheet.RecalculateAllFormulas();

            var totalCell = worksheet.Cell(currentRow, 2);
            totalCell.Value = "TOPLAM";

            // Hücreye arka plan rengi veriyoruz, FromColor ya da FromArgb fonksiyonu ile de özel renkler oluşturulabilir
            totalCell.Style.Fill.SetBackgroundColor(XLColor.GreenPigment);

            // Hücrenin fontunu kalın yapıyoruz
            totalCell.Style.Font.SetBold();

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

            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 2)).Style.Font.SetBold();

            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 1)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 1)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 1)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(2, 1), worksheet.Cell(currentRow - 1, 1)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

            worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(currentRow - 1, 4)).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(currentRow - 1, 4)).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
            worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(currentRow - 1, 4)).Style.Border.SetOutsideBorderColor(XLColor.BluePigment);
            worksheet.Range(worksheet.Cell(2, 2), worksheet.Cell(currentRow - 1, 4)).Style.Border.SetInsideBorderColor(XLColor.BluePigment);

            // Hücreyi yatayda sağa yasladık.
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 4)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            // hücresiyi dikeyde ortaladık
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 4)).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            
            // Hücrenin rengini html hex color code ile kırmızı olarak belirttik
            worksheet.Range(worksheet.Cell(2, 3), worksheet.Cell(currentRow - 1, 4)).Style.Font.SetFontColor(XLColor.FromHtml("#538DD5"));

            //Hücreyi genişliğinin içeriğe göre ayarlanmasını sağladık
            worksheet.Columns("A:G").AdjustToContents();

            using var memStream = new MemoryStream();
            workbook.SaveAs(memStream);
            return memStream.ToArray();
        }
    }
}