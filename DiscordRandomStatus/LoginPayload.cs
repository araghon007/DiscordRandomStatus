using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCustomStatus
{
    class MfaRequest
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "gift_code_sku_id")]
        public string GiftCode { get; set; }

        [JsonProperty(PropertyName = "login_source")]
        public string LoginSource { get; set; }

        [JsonProperty(PropertyName = "ticket")]
        public string Ticket { get; set; }

        public MfaRequest(string code, string ticket)
        {
            Code = code;
            Ticket = ticket;
        }
    }
    class MfaPayload
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; internal set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; internal set; }

        [JsonProperty(PropertyName = "token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; internal set; }
    }

    class LoginRequest
    {
        [JsonProperty(PropertyName = "captcha_key", NullValueHandling = NullValueHandling.Ignore)]
        public string CaptchaKey { get; set; }

        [JsonProperty(PropertyName = "email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "gift_code_sku_id")]
        public string GiftCode { get; set; }

        [JsonProperty(PropertyName = "login_source")]
        public string LoginSource { get; set; }

        [JsonProperty(PropertyName = "password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "undelete")]
        public bool Undelete { get; set; }

        public LoginRequest(string email, string password, bool undelete = false)
        {
            Email = email;
            Password = password;
            Undelete = undelete;
        }
    }

    class LoginPayload
    {
        [JsonProperty(PropertyName = "captcha_key")]
        public string[] CaptchaKey { get; internal set; }

        [JsonProperty(PropertyName = "email")]
        public string[] Email { get; internal set; }

        [JsonProperty(PropertyName = "gift_code_sku_id")]
        public string[] GiftCode { get; internal set; }

        [JsonProperty(PropertyName = "login_source")]
        public string[] LoginSource { get; internal set; }

        [JsonProperty(PropertyName = "password")]
        public string[] Password { get; internal set; }

        [JsonProperty(PropertyName = "token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; internal set; }

        [JsonProperty(PropertyName = "ticket", NullValueHandling = NullValueHandling.Ignore)]
        public string Ticket { get; internal set; }

        [JsonProperty(PropertyName = "undelete")]
        public bool? Undelete { get; internal set; }
    }
}
