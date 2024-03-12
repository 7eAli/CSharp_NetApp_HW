using _1;



namespace Client_test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestReceive()
        {
            Client client = new Client();

            Assert.IsTrue(client.Receive("answer").Result);
            Assert.IsFalse(client.Receive(string.Empty).Result);
        }
    }
}