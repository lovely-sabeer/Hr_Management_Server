using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.ResponseModal
{
    using System.Net;
    public class JsonResultVm<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Title { get; set; }
       // public int? TotalCount { get; set; }
        public string? ErrorReference { get; set; }

        public static JsonResultVm<T> SuccessResponse(string title, T data, int totalCount, string? message = "Successful")
        {
            return new JsonResultVm<T>
            {
                Success = true,
                Data = data,
                StatusCode = HttpStatusCode.OK,
                Message = message,
          //      TotalCount = totalCount,
                Title = title
            };
        }

        public static JsonResultVm<T> FailResponse(string title, string message, T? data = default(T))
        {
            return new JsonResultVm<T>
            {
                Success = false,
                Data = data,
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message,
                Title = title
            };
        }
    }
        public class CommonPaginatedResponse<TData>
        {
            // public List<THeader> Headers { get; set; } = new();
            public List<TData> Data { get; set; } = new();
            public int TotalCount { get; set; }
            public int? PageNumber { get; set; }
            public int? PageLimit { get; set; }
            public int TotalPages { get; set; }
            public int? NextPage { get; set; }
            public int? PreviousPage { get; set; }
            public Dictionary<string, int>? Summary { get; set; }

            public CommonPaginatedResponse(List<TData> rows, int count, int? pageNumber, int? pageLimit)
            {
                //Headers = headers;
                Data = rows;
                TotalCount = count;
                PageNumber = pageNumber;
                PageLimit = pageLimit;
                TotalPages = (int)Math.Ceiling(count / (double)(pageLimit ?? count));
                var currentPage = pageNumber ?? 1;

                NextPage = (currentPage * pageLimit) < TotalCount ? currentPage + 1 : null;
                PreviousPage = currentPage > 1 ? currentPage - 1 : null;
            }
        }
    }
