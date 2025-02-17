using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace HangfireJob
{
    public class CatFactUpsertJob : ICatFactUpsertJob
    {
        private readonly ICatFactRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public CatFactUpsertJob(ICatFactRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _httpClient = new HttpClient();
            _apiUrl = configuration["MeowFactsApiUrl"] ?? throw new Exception("MeowFacts API URL is missing!");
        }

        public async Task Execute()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            var factJson = JsonSerializer.Deserialize<MeowFactResponse>(response);

            if (factJson?.Data.Length > 0)
            {
                var newFact = new CatFact(factJson.Data[0]);

                if (!await _repository.ExistsAsync(newFact.Fact))
                {
                    await _repository.AddAsync(newFact);
                }
            }
        }
    }
}
