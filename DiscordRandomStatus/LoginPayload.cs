using CompactJson;

namespace DiscordRandomStatus
{
    class MfaRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("gift_code_sku_id")]
        public string GiftCode { get; set; }

        [JsonProperty("login_source")]
        public string LoginSource { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        public MfaRequest(string code, string ticket)
        {
            Code = code;
            Ticket = ticket;
        }
    }
    class MfaPayload
    {
        [JsonProperty("code")]
        public int Code { get; internal set; }

        [JsonProperty("message")]
        public string Message { get; internal set; }

        [JsonProperty("token")]
        public string Token { get; internal set; }
    }

    class LoginRequest
    {
        [JsonProperty("captcha_key")]
        public string CaptchaKey { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("gift_code_sku_id")]
        public string GiftCode { get; set; }

        [JsonProperty("login_source")]
        public string LoginSource { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("undelete")]
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
        [JsonProperty("captcha_key")]
        public string[] CaptchaKey { get; internal set; }

        [JsonProperty("email")]
        public string[] Email { get; internal set; }

        [JsonProperty("gift_code_sku_id")]
        public string[] GiftCode { get; internal set; }

        [JsonProperty("login_source")]
        public string[] LoginSource { get; internal set; }

        [JsonProperty("password")]
        public string[] Password { get; internal set; }

        [JsonProperty("token")]
        public string Token { get; internal set; }

        [JsonProperty("ticket")]
        public string Ticket { get; internal set; }

        [JsonProperty("undelete")]
        public bool? Undelete { get; internal set; }

        [JsonProperty("errors")]
        public object Errors { get; internal set; }
    }
}
