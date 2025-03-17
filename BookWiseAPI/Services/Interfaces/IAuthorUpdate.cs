using BookWiseAPI.Model;
using DataLibrary;

namespace BookWiseAPI.Services.Interfaces
{
    public interface IAuthorUpdate
    {
        Task<Author> Update(string findAuthor, AuthorDto authorDto);
    }
}
