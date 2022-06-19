using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CompactJson;

namespace DiscordRandomStatus
{
    // List of strings useful for the API
    static class DiscordAPI
    {
        public static string apiUrl => $"{apiEndpoint}/v{apiVersion}";
        public static string authUrl => $"{apiEndpoint}/v{apiVersion}/auth";
        public static int apiVersion => 9;
        public static string baseAddress => "https://discordapp.com";
        public static string apiEndpoint => "/api";
        public static string loginEndpoint => $"{authUrl}/login";
        public static string mfaEndpoint => $"{authUrl}/mfa/totp";
        public static string fingerprintEndpoint => $"{authUrl}/fingerprint";
        public static string experimentsEndpoint => $"{apiUrl}/experiments";
        public static string userEndpoint => $"{apiUrl}/users/@me";
        public static string settingsEndpoint => $"{userEndpoint}/settings";
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

            DefaultRequestHeaders.Add("Accept-Encoding", new [] { "gzip", "deflate" });
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
                Debug.WriteLine("ERROR: " + e);
            }

            return response;
        }
    }

    class JsonContent : StringContent
    {
        public JsonContent(object content) : base(Serializer.ToString(content, false), Encoding.UTF8, "application/json")
        {
            // I forgot why I have to set the character set to null, but if anyone figures this out, you can have a cookie
            Headers.ContentType.CharSet = null;
            Headers.Add("Origin", "https://discord.com");
            
            //Headers.Add("Referer", "https://discord.com/login");

            // Find a proper way of creating client properties - json data encoded using base64
            Headers.Add("X-Super-Properties", Convert.ToBase64String(Encoding.UTF8.GetBytes(Serializer.ToString(new ClientProperties(), false))));
        }
    }

    // Client
    public class DiscordClient
    {
        // Ready event
        public event DiscordEventHandler<ReadyEventArgs> Ready;
        // Login error event
        public event DiscordEventHandler<LoginErrorEventArgs> LoginErrorEvent;
        public event DiscordEventHandler<LoginEventArgs> LoginCaptchaEvent;
        public event DiscordEventHandler<LoginEventArgs> LoginMfaEvent;
        public event DiscordEventHandler<LoginErrorEventArgs> LoginMfaErrorEvent;
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
                LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Invalid token"));
                Debug.WriteLine("Invalid token");
            }
            else if(response.IsSuccessStatusCode)
            {
                DataStore.SaveToken(token);
                Ready?.Invoke(new ReadyEventArgs(this));
            }
        }

        public void Login(string email, string password)
        {
            Login(email, password, null);
        }

        // Logs in using email and password
        public async void Login(string email, string password, string captchaKey)
        {
            client.DefaultRequestHeaders.Remove("Referer");
            client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/login");

            var log = new LoginRequest(email, password, captchaKey);

            var content = new JsonContent(log);

            var response = await client.PostAsync(DiscordAPI.loginEndpoint, content);
            var ree = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(ree);
            var responseObject = Serializer.Parse<LoginPayload>(ree);

            client.DefaultRequestHeaders.Remove("Referer");
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (responseObject.Errors != null)
                {
                    LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Wrong email or password"));
                }
                if (responseObject.CaptchaKey != null)
                {
                    if(responseObject.CaptchaService == "hcaptcha")
                    {
                        LoginCaptchaEvent?.Invoke(new LoginEventArgs(this, responseObject));
                    }
                    else
                    {
                        LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, $"You need to solve a {responseObject.CaptchaService}, unfortunately I don't support that one"));
                    }
                }
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
                        LoginMfaEvent?.Invoke(new LoginEventArgs(this, responseObject));
                    }
                    else
                    {
                        LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Unknown error"));
                        Debug.WriteLine("Unknown error");
                    }
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(responseObject.Token);
                    client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");
                    DataStore.SaveToken(responseObject.Token);
                    Ready?.Invoke(new ReadyEventArgs(this));
                }
            }
            else
            {
                LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Error " + response.StatusCode));
                Debug.WriteLine("Error " + response.StatusCode);
            }
        }

        public async void LoginMfa(string ticket, string code)
        {
            var log = new MfaRequest(code, ticket);

            var content = new JsonContent(log);

            var response = await client.PostAsync(DiscordAPI.mfaEndpoint, content);
            var responseObject = Serializer.Parse<MfaPayload>(await response.Content.ReadAsStringAsync());

            client.DefaultRequestHeaders.Remove("Referer");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (responseObject.Message != null && responseObject.Code == 60008)
                {
                    LoginMfaErrorEvent?.Invoke(new LoginErrorEventArgs(this, responseObject.Message));
                }
                else
                {
                    LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, responseObject.Message));
                    Debug.WriteLine(responseObject.Message);
                }
            }
            else if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseObject.Token))
                {
                    LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Unknown error"));
                    Debug.WriteLine("Unknown error");
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(responseObject.Token);
                    client.DefaultRequestHeaders.Add("Referer", "https://discordapp.com/channels/@me");
                    DataStore.SaveToken(responseObject.Token);
                    Ready?.Invoke(new ReadyEventArgs(this));
                }
            }
            else
            {
                LoginErrorEvent?.Invoke(new LoginErrorEventArgs(this, "Error " + response.StatusCode));
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
