using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class Object
    {
        public Zone Zone;

        public Server Server => Zone.Server;

        public event Action OnStart;

        public event Action OnDestroyed;

        public event Action OnTick;

        public static Object Instantiate(Type type, Zone zone)
        {
            if (Activator.CreateInstance(type) is Object instance)
            {
                instance.Zone = zone;

                zone.RegisterObject(instance);

                return instance;
            }

            Logger.Error($"{type.FullName} does not inherit from Object but is being Created as one.");
            return null;
        }

        public static T Instantiate<T>(Zone zone) where T : Object
        {
            return Instantiate(typeof(T), zone) as T;
        }

        public static void Start(Object obj)
        {
            obj.OnStart?.Invoke();
        }

        public static void Destroy(Object obj)
        {
            obj.Zone.UnRegisterObject(obj);

            obj.OnDestroyed?.Invoke();
        }

        public static void Destroy(Object obj, float time)
        {
            Task.Run(async () =>
            {
                Logger.Debug($"Destroying {obj} in {(int) (time * 1000)} ms");

                await Task.Delay((int) (time * 1000));

                Logger.Debug($"Destroying {obj}");

                obj.Zone.UnRegisterObject(obj);

                obj.OnDestroyed?.Invoke();
            });
        }

        public static void Update(Object obj)
        {
            obj.OnTick?.Invoke();
        }
    }
}