using Microsoft.AspNetCore.SignalR;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace SignalRStreamTestApp
{
    public class TestHub: Hub
    {
        public IAsyncEnumerable<double> TestStrteam(CancellationToken cancellationToken)
        {
            List<PriceInfo> priceInfos = new List<PriceInfo>()
            {
                new PriceInfo{Symbol="APPL", Price = 2300},
                new PriceInfo{Symbol="APPL", Price = 2310},
                new PriceInfo{Symbol="APPL", Price = 2330},
                new PriceInfo{Symbol="APPL", Price = 2100},
                new PriceInfo{Symbol="APPL", Price = 2200},
                new PriceInfo{Symbol="APPL", Price = 2360},
                new PriceInfo{Symbol="APPL", Price = 2320},
                new PriceInfo{Symbol="APPL", Price = 2300},
            };


            PriceService priceService = new PriceService();

            priceService.PriceChange.ToAsyncEnumerable();

            priceService.Start();

            return priceService.PriceChange.StartWith(priceInfos).Select(p=>p.Price).ToAsyncEnumerable();           

        }

        private async Task WriteItems(ChannelWriter<string> writer, int delay, CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var currentDate = DateTime.Now;
                await writer.WriteAsync($"Test message streamed from server at {currentDate.ToLongDateString()} {currentDate.ToLongTimeString()}", cancellationToken);
                await Task.Delay(delay, cancellationToken);
            }

            writer.TryComplete();
        }

        
    }

    public class PriceService
    {
        private Subject<PriceInfo> priceServiceSubject;

        public PriceService()
        {
            priceServiceSubject = new Subject<PriceInfo>();
        }

        public void Start()
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    priceServiceSubject.OnNext(new PriceInfo() { Symbol = "APPL", Price = new Random().Next(2000, 5000) });
                    Thread.Sleep(1000);
                }

                priceServiceSubject.OnCompleted();
            });
        }

        public IObservable<PriceInfo> PriceChange => priceServiceSubject.AsObservable();
    }

    public class PriceInfo
    {
        public string Symbol { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Symbol}: ${Price}";
        }
    }
}
