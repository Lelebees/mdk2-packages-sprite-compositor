namespace IngameScript
{
    internal partial class Program
    {
        Coroutines _coroutines;
        public Coroutines Coroutines => _coroutines ?? (_coroutines = new Coroutines(this));
    }
}