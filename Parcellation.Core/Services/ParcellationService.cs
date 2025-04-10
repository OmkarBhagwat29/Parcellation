

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UrbanDesign.Core.Dtos;

namespace UrbanDesign.Core.Services
{
    public class ParcellationService
    {
        public string BaseAddress { get; private set; } = "http://localhost:5187";
        HttpClient _client = new HttpClient();

        public ParcellationService()
        {
            _client.BaseAddress = new Uri(BaseAddress);
        }

        public async Task<HttpResponseMessage> CreateParcellationAsync(CreateParcellationDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            var obj = new
            {
                boundary = dto.Boundary,
                roadNetwork = dto.RoadNetwork,
                majorRoadWidth = dto.MajorRoadWidth,
                minorRoadWidth = dto.MinorRoadWidth,
                caller = dto.caller.ToString()
            };
            var response = await _client.PostAsJsonAsync("/parcellation", obj);

            return response;

        }

        public async Task DeleteAllParcellationInputData()
        {
            var res = await _client.DeleteAsync("/parcellation/all");
        }

        public async Task GetAllParcellationInputsAsync()
        {
            var res = await _client.GetAsync("/parcellation/inputs/all");

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var stream = await res.Content.ReadAsStreamAsync();

                var data = await JsonSerializer.DeserializeAsync<GetAllParcellationInputDto[]>(stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }


    }
}
