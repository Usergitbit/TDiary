using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Api.Exceptions
{
    public class DuplicateIdException : Exception
    {

        public DuplicateIdException(Guid id, string entity) : base($"An {entity} with the id {id} already exists.")
        {

        }
    }
}
