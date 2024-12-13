using BlogApp.Models;

namespace BlogApp.Services
{
    public interface IBlogPostService
    {
        Task<List<BlogPost>> GetAllAsync();
        Task<PagedResult<BlogPost>> GetPagedResultsAsync(int page, int pageSize, string loggedInUserId, bool filterCreatedBy = false);
        Task<BlogPost> GetAsync(string id);
        Task CreateBlogPost(BlogPost newPost, string userId);
        Task UpdateAsync(string id, BlogPost post);
        Task DeleteAsync(string id);
        Task LikePost(string id);
    }
}


