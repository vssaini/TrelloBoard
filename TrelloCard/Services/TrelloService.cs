using Manatee.Trello;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        private async Task<bool> IsPremiumMember()
        {
            // Ref - https://community.atlassian.com/t5/Trello-questions/Is-there-any-API-available-to-get-check-trello-user-account-is/qaq-p/641392

            throw new NotImplementedException();           
            
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

           await AttachImageAndVideoFilesAsync(card);

            return card;
        }

        private static async Task AttachImageAndVideoFilesAsync(ICard card)
        {
            // Ref - https://support.atlassian.com/trello/docs/adding-attachments-to-cards/
            // There is a 10 MB file upload limit per attachment. However, paid members have a 250 MB file upload limit per attachment. There is no account data storage limit in any Trello plan.

            // The 10MB file upload only applies to files uploaded from your computer.It does not apply to files attached from Google Drive, Dropbox, Box or OneDrive.

            var imgBytes = GetImageBytes();
            await card.Attachments.Add(imgBytes, "Logo.png");

            var videoBytes = GetVideoBytes();
            await card.Attachments.Add(videoBytes, "Video.mp4");
        }

        private static byte[] GetImageBytes()
        {
            var imagePath = HttpContext.Current.Server.MapPath("~/Attachments/Logo.png");
            return File.ReadAllBytes(imagePath);
        }

        private static byte[] GetVideoBytes()
        {
            // NOTE - This commented code can be used for process directly from request
            //    using (Stream requestStream = request.GetRequestStream())
            //    using (Stream video = File.OpenRead("Path"))
            //    {
            //        byte[] buffer = new byte[4096];

            //        while (true)
            //        {
            //            int bytesRead = video.Read(buffer, 0, buffer.Length);

            //            if (bytesRead == 0) break;
            //            requestStream.Write(buffer, 0, bytesRead);
            //        }
            //    }

            var videoFilePath = HttpContext.Current.Server.MapPath("~/Attachments/Video.mp4");
            return File.ReadAllBytes(videoFilePath);
        }
    }
}
