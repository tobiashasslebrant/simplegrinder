using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrind.Net
{
	public class SimpleWebClient
	{
		readonly HttpClient _httpClient;

		public SimpleWebClient(int timeOut, Dictionary<string, string> defaultRequestHeaders, Dictionary<string, string> cookies)
		{

			var cookieContainer = new CookieContainer();
			if(cookies != null)
				foreach (var cookie in cookies)
					cookieContainer.Add(new Cookie(cookie.Key,cookie.Value));
				
			_httpClient = new HttpClient(new HttpClientHandler
			{
				AllowAutoRedirect = false,
				CookieContainer = cookieContainer
			})
			{
				Timeout = TimeSpan.FromSeconds(timeOut)
			};

			if(defaultRequestHeaders != null)
				foreach (var defaultRequestHeader in defaultRequestHeaders)
					_httpClient.DefaultRequestHeaders.Add(defaultRequestHeader.Key,defaultRequestHeader.Value);
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