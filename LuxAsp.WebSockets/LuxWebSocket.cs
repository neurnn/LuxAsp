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
    /// <summary>
    /// WebSocket Wrapper.
    /// </summary>
    public sealed class LuxWebSocket : ILuxInfrastructure<HttpContext>, ILuxInfrastructure<IServiceProvider>, ILuxInfrastructure<WebSocket>, IDisposable
    {
        private HttpContext m_HttpContext;

        internal LuxWebSocket(LuxWebSocketManager Manager, HttpContext Context, WebSocket Instance)
        {
            m_HttpContext = Context;
            this.Instance = Instance;
            this.Manager = Manager;
        }

        /// <summary>
        /// WebSocket Instance.
        /// </summary>
        public WebSocket Instance { get; private set; }

        /// <summary>
        /// WebSocket Manager Instance.
        /// </summary>
        public LuxWebSocketManager Manager { get; private set; }

        /// <summary>
        /// Gets IServiceProvider that is from HttpContext.
        /// </summary>
        IServiceProvider ILuxInfrastructure<IServiceProvider>.Instance => m_HttpContext.RequestServices;

        /// <summary>
        /// Gets the HttpContext.
        /// </summary>
        HttpContext ILuxInfrastructure<HttpContext>.Instance => m_HttpContext;

        /// <summary>
        /// Handle WebSocket Communication.
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        internal async Task OnHandle(CancellationToken Token = default)
        {
            Manager.Add(this);

            while (true)
            {
            }

            /* Close the WebSocket here.*/
            await Instance.CloseAsync(WebSocketCloseStatus.NormalClosure, "", default);
        }

        /// <summary>
        /// Dispose the WebSocket Wrapper.
        /// </summary>
        public void Dispose()
        {
            Manager?.Remove(this);
            Manager = null;

            Instance?.Dispose();
            Instance = null;
        }
    }
}
