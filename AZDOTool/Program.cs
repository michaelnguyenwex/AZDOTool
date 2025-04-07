using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Default values for optional parameters
        string automatedTestName = null; // Default value for optional parameter
        string organization = "WexHealthTech";    // Default value for optional parameter
        string project = "QA";                    // Default value for optional parameter
        string personalAccessToken = null;    // Default value for optional parameter

        // Required parameters (will be set from command-line arguments)
        string testCaseId = null;
        string automatedTestStorage = "Evolution1.OnDemand.QA.ConsumerInvestmentPortal.AcceptanceTests.dll";

        // Parse command-line arguments
        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--testcaseid":
                        if (i + 1 < args.Length)
                        {
                            testCaseId = args[++i];
                        }
                        break;
                    case "--automatedteststorage":
                        if (i + 1 < args.Length)
                        {
                            automatedTestStorage = args[++i];
                        }
                        break;
                    case "--automatedtestname":
                        if (i + 1 < args.Length)
                        {
                            automatedTestName = args[++i];
                        }
                        break;
                    case "--organization":
                        if (i + 1 < args.Length)
                        {
                            organization = args[++i];
                        }
                        break;
                    case "--project":
                        if (i + 1 < args.Length)
                        {
                            project = args[++i];
                        }
                        break;
                    case "--pat":
                        if (i + 1 < args.Length)
                        {
                            personalAccessToken = args[++i];
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {args[i]}");
                }
            }

            // Validate required parameters
            if (string.IsNullOrEmpty(testCaseId))
            {
                throw new ArgumentException("Required parameter --testCaseId is missing.");
            }
            if (string.IsNullOrEmpty(automatedTestName))
            {
                throw new ArgumentException("Required parameter --automatedTestName is missing.");
            }
            if (string.IsNullOrEmpty(personalAccessToken))
            {
                throw new ArgumentException("Required parameter --pat is missing.");
            }
        }
        catch (ArgumentException ex)
        {
            // Display usage instructions if parsing fails
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Usage: AzdoTestCaseUpdater.exe --testCaseId <id> --automatedTestStorage <storage> [--automatedTestName <name>] [--organization <org>] [--project <project>] [--pat <token>]");
            Console.WriteLine("Example: AzdoTestCaseUpdater.exe --testCaseId 488728 --automatedTestStorage Test.dll --automatedTestName Test.test12 --organization WexHealthTech --project QA --pat 123key");
            return;
        }

        // API URL
        string apiUrl = $"https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/{testCaseId}?api-version=7.1-preview.3";
        Console.WriteLine(apiUrl);

        // Ensure PAT is not null or empty
        if (string.IsNullOrEmpty(personalAccessToken))
        {
            throw new ArgumentException("Personal Access Token (PAT) is not defined. Provide it via --pat or set it as an environment variable.");
        }
        Console.WriteLine("PAT validation completed");

        // Encode the PAT for Basic Authentication
        string encodedPat = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}"));
        Console.WriteLine("PAT encoding completed");

        // Set up HttpClient with headers
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedPat);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        Console.WriteLine("Request headers added successfully");

        // Define the request body as a JSON patch document
        var patchOperations = new[]
        {
            new { op = "add", path = "/fields/Microsoft.VSTS.TCM.AutomatedTestName", value = automatedTestName },
            new { op = "add", path = "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage", value = automatedTestStorage },
            new { op = "add", path = "/fields/Microsoft.VSTS.TCM.AutomatedTestType", value = "Unit Test" },
            new { op = "add", path = "/fields/Microsoft.VSTS.TCM.AutomatedTestId", value = "123" }
        };
        string body = JsonSerializer.Serialize(patchOperations);
        Console.WriteLine("Request body defined successfully");

        // Make the REST API call
        try
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json-patch+json");
            HttpResponseMessage response = await client.PatchAsync(apiUrl, content);

            // Extract and log HTTP status code and response content
            int statusCode = (int)response.StatusCode;
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("HTTP request completed successfully");
            Console.WriteLine($"HTTP Status Code: {statusCode}");
            Console.WriteLine($"Response Body: {responseBody}");

            // Ensure the response is successful
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP-specific errors
            if (ex.StatusCode.HasValue)
            {
                int statusCode = (int)ex.StatusCode;
                string responseBody = ex.Message; // HttpClient exceptions don't always include the response body directly

                Console.WriteLine("HTTP request failed");
                Console.WriteLine($"HTTP Status Code: {statusCode}");
                Console.WriteLine($"Error Response Body: {responseBody}");
            }
            else
            {
                Console.WriteLine("HTTP request failed with an unknown error");
            }

            // Log the exception message
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            Console.WriteLine("An unexpected error occurred");
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }

}