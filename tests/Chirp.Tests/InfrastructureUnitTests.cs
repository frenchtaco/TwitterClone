using Chirp.CDTO;
using Chirp.Infrastructure;
using Chirp.Interfaces;
using Chirp.ODTO;
using DBContext;
using Enums.ACO;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IUT
{
    public class InfrastructureUnitTests
    {
        private readonly InMemoryTestController DB;
        private readonly DatabaseContext _dbContext;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICheepRepository _cheepRepository;
        private readonly ILikeDisRepository _likeDisRepository;

        private Author Author1 { get; set; } = null!;
        private Author Author2 { get; set; } = null!;
        private Cheep Cheep1 { get; set; } = null!;
        private Cheep Cheep2 { get; set; } = null!;
        private List<Cheep> Cheeps { get; set; } = null!;
        private List<Author> Authors { get; set; } = null!;

        public InfrastructureUnitTests()
        {
            DB = new InMemoryTestController();
            _dbContext = DB.GetDatabaseContext();

            _authorRepository  = new AuthorRepository(_dbContext);
            _likeDisRepository = new LikeDisRepository(_dbContext, _authorRepository);
            _cheepRepository   = new CheepRepository(_dbContext, _authorRepository, _likeDisRepository);

            InitiateGlobalAuthors();
        }

        /********************************
        
            INITIATE GLOBAL VARIABLES
        
        *********************************/
        public bool InitiateGlobalAuthors()
        {
            Author1 = _authorRepository.CreateAuthor("JohnnySins", "js@itu.dk");
            Author2 = _authorRepository.CreateAuthor("DarthMaul", "dm@itu.dk");

            if(Author1 != null || Author2 != null) return false;
            return true;
        }


        /*******************
        
            AUTHOR TESTS
        
        *******************/
        [Fact]
        private void CreateAuthorsTest()
        {
            for (int i = 1; i <= 64; i++)
            {
                Author newAuthor = _authorRepository.CreateAuthor($"AuthorName{i}", $"author{i}@id.com");
                
                Authors.Add(newAuthor);
                _dbContext.Authors.Add(newAuthor);
            }

            Assert.Equal(_dbContext.Authors.Count(), Authors.Count);
        }

        [Fact]
        public void TestFollowAndUnfollow()
        {
            // 01. To begin with, we assert they have 
            int Author1NumFollowers = Author1.Followers.Count;
            int Author2NumFollowers = Author2.Followers.Count;

            Assert.Equal(0, Author1NumFollowers);
            Assert.Equal(0, Author2NumFollowers);
        }

        [Fact]
        public async void GetTop4CheepsFromAuthorTest()
        {
            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(1000);
                CheepDTO newCheep = new CheepDTO($"Author1 - cheep{i+1}", "Author1");
                await _cheepRepository.CreateCheep(newCheep);
            }

            IEnumerable<Cheep> _top4Cheeps = await _cheepRepository.GetTop4FromAuthor("Author1");
            List<Cheep> top4Cheeps = _top4Cheeps.ToList();

            Assert.Equal(4, _top4Cheeps.Count());
            Assert.Equal("Author1 cheep1", top4Cheeps[0].Text);
            Assert.Equal("Author1 cheep2", top4Cheeps[1].Text);
            Assert.Equal("Author1 cheep3", top4Cheeps[2].Text);
            Assert.Equal("Author1 cheep4", top4Cheeps[3].Text);
        }

        /*******************
        
            CHEEP TESTS
        
        *******************/
        [Fact]
        public async Task CreateCheepsTest()
        {
            for (int i = 1; i <= 64; i++)
            {
                CheepDTO cheepDTO = new CheepDTO(testSentences[i - 1], Authors[i - 1].UserName);
                Cheep newCheep = await _cheepRepository.CreateCheep(cheepDTO);
                
                Cheeps.Add(newCheep);
                _dbContext.Cheeps.Add(newCheep);
            }

            Assert.Equal(64, Cheeps.Count);

            for (int i = 1; i <= 64; i++)
            {
                Assert.Equal(Cheeps[i - 1].Text, testSentences[i - 1]);
            }
        }

        [Fact]
        public async void GetCheepsTest()
        {
            IEnumerable<Cheep> _cheeps = await _cheepRepository.GetCheeps(0, "");
            List<Cheep> cheeps = _cheeps.ToList();

            Assert.Equal(32, _cheeps.Count());
            Assert.Equal("The quick brown fox jumps over the lazy dog.", cheeps[0].Text);
            Assert.Equal("Keep calm and code on.", cheeps[31].Text);
        }



        [Fact]
        public async void LikeAndDislikeTest()
        {
            bool IsLike = true;
            string Author1UserName = Author1.UserName;
            string Author2UserName = Author2.UserName;

            IEnumerable<Cheep> Top4Cheeps = await _cheepRepository.GetTop4FromAuthor(Author1UserName);
            List<Cheep> Top4CheepsList = Top4Cheeps.ToList<Cheep>();
            
            foreach (Cheep cheeps in Top4CheepsList)
            {
                await _cheepRepository.GiveOpinionOfCheep(IsLike, cheeps.CheepId, Author2UserName);
            }

            Dictionary<int, CheepOpinionDTO> AuthorOpinionOfCheeps = new Dictionary<int, CheepOpinionDTO>();

            foreach (Cheep cheeps in Top4CheepsList)
            {
                CheepOpinionDTO opinion = await _likeDisRepository.GetAuthorCheepOpinion(cheeps.CheepId, Author2UserName);
                AuthorOpinionOfCheeps.Add(cheeps.CheepId, opinion);
            }

            int totalNumLikes = 0, totalNumDislikes = 0;

            foreach (KeyValuePair<int, CheepOpinionDTO> entry in AuthorOpinionOfCheeps)
            {
                CheepOpinionDTO value = entry.Value;

                int numLikes = value.NumLikes;
                int numDislikes = value.NumDislikes;
                AuthorCheepOpinion aco = value.AuthorCheepOpinion;
                
                Assert.Equal(AuthorCheepOpinion.LIKES, value.AuthorCheepOpinion);
                Assert.Equal(1, numLikes);
                Assert.Equal(0, numDislikes);

                totalNumLikes += numLikes;
                totalNumDislikes += numDislikes;
            }

            Assert.Equal(4, totalNumLikes);
            Assert.Equal(0, totalNumDislikes);
        }

        [Fact]
        public void OrderCheepsByTest()
        {

            // 01. Have Author2 like a specific Cheep.
            // 02. Order them.
            // 03. Then that Cheep should be at the top.

        }





        readonly string[] testSentences = {
            "The quick brown fox jumps over the lazy dog.",
            "Coding is fun and challenging.",
            "Rainy days make me feel cozy.",
            "The sun sets in the west.",
            "C# is a powerful programming language.",
            "Coffee is the fuel for programmers.",
            "Winter is coming.",
            "Music soothes the soul.",
            "Books open new worlds.",
            "The sky is full of stars at night.",
            "Learning is a lifelong journey.",
            "Pizza is the best comfort food.",
            "Laughter is contagious.",
            "Hard work pays off.",
            "Time flies when you're having fun.",
            "Hiking is a great way to exercise.",
            "Smile, it's contagious!",
            "Challenges make life interesting.",
            "Technology is constantly evolving.",
            "Dream big, work hard.",
            "The early bird catches the worm.",
            "Kindness matters.",
            "The world is full of possibilities.",
            "Keep calm and code on.",
            "Success is a journey, not a destination.",
            "Coffee first, adulting second.",
            "Happiness is a choice.",
            "Make every moment count.",
            "Creativity is intelligence having fun.",
            "Life is short, make it sweet.",
            "A journey of a thousand miles begins with a single step.",
            "The best is yet to come.",
            "Gratitude turns what we have into enough.",
            "Live, love, laugh.",
            "Change is the only constant.",
            "Nature is the best therapy.",
            "Work smart, not just hard.",
            "Positive vibes only.",
            "Believe you can and you're halfway there.",
            "Adventure awaits.",
            "Explore the unknown.",
            "Never give up on your dreams.",
            "Success is not final, failure is not fatal: It is the courage to continue that counts.",
            "In the middle of difficulty lies opportunity.",
            "Be yourself; everyone else is already taken.",
            "The only limit to our realization of tomorrow will be our doubts of today.",
            "Strive for progress, not perfection.",
            "Life is 10% what happens to us and 90% how we react to it.",
            "Don't watch the clock; do what it does. Keep going.",
            "The only way to do great work is to love what you do.",
            "You are never too old to set another goal or to dream a new dream.",
            "The purpose of our lives is to be happy.",
            "Don't count the days, make the days count.",
            "You miss 100% of the shots you don't take.",
            "The only source of knowledge is experience.",
            "You can't stop the waves, but you can learn to surf.",
            "Life is what happens when you're busy making other plans.",
            "Every moment is a fresh beginning."
        };
    }
}