using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.IndexedDB.SchemaBuilder
{
    public abstract class AbstractSchemaBuilder<T>
    {
        protected Dictionary<string, IndexSpec> properties = new Dictionary<string, IndexSpec>();
        protected IndexSpec primaryKey = new() { Name = "id", KeyPath = "id", Auto = false };
        protected string storeName;

        protected void AddBasePropertiesInternal()
        {
            properties["createdAt"] = new IndexSpec { Name = "createdAt", KeyPath = "createdAt" };
            properties["createdAtUtc"] = new IndexSpec { Name = "createdAtUtc", KeyPath = "createdAtUtc" };
            properties["modifiedtAt"] = new IndexSpec { Name = "modifiedtAt", KeyPath = "modifiedtAt" };
            properties["modifiedAtUtc"] = new IndexSpec { Name = "modifiedAtUtc", KeyPath = "modifiedAtUtc" };
            properties["timeZone"] = new IndexSpec { Name = "timeZone", KeyPath = "timeZone" };
            properties["userId"] = new IndexSpec { Name = "userId", KeyPath = "userId" };
        }
    }
}
