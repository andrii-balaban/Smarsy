namespace Smarsy.Logic
{
    using System.Collections.Generic;

    public interface IDatabaseLogic
    {
        void UpsertLessons(List<string> lessons);
    }
}
