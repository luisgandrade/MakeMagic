using MakeMagic.MakeMagicApi.Models;
using MakeMagic.Utils;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakeMagic.MakeMagicApi
{
    /// <summary>
    /// Cliente para acesso a Potter API 
    /// </summary>
    public class MakeMagicApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MakeMagicApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        //para testes
        protected MakeMagicApiClient()
        {

        }

        private TParsingType TryParseResponse<TParsingType>(string response)
        {
            TParsingType apiErrorObject = default;
            try 
            {
                apiErrorObject = JsonConvert.DeserializeObject<TParsingType>(response, new JsonSerializerSettings
                {
                    //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            //se não conseguir parsear, deixamos passar. A ideia é que o caller trate a questão de não ter desserializado corretamente.
            catch(Newtonsoft.Json.JsonException){ }

            return apiErrorObject;
        }

        /// <summary>
        /// Busca registro da casa <paramref name="houseId"/> na API.
        /// </summary>
        public virtual async Task<Result<HouseModel>> GetHouse(string houseId)
        {
            if (string.IsNullOrWhiteSpace(houseId))
                throw new ArgumentNullException(nameof(houseId));

            using (var response = await _httpClient.GetAsync($"v1/houses/{houseId}?key={_apiKey}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseContents = await response.Content.ReadAsStringAsync();
                    var responseAsErrorObject = TryParseResponse<ErrorModel>(responseContents); //vai conseguir desserializar se houseId for um valor com formato diferente do esperado
                    if (responseAsErrorObject != null)
                        return Result<HouseModel>.Failed(ErrorLevel.UnrecoverableError, "Os dados informados não estavam num formato esperado");
                    var houses = TryParseResponse<HouseModel[]>(responseContents);
                    if (houses == null)
                        return Result<HouseModel>.Failed(ErrorLevel.UnrecoverableError, "Resposta ineseperada do servidor.");
                    return Result<HouseModel>.Success(houses.SingleOrDefault());
                }
                else 
                    return Result<HouseModel>.Failed(ErrorLevel.UnrecoverableError, "Erro de conexão com a API.");
            }
        }
    }
}
