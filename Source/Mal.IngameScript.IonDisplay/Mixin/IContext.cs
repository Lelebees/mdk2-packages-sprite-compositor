using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IContext
    {
        IMyTextSurface Surface { get; }
        T Lease<T>() where T : class, new();
    }
}