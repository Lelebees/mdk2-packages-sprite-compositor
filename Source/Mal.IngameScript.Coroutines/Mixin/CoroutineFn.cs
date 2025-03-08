using System.Collections.Generic;

namespace IngameScript
{
    public delegate IEnumerator<When> CoroutineFn(CancellationToken token);
}