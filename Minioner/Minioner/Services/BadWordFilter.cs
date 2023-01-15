using Discord;
using Minioner.Common;
using Newtonsoft.Json;

namespace Minioner.Services
{
    public class BadWordFilter
    {
        public string IsBad { get; set; }
        public async Task<string> CheckInput(string input)
        {
            using (var client = new HttpClient())
            {
                // TODO abstract this to neutrino service
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://neutrinoapi.net/bad-word-filter"))
                {
                    // add user id and api key to header
                    request.Headers.TryAddWithoutValidation("API-Key", Globals.NeutrinoAPIKey);
                    request.Headers.TryAddWithoutValidation("User-ID", "minioner");
                    // add url encoded fields
                    var dict = new Dictionary<string, string>
                    {
                        { "content", input },
                        { "catalog", "strict" }
                    };
                    request.Content = new FormUrlEncodedContent(dict);
                    // send request
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            Stream responseStream = await response.Content.ReadAsStreamAsync();
                            using (var responseStreamReader = new StreamReader(responseStream))
                            {
                                using (var jsonTextReader = new JsonTextReader(responseStreamReader))
                                {
                                    return await response.Content.ReadAsStringAsync();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                           await Logger.Log(LogSeverity.Error, $"{nameof(BadWordFilter)}", e.Message);
                        }
                    }
                    request.Dispose();
                }
                client.Dispose();
            }
            return "error";
        }
    }
}
