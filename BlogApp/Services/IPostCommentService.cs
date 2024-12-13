using BlogApp.Models;

namespace BlogApp.Services;

public interface IPostCommentService
{
    Task<PostComments> AddComment(PostComments comment);
    Task<List<PostComments>> GetAllAsync();
    Task<List<PostComments>> GetByPostAsync(string postId);
    Task<PostComments> GetAsync(string id);
    Task DeleteAsync(string id);
    Task LikeComment(string id);

}






