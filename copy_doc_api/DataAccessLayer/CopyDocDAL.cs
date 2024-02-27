using copy_doc_api.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using Newtonsoft.Json.Linq;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace copy_doc_api.DataAccessLayer
{
    public class CopyDocDAL : ICopyDocDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        string tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress, Prefix_URL, BaseAddress,HostName;
        
        public CopyDocDAL(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {           
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            scope = _configuration["TokenDetails:Scope"];
            grantType = _configuration["TokenDetails:GrantType"];
            Prefix_URL = _configuration["APIDetails:Prefix_URL"];
            BaseAddress = _configuration["APIDetails:BaseAddress"];
            HostName = _configuration["TokenDetails:HostName"];
            TokenBaseAddress = _configuration["TokenDetails:BaseAddress"];
            //tenantId = _configuration["TokenDetails:TenantId"];
            //clientId = _configuration["TokenDetails:ClientId"];
            //clientSecret = _configuration["TokenDetails:ClientSecret"];
            //siteId = _configuration["TokenDetails:SiteId"];
        }
        public void SetSiteConfigurations(string sharePointSiteName)
        {
            // Retrieve site-specific configuration settings based on the provided site name
            tenantId = _configuration[$"TokenDetails:{sharePointSiteName}:TenantId"];
            clientId = _configuration[$"TokenDetails:{sharePointSiteName}:ClientId"];
            clientSecret = _configuration[$"TokenDetails:{sharePointSiteName}:ClientSecret"];
        }
        public async Task<AccessToken> GenerateAccessTokenAsync(string tenantId, string clientId, string clientSecret, string scope, string grantType, string BaseAddress,string sharePointSiteName)
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                try
                {
                    string tokenEndpoint = $"{_configuration["TokenDetails:TokenURL"]}{tenantId}/oauth2/v2.0/token";
                    var formData = new List<KeyValuePair<string, string>>
                    {
                         new KeyValuePair<string, string>("grant_type",grantType),
                         new KeyValuePair<string, string>("client_id", clientId),
                         new KeyValuePair<string, string>("client_secret", clientSecret),
                         new KeyValuePair<string, string>("scope", scope)
                    };
                    HttpContent content = new FormUrlEncodedContent(formData);
                    HttpResponseMessage response = await client.PostAsync(tokenEndpoint, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string tokenResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<AccessToken>(tokenResponse);
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return null;
                }
            }
        }
        public async Task<string> GetSourceFolderId(string sourceFolderName,string sharePointSiteName)
        {   try
            {
                AccessToken accessTokenResponse = await GenerateAccessTokenAsync(tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress,sharePointSiteName);
                using (HttpClient client = new HttpClient())
                {
                    string siteId = await GetSiteId(sharePointSiteName);
                    string URI_SrcFolderId = Prefix_URL + "sites/" + siteId + "/drive/root/children?$filter=name eq '" + sourceFolderName + "'&$select=id";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenResponse.accessToken}");
                    HttpResponseMessage response = await client.GetAsync(URI_SrcFolderId);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(jsonResponse);
                        if (jsonObject["value"].HasValues)
                        {
                            string id = (string)jsonObject["value"][0]["id"];
                            return id;
                        }
                        else
                        {
                            return "Not Found";
                        }
                    }
                    else
                    {  string jsonResponse = "Not Found";
                        return jsonResponse;
                    }
                    }
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> GetSourceDocumentId(string sourceFolderName, string sourceDocumentName,string sharePointSiteName)
        {
            try
            {
                AccessToken accessTokenResponse = await GenerateAccessTokenAsync(tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress, sharePointSiteName);
                string siteId = await GetSiteId(sharePointSiteName);
                string SrcFolderId = await GetSourceFolderId(sourceFolderName, sharePointSiteName);
                using (HttpClient client = new HttpClient())
                {
                    string URI_SrcDocId = Prefix_URL + "sites/" + siteId + "/drive/items/" + SrcFolderId + "/children?$filter=name eq '" + sourceDocumentName + "'&$select=id";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenResponse.accessToken}");
                    HttpResponseMessage response = await client.GetAsync(URI_SrcDocId);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(jsonResponse);
                        if (jsonObject["value"].HasValues)
                        {
                            string id = (string)jsonObject["value"][0]["id"];
                            return id;
                        }
                        else
                        {
                            return "Not Found";
                        }
                    }
                    else
                    {
                        string jsonResponse = "Not Found";
                        return jsonResponse;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> GetTargetFolderId(string targetFolderName,string sharePointSiteName)
        {
            try
            {
                AccessToken accessTokenResponse = await GenerateAccessTokenAsync(tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress, sharePointSiteName);
                using (HttpClient client = new HttpClient())
                {
                    string siteId = await GetSiteId(sharePointSiteName);
                    string URI_TargetFolderId = Prefix_URL + "sites/" + siteId + "/drive/root/children?$filter=name eq '" + targetFolderName + "'&$select=id";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenResponse.accessToken}");
                    HttpResponseMessage response = await client.GetAsync(URI_TargetFolderId);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(jsonResponse);
                        if (jsonObject["value"].HasValues)
                        {
                            string id = (string)jsonObject["value"][0]["id"];
                            return id;
                        }
                        else
                        {
                            return "Not Found";
                        }
                    }
                    else
                    {
                        string jsonResponse = "Not Found";
                        return jsonResponse;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> GetSiteId(string sharePointSiteName)
        {
            try
            {
               SetSiteConfigurations(sharePointSiteName);
                AccessToken accessTokenResponse = await GenerateAccessTokenAsync(tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress, sharePointSiteName);
                using (HttpClient client = new HttpClient())
                {
                    string URI_SharePointSiteId = Prefix_URL + "sites/"+HostName+ ":/sites/" + sharePointSiteName+ "?$select=id";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenResponse.accessToken}");
                    HttpResponseMessage response = await client.GetAsync(URI_SharePointSiteId);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(jsonResponse);
                       AccessToken siteResponse = JsonConvert.DeserializeObject<AccessToken>(jsonResponse);
                        string siteId = siteResponse.Id;
                        return siteId;
                    }
                    else
                    {
                        string jsonResponse = "Not Found";
                        return jsonResponse;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Request failed with status code {ex.StatusCode}");
            }
        }
        public async Task<string> Copy(string sharePointSiteName, string sourceFolderName, string sourceDocumentName, string targetFolderName)
        {
            string siteId = await GetSiteId(sharePointSiteName);
            AccessToken accessTokenResponse = await GenerateAccessTokenAsync(tenantId, clientId, clientSecret, scope, grantType, TokenBaseAddress, sharePointSiteName);
            string SrcDocId = await GetSourceDocumentId(sourceFolderName, sourceDocumentName, sharePointSiteName);
            string targetFolderId = await GetTargetFolderId(targetFolderName, sharePointSiteName);
            using (HttpClient client = new HttpClient())
            {
                try
                {
                        string URI_CopyDoc = Prefix_URL + "sites/" + siteId + "/drive/items/" + SrcDocId + "/copy";
                        string requestBody = @"{
                          ""parentReference"": {
                            ""id"": """ + targetFolderId + @"""
                                }
                             }";
                        HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenResponse.accessToken}");
                        HttpResponseMessage response = await client.PostAsync(URI_CopyDoc, content);
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonContent = await response.Content.ReadAsStringAsync();
                            return jsonContent;
                        }
                        else
                        {
                            throw new HttpRequestException($"Request failed with status code {response}");
                        }
                }
                catch (HttpRequestException ex)
                {
                    if (ex.Message.Contains("Conflict"))
                    {
                        return "Not Found,Check the spellings and try again";
                    }
                    if (ex.Message.Contains("Not Found"))
                    {
                      return "Not Found,Check the spellings and try again" ;
                    }
                    throw new HttpRequestException($"Request failed {ex.Message}");
                }
            }

        }
    }
}
