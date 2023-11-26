using Enums.ACO;
using Chirp.Models;
namespace Chirp.Interfaces;

public interface ILikeDisRepository
{
    public Task<CheepLikeDis?> GetCheepLikeDis(int CheepId);
    public Task<AuthorCheepOpinion> GetAuthorCheepOpinion(int CheepId, string AuthorName);
    public CheepLikeDis CreateLikeDisSchema(Cheep cheep);
}