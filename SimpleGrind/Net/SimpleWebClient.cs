using SimpleGrind.Parameters;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrind.Net
{
    public interface ISimpleWebClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostJsonAsync(string url, string json);
        Task<HttpResponseMessage> PutJsonAsync(string url, string json);
        Task<HttpResponseMessage> DeleteAsync(string url);

        HttpResponseMessage Get(string url);
        HttpResponseMessage PostJson(string url, string json);
        HttpResponseMessage PutJson(string url, string json);
        HttpResponseMessage Delete(string url);

    }

    public class SimpleWebClient : ISimpleWebClient
    {
		readonly HttpClient _httpClient;

        public SimpleWebClient(IRequestParameters requestParameters)
        {
       	    var cookieContainer = new CookieContainer();
			foreach (var cookie in requestParameters.Cookies)
				cookieContainer.Add(new Cookie(cookie.Key,cookie.Value));
				
			_httpClient = new HttpClient(new HttpClientHandler
			{
				AllowAutoRedirect = false,
				CookieContainer = cookieContainer
			})
			{
				Timeout = TimeSpan.FromSeconds(requestParameters.TimeOut)
			};

			foreach (var header in requestParameters.Headers)
				_httpClient.DefaultRequestHeaders.Add(header.Key,header.Value);
	    }

        StringContent Content(string json) => new StringContent(json, Encoding.UTF8, "application/json");

        public async Task<HttpResponseMessage> GetAsync(string url) => await _httpClient.GetAsync(url);
        public async Task<HttpResponseMessage> PostJsonAsync(string url, string json) => await _httpClient.PostAsync(url, Content(json));
		public async Task<HttpResponseMessage> PutJsonAsync(string url, string json) => await _httpClient.PutAsync(url, Content(json));
		public async Task<HttpResponseMessage> DeleteAsync(string url) => await _httpClient.DeleteAsync(url);

        public HttpResponseMessage Get(string url) => GetAsync(url).Result;
        public HttpResponseMessage PostJson(string url, string json) => PostJsonAsync(url, json).Result;
        public HttpResponseMessage PutJson(string url, string json) => PutJsonAsync(url, json).Result;
        public HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;
	}
}