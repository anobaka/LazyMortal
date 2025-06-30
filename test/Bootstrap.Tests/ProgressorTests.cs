using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bootstrap.Tests
{
    [TestClass]
    public class ProgressorTests
    {
        [TestMethod]
        public async Task Test()
        {
            var uniqueProgresses = new HashSet<int>();
            var bp = new BProgressor(p =>
            {
                Assert.IsTrue(uniqueProgresses.Add(p));
                Console.WriteLine(p);
                return Task.CompletedTask;
            });

            await using (var bpc = bp.CreateNewScope(0, 20))
            {
                for (var i = 0; i < 100; i += 2)
                {
                    await bpc.Add(2);
                }
            }

            await using (var bpc = bp.CreateNewScope(20, 50))
            {
                for (var i = 0; i < 50; i += 2)
                {
                    await bpc.Set(i);
                }
            }

            await using (var bpc = bp.CreateNewScope(70, 30))
            {
                for (var i = 0; i < 100; i += 2)
                {
                    await bpc.Add(2);
                }
            }
        }
    }
}
