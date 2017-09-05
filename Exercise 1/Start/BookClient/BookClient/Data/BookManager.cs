using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookClient.Data
{
    public class BookManager
    {
        public Task<IEnumerable<Book>> GetAll()
        {
            // TODO: use BookClient.GetBooksAsync to retrieve books
            throw new NotImplementedException();
        }

        public Task<Book> Add(string title, string author, string genre)
        {
            // TODO: use BookClient.CreateBook to add a new book
            throw new NotImplementedException();
        }

        public Task Update(Book book)
        {
            // TODO: use UpdateBookAsync to update a book
            throw new NotImplementedException();
        }

        public Task Delete(string isbn)
        {
            // TODO: use DeleteBookAsync to delete a book
            throw new NotImplementedException();
        }
    }
}

