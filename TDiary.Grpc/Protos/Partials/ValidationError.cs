using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Grpc.Protos
{
    public partial class ValidationError
    {
        public ValidationError(string property, IEnumerable<string> reasons):base()
        {
            Property = property;
            Reasons.Add(reasons);
        }
    }
}
