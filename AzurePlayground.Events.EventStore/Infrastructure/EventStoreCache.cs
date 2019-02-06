using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventStore.ClientAPI;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.EventStore
{
    public class CacheState<TKey, TCacheItem>
    {
        public CacheState()
        {
            State = new ConcurrentDictionary<TKey, TCacheItem>();
            IsStale = true;
        }

        public bool IsStale { get; set; }

        public ConcurrentDictionary<TKey, TCacheItem> State { get; }
    }

    public abstract class EventStoreCache<TKey, TCacheItem, TOutput> : IDisposable, IEventStoreCache<TKey, TCacheItem, TOutput>, ICanLog
    {
        private readonly IConnectableObservable<IConnected<IEventStoreConnection>> _connectionChanged;
        private readonly IScheduler _eventLoopScheduler = new EventLoopScheduler();
        private readonly SerialDisposable _eventsConnection = new SerialDisposable();
        private readonly SerialDisposable _eventsSubscription = new SerialDisposable();
        private readonly BehaviorSubject<CacheState<TKey, TCacheItem>> _cacheStatedUpdates = new BehaviorSubject<CacheState<TKey, TCacheItem>>(new CacheState<TKey, TCacheItem>());
        private IConnectableObservable<RecordedEvent> _events = Observable.Never<RecordedEvent>().Publish();
        private bool _isCaughtUp;
        private CompositeDisposable Disposables { get; }

        protected Dictionary<string, Type> EventTypes { get; private set; }
        protected abstract void UpdateCacheState(IDictionary<TKey, TCacheItem> currentCacheState, RecordedEvent evt);
        protected abstract bool IsValidUpdate(TOutput update);
        protected abstract TOutput CreateResponseFromCacheState(CacheState<TKey, TCacheItem> container);
        protected abstract TOutput MapSingleEventToUpdateDto(IDictionary<TKey, TCacheItem> currentCacheState, RecordedEvent evt);
        protected abstract TOutput GetDisconnectedStaleUpdate();

        public CacheState<TKey, TCacheItem> CacheState { get; } = new CacheState<TKey, TCacheItem>();

        protected EventStoreCache(IObservable<IConnected<IEventStoreConnection>> eventStoreConnectionStream)
        {

            Disposables = new CompositeDisposable(_eventsConnection, _eventsSubscription);

            _connectionChanged = eventStoreConnectionStream.ObserveOn(_eventLoopScheduler)
                                                           .Publish();

            EventTypes = AppCore.Instance.GetAll<IMutable<Guid, Trade>>().ToDictionary(ev => ev.Name, ev => ev.GetType());

            Disposables.Add(_connectionChanged.Connect());

            Disposables.Add(_connectionChanged.Subscribe(x =>
            {
                if (x.IsConnected)
                {

                    this.LogInformation("Connected to EventStore");
                    
                    Initialize(x.Value);
                }
                else
                {
                    this.LogInformation("Disconnected from EventStore");

                    if (!CacheState.IsStale)
                    {
                        CacheState.IsStale = true;
                        _cacheStatedUpdates.OnNext(CacheState);
                    }
                }
            }));
        }

        public IObservable<TOutput> GetOutputStream()
        {
            return GetOutputStreamInternal().SubscribeOn(_eventLoopScheduler)
                                        .TakeUntil(_connectionChanged.Where(x => x.IsConnected))
                                        .Repeat();
        }

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }

        protected virtual bool IsMatchingEventType(string eventType)
        {
            return EventTypes.ContainsKey(eventType);
        }

        private void Initialize(IEventStoreConnection connection)
        {

            this.LogInformation("Initializing Cache");

            CacheState.IsStale = true;
            CacheState.State.Clear();
            _isCaughtUp = false;

            _events = GetAllEvents(connection).Where(x => IsMatchingEventType(x.EventType))
                                              .SubscribeOn(_eventLoopScheduler)
                                              .Publish();

            _eventsSubscription.Disposable = _events.Subscribe(evt =>
            {
                UpdateCacheState(CacheState.State, evt);

                if (_isCaughtUp)
                {
                    _cacheStatedUpdates.OnNext(CacheState);
                }
            });
            _eventsConnection.Disposable = _events.Connect();
        }

        private IObservable<TOutput> GetOutputStreamInternal()
        {
            return Observable.Create<TOutput>(obs =>
            {

                this.LogInformation("Got stream request from client");

                var sotw = _cacheStatedUpdates
                                    .TakeUntilInclusive(x => !x.IsStale)
                                    .Select(CreateResponseFromCacheState);

                return sotw.Concat(_events.Select(evt => MapSingleEventToUpdateDto(CacheState.State, evt)))
                           .Merge(_connectionChanged.Where(x => !x.IsConnected).Select(_ => GetDisconnectedStaleUpdate()))
                           .Where(IsValidUpdate)
                           .Subscribe(obs);
            });
        }

        private IObservable<RecordedEvent> GetAllEvents(IEventStoreConnection connection)
        {
            return Observable.Create<RecordedEvent>(o =>
            {

                this.LogInformation("Getting events from EventStore");

                Task onEvent(EventStoreCatchUpSubscription _, ResolvedEvent e)
                {
                    _eventLoopScheduler.Schedule(() =>
                    {
                        o.OnNext(e.Event);
                    });

                    return Task.CompletedTask;
                }

                void onCaughtUp(EventStoreCatchUpSubscription evt)
                {
                    _eventLoopScheduler.Schedule(() =>
                    {

                        this.LogInformation("Caught up to live events. Publishing State");

                        _isCaughtUp = true;
                        CacheState.IsStale = false;
                        _cacheStatedUpdates.OnNext(CacheState);
                    });
                }

                var subscription = connection.SubscribeToAllFrom(null, CatchUpSubscriptionSettings.Default, onEvent, onCaughtUp);

                this.LogInformation($"Subscribed to EventStore.");

                return Disposable.Create(() =>
                {
                    this.LogInformation($"Stopping EventStore subscription");

                    subscription.Stop();
                });
            });
        }
    }
}