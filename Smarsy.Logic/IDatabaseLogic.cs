using System.Collections.Generic;

namespace Smarsy.Logic
{
    public interface IDatabaseLogic
    {
        void UpsertLessons(List<string> lessons);
    }
}
