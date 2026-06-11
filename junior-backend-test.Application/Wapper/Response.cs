using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Wapper
{
    public interface IResponse
    {
        bool IsSucceeded { get; }
    }

    public class Response<T> : IResponse
    {
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSucceeded { get; set; }
        public DateTime Timestamp { get; set; }

        public Response()
        {
            Timestamp = DateTime.Now;
        }

        public static Response<T> Success(T data, string message = "Success")
        {
            return new Response<T>
            {
                Data = data,
                Message = message,
                IsSucceeded = true
            };
        }
        public static Response<T> Fail(string message)
        {
            return new Response<T>
            {
                Message = message,
                IsSucceeded = false
            };
        }
    }
}
