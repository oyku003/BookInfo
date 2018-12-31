using BookInfo.BusinessLayer.Abstract;
using BookInfo.Common.Helpers;
using BookInfo.DataAccessLayer.EntityFramework;
using BookInfo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.BusinessLayer
{
    public class BookSessionInfoManager : ManagerBase<BookSessionInfo>
    {
        private readonly Repository<BookSessionInfo> repo_sessionInfo = new Repository<BookSessionInfo>();

        public void SaveBookInfo(int id)
        {
            string userIp = IpHelper.GetUserIP();

            if (string.IsNullOrWhiteSpace(userIp))
            {
                return;
            }
            BookSessionInfo bookSessionInfo = new BookSessionInfo()
            {
                BookId = id,
                SessionId = userIp
            };

            repo_sessionInfo.Insert(bookSessionInfo);
        }
    }
}
