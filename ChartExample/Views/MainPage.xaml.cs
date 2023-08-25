using GemBox.Pdf.Content;
using GemBox.Spreadsheet.Charts;
using GemBox.Spreadsheet;
using GemBox.Pdf;

namespace ChartExample.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var captureResult = await btnCapture.CaptureAsync();
        var stream = await captureResult.OpenReadAsync();
        using MemoryStream streamStream = new();
        await streamStream.CopyToAsync(stream);

        using (var document = new PdfDocument())
        {
            var page = document.Pages.Add();
            double x = 50;
            double y = page.Size.Height;

            using (var formattedText = new PdfFormattedText())
            {
                formattedText.Append("The following chart is imported from a PDF that was created with GemBox.Spreadsheet.");
                page.Content.DrawText(formattedText, new PdfPoint(x, y - 50));
            }

            // Create chart and save it as PDF stream.
            var chart = CreateChart(400, 200);
            var chartAsPdf = new MemoryStream();
            chart.Format().Save(chartAsPdf, GemBox.Spreadsheet.SaveOptions.PdfDefault);

            // Add chart to PDF page.
            using (var chartDocument = PdfDocument.Load(chartAsPdf))
                document.AppendPage(chartDocument, 0, 0, new PdfPoint(x, y - chart.Position.Height - 60));

            document.Save("Chart.pdf");
        }
    }

    static ExcelChart CreateChart(double width, double height)
    {
        var workbook = new ExcelFile();
        var worksheet = workbook.Worksheets.Add("Chart");

        worksheet.Cells["A1"].Value = "Name";
        worksheet.Cells["A2"].Value = "John Doe";
        worksheet.Cells["A3"].Value = "Fred Nurk";
        worksheet.Cells["A4"].Value = "Hans Meier";
        worksheet.Cells["A5"].Value = "Ivan Horvat";

        worksheet.Cells["B1"].Value = "Salary";
        worksheet.Cells["B2"].Value = 3600;
        worksheet.Cells["B3"].Value = 2580;
        worksheet.Cells["B4"].Value = 3200;
        worksheet.Cells["B5"].Value = 4100;

        worksheet.Columns[1].Style.NumberFormat = "\"$\"#,##0";

        var chart = worksheet.Charts.Add(ChartType.Bar,
            new AnchorCell(worksheet.Cells["A1"], true), width, height, LengthUnit.Point);

        chart.SelectData(worksheet.Cells.GetSubrangeAbsolute(0, 0, 4, 1), true);
        return chart;
    }
}

public static class PdfDocumentExtension
{
    public static PdfFormContent AppendPage(this PdfDocument destination, PdfDocument source,
        int sourcePageIndex, int destinationPageIndex, PdfPoint destinationBottomLeft)
    {
        var form = source.Pages[sourcePageIndex].ConvertToForm(destination);
        var group = destination.Pages[destinationPageIndex].Content.Elements.AddGroup();

        var formContent = group.Elements.AddForm(form);
        formContent.Transform = PdfMatrix.CreateTranslation(destinationBottomLeft.X, destinationBottomLeft.Y);
        return formContent;
    }
}

