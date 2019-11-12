using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCustomStatus
{
    // List of strings useful for the API
    static class DiscordAPI
    {
        public static string apiUrl { get { return $"{apiEndpoint}/v{apiVersion}"; } }
        public static int apiVersion { get { return 6; } }
        public static string baseAddress { get { return "https://discordapp.com"; } }
        public static string apiEndpoint { get { return "/api"; } }
        public static string loginEndpoint { get { return $"{apiUrl}/auth/login"; } }
        public static string mfaEndpoint { get { return $"{apiUrl}/auth/mfa/totp"; } }
        public static string userEndpoint { get { return $"{apiUrl}/users/@me"; } }
        public static string settingsEndpoint { get { return $"{userEndpoint}/settings"; } }
        public static string guildsEndpoint { get { return $"{userEndpoint}/guilds"; } }
        public static string gatewayEndpoint { get { return $"{apiUrl}/gateway"; } }
    }

    class DiscordRestClient : HttpClient
    {
        // Rest client
        public DiscordRestClient() : base(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true)
        {
            BaseAddress = new Uri(DiscordAPI.baseAddress);
            DefaultRequestHeaders.Clear();

            // I forgot why this is here
            DefaultRequestHeaders.ExpectContinue = false;

            DefaultRequestHeaders.Add("Accept-Encoding", new string[] { "gzip", "deflate", "br" });
            DefaultRequestHeaders.Add("Accept-Language", "en-US");
            DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) discord/0.0.690 Chrome/69.0.3497.128 Electron/4.0.8 Safari/537.36");
            DefaultRequestHeaders.Add("Accept", "*/*");
        }

        // PATCH, where POST isn't enough 
        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }
    }

    class JsonContent : StringContent
    {
        public JsonContent(object content) : base(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
        {
            // I forgot why I have to set the character set to null, but if anyone figures this out, you can have a cookie
            Headers.ContentType.CharSet = null;
            Headers.Add("Origin", "https://discordapp.com");

            // Find a proper way of creating client properties - json data encoded using base64
            Headers.Add("X-Super-Properties", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ClientProperties()))));

            // Figure out how to generate this one, quick - composed of snowflake and something that may have connection to the way tokens are generated
            long fingerprintSnowflake = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 1420070400000) << 22;

            Headers.Add("X-Fingerprint", $"{fingerprintSnowflake}.R3Uv_45gCvcCNmYX8r4xw_b2A2A");

            // I don't know
            Headers.Add("X-Debug-Options", "trace,canary,logGatewayEvents,logOverlayEvents");
        }
    }

    // Client
    public class DiscordClient
    {
        // Ready event
        public event DiscordEventHandler<ReadyEventArgs> Ready;
        // Login error event
        public event DiscordEventHandler<LoginErrorEventArgs> LoginError;
        private readonly DiscordRestClient client = new DiscordRestClient();

        // Login using token, sets Authorization header and tests if the token is valid by sending a GET request to user API endpoint
        public async void Login(string token)
        {
            client.DefaultRequestHeaders.Remove("Referer");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");

            var response = await client.GetAsync(DiscordAPI.userEndpoint);
            Debug.WriteLine($"{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                LoginError(new LoginErrorEventArgs(this, new string[] { "Invalid token" }));
                Debug.WriteLine("Invalid token");
            }
            else if(response.IsSuccessStatusCode)
            {
                DataStore.SaveToken(token);
                Ready(new ReadyEventArgs(this));
            }
        }

        // Logs in using email and password
        public async void Login(string email, string password, string code)
        {
            client.DefaultRequestHeaders.Remove("Referer");
            client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/login");

            var log = new LoginRequest(email, password);

            var content = new JsonContent(log);

            var response = await client.PostAsync(DiscordAPI.loginEndpoint, content);
            var responseObject = JsonConvert.DeserializeObject<LoginPayload>(await response.Content.ReadAsStringAsync());

            client.DefaultRequestHeaders.Remove("Referer");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = new List<string> { };
                if (responseObject.Email != null)
                {
                    foreach (var mail in responseObject.Email)
                    {
                        errors.Add(mail);
                        Debug.WriteLine(mail);
                    }
                }
                if (responseObject.Password != null)
                {
                    foreach (var pass in responseObject.Password)
                    {
                        errors.Add(pass);
                        Debug.WriteLine(pass);
                    }
                }
                if (responseObject.CaptchaKey != null)
                {
                    foreach (var captcha in responseObject.CaptchaKey)
                    {
                        errors.Add(captcha);
                        Debug.WriteLine(captcha);
                    }
                }
                if (!string.IsNullOrEmpty(responseObject.Token))
                {
                    errors.Add(responseObject.Token);
                    Debug.WriteLine(responseObject.Token);
                }
                if (responseObject.Undelete.HasValue)
                {
                    errors.Add("Undelete: " + responseObject.Undelete);
                    Debug.WriteLine("Undelete: " + responseObject.Undelete);
                }
                if (responseObject.LoginSource != null)
                {
                    foreach (var login in responseObject.LoginSource)
                    {
                        errors.Add(login);
                        Debug.WriteLine(login);
                    }
                }
                if (responseObject.GiftCode != null)
                {
                    foreach (var gift in responseObject.GiftCode)
                    {
                        errors.Add(gift);
                        Debug.WriteLine(gift);
                    }
                }
                LoginError(new LoginErrorEventArgs(this, errors.ToArray()));
            }
            else if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseObject.Token))
                {
                    if (!string.IsNullOrEmpty(responseObject.Ticket))
                    {
                        client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/login");
                        // Adds a delay so we don't get ratelimited
                        //await Task.Delay(1500);
                        LoginMfa(responseObject.Ticket, code);
                    }
                    else
                    {
                        LoginError(new LoginErrorEventArgs(this, new string[] { "Unknown error" }));
                        Debug.WriteLine("Unknown error");
                    }
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(responseObject.Token);
                    client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");
                    DataStore.SaveToken(responseObject.Token);
                    Ready(new ReadyEventArgs(this));
                }
            }
            else
            {
                LoginError(new LoginErrorEventArgs(this, new string[] { "Error " + response.StatusCode }));
                Debug.WriteLine("Error " + response.StatusCode);
            }
        }

        public async void LoginMfa(string ticket, string code)
        {
            var log = new MfaRequest(code, ticket);

            var content = new JsonContent(log);

            var response = await client.PostAsync(DiscordAPI.mfaEndpoint, content);
            var responseObject = JsonConvert.DeserializeObject<MfaPayload>(await response.Content.ReadAsStringAsync());

            client.DefaultRequestHeaders.Remove("Referer");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = new List<string> { };
                if (responseObject.Message != null)
                {
                    errors.Add(responseObject.Message);
                    Debug.WriteLine(responseObject.Message);
                }
                if (responseObject.Token != null)
                {
                    errors.Add(responseObject.Token);
                    Debug.WriteLine(responseObject.Token);
                }
                LoginError(new LoginErrorEventArgs(this, errors.ToArray()));
            }
            else if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseObject.Token))
                {
                    LoginError(new LoginErrorEventArgs(this, new string[] { "Unknown error" }));
                    Debug.WriteLine("Unknown error");
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(responseObject.Token);
                    client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");
                    DataStore.SaveToken(responseObject.Token);
                    Ready(new ReadyEventArgs(this));
                }
            }
            else
            {
                LoginError(new LoginErrorEventArgs(this, new string[] { "Error " + response.StatusCode }));
                Debug.WriteLine("Error " + response.StatusCode);
            }
        }

        public async void SetStatus(CustomStatus status)
        {
            var state = new UserSettings(status);

            var content = new JsonContent(state);

            await client.PatchAsync(DiscordAPI.settingsEndpoint, content);

        }
    }
}
