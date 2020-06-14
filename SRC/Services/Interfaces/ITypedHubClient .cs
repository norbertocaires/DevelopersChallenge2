using Nibo.Models.ViewModels;
using System.Threading.Tasks;

namespace Nibo.Services.Interfaces {
	public interface ITypedHubClient {
        Task UpdatePercent(string msg);

        Task Sucess(ImportViewModel import);

        Task Error(string msg);
    }
}
