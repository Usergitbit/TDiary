using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDiary.Api.Exceptions;

namespace TDiary.Api.Protos
{
    public partial class ErrorInfo
    {
        public ErrorInfo(Exception ex) : base()
        {
            ErrorType = ErrorType.Internal;
            Reason = $"{ex.Message}\n{ex.StackTrace}";
        }

        public ErrorInfo(DuplicateIdException ex) : base()
        {
            ErrorType = ErrorType.BadRequest;
            Reason = ex.Message;
        }

        public ErrorInfo(IReadOnlyDictionary<string, IEnumerable<string>> propertyValidationFailures) : base()
        {
            ErrorType = ErrorType.Validation;
            var stringBuilder = new StringBuilder();
            foreach(var key in propertyValidationFailures)
            {
                stringBuilder.AppendLine($"Property {key.Key} invalid. Reasons:");
                foreach (var failure in propertyValidationFailures[key.Key])
                    stringBuilder.AppendLine($"\t{failure}");
            }
            Reason = stringBuilder.ToString();
        }
    }
}

