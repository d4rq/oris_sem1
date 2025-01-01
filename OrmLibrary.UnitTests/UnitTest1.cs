using Moq;
using OrmLibrary.UnitTests.Models;
using System.Data;

namespace OrmLibrary.UnitTests
{
    public class Tests(OrmContext<UserInfo> dbContext)
    {
        private OrmContext<UserInfo> _dbContext = dbContext;

        [Test]
        public void GetById_When_()
        {
            var dbConnection = new Mock<IDbConnection>();
            var dbCommand = new Mock<IDbCommand>();
            var dataReader = new Mock<IDataReader>();
            var dataParameter = new Mock<IDbDataParameter>();
            var dataParameterCollection = new Mock<IDataParameterCollection>();
            _dbContext = new OrmContext<UserInfo>(dbConnection.Object);

            var userId = Guid.NewGuid();
            var userInfo = new UserInfo
            {
                Id = 1,
                Age = 20,
                Name = "asd",
                Email = "asd@mail.ru",
                Gender = 1
            };

            dataReader.SetupSequence(r => r.Read()).Returns(true);

            dataReader.Setup(r => r["Id"]).Returns(userInfo.Id);
            dataReader.Setup(r => r["Age"]).Returns(userInfo.Age);
            dataReader.Setup(r => r["Name"]).Returns(userInfo.Name);
            dataReader.Setup(r => r["Email"]).Returns(userInfo.Email);
            dataReader.Setup(r => r["Gender"]).Returns(userInfo.Gender);

            dbCommand.Setup(c => c.ExecuteReader()).Returns(dataReader.Object);
            dbCommand.Setup(c => c.CreateParameter()).Returns(dataParameter.Object);
            dbCommand.Setup(c => c.Parameters).Returns(dataParameterCollection.Object);
            dbConnection.Setup(c => c.CreateCommand()).Returns(dbCommand.Object);

            var result = _dbContext.ReadById(userId);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(userInfo.Id, Is.EqualTo(result.Id));
                Assert.That(userInfo.Age, Is.EqualTo(result.Age));
                Assert.That(userInfo.Name, Is.EqualTo(result.Name));
                Assert.That(userInfo.Email, Is.EqualTo(result.Email));
                Assert.That(userInfo.Gender, Is.EqualTo(result.Gender));
            });
        }
    }
}