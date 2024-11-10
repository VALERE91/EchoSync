using EchoSync;

namespace CommonGameplayCode;

public interface IWorld : ITickable
{
    T? SpawnObject<T>(params object[] constructorParams) where T: IWorldObject;
    void UnspawnObject(IWorldObject obj);
}

public class World : IWorld
{
    private readonly List<IWorldObject> _objects = [];
    
    public T? SpawnObject<T>(params object[] constructorParams) where T: IWorldObject
    {
        // Spawn the object
        var obj = (T?)Activator.CreateInstance(typeof(T), constructorParams);
        if (obj == null) return obj;
        _objects.Add(obj);
        obj.Start();
        return obj;
    }
    
    public void UnspawnObject(IWorldObject obj)
    {
        // Unspawn the object
        _objects.Remove(obj);
    }

    public void Tick(float deltaTimeSeconds)
    {
        // Tick all objects
        foreach (var obj in _objects)
        {
            obj.Tick(deltaTimeSeconds);
        }
    }
}