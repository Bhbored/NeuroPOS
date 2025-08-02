using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using NeuroPOS.MVVM.Model;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PointF = Syncfusion.Drawing.PointF;

namespace NeuroPOS.Services;

public static class InvoicePdfService
{
    public static async Task<byte[]> BuildAsync(Transaction tx)
    {
        using var doc = new PdfDocument();
        var page = doc.Pages.Add();
        var gfx = page.Graphics;

        var hFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
        gfx.DrawString("INVOICE", hFont, PdfBrushes.DarkBlue,
            new PointF(page.GetClientSize().Width / 2, 20),
            new PdfStringFormat { Alignment = PdfTextAlignment.Center });


        var metaFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        var when = tx.Date.Date.ToString("yyyy-MM-dd HH:mm");   // adjust to your field
        gfx.DrawString($"Invoice #{tx.Id}\nDate: {when}",
            metaFont, PdfBrushes.Black, new PointF(20, 60));

        var grid = new PdfGrid();
        grid.Columns.Add(5);
        var header = grid.Headers.Add(1)[0];
        header.Cells[0].Value = "Name";
        header.Cells[1].Value = "Category";
        header.Cells[2].Value = "Price";
        header.Cells[3].Value = "Qty";
        header.Cells[4].Value = "Total";

        foreach (var l in tx.Lines)
        {
            var row = grid.Rows.Add();
            row.Cells[0].Value = l.Name;
            row.Cells[1].Value = l.CategoryName;
            row.Cells[2].Value = l.Price.ToString("C");
            row.Cells[3].Value = l.Stock.ToString();
            row.Cells[4].Value = (l.Price * l.Stock).ToString("C");
        }

        grid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent5);
        grid.Draw(page, new PointF(20, 110));

        // Grand total
        var total = tx.Lines.Sum(l => l.Price * l.Stock);
        var tFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
        gfx.DrawString($"TOTAL: {total:C}", tFont, PdfBrushes.Black,
            new PointF(page.GetClientSize().Width - 150, page.GetClientSize().Height - 50));

        await using var ms = new MemoryStream();
        doc.Save(ms);
        return ms.ToArray();

    }

    public static async Task<string> SaveAsync(byte[] bytes, string fileName,
                                               CancellationToken ct = default)
    {
        await using var stream = new MemoryStream(bytes);

        var result = await FileSaver.Default.SaveAsync(fileName, stream, ct);

        if (!result.IsSuccessful || string.IsNullOrWhiteSpace(result.FilePath))
            throw result.Exception ?? new Exception("The file could not be saved.");

        return result.FilePath;
    }

    public static Task ShareAsync(string filePath) =>
        Share.RequestAsync(new ShareFileRequest
        {
            Title = Path.GetFileName(filePath),
            File = new ShareFile(filePath)
        });

    public static Task OpenAsync(string filePath) =>
        Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
}
