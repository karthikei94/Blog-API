using BlogApp.Models;

namespace BlogApp.Services;

public interface IPostCommentService
{
    Task AddComment(PostComments comment);
    Task<List<PostComments>> GetAllAsync();
    Task<List<PostComments>> GetByPostAsync(int postId);
    Task<PostComments> GetAsync(int id);
    Task DeleteAsync(int id);
    Task LikeComment(int id);

}






