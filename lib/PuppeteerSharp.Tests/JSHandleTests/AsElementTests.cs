using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class AsElementTests : PuppeteerPageBaseTest
    {
        public AsElementTests() : base()
        {
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.asElement", "should work")]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.body");
            var element = aHandle as IElementHandle;
            Assert.That(element, Is.Not.Null);
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.asElement", "should return null for non-elements")]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("2");
            var element = aHandle as IElementHandle;
            Assert.That(element, Is.Null);
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.asElement", "should return ElementHandle for TextNodes")]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await Page.SetContentAsync("<div>ee!</div>");
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.querySelector('div').firstChild");
            var element = aHandle as IElementHandle;
            Assert.That(element, Is.Not.Null);
            Assert.That(await Page.EvaluateFunctionAsync<bool>("e => e.nodeType === HTMLElement.TEXT_NODE", element), Is.True);
        }
    }
}
