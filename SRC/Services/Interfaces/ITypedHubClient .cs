using System;
using System.Threading.Tasks;

namespace Nibo.Services.Interfaces {
	public interface ITypedHubClient {
        Task UpdatePercent(string msg);

        Task Sucess(string msg);

        Task Error(string msg);
    }
}
