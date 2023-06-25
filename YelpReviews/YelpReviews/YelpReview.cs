using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1;
public class YelpReview
{
    private static readonly HttpClient client = new HttpClient();

    private static readonly string apiKey =
        "nCKMcyMrujldcH__niHKHSWpkgmKVWzNplar6BZhrhKdr4osjRWTpj5xrBdAjd782QyO7t3wiWmY4mk2kJsEyr92C_Lba5Z0P-_Wstf9NupkSmqplIohJxyln-mWZHYx";
    public YelpReview()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }
    public async Task<List<YelpPlace>> Execute(string location, int limit)
    {
        List<YelpPlace> places = new List<YelpPlace>();
        var businessUri = $"https://api.yelp.com/v3/businesses/search?location={location}&sort_by=best_match&limit={limit}";
        var body = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri(businessUri)));
        var json = JObject.Parse(await body.Content.ReadAsStringAsync());
        var businesses = json["businesses"];
        if(businesses == null)
        {
            return new List<YelpPlace>();
        }
        foreach (var business in businesses)
        {
            string id = business["id"].ToString();
            var reviewsUri = new Uri($"https://api.yelp.com/v3/businesses/{id}/reviews?limit=3&sort_by=yelp_sort");
            body = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, reviewsUri));
            json = JObject.Parse(await body.Content.ReadAsStringAsync());
            var reviewsBody = json["reviews"];
            YelpPlace place = new YelpPlace(business["name"].ToString());
            foreach (var text in reviewsBody)
            {
                place.AddReview(text["text"].ToString());
            }
            places.Add(place);
        }

        return places;
    }
}