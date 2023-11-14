using Chirp.Models;
using Chirp.ADTO;

namespace Chirp.Interfaces;

public interface IAuthorRepository
{
    public void CreateNewAuthor(AuthorDTO authorDTO);
    public Task<Author> GetAuthorByName(string name); 

}