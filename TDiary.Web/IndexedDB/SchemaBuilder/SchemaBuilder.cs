using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace TDiary.Web.IndexedDB.SchemaBuilder
{
    public class SchemaBuilder<T> : AbstractSchemaBuilder<T>
    {

        public SchemaBuilder()
        {
        }

        public StoreSchema Build()
        {
            return new StoreSchema
            {
                Name = storeName,
                PrimaryKey = primaryKey,
                Indexes = properties.Select(x => x.Value).ToList()
            };
        }
        public SchemaBuilder<T> BaseProperties()
        {
            AddBasePropertiesInternal();

            return this;
        }

        public SchemaBuilder<T> StoreName(string name)
        {
            storeName = name;

            return this;
        }

        public SchemaBuilder<T> PrimaryKey(string name, string keyPath = null, bool auto = false)
        {
            keyPath = keyPath ?? name;

            primaryKey = new()
            {
                Name = name,
                KeyPath = keyPath,
                Auto = auto
            };

            return this;
        }

        public SchemaBuilder<T> Property(string name, string keyPath = null, bool auto = false, bool unique = false)
        {
            keyPath = keyPath ?? null;
            properties[name] = new IndexSpec
            {
                Name = name,
                KeyPath = keyPath,
                Auto = auto,
                Unique = unique
            };

            return this;
        }
    }
}
