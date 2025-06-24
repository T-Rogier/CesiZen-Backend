using CesiZen_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen_Backend.Tests.Helpers
{
    public class Builder
    {
        public static Activity BuildActivity(
            int id,
            string title,
            TimeSpan duration,
            bool activated,
            bool deleted,
            ActivityType type,
            params string[] categories)
        {
            var user = User.Create($"user{id}", $"u{id}@ex.com", "pwdpwd", UserRole.User);
            SetPrivateId(user, id * 10);
            var cats = categories
                .Select(n => { var c = Category.Create(n, "TestIcon"); return c; })
                .ToList();

            var act = Activity.Create(
                title,
                "content",
                "desc",
                "img",
                duration,
                user,
                cats,
                type,
                activated
            );
            if(deleted) {
                act.Delete();
            }
            SetPrivateId(act, id);
            return act;
        }

        static void SetPrivateId<T>(T entity, int id)
        {
            var prop = typeof(T).GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            prop.SetValue(entity, id);
        }
    }
}
