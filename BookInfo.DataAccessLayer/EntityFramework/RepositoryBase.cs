using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfo.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        protected static DatabaseContext context;
        private static object _lockSync = new object();
        protected RepositoryBase()//class newlenemez sadece miras alan newler.O yuzden protected yaptık.
        {
            CreateContext();
        }

        private static void CreateContext()
        {
            if (context == null)
            {
                lock (_lockSync)//aynı anda 2 thread gelip 2 newleme yapmasın diye.Multi-thread uygulamalr için gecerli.
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }

                }

            }

        }
    }
}
