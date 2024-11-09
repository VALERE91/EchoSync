using EchoSync;

namespace CommonGameplayCode;

public class Engine(int frameRate)
{
    private readonly List<ITickable> _tickables = new List<ITickable>();

    public bool ShouldStop { get; set; } = false;

    public void AddTickable(ITickable tickable)
    {
        // Add the tickable to the list of tickables
        _tickables.Add(tickable);
    }
    
    public void RemoveTickable(ITickable tickable)
    {
        // Remove the tickable from the list of tickables
        _tickables.Remove(tickable);
    }
    
    private void Tick(float deltaTime)
    {
        // Tick all tickables
        foreach (var tickable in _tickables)
        {
            tickable.Tick(deltaTime);
        }
    }
    
    public void Run()
    {
        float framePeriod = 1f / frameRate;
        
        while (!ShouldStop)
        {
            var beginFrame = DateTime.Now;
            Tick(framePeriod);
            var endFrame = DateTime.Now;
            
            var frameDuration = (endFrame - beginFrame).TotalMilliseconds;
            if (frameDuration < framePeriod * 1000f)
            {
                Thread.Sleep((int)(framePeriod * 1000 - frameDuration));
            }
        }
    }
}