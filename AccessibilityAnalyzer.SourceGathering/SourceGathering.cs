using Microsoft.Playwright;

namespace AccessibilityAnalyzer.SourceGathering;

public class SourceGathering
{
    public async Task<bool> Blub(string url, string outputDir)
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Set to false if you want to see the browser
            });

            await CapturePageAsync(browser, url, outputDir, "normal");
            await CapturePageAsync(browser, url, outputDir, "mobile");

            // normalImageUrl = await UploadImage("./output/screenshot_normal.png");
            //var mobileImageUrl = await UploadImage("./output/screenshot_mobile.png");

            // Close Browser
            await browser.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
        
        return true;
    }
    
    async Task CapturePageAsync(IBrowser browser, string url, string outputDir, string mode)
    {
        try
        {
            var contextOptions = new BrowserNewContextOptions();
            if (mode == "mobile")
            {
                contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 375, Height = 667 },
                    IsMobile = true
                };
            }

            var context = await browser.NewContextAsync(contextOptions);
            var page = await context.NewPageAsync();

            Console.WriteLine($"Navigating to: {url} in {mode} mode");
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

            // Capture Screenshot
            string screenshotPath = Path.Combine(outputDir, $"screenshot_{mode}.png");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            Console.WriteLine($"Screenshot saved: {screenshotPath}");

            // Extract Full HTML
            if (mode == "mobile")
            {
                Console.WriteLine("Skipping html extraction for mobile mode");
                return;
            }

            string htmlContent = await page.ContentAsync();
            string htmlPath = Path.Combine(outputDir, $"page_{mode}.html");
            await File.WriteAllTextAsync(htmlPath, htmlContent);
            Console.WriteLine($"HTML content saved: {htmlPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while capturing the page in {mode} mode: {ex.Message}");
        }
    }

}