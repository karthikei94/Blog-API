using BlogApp.Models;

namespace BlogApp.Services
{
    public interface IBlogPostService
    {
        Task<List<BlogPost>> GetAllAsync();
        Task<PagedResult<BlogPost>> GetPagedResultsAsync(int page, int pageSize);
        Task<BlogPost> GetAsync(int id);
        Task CreateBlogPost(BlogPost newPost);
        Task UpdateAsync(int id, BlogPost post);
        Task DeleteAsync(int id);
        Task LikePost(int id);
    }
}


