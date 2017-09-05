using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace BookClient.Data
{
    public class BookManager
    {
        const string Url = "http://xam150.azurewebsites.net/services/books.svc";
        string authorizationKey;

        private BooksClient GetClient()
        {
            return new BooksClient(
                new BasicHttpBinding(), new EndpointAddress(Url));
        }

        public Task<bool> Login()
        {
            var tcs = new TaskCompletionSource<bool>();

            var client = GetClient();

            client.LoginCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    authorizationKey = e.Result;
                    tcs.SetResult(!string.IsNullOrEmpty(authorizationKey));
                }
            };

            client.LoginAsync();

            return tcs.Task;
        }

        private async Task AddHeader()
        {
            if (string.IsNullOrEmpty(authorizationKey))
            {
                await Login();
            }

            // Add a SOAP Header to an outgoing request
            MessageHeader header = MessageHeader.CreateHeader(
                "Authorization", "", authorizationKey);
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            // Add the HTTP header version
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers["Authorization"] = authorizationKey;
            OperationContext.Current
                .OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            // TODO: use BookClient.GetBooksAsync to retrieve books
            var tcs = new TaskCompletionSource<IEnumerable<Book>>();
            var client = GetClient();

            using (var context = new OperationContextScope(client.InnerChannel))
            {
                await AddHeader();

                client.GetBooksCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else
                    {
                        tcs.SetResult(e.Result);
                    }
                };

                client.GetBooksAsync();
            }
            return await tcs.Task;
        }

        public async Task<Book> Add(string title, string author, string genre)
        {
            var tcs = new TaskCompletionSource<Book>();
            var client = GetClient();

            using (var context = new OperationContextScope(client.InnerChannel))
            {
                await AddHeader();

                var book = new Book()
                {
                    Title = title,
                    Authors = new[] { author },
                    ISBN = string.Empty,
                    Genre = genre,
                    PublishDate = DateTime.Now.Date,
                };

                client.CreateBookCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else
                    {
                        tcs.SetResult(e.Result.Book);
                    }
                };

                client.CreateBookAsync(book);

                return await tcs.Task;
            }
        }

        public async Task Update(Book book)
        {
            // TODO: use UpdateBookAsync to update a book
            var tcs = new TaskCompletionSource<string>();
            var client = GetClient();

            using (var context = new OperationContextScope(client.InnerChannel))
            {
                await AddHeader();

                client.UpdateBookCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else
                    {
                        tcs.SetResult(e.Result.Message);
                    }
                };

                client.UpdateBookAsync(book);
            }
        }

        public async Task Delete(string isbn)
        {
            // TODO: use DeleteBookAsync to delete a book
            var tcs = new TaskCompletionSource<string>();
            var client = GetClient();

            using (new OperationContextScope(client.InnerChannel))
            {
                await AddHeader();

                client.DeleteBookCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        tcs.SetCanceled();
                    }
                    else if (e.Error != null)
                    {
                        tcs.SetException(e.Error);
                    }
                    else
                    {
                        tcs.SetResult(e.Result.Message);
                    }
                };

                client.DeleteBookAsync(isbn);
                await tcs.Task;
            }
        }
    }
}

