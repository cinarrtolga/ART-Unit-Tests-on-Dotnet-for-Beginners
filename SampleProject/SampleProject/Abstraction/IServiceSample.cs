using System.Threading.Tasks;

namespace SampleProject.Abstraction
{
	public interface IServiceSample
	{
		Task Insert();
        Task Update();
        Task Delete();
	}
}

