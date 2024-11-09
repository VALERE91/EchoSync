using EchoSync;

namespace CommonGameplayCode;

public class World : ITickable
{
    private readonly List<IWorldObject> _objects = [];
    
    public T SpawnObject<T>() where T: IWorldObject, new()
    {
        // Spawn the object
        var obj = new T();
        _objects.Add(obj);
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