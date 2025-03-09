using Microsoft.Playwright;

namespace AccessibilityAnalyzer.SourceGathering;

public class SourceGathering
{
    public async Task<PageData?> GetPageData(Uri uri)
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var (desktopHtmlContent, desktopScreenshot) = await CapturePageAsync(browser, uri, "normal");
            var (_, mobileScreenshot) = await CapturePageAsync(browser, uri, "mobile");

            await browser.CloseAsync();

            if (desktopHtmlContent == null || desktopScreenshot == null || mobileScreenshot == null)
                return null;

            return new PageData(uri.AbsoluteUri, desktopHtmlContent, desktopScreenshot, mobileScreenshot);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    private async Task<(string? HtmlContent, byte[]? Screenshot)> CapturePageAsync(
        IBrowser browser,
        Uri uri,
        string mode
    )
    {
        try
        {
            var contextOptions = new BrowserNewContextOptions();
            if (mode == "mobile")
                contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 375, Height = 667 },
                    IsMobile = true
                };

            var context = await browser.NewContextAsync(contextOptions);
            var page = await context.NewPageAsync();

            Console.WriteLine($"Navigating to: {uri.AbsoluteUri} in {mode} mode");
            await page.GotoAsync(uri.AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            // Capture Screenshot
            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
            Console.WriteLine("Screenshot saved.");


            var htmlContent = await page.ContentAsync();
            Console.WriteLine("HTML content saved");

            return (htmlContent, screenshotBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while capturing the page in {mode} mode: {ex.Message}");
            return (null, null);
        }
    }
}

public record PageData(string Url, string HtmlContent, byte[] DesktopScreenshot, byte[] MobileScreenshot);