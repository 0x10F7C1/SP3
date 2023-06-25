
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
    Console.WriteLine("------------------------");
    Console.WriteLine("while(true) Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
    Console.WriteLine($"Unesite lokaciju");
    string place = Console.ReadLine();
    Console.WriteLine($"Unesite limit");
    int limit = Int32.Parse(Console.ReadLine());
    Console.WriteLine("------------------------");

    var source = Observable.Create<YelpPlace>(async (observer) =>
    {
        try
        {
            Console.WriteLine("observable method Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
            var places = await new YelpReview().Execute(place, limit);
            if (places.Count == 0)
            {
                Console.WriteLine("Ovo mesto nije podrzano");
            }
            else
            {
                foreach (var place in places)
                {
                    observer.OnNext(place);
                }
                
            }
            observer.OnCompleted();
        }
        catch(Exception ex)
        {
            observer.OnError(ex);
        }
        return Disposable.Empty;
    }).SubscribeOn(Scheduler.Default);
    
    IConnectableObservable<YelpPlace> connectable = Observable.Publish(source);

    var SubscriberP = connectable.ObserveOn(Scheduler.CurrentThread).Subscribe(yelpPlace =>
    {
        //Console.WriteLine("subscriber Positive Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
        int positives = 0;
        foreach (var review in yelpPlace.Reviews)
        {
            var result = analyzer.PolarityScores(review);
            if (result.Positive > result.Negative) positives++;
        }
        Console.WriteLine($"Za mesto {yelpPlace.PlaceName} ima pozitivnih : {positives}");
    });

    var SubscriberN = connectable.ObserveOn(Scheduler.CurrentThread).Subscribe(yelpPlace =>
    {
        //Console.WriteLine("subscriber Positive Executing on the thread " + Thread.CurrentThread.ManagedThreadId);
        int negatives = 0;
        foreach(var review in yelpPlace.Reviews)
        {
            var result = analyzer.PolarityScores(review);
            if(result.Negative > result.Positive) negatives++;
        }
        Console.WriteLine($"Za mesto {yelpPlace.PlaceName} ima negativnih : {negatives}");
    });

    connectable.Connect();
    await connectable.LastOrDefaultAsync();
    SubscriberN.Dispose();
    SubscriberP.Dispose();
}





