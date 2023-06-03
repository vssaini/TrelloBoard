using Manatee.Trello;
using System.Threading.Tasks;

namespace TrelloCard.Contracts
{
    public interface ITrelloService
    {
        Task<ICard> CreateCardInBoardAsync();
    }
}
