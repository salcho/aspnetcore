// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BasicTestApp;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using Microsoft.AspNetCore.Testing;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.Tests
{
    public class EventCallbackTest : ServerTestBase<ToggleExecutionModeServerFixture<Program>>
    {
        public EventCallbackTest(
            BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<Program> serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
        }

        protected override void InitializeAsyncCore()
        {
            // On WebAssembly, page reloads are expensive so skip if possible
            Navigate(ServerPathBase, noReload: _serverFixture.ExecutionMode == ExecutionMode.Client);
            Browser.MountTestComponent<BasicTestApp.EventCallbackTest.EventCallbackCases>();
        }

        [Theory]
        [QuarantinedTest("https://github.com/dotnet/aspnetcore/issues/23643")]
        [InlineData("capturing_lambda")]
        [InlineData("unbound_lambda")]
        [InlineData("unbound_lambda_nested")]
        [InlineData("unbound_lambda_strongly_typed")]
        [InlineData("unbound_lambda_child_content")]
        [InlineData("unbound_lambda_bind_to_component")]
        public void EventCallback_RerendersOuterComponent(string @case)
        {
            var target = Browser.FindElement(By.CssSelector($"#{@case} button"));
            var count = Browser.FindElement(By.Id("render_count"));
            Browser.Equal("Render Count: 1", () => count.Text);
            target.Click();
            Browser.Equal("Render Count: 2", () => count.Text);
        }
    }
}
