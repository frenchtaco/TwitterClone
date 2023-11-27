using Chirp.Models;
using Enums.ACO;

namespace Chirp.ODTO;

public record CO_Schema_DTO(CheepLikeDis CheepOpinionSchema, int NumLikes, int NumDislikes);
public record CO_AuthorOpinion_DTO(AuthorCheepOpinion AuthorCheepOpinion, int NumLikes, int NumDislikes);