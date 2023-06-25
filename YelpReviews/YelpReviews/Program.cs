
using System.Net.Http.Headers;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ConsoleApp1;
using Newtonsoft.Json.Linq;
using VaderSharp2;

SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();

while (true)
{
    Console.WriteLine("while(true) Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
    Console.WriteLine($"Unesite lokaciju");
    string place = Console.ReadLine();
    Console.WriteLine($"Unesite limit");
    int limit = Int32.Parse(Console.ReadLine());

    var source = Observable.Create<YelpPlace>(async (observer) =>
    {
        Console.WriteLine("observable method Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
        var places = await new YelpReview().Execute(place, limit);
        foreach (var place in places)
        {
            observer.OnNext(place);
        }

        observer.OnCompleted();
        return Disposable.Empty;
    }).SubscribeOn(Scheduler.Default);
    
    IConnectableObservable<YelpPlace> connectable = Observable.Publish(source);

    connectable.ObserveOn(Scheduler.Default).Subscribe(yelpPlace =>
    {
        Console.WriteLine("subscriber Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
        int positives = 0, negatives = 0;
        foreach (var review in yelpPlace.Reviews)
        {
            var result = analyzer.PolarityScores(review);
            if (result.Positive > result.Negative) positives++;
            else negatives++;
        }
        Console.WriteLine($"Za mesto {yelpPlace.PlaceName} ima pozitivnih : {positives} i negativnih {negatives}");
    });
    
    connectable.Connect();
    await connectable.LastAsync();
    Console.WriteLine("Await je gotov");
}





