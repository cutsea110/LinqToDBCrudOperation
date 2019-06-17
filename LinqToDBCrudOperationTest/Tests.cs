using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;

using System.Data.SqlTypes;

namespace LinqToDBCrudOperationTest
{
    [TestFixture]
    public class Tests
    {
        static Tests()
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (msg, context) => Debug.WriteLine(msg, context);
        }

        [Table]
        public class TestTable
        {
            [PrimaryKey, Identity]
            public int ID;
            [Column(Length = 50), NotNull]
            public string Name;
            [Column(Length = 250), Nullable]
            public string Description;
        }

        [Test]
        public void CreateTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using(var db = new DataConnection(configString))
            {
                try{ db.DropTable<TestTable>(); } catch { }
                db.CreateTable<TestTable>();
            }
        }
    }
}
