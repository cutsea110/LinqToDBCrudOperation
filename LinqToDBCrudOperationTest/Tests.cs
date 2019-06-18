using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System.Linq;
using System.Diagnostics;

namespace LinqToDBCrudOperationTest
{
    [TestFixture]
    public class Tests
    {
        static Tests()
        {
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (msg, context) => Console.WriteLine(msg, context);
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

        [Table]
        public class TestTable3
        {
            [PrimaryKey] public int ID;
            [Column(Length = 50), NotNull] public string Name;
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
                try { db.DropTable<TestTable3>(); } catch { }
                db.CreateTable<TestTable3>();
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
                            Name = t.Name + " II",
                            CreatedOn = t.CreatedOn.Value.AddDays(1),
                        });
            }
        }

        [Test]
        public void InsertTest4([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.GetTable<TestTable>()
                    .Where(t => t.Name == "Crazy Frog")
                    .Into(db.GetTable<TestTable2>())
                        .Value(t => t.Name, t => t.Name + " II")
                        .Value(t => t.CreatedOn, t => t.CreatedOn.Value.AddDays(1))
                    .Insert();
            }
        }

        [Test]
        public void InsertWithIdentityTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                var identity = db.GetTable<TestTable>()
                    .InsertWithIdentity(() => new TestTable
                    {
                        Name = "Crazy Frog",
                        CreatedOn = Sql.CurrentTimestamp,
                    });
                Console.WriteLine(identity);
            }
        }

        [Test]
        public void InsertOrUpdateTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                var identity = db.GetTable<TestTable3>()
                    .InsertOrUpdate(
                        () => new TestTable3
                        {
                            ID = 5,
                            Name = "Crazy Frog",
                        },
                        t => new TestTable3
                        {
                            Name = "Crazy Frog IV",
                        });
                Console.WriteLine(identity);
            }
        }

        [Test]
        public void InsertOrReplaceTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.InsertOrReplace(
                    new TestTable3
                    {
                        ID = 5,
                        Name = "Crazy Frog",
                    });
            }
        }

        [Test]
        public void UpdateTest([ValuesAttribute(ProviderName.SqlServer, ProviderName.PostgreSQL)]string configString)
        {
            using (var db = new DataConnection(configString))
            {
                db.Update(
                    new TestTable3
                    {
                        ID = 5,
                        Name = "Crazy Frog",
                    });
            }
        }
    }
}
