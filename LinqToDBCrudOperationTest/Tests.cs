using System;
using System.Diagnostics;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;

using System.Data.SqlTypes;
using System.Linq;

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
            [PrimaryKey, Identity] public int ID;
            [Column(Length = 50), NotNull] public string Name;
            [Column(Length = 250), Nullable] public string Description;
            [Column] public DateTime? CreatedOn;
        }

        [Table]
        public class TestTable2
        {
            [PrimaryKey, Identity] public int ID;
            [Column(Length = 50), NotNull] public string Name;
            [Column(Length = 250), Nullable] public string Description;
            [Column] public DateTime? CreatedOn;
        }

        [Test]
        public void CreateTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                try { db.DropTable<TestTable>(); } catch { }
                db.CreateTable<TestTable>();
                try { db.DropTable<TestTable2>(); } catch { }
                db.CreateTable<TestTable2>();
            }
        }

        [Test]
        public void InsertTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.Insert<TestTable>(new TestTable
                {
                    Name = "Crazy Frog",
                });
            }
        }

        [Test]
        public void InsertTest2([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.GetTable<TestTable>()
                    .Insert(() => new TestTable
                    {
                        Name = "Crazy Frog",
                        CreatedOn = Sql.CurrentTimestamp,
                    });
            }
        }

        [Test]
        public void InsertTest3([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.GetTable<TestTable>()
                    .Where(t => t.Name == "Crazy Frog")
                    .Insert(
                        db.GetTable<TestTable2>(),
                        t => new TestTable2
                        {
                            Name = t.Name + "II",
                            CreatedOn = t.CreatedOn.Value.AddDays(1),
                        });
            }
        }
    }
}
