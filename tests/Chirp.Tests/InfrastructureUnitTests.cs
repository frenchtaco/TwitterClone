using Chirp.CDTO;
using Chirp.Infrastructure;
using Chirp.Interfaces;
using DBContext;

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
            Assert.Equal(0, Author1NumFollowers);
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
        private async Task CreateCheepsTest()
        {
            for (int i = 1; i <= 64; i++)
            {
                CheepDTO cheepDTO = new CheepDTO($"String{i}", Authors[i - 1].UserName);
                Cheep newCheep = await _cheepRepository.CreateCheep(cheepDTO);
                
                Cheeps.Add(newCheep);
                _dbContext.Cheeps.Add(newCheep);
            }

            Assert.Equal(64, Cheeps.Count);

            // foreach (Cheep cheep in Cheeps)
            // {
                

            //     Assert.Equal(cheep);
            // }
        }

        [Fact]
        public async void GetCheepsTest()
        {
            IEnumerable<Cheep> _cheeps = await _cheepRepository.GetCheeps(0, "");
            List<Cheep> cheeps = _cheeps.ToList();

            Assert.Equal(32, _cheeps.Count());
            Assert.Equal("String1", cheeps[0].Text);
            Assert.Equal("String31", cheeps[31].Text);

            //Assert.Equal();
        }


        [Fact]
        public void LikeAndDislikeTest()
        {
            

            //Assert.Equal();
        }

        [Fact]
        public void OrderCheepsByTest()
        {

        }
    }
}