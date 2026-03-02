using SheetsEF.Models;
using Xunit;

namespace Tests.SheetsEF
{
    public class ApplicationContextBaseTests
    {
        [Fact]
        public void GetData_NonExistentKey_ReturnsDefault()
        {
            using var context = new ApplicationContextBase();

            var result = context.GetData<string>("nonexistent");

            Assert.Null(result);
        }

        [Fact]
        public void BackupDataToSheets_ReturnsTrue()
        {
            using var context = new ApplicationContextBase();

            Assert.True(context.BackupDataToSheets());
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var context = new ApplicationContextBase();
            context.Dispose();
        }
    }
}
