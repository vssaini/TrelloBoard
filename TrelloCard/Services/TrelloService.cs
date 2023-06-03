using Manatee.Trello;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TrelloCard.Contracts;

namespace TrelloCard.Services
{
    public class TrelloService : ITrelloService
    {
        // Ref - https://developer.atlassian.com/cloud/trello/guides/rest-api/api-introduction/

        // To create Trello API one need to create a Trello Power Up
        // From there generate API key
        // Then generate token manually using https://trello.com/1/authorize?expiration=never&name=KSAllen&scope=read,write&response_type=token&key={APIKey}

        public async Task<ICard> CreateCardInBoardAsync()
        {
            var board = await GetSpecificBoardAsync();
            var card = await CreateCardInBoardList(board);

            return card;
        }

        private static async Task<IBoard> GetSpecificBoardAsync()
        {
            var auth = GetTrelloAuthorization();

            var factory = new TrelloFactory();
            var me = await factory.Me(auth);

            // Refreshing is must
            await me.Boards.Refresh();

            var boardName = ConfigurationManager.AppSettings["TrelloBoardName"];
            var board = me.Boards.FirstOrDefault(b => b.Name == boardName);
            return board;
        }

        private static TrelloAuthorization GetTrelloAuthorization()
        {
            var auth = new TrelloAuthorization
            {
                AppKey = ConfigurationManager.AppSettings["TrelloApiKey"],
                UserToken = ConfigurationManager.AppSettings["TrelloUserToken"]
            };

            return auth;
        }

        private static async Task<ICard> CreateCardInBoardList(IBoard board)
        {
            // We don't care about the board; we just want the lists. By just refreshing
            // the list collection, we reduce the amount of data that must be retrieved.
            await board.Lists.Refresh();

            // Refreshing the list collection downloaded all of the data for the lists as well.
            var listName = ConfigurationManager.AppSettings["TrelloListName"];
            var targetList = board.Lists.FirstOrDefault(l => l.Name == listName);

            if (targetList == null) return null;

            var card = await targetList.Cards
                .Add("Complete Trello API task", "We must complete this task as soon as possible", dueDate: null);

            return card;
        }
    }
}
