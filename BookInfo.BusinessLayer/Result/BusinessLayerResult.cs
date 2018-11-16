using BookInfo.Entities.Messages;
using System.Collections.Generic;

namespace BookInfo.BusinessLayer.Result
{
    public class BusinessLayerResult<T> where T : class
    {
        public List<ErrorMessageObj> Errors { get; set; }

        public T Result { get; set; }

        public BusinessLayerResult()
        {
            Errors = new List<ErrorMessageObj>();
        }
        public void AddError(int code, string message)
        {
            Errors.Add(new ErrorMessageObj()
            {
                Code = code,
                Message = message
            });
        }

    }
}
