using System.Collections.Generic;

namespace Core.Models
{
    public interface IToDoDBmanager
    {
        List<Thing> GetThing(string Token, bool Visibility);
        bool NewThing(Thing thing, string Token);
        bool ChangeThing(Thing thing, string Token);
        bool Recycle(RecycleThing todoId, string Token, bool Visibility);
        bool Delete(RecycleThing todoId, string Token);
    }
}