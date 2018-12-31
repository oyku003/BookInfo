using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.Rules;
using BookInfo.BusinessLayer;
using BookInfo.Entities;

namespace BookInfo.ML
{
    public class AprioriProcess
    {
        private readonly BookSessionInfoManager bookSessionInfoManager = new BookSessionInfoManager();
        private readonly SimilarBookManager similarBookManager = new SimilarBookManager();

        public void CreateAprioriRules()
        {
            var associationBookList = bookSessionInfoManager.List().GroupBy(x => x.SessionId).ToArray();
            SortedSet<int>[] dataset = new SortedSet<int>[associationBookList.Count()];

            for (int i = 0; i < associationBookList.Count(); i++)
            {
                SortedSet<int> sortedSet = new SortedSet<int>() { };

                foreach (var item1 in associationBookList[i])
                {
                    sortedSet.Add(item1.BookId);
                }

                dataset[i] = sortedSet;
            }

            Apriori apriori = new Apriori(threshold: 3, confidence: 0.5);
            AssociationRuleMatcher<int> classifier = apriori.Learn(dataset);
            int[][] matches = classifier.Decide(new[] { 1 });

            AssociationRule<int>[] rules = classifier.Rules;

            InsertSimilarBook(rules);

            //foreach (var item in rules)
            //{
            //    item.X.ToString();
            //    item.ToString();
            //}
            //return rules;

        }
        private void InsertSimilarBook(AssociationRule<int>[] rules)
        {
            try
            {
                var similar = similarBookManager.List();
                foreach (var item in similar)
                {
                    similarBookManager.Delete(item);
                }

                foreach (var item in rules)
                {
                    SimilarBook similarBook = new SimilarBook();
                    foreach (var item1 in item.X)
                    {
                        similarBook.BookId = Convert.ToInt32(item1);
                    }
                    foreach (var item1 in item.Y)
                    {
                        similarBook.SimilarBookId = Convert.ToInt32(item1);
                    }                  

                    similarBookManager.Insert(similarBook);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
