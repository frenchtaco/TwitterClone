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

        private Author Author1 { get; set; } = null!;
        private Author Author2 { get; set; } = null!;
        private Cheep Cheep1 { get; set; } = null!;
        private Cheep Cheep2 { get; set; } = null!;

        public InfrastructureUnitTests()
        {
            DB = new InMemoryTestController();
            _dbContext = DB.GetDatabaseContext();

            _authorRepository = new AuthorRepository(_dbContext);
        }

        public bool CreateAuthors()
        {
            Author1 = _authorRepository.CreateAuthor("JohnnySins", "js@itu.dk");
            Author2 = _authorRepository.CreateAuthor("DarthMaul", "dm@itu.dk");

            if(Author1 != null || Author2 != null) return false;
            return true;
        }

        [Fact]
        public void CreateAuthorsTest()
        {
            bool result = CreateAuthors();
            
            Assert.True(result);
        }

        [Fact]
        public void TestCreateCheep()
        {
            

            //Assert.Equal();
        }

        [Fact]
        public void TestCreateAuthor()
        {
            

            //Assert.Equal();
        }

        [Fact]
        public void TestFollowAndUnfollow()
        {
            // 01. To begin with, we assert they have 

            //Assert.Equal();
        }

        [Fact]
        public void TestLikeAndDislike()
        {
            

            //Assert.Equal();
        }
    }
}