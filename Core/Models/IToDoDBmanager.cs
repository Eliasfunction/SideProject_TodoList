using System.Collections.Generic;

namespace Core.Models
{
    public interface IToDoDBmanager
    {
        List<Thing> GetThing(string UserName);
        bool NewThing(Thing thing, string Token);
        public bool ChangeThing(Thing thing, string Token);
        public bool Recycle(RecycleThing todoId, string Token);
    }
}