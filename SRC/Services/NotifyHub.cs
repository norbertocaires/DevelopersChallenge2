using Microsoft.AspNetCore.SignalR;
using Nibo.Services.Interfaces;

namespace Nibo.Services {
	public class NotifyHub : Hub<ITypedHubClient> {

        public string GetConnectionId() {
            return Context.ConnectionId;
        }
    }
}
