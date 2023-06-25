namespace ConsoleApp1;

public class YelpPlace
{
    private string placeName;
    private List<string> reviews = new List<string>();

    public List<string> Reviews
    {
        get { return reviews; }
    }

    public string PlaceName
    {
        get { return placeName; }
    }

    public YelpPlace(string placeName)
    {
        this.placeName = placeName;
    }

    public void AddReview(string review)
    {
        reviews.Add(review);
    }
}