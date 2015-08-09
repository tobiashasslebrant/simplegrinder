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
        HttpResponseMessage Delete(string url);
        Task<HttpResponseMessage> DeleteAsync(string url);
        HttpResponseMessage Get(string url);
        Task<HttpResponseMessage> GetAsync(string url);
        HttpResponseMessage PostJson(string url, string json);
        Task<HttpResponseMessage> PostJsonAsync(string url, string json);
        HttpResponseMessage PutJson(string url, string json);
        Task<HttpResponseMessage> PutJsonAsync(string url, string json);
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

		public async Task<HttpResponseMessage> GetAsync(string url)
		{
			return await _httpClient.GetAsync(url);
		}
		
		public HttpResponseMessage Get(string url)
		{
			return GetAsync(url).Result;
		}

		public async Task<HttpResponseMessage> PostJsonAsync(string url, string json)
		{
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			return await _httpClient.PostAsync(url, content);
		}
		
		public HttpResponseMessage PostJson(string url, string json)
		{
			return PostJsonAsync(url, json).Result;
		}

		public async Task<HttpResponseMessage> PutJsonAsync(string url, string json)
		{
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			return await _httpClient.PutAsync(url, content);
		}

		public HttpResponseMessage PutJson(string url, string json)
		{
			return PutJsonAsync(url, json).Result;
		}

		public async Task<HttpResponseMessage> DeleteAsync(string url)
		{
			return await _httpClient.DeleteAsync(url);
		}

		public HttpResponseMessage Delete(string url)
		{
			return DeleteAsync(url).Result;
		}
	}
}