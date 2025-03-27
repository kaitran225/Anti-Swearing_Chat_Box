using AntiSwearingChatBox.Repository.Models;
using AntiSwearingChatBox.Repository.IRepositories;
using System.Collections.Generic;
using System.Linq;

namespace AntiSwearingChatBox.Repository.Repositories
{
    public class FilteredWordsRepository : RepositoryBase<FilteredWords>, IFilteredWordsRepository
    {
        public FilteredWordsRepository(AntiSwearingChatBoxContext context) : base(context)
        {
        }
    }
}
