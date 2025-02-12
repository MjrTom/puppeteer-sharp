// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System.Threading.Tasks;

namespace PuppeteerSharp.Bidi;

internal class BidiFrameRealm(WindowRealm realm, BidiFrame frame) : BidiRealm(realm, frame.TimeoutSettings, frame.BidiPage.BidiBrowser.LoggerFactory)
{
    private readonly WindowRealm _realm = realm;
    private bool _bindingsInstalled;

    public static BidiFrameRealm From(WindowRealm realm, BidiFrame frame)
    {
        var frameRealm = new BidiFrameRealm(realm, frame);
        frameRealm.Initialize();
        return frameRealm;
    }

    public override async Task<IJSHandle> GetPuppeteerUtilAsync()
    {
        var installTcs = new TaskCompletionSource<bool>();

        if (!_bindingsInstalled)
        {
            // TODO: Implement
            installTcs.TrySetResult(true);
            _bindingsInstalled = true;
        }

        await installTcs.Task.ConfigureAwait(false);
        return await base.GetPuppeteerUtilAsync().ConfigureAwait(false);
    }

    protected override void Initialize()
    {
        base.Initialize();

        _realm.Updated += (_, __) =>
        {
            (Environment as Frame)?.ClearDocumentHandle();
            _bindingsInstalled = false;
        };
    }
}
