using Newtonsoft.Json;
using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SMG.DB.Helpper
{
    internal class graphqlHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public graphqlHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _endpoint = "http://localhost:5000/api/v1/";
        }

        public T ExecuteQuery<T>(string query, object variables = null)
        {
            try
            {
                var payload = new
                {
                    query,
                    variables
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Gửi request đồng bộ và chờ kết quả
                
                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJmcmVzaCI6ZmFsc2UsImlhdCI6MTczNjc4NDc0NCwianRpIjoiYTgzNmVjN2MtMDFhMi00ZmZkLWJlMTktNzFmMmUzYjdjNGI5IiwidHlwZSI6ImFjY2VzcyIsInN1YiI6IjEiLCJuYmYiOjE3MzY3ODQ3NDQsImNzcmYiOiI5NTlhOTEzZi00MWZiLTQwMzktYTEyYS1iOTUwN2MzZDIxNmIiLCJleHAiOjE3MzY3ODU2NDR9.-IGEJdOi2o9T_Ci9Md3MwCHs_Gg92Tivxn7_TQ4z7Ms";
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                HttpResponseMessage response = _httpClient.PostAsync(_endpoint, httpContent).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"GraphQL request failed: {response.StatusCode}");
                }

                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var result = JsonConvert.DeserializeObject<GraphQLResponse<T>>(responseContent);

                if (result.Errors != null && result.Errors.Length > 0)
                {
                    throw new Exception($"GraphQL error: {result.Errors[0].Message}");
                }

                return result.Data;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                return default(T);
            }
        }
    }

    public class GraphQLResponse<T>
    {
        public T Data { get; set; }
        public GraphQLError[] Errors { get; set; }
    }

    public class GraphQLError
    {
        public string Message { get; set; }
    }
}
