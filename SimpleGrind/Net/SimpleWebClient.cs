using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrind.Net
{
	public class SimpleWebClient
	{
		readonly HttpClient _httpClient;

		public SimpleWebClient(int timeOut)
		{
			_httpClient = new HttpClient(new HttpClientHandler
			{
				AllowAutoRedirect = false
			})
			{
				Timeout = TimeSpan.FromSeconds(timeOut),
			};
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