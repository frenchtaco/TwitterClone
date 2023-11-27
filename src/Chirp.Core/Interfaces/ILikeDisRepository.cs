using Enums.ACO;
using Chirp.Models;
using Chirp.ODTO;
namespace Chirp.Interfaces;

public interface ILikeDisRepository
{
    public Task<CheepLikeDis?> GetCheepLikeDis(int CheepId);
    public Task<CO_AuthorOpinion_DTO> GetAuthorCheepOpinion(int CheepId, string AuthorName);
    public CheepLikeDis CreateLikeDisSchema(Cheep cheep);
    public Task<CO_Schema_DTO> GetCheepLikesAndDislikes(int CheepId);
}