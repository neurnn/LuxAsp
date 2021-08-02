using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp
{
    public sealed class LuxWebSocketManager : IDisposable
    {
        private List<LuxWebSocket> m_WebSockets;
        private CancellationTokenSource m_Disposed;

        /// <summary>
        /// Initialize the WebSocket Manager instance.
        /// </summary>
        internal LuxWebSocketManager(IServiceProvider Services)
        {
            m_WebSockets = new List<LuxWebSocket>();
            m_Disposed = new CancellationTokenSource();
        }

        /// <summary>
        /// Add WebSocket to the Collection.
        /// </summary>
        /// <param name="WebSocket"></param>
        internal void Add(LuxWebSocket WebSocket)
        {
            lock (m_WebSockets)
                m_WebSockets.Add(WebSocket);
        }

        /// <summary>
        /// Remove WebSocket from the Collection.
        /// </summary>
        /// <param name="WebSocket"></param>
        internal void Remove(LuxWebSocket WebSocket)
        {
            lock (m_WebSockets)
                m_WebSockets.Remove(WebSocket);
        }

        /// <summary>
        /// Get WebSocket Collection's Snapshot.
        /// </summary>
        /// <returns></returns>
        public LuxWebSocket[] GetSockets(Predicate<LuxWebSocket> Match = null)
        {
            lock (m_WebSockets)
            {
                if (Match is null) return m_WebSockets.ToArray();
                return m_WebSockets.Where(X => Match(X)).ToArray();
            }
        }

        /// <summary>
        /// Handle the WebSocket request.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public async Task<bool> OnRequest(HttpContext Context, CancellationToken Token = default)
        {
            if (!Context.WebSockets.IsWebSocketRequest)
                return false;

            var WebSocket = await Context.WebSockets.AcceptWebSocketAsync();
            if (WebSocket is null)
                return false;

            if (m_Disposed.IsCancellationRequested)
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "", default);
                try { WebSocket.Dispose(); } catch { }
                return true;
            }

            using var CTS = CancellationTokenSource.CreateLinkedTokenSource(m_Disposed.Token, Token);
            using(var Wrapper = new LuxWebSocket(this, Context, WebSocket))
            {
                try { await Wrapper.OnHandle(CTS.Token); }
                catch (OperationCanceledException) { }
            }

            return true;
        }

        /// <summary>
        /// Dispose the WebSocket Manager instance.
        /// </summary>
        public void Dispose()
        {
            lock (m_Disposed)
            {
                if (m_Disposed.IsCancellationRequested)
                    return;

                m_Disposed.Cancel();
            }
        }
    }
}
