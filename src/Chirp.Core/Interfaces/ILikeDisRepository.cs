using Enums.ACO;
using Chirp.Models;
using Chirp.ODTO;
namespace Chirp.Interfaces;

public interface ILikeDisRepository
{
    public Task<CheepLikeDis?> GetCheepLikeDis(int CheepId);
    public CheepLikeDis CreateLikeDisSchema(Cheep cheep);
    public Task<CheepOpinionDTO> GetAuthorCheepOpinion(int CheepId, string AuthorName);
    public Task<CheepOpinionDTO> GetCheepLikesAndDislikes(int CheepId);
}