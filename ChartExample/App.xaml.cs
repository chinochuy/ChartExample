using GemBox.Pdf;
using GemBox.Spreadsheet;

namespace ChartExample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        // If using the Professional version, put your GemBox.Pdf serial key below.
        ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        // If using the Professional version, put your GemBox.Spreadsheet serial key below.
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        MainPage = new AppShell();
	}
}
