using API.Helper;
using System.Text.Json;

namespace API.Extensions
{
    public static class HttpExtensions
    {

        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header) 
        { 
            //set all the options for the json
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy= JsonNamingPolicy.CamelCase };

            //convert from object to json so it can go into the header
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));

            //due to CORS policy need to explicitly expose the custom header to the browser
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }


    }
}
