using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BomRepo.BRMaster.DTO;
using BomRepo.BRXXXXX.DTO;
using BomRepo.ErrorsCatalog;
using System.IdentityModel.Tokens.Jwt;

namespace BomRepo.REST.Client
{
    public class BomRepoServiceClient
    {
        #region Singleton
        private static BomRepoServiceClient singleton;
        public static BomRepoServiceClient Instance {
            get {
                if (singleton == null) singleton = new BomRepoServiceClient();
                return singleton;
            }
        }
        #endregion

        private static readonly string secretKey = "1KillsAll_secretkey!7809";
        private ErrorDefinition latestErrorDefinition;
        private string username;
        private string password;

        /// <summary>
        /// For local use only, if true username and password were provided already from outside
        /// </summary>
        private bool AuthenticationProvided
        {
            get
            {
                return username != null && username != string.Empty;
            }
        }

        /// <summary>
        /// If true indicates that the latest operation failed. Error details are located in Error property.
        /// </summary>
        public bool ErrorOccurred {
            get {
                return latestErrorDefinition != null;
            }
        }

        /// <summary>
        /// Returns the the error definition of the last operation
        /// </summary>
        public ErrorDefinition Error {
            get {
                return latestErrorDefinition;
            }
        }

        /// <summary>
        /// Connects users with the service
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect(string Username, string Password) {
            //Initialize username and password
            username = Username;
            password = Password;

            //Initialize service with this credentials
            string accessToken = await InitializeCommunication();

            //If everything goes well then ErrorOccurred should be false
            return !ErrorOccurred;
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A List<User></returns>
        public async Task<UserDTO> GetUser(string username) {
            //Initialize communication and get authentication header
            string authHeader = await InitializeCommunication();

            //If something happen then return null and lets the external user take care of the error.
            if (ErrorOccurred) return null;

            //If everything goes well then lets try to get the data from the service with the authorization header obtained from the InitializationMethod
            //Define the total of requests we can send to the service, if token is valid only one will be needed. If token is invalid then two
            //If at the second request we get no positive response then something is happening with the service or credentials are
            //incorrect, in this case change the latesterrordefinition indicating that
            int requests = 1;
            while (requests <= 2) {
                HttpResponseMessage response = null;
                using (var client = new BomRepoHttpClient("application/json", authHeader))
                {
                    response = await client.GetAsync("api/v1/users/" + username);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        UserDTO entity = JsonConvert.DeserializeObject<UserDTO>(jsonData);
                        if (entity.Costumers.Count() == 0)
                            latestErrorDefinition = ErrorCatalog.NoCostumerAssociation;
                        return entity;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                        latestErrorDefinition = ErrorCatalog.InvalidCredentials;
                        return null;
                    }
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) authHeader = await RefreshToken();
                requests += 1;
            }

            latestErrorDefinition = ErrorCatalog.ServiceUnavailableOrInvalidCredentials;
            return null;
        }

        public async Task<List<ProjectDTO>> GetProjects(string costumernumber) {
            //Initialize communication and get authentication header
            string authHeader = await InitializeCommunication();

            //If something happen then return null and lets the external user take care of the error.
            if (ErrorOccurred) return null;

            //If everything goes well then lets try to get the data from the service with the authorization header obtained from the InitializationMethod
            //Define the total of requests we can send to the service, if token is valid only one will be needed. If token is invalid then two
            //If at the second request we get no positive response then something is happening with the service or credentials are
            //incorrect, in this case change the latesterrordefinition indicating that
            int requests = 1;
            while (requests <= 2)
            {
                HttpResponseMessage response = null;
                using (var client = new BomRepoHttpClient("application/json", authHeader))
                {
                    response = await client.GetAsync("api/v1/costumers/" + costumernumber + "/projects");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        List<ProjectDTO> entity = JsonConvert.DeserializeObject<List<ProjectDTO>>(jsonData);
                        return entity;
                    }
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) authHeader = await RefreshToken();
                requests += 1;
            }

            latestErrorDefinition = ErrorCatalog.ServiceUnavailableOrInvalidCredentials;
            return null;
        }

