using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OrderingCoreAPIClient
{
    public class ApiClientBase
    {
        protected readonly HttpClient _httpClient;
        protected string BaseEndpoint { get; set; }

        public ApiClientBase(string baseEndpoint)
        {
            BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException("baseEndpoint");

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseEndpoint);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<T> CreateAsync<T>(string relativePath, T item)
        {
            T local = default(T);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                relativePath, item);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                local = await response.Content.ReadAsAsync<T>();
            }
            return local;      
        }

        protected async Task<T> GetAsync<T>(string relativePath, Guid id)
        {
            T item = default(T);
            HttpResponseMessage response = 
                await _httpClient.GetAsync($"{relativePath}/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                item = await response.Content.ReadAsAsync<T>();
            }
            return item;
        }

        protected async Task<List<T>> GetAllAsync<T>(string relativePath)
        {
            List<T> items = null;
            HttpResponseMessage response = await _httpClient.GetAsync(relativePath);
            if (response.IsSuccessStatusCode)
            {
                items = await response.Content.ReadAsAsync<List<T>>();
            }
            return items;
        }

        protected async Task<T> UpdateAsync<T>(string relativePath, Guid id, T item)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
                $"{relativePath}/{id.ToString()}", item);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>();
        }

        protected async Task<HttpStatusCode> DeleteAsync(string relativePath, Guid id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(
                $"{relativePath}/{id.ToString()}");
            return response.StatusCode;
        }
    }
}
