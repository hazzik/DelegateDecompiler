using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup90AdditionalFeatures
{
	class Test02IQueryableClosuresTests

	{
		private ClassEnvironment classEnv;

		[OneTimeSetUp]
		public void SetUpFixture()
		{
			classEnv = new ClassEnvironment();
		}

		[Test]
		public void Test_CanUseIQueryableClosure()
		{
			using (var env = new MethodEnvironment(classEnv))
			{
				//ATTEMPT
				env.AboutToUseDelegateDecompiler();

				var dogs = env.Db.Set<Animal>().Where(it => it.Species == "Canis lupus");
				var query = env.Db.Set<Person>().Where(it => it.Animals.Intersect(dogs).Any()).Decompile();
				var list = query.ToList();

				//VERIFY
				Assert.AreEqual(1, list.Count());
			}
		}

		[Test, Ignore("Not supported yet")]
		public void Test_CanUseRefIQueryableClosure()
		{
			using (var env = new MethodEnvironment(classEnv))
			{
				//SETUP
				env.AboutToUseDelegateDecompiler();

				var dogs = env.Db.Set<Animal>().Where(it => it.Species == "Canis lupus");
				var query = env.Db.Set<Person>().Where(it => it.Animals.Intersect(dogs).Any()).Decompile();

				//ATTEMPT
				dogs = dogs.Where(it => it.Age > 10);
				query = env.Db.Set<Person>().Where(it => it.Animals.Intersect(dogs).Any()).Decompile();
				var list = query.ToList();

				//VERIFY
				Assert.AreEqual(0, list.Count());
			}
		}
	}
}