using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryBackOffice.Models
{
    public class PageResults<T>
    {
        public List<T> Items {get; set;} = new();
        public int Page {get; set;}
        public int PageSize {get; set;}
        public int TotalItems {get; set;}

        public int TotalPages => (int)Math.Ceiling((double)TotalItems/PageSize);
    }
}