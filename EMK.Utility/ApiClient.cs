namespace EMK.Utility
{
    public class ApiClient : HttpClient
    {
        // ── Auth state ────────────────────────────────────────────────────────
        public string Token         { get; private set; } = string.Empty;
        public User?  LoggedInUser  { get; private set; }

        public ApiClient(string baseAddress)
        {
            BaseAddress = new Uri(baseAddress);
        }

        public void ClearToken()
        {
            Token = string.Empty;
            LoggedInUser = null;
            DefaultRequestHeaders.Authorization = null;
        }

        // ── Authentication ────────────────────────────────────────────────────

        /// <summary>
        /// Authenticates the user and stores the JWT token for subsequent requests.
        /// </summary>
        public HttpStatusCode Authenticate(string userId, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(Token))
                    return HttpStatusCode.OK;   // already authenticated

                return GetToken(userId, password);
            }
            catch (Exception) { throw; }
        }

        private HttpStatusCode GetToken(string userId, string password)
        {
            try
            {
                HttpClient client = new HttpClient { BaseAddress = this.BaseAddress };

                var payload = new Dictionary<string, string>
                {
                    { "userId",   userId   },
                    { "password", password }
                };

                string serialized = JsonConvert.SerializeObject(payload);
                var content       = new StringContent(serialized);
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response =
                    client.PostAsync("User/authenticate", content).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var values    = JsonConvert.DeserializeObject<Dictionary<string, string>>(result)!;

                    Token = values["Token"];

                    LoggedInUser = new User
                    {
                        Id             = Guid.Parse(values["Id"]),
                        FirstName      = values["FirstName"],
                        LastName       = values["LastName"],
                        UserId         = values["UserId"],
                        Email          = values["Email"],
                        SharedBudgetId = values.ContainsKey("SharedBudgetId") && !string.IsNullOrEmpty(values["SharedBudgetId"])
                                            ? Guid.Parse(values["SharedBudgetId"])
                                            : null,
                        BudgetRole     = values.ContainsKey("BudgetRole") && !string.IsNullOrEmpty(values["BudgetRole"])
                                            ? Enum.Parse<BudgetRole>(values["BudgetRole"])
                                            : null
                    };

                    DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", Token);

                    return HttpStatusCode.OK;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    var values    = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    throw new Exception(values?["message"] ?? "Authentication failed.");
                }

                return response.StatusCode;
            }
            catch (Exception) { throw; }
        }

        // ── GetList ───────────────────────────────────────────────────────────

        public List<T> GetList<T>(string controller)
        {
            try
            {
                HttpResponseMessage response = this.GetAsync(controller).Result;
                string result               = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var items = (JArray)JsonConvert.DeserializeObject(result)!;
                    return items.ToObject<List<T>>()!;
                }
                throw new Exception(response.ReasonPhrase);
            }
            catch (Exception) { throw; }
        }

        public List<T> GetList<T>(string controller, Guid id)
        {
            try { return GetList<T>(controller + "/" + id.ToString()); }
            catch (Exception) { throw; }
        }

        public List<T> GetList<T>(string controller, string route)
        {
            try { return GetList<T>(controller + "/" + route); }
            catch (Exception) { throw; }
        }

        public List<T> GetList<T>(string controller, string route, Guid id)
        {
            try { return GetList<T>(controller + "/" + route + "/" + id.ToString()); }
            catch (Exception) { throw; }
        }

        public List<T> GetList<T>(string controller, string route, int p1, int p2)
        {
            try { return GetList<T>($"{controller}/{route}/{p1}/{p2}"); }
            catch (Exception) { throw; }
        }

        // ── GetItem ───────────────────────────────────────────────────────────

        public T GetItem<T>(string controller)
        {
            try
            {
                HttpResponseMessage response = this.GetAsync(controller).Result;
                string result               = response.Content.ReadAsStringAsync().Result;
                return (T)JsonConvert.DeserializeObject(result, typeof(T))!;
            }
            catch (Exception) { throw; }
        }

        public T GetItem<T>(string controller, Guid id)
        {
            try { return GetItem<T>(controller + "/" + id.ToString()); }
            catch (Exception) { throw; }
        }

        public T GetItem<T>(string controller, string route)
        {
            try { return GetItem<T>(controller + "/" + route); }
            catch (Exception) { throw; }
        }

        public T GetItem<T>(string controller, string route, Guid id)
        {
            try { return GetItem<T>(controller + "/" + route + "/" + id.ToString()); }
            catch (Exception) { throw; }
        }

        // ── Post ─────────────────────────────────────────────────────────────

        public HttpResponseMessage Post<T>(T item, string controller)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(item);
                var content       = new StringContent(serialized);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return this.PostAsync(controller, content).Result;
            }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Post<T>(T item, string controller, bool rollback)
        {
            try { return Post(item, $"{controller}/{rollback}"); }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Post<T>(T item, string controller, string route)
        {
            try { return Post(item, controller + "/" + route); }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Post<T>(T item, string controller, string route, bool rollback)
        {
            try { return Post(item, $"{controller}/{route}/{rollback}"); }
            catch (Exception) { throw; }
        }

        // ── Post (no body — action endpoints) ────────────────────────────────

        public HttpResponseMessage PostEmpty(string controller)
        {
            try
            {
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return this.PostAsync(controller, content).Result;
            }
            catch (Exception) { throw; }
        }

        // ── Put ──────────────────────────────────────────────────────────────

        public HttpResponseMessage Put<T>(T item, string controller, Guid id)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(item);
                var content       = new StringContent(serialized);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return this.PutAsync(controller + "/" + id.ToString(), content).Result;
            }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Put<T>(T item, string controller, Guid id, bool rollback)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(item);
                var content       = new StringContent(serialized);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return this.PutAsync($"{controller}/{id}/{rollback}", content).Result;
            }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Put<T>(T item, string controller, string route)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(item);
                var content       = new StringContent(serialized);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return this.PutAsync(controller + "/" + route, content).Result;
            }
            catch (Exception) { throw; }
        }

        // ── Delete ────────────────────────────────────────────────────────────

        public HttpResponseMessage Delete(string controller, Guid id)
        {
            try { return this.DeleteAsync(controller + "/" + id.ToString()).Result; }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Delete(string controller, Guid id, bool rollback)
        {
            try { return this.DeleteAsync($"{controller}/{id}/{rollback}").Result; }
            catch (Exception) { throw; }
        }

        public HttpResponseMessage Delete(string controller, string route, Guid id)
        {
            try { return this.DeleteAsync($"{controller}/{route}/{id}").Result; }
            catch (Exception) { throw; }
        }

        // ── Result helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the "id" value from a standard API insert response.
        /// </summary>
        public Guid GetInsertedId(HttpResponseMessage response)
        {
            try
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var dict      = JsonConvert.DeserializeObject<Dictionary<string, string>>(result)!;
                return Guid.Parse(dict["id"]);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Extracts the "rowsaffected" value from a standard API update/delete response.
        /// </summary>
        public int GetRowsAffected(HttpResponseMessage response)
        {
            try
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var dict      = JsonConvert.DeserializeObject<Dictionary<string, string>>(result)!;
                int.TryParse(dict["rowsaffected"], out int rows);
                return rows;
            }
            catch (Exception) { throw; }
        }
    }
}