        /// <summary>
        /// Push a container and its content to the user project branch. If a container with the same name already exists the
        /// it will be replaced by this one.
        /// </summary>
        /// <param name="costumernumber">Required string indicating costumer number</param>
        /// <param name="username">Required string indicating username</param>
        /// <param name="projectnumber">Required string indicating project number</param>
        /// <param name="containername">Required string indicating the name of the container</param>
        /// <param name="parts">Required list of PartPlacementDTO object</param>
        /// <returns></returns>
        public async Task<bool> PushContainer(string costumernumber, string username, string projectnumber, string containername, List<PartPlacementDTO> parts) {
            //Initialize communication and get authentication header
            string authHeader = await InitializeCommunication();

            //If something happen then return null and lets the external user take care of the error.
            if (ErrorOccurred) return false;

            //Prepare data to transmit
            PartsContainerDTO container = new PartsContainerDTO();
            container.ParentPartName = containername;
            container.Placements = parts;

            //If everything goes well then lets try to get the data from the service with the authorization header obtained from the InitializationMethod
            //Define the total of requests we can send to the service, if token is valid only one will be needed. If token is invalid then two
            //If at the second request we get no positive response then something is happening with the service or credentials are
            //incorrect, in this case change the latesterrordefinition indicating that
            string url = "api/v1/costumers/" + costumernumber + "/branches/" + username + "?projectnumber=" + projectnumber;
            var jsonData = JsonConvert.SerializeObject(container);
            using (HttpContent content = new StringContent(jsonData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                int requests = 1;
                while (requests <= 2)
                {
                    HttpResponseMessage response = null;
                    using (var client = new BomRepoHttpClient("application/json", authHeader))
                    {
                        response = await client.PostAsync(url, content);
                        if (response.IsSuccessStatusCode)
                        {
                            latestErrorDefinition = null;
                            return true;
                        }
                        else if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                        {
                            jsonData = response.Content.ReadAsStringAsync().Result;
                            latestErrorDefinition = JsonConvert.DeserializeObject<ErrorDefinition>(jsonData);
                            return false;
                        }
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) authHeader = await RefreshToken();
                    requests += 1;
                }
            }

            latestErrorDefinition = ErrorCatalog.ServiceUnavailableOrInvalidCredentials;
            return false;
        }

        #region Helpers
        /// <summary>
        /// Initializes internal flags before start communicating with the service
        /// </summary>
        private async Task<string> InitializeCommunication()
        {
            //Reset error
            latestErrorDefinition = null;

            //Verify if credentials were sent
            if (!AuthenticationProvided)
            {
                latestErrorDefinition = ErrorCatalog.MissingCredentials;
                return string.Empty;
            }

            //Get Access Token
            Tuple<bool, string> accessToken = await TokenProviderMiddleware.Instance.GetAuthorizationHeaderValue(username, password);

            //Verify we were able to get the access token. If Tuple.Item1 is false then something happen with the authorization server
            //and no services are available
            if (!accessToken.Item1)
            {
                this.username = string.Empty;
                this.password = string.Empty;
                latestErrorDefinition = TokenProviderMiddleware.Instance.Error;
                return string.Empty;
            }

            //You can also have the status of the connection using IsConnected property
            return accessToken.Item2;
        }

        /// <summary>
        /// Get a new access token from the server
        /// </summary>
        /// <returns></returns>
        public async Task<string> RefreshToken() {
            TokenProviderMiddleware.Instance.RefreshToken();
            return await InitializeCommunication();
        }

        /// <summary>
        /// Authorize allow this class to connect and authenticate with the service.
        /// </summary>
        /// <param name="username">Define the username</param>
        /// <param name="password">Define the password</param>
        /// <returns></returns>
        private async Task<string> Authorize(string username, string password)
        {
            //Reset latestErrorDescription
            latestErrorDefinition = null;

            //Get Access Token
            Tuple<bool, string> accessToken = await TokenProviderMiddleware.Instance.GetAuthorizationHeaderValue(username, password);

            //Verify we were able to get the access token. If Tuple.Item1 is false then something happen with the authorization server
            //and no services are available
            if (!accessToken.Item1)
            {
                this.username = string.Empty;
                this.password = string.Empty;
                latestErrorDefinition = TokenProviderMiddleware.Instance.Error;
                return string.Empty;
            }

            //Set Username and Password
            this.username = username;
            this.password = password;

            //Return true
            //You can also have the status of the connection using IsConnected property
            return accessToken.Item2;
        }
        #endregion
    }

    public class BomRepoHttpClient : HttpClient {
        private const string apiBaseUrl = "https://localhost/";
        public BomRepoHttpClient(string MediaType, string AuthorizationHeaderValue) {
            BaseAddress = new Uri(apiBaseUrl);
            DefaultRequestHeaders.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            DefaultRequestHeaders.Add("Authorization", AuthorizationHeaderValue);
            DefaultRequestHeaders.Add("User-Agent", "autocad");
        }
    }

    public class TokenProviderMiddleware {
        #region Singleton
        private static TokenProviderMiddleware singleton;
        public static TokenProviderMiddleware Instance {
            get {
                if (singleton == null) singleton = new TokenProviderMiddleware();
                return singleton;
            }
        }
        #endregion
        private AccessToken accessToken;
        private ErrorDefinition latestErrorDefinition;
        private string username;
        private string password;
        public bool ErrorOccurred {
            get
            {
                return latestErrorDefinition != null;
            }
        }
        public ErrorDefinition Error {
            get {
                return latestErrorDefinition;
            }
        }
        public async Task<Tuple<bool,string>> GetAuthorizationHeaderValue(string username, string password) {
            latestErrorDefinition = null;
            if (accessToken == null | this.username != username | this.password != password)
            {
                this.username = username;
                this.password = password;

                string hashedPassword = ClientHelper.HashPlainText(password);
                string usernamePassword = username + ":" + hashedPassword;
                string encodedUsernamePassword = ClientHelper.Base64Encode(usernamePassword);

                using (var client = new BomRepoHttpClient("application/x-www-form-urlencoded", "Basic " + encodedUsernamePassword))
                {
                    string url = "api/v1/token";
                    HttpResponseMessage response = await client.PostAsync(url, null);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = response.Content.ReadAsStringAsync().Result;
                        accessToken = JsonConvert.DeserializeObject<AccessToken>(jsonData);
                        return new Tuple<bool, string>(true, "Bearer " + accessToken.access_token);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        latestErrorDefinition = ErrorCatalog.BomRepoServiceUnavailable;
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        latestErrorDefinition = ErrorCatalog.InvalidCredentials;

                    accessToken = null;
                    return new Tuple<bool, string>(false, string.Empty);
                }
            }
            return new Tuple<bool, string>(true, "Bearer " + accessToken.access_token);
        }
        public async void RefreshToken() {
            accessToken = null;
            await GetAuthorizationHeaderValue(username, password);
        }
        public List<string> GetUserCostumerNumbers() {
            //Get jwt
            var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken.access_token);
            return null;
        }
    }

    public class AccessToken {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
