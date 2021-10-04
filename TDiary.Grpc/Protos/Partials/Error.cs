using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Grpc.Protos
{
    public partial class Error
    {
        public Error(EventData eventData, IReadOnlyDictionary<string, IEnumerable<string>> propertyValidationErrors) : base()
        {
            EventId = eventData.Id;
            EntityId = eventData.EntityId;
            Entity = eventData.Entity;
            ErrorType = ErrorType.Validation;
            Reason = "Validation faliure, check validation_errors for detailed information.";
            foreach (var keyValuePair in propertyValidationErrors)
            {
                var validationError = new ValidationError(keyValuePair.Key, propertyValidationErrors[keyValuePair.Key]);
                ValidationErrors.Add(validationError);
            }
        }
    }
}
