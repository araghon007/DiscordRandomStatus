using CompactJson;

namespace DiscordRandomStatus
{
    public class MfaRequest
    {
        [JsonProperty("code")]
        [JsonEmitNullValue]
        public string Code { get; set; }

        [JsonProperty("gift_code_sku_id")]
        [JsonEmitNullValue]
        public string GiftCode { get; set; }

        [JsonProperty("login_source")]
        [JsonEmitNullValue]
        public string LoginSource { get; set; }

        [JsonProperty("ticket")]
        [JsonEmitNullValue]
        public string Ticket { get; set; }

        public MfaRequest() { }

        public MfaRequest(string code, string ticket)
        {
            Code = code;
            Ticket = ticket;
        }
    }

    public class MfaPayload
    {
        [JsonProperty("code")]
        public int Code { get; internal set; }

        [JsonProperty("message")]
        public string Message { get; internal set; }

        [JsonProperty("token")]
        public string Token { get; internal set; }
    }

    public class LoginRequest
    {
        [JsonProperty("login")]
        [JsonEmitNullValue]
        public string Login { get; set; }

        [JsonProperty("password")]
        [JsonEmitNullValue]
        public string Password { get; set; }

        [JsonProperty("undelete")]
        [JsonEmitNullValue]
        public bool Undelete { get; set; }

        [JsonProperty("captcha_key")]
        [JsonEmitNullValue]
        public string CaptchaKey { get; set; }

        [JsonProperty("login_source")]
        [JsonEmitNullValue]
        public string LoginSource { get; set; }

        [JsonProperty("gift_code_sku_id")]
        [JsonEmitNullValue]
        public string GiftCode { get; set; }

        public LoginRequest() 
        {
            Undelete = false;
        }

        public LoginRequest(string login, string password) : this(login, password, false) { }

        public LoginRequest(string login, string password, string captchaKey) : this(login, password, false, captchaKey) { }

        public LoginRequest(string login, string password, bool undelete) : this(login, password, undelete, null) { }

        public LoginRequest(string login, string password, bool undelete, string captchaKey)
        {
            Login = login;
            Password = password;
            Undelete = undelete;
            CaptchaKey = captchaKey;
        }
    }

    public class LoginPayload
    {
        [JsonProperty("captcha_key")]
        public string[] CaptchaKey { get; set; }

        [JsonProperty("captcha_sitekey")]
        public string CaptchaSiteKey { get; set; }

        [JsonProperty("captcha_service")]
        public string CaptchaService { get; set; }

        [JsonProperty("login")]
        public string[] Login { get; internal set; }

        [JsonProperty("gift_code_sku_id")]
        public string[] GiftCode { get; internal set; }

        [JsonProperty("login_source")]
        public string[] LoginSource { get; internal set; }

        [JsonProperty("password")]
        public string[] Password { get; internal set; }

        [JsonProperty("mfa")]
        public bool MFA { get; internal set; }

        [JsonProperty("sms")]
        public bool SMS { get; internal set; }

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
