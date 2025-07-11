using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class PageQuerySelectorEvalTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorEvalTests() : base()
        {
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$eval", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var idAttribute = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id");
            Assert.That(idAttribute, Is.EqualTo("testAttribute"));
        }

        [Test, Ignore("previously not marked as a test")]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var section = await Page.QuerySelectorAsync("section");
            var idAttribute = await section.EvaluateFunctionAsync<string>("e => e.id");
            Assert.That(idAttribute, Is.EqualTo("testAttribute"));
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$eval", "should accept arguments")]
        public async Task ShouldAcceptArguments()
        {
            await Page.SetContentAsync("<section>hello</section>");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, suffix) => e.textContent + suffix", " world!");
            Assert.That(text, Is.EqualTo("hello world!"));
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$eval", "should accept ElementHandles as arguments")]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>hello</section><div> world</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, div) => e.textContent + div.textContent", divHandle);
            Assert.That(text, Is.EqualTo("hello world"));
        }

        [Test, PuppeteerTest("queryselector.spec", "Page.$eval", "should throw error if no element is found")]
        public void ShouldThrowErrorIfNoElementIsFound()
        {
            var exception = Assert.ThrowsAsync<SelectorException>(()
                => Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id"));
            Assert.That(exception.Message, Does.Contain("failed to find element matching selector"));
        }
    }
}
