using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class WaitForResponseTests : PuppeteerPageBaseTest
    {
        public WaitForResponseTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }")
            );
            Assert.That(task.Result.Url, Is.EqualTo(TestConstants.ServerUrl + "/digits/2.png"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should work with predicate")]
        public async Task ShouldWorkWithPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(response => response.Url == TestConstants.ServerUrl + "/digits/2.png");

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.That(task.Result.Url, Is.EqualTo(TestConstants.ServerUrl + "/digits/2.png"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should work with async predicate")]
        public async Task ShouldWorkWithAsyncPredicate()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(async (IResponse response) =>
            {
                await Task.Delay(1);
                return response.Url == TestConstants.ServerUrl + "/digits/2.png";
            });

            await Task.WhenAll(
            task,
            Page.EvaluateFunctionAsync(@"() => {
                fetch('/digits/1.png');
                fetch('/digits/2.png');
                fetch('/digits/3.png');
            }")
            );
            Assert.That(task.Result.Url, Is.EqualTo(TestConstants.ServerUrl + "/digits/2.png"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should respect timeout")]
        public async Task ShouldRespectTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false, new WaitForOptions(1)));

            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should respect default timeout")]
        public async Task ShouldRespectDefaultTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.WaitForResponseAsync(_ => false));

            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.waitForResponse", "should work with no timeout")]
        public async Task ShouldWorkWithNoTimeout()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var task = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/digits/2.png", new WaitForOptions(0));

            await Task.WhenAll(
                task,
                Page.EvaluateFunctionAsync(@"() => setTimeout(() => {
                    fetch('/digits/1.png');
                    fetch('/digits/2.png');
                    fetch('/digits/3.png');
                }, 50)")
            );
            Assert.That(task.Result.Url, Is.EqualTo(TestConstants.ServerUrl + "/digits/2.png"));
        }
    }
}
