using Chirp.Models;
using Enums.ACO;

namespace Chirp.ODTO;

public record CheepOpinionDTO(CheepLikeDis CheepOpinionSchema, AuthorCheepOpinion AuthorCheepOpinion, int NumLikes, int NumDislikes);