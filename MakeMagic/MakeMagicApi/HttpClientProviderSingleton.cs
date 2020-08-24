using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MakeMagic.MakeMagicApi
{
    public class HttpClientProviderSingleton
    {
        /// <summary>
        /// Dicionário estático que contém todos os <see cref="HttpClient"/> indexados por uma <see cref="Uri"/> para o qual esse cliente fará requisições.
        /// Como estamos usando um <see cref="ConcurrentDictionary{TKey, TValue}"/>, essa instância é thread-safe.
        /// </summary>
        /// <remarks>Mantemos registros de <see cref="HttpClient"/> abertos por URI pois cada <see cref="HttpClient"/> faz bind a uma porta TCP. </remarks>
        private static readonly ConcurrentDictionary<Uri, HttpClient> _existingHttpClientForUris = new ConcurrentDictionary<Uri, HttpClient>();

        public static HttpClient GetHttpClientForBaseUri(Uri baseUri)
        {
            HttpClient httpClient = null;
            if (!_existingHttpClientForUris.TryGetValue(baseUri, out httpClient))
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = baseUri;
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                _existingHttpClientForUris.TryAdd(baseUri, httpClient);
            }

            return httpClient;

        }
    }
}
