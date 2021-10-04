using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDiary.Common.Exceptions;

namespace TDiary.Grpc.Protos
{
    public partial class ErrorInformation
    {
        public ErrorInformation(Exception ex) : base()
        {
            var error = new Error
            {
                ErrorType = ErrorType.Internal,
                Reason = $"{ex.Message}\n{ex.StackTrace}"
            };
            Errors.Add(error);
        }

        public ErrorInformation(DuplicateIdException ex) : base()
        {
            var error = new Error
            {
                ErrorType = ErrorType.DuplicateId,
                Reason = ex.Message
            };

            Errors.Add(error);
        }

        public ErrorInformation(IEnumerable<Error> errors) : base()
        {
            Errors.Add(errors);
        }

        public ErrorInformation(Error error) : base()
        {
            Errors.Add(error);
        }

    }
}

